using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MonoBehaviours
{
    public class UnitSelectionManager : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                var mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<UnitMover, Selected>()
                    .Build(entityManager);

                var unitMoverArr = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
                for (var i = 0; i < unitMoverArr.Length; i++)
                {
                    var unitMover = unitMoverArr[i];
                    unitMover.TargetPosition = mouseWorldPosition;
                    unitMoverArr[i] = unitMover;
                }

                entityQuery.CopyFromComponentDataArray(unitMoverArr);
            }
        }
    }
}
