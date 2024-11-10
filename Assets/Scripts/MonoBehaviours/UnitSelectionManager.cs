using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace MonoBehaviours
{
    public class UnitSelectionManager : MonoBehaviour
    {
        public static UnitSelectionManager Instance { get; private set; }

        public event EventHandler OnSelectionAreaStart;
        public event EventHandler OnSelectionAreaEnd;

        private Vector2 _selectionStartMousePosition;

        private void Awake()
        {
            Instance = this;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _selectionStartMousePosition = Input.mousePosition;
                OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetMouseButtonUp(0))
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                // deselect previously selected units
                var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

                var entities = entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    entityManager.SetComponentEnabled<Selected>(entity, false);
                }

                // get selectionAreaRect and check if we should select
                var selectionAreaRect = GetSelectionAreaRect();
                var selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
                var multiSelectSizeMin = 40f;

                if (selectionAreaSize > multiSelectSizeMin)
                {
                    // Multi-select Logic
                    entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);

                    entities = entityQuery.ToEntityArray(Allocator.Temp);
                    var localTransforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                    for (var i = 0; i < localTransforms.Length; i++)
                    {
                        var unitLocalTransform = localTransforms[i];
                        var unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                        if (selectionAreaRect.Contains(unitScreenPosition))
                        {
                            entityManager.SetComponentEnabled<Selected>(entities[i], true);
                        }
                    }
                }
                else
                {
                    // Single select logic
                    entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                    var physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                    var collisionWorld = physicsWorldSingleton.CollisionWorld;
                    var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    const int unitsLayer = 6;
                    var raycastInput = new RaycastInput
                    {
                        Start = cameraRay.GetPoint(0f),
                        End = cameraRay.GetPoint(9999f),
                        Filter = new CollisionFilter
                        {
                            GroupIndex = 0,
                            BelongsTo = ~0u, // flipping 0 with ~ results in every bit being set
                            CollidesWith = 1u << unitsLayer, // bit-shift by the layer number
                        },
                    };
                    if (collisionWorld.CastRay(raycastInput, out var raycastHit))
                    {
                        if (entityManager.HasComponent<Unit>(raycastHit.Entity))
                        {
                            entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                        }
                    }
                }

                OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
            }

            if (Input.GetMouseButtonDown(1))
            {
                var mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(entityManager);

                var unitMovers = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);

                var movePositionArray = GenerateMovePositionArray(mouseWorldPosition, unitMovers.Length);

                for (var i = 0; i < unitMovers.Length; i++)
                {
                    var unitMover = unitMovers[i];
                    unitMover.TargetPosition = movePositionArray[i];
                    unitMovers[i] = unitMover;
                }

                entityQuery.CopyFromComponentDataArray(unitMovers);
            }
        }

        public Rect GetSelectionAreaRect()
        {
            Vector2 selectionEndMousePosition = Input.mousePosition;

            Vector2 lowerLeftCorner = new Vector2(
                Mathf.Min(_selectionStartMousePosition.x, selectionEndMousePosition.x),
                Mathf.Min(_selectionStartMousePosition.y, selectionEndMousePosition.y)
            );

            Vector2 upperRightCorner = new Vector2(
                Mathf.Max(_selectionStartMousePosition.x, selectionEndMousePosition.x),
                Mathf.Max(_selectionStartMousePosition.y, selectionEndMousePosition.y)
            );

            return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, upperRightCorner.x - lowerLeftCorner.x, upperRightCorner.y - lowerLeftCorner.y);
        }

        /*
         * Generates move positions for units in an expanding rings pattern where
         * each outer ring has more positions than its neighbouring inner ring.
         */
        private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
        {
            var positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
            if (positionCount == 0)
            {
                // no positions
                return positionArray;
            }

            positionArray[0] = targetPosition;
            if (positionCount == 1)
            {
                // just one position
                return positionArray;
            }

            // setup state for generating positions
            var ringSize = 2.2f;
            var ring = 0;
            var positionIndex = 1;

            while (positionIndex < positionCount)
            {
                // sets number of positions in a ring relative to which ring we are on
                var ringPositionCount = 3 + ring * 2;

                for (var i = 0; i < ringPositionCount; i++)
                {
                    // divide a circle by the number of positions to get an angle to rotate with
                    var angle = i * (math.PI2 / ringPositionCount);

                    // ringVector is ring size * which ring we're on, rotated by the angle
                    var ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));

                    // add the computed vector to the target position
                    var ringPosition = targetPosition + ringVector;

                    // update the return array
                    positionArray[positionIndex] = ringPosition;
                    positionIndex++;

                    // exit on max positions filled
                    if (positionIndex >= positionCount)
                    {
                        break;
                    }
                }

                ring++;
            }

            return positionArray;
        }
    }
}
