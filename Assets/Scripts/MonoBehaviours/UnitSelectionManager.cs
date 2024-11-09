using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.EventSystems;

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

                // get all selected entities to reset their selected status to false
                var entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

                var entities = entityQuery.ToEntityArray(Allocator.Temp);
                foreach (var entity in entities)
                {
                    entityManager.SetComponentEnabled<Selected>(entity, false);
                }

                // Looks for units that are selectable
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);

                entities = entityQuery.ToEntityArray(Allocator.Temp);
                var localTransforms = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

                // cache selectionAreaRect
                var selectionAreaRect = GetSelectionAreaRect();

                for (var i = 0; i < localTransforms.Length; i++)
                {
                    var unitLocalTransform = localTransforms[i];
                    var unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition))
                    {
                        entityManager.SetComponentEnabled<Selected>(entities[i], true);
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
                for (var i = 0; i < unitMovers.Length; i++)
                {
                    var unitMover = unitMovers[i];
                    unitMover.TargetPosition = mouseWorldPosition;
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
    }
}
