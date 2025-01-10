using System.Collections.Generic;   
using UnityEngine;

namespace Game_Play
{
    public class CanRamp : MonoBehaviour
    {
        private const float SpaceBetweenCans = 1f;
        public List<Can> assignedCans = new();
        private Vector3 firstSpawnPosition;
        public Vector3 offset;
        public bool IsMovingForward { get; set; }
        //private Coroutine colorCheckCoroutine;


        public void FirstPositionsAssign()
        {
            firstSpawnPosition = transform.position + offset;
            for (var i = 0; i < assignedCans.Count; i++)
            {
                assignedCans[i].transform.position = new Vector3(firstSpawnPosition.x,
                    firstSpawnPosition.y,
                    firstSpawnPosition.z + i + assignedCans.Count);
            }
            AssignPositions();
        }

        private void AssignPositions()
        {
            IsMovingForward = true;
            for (var i = 0; i < assignedCans.Count; i++)
            {
                Vector3 endValue = new(
                    firstSpawnPosition.x,
                    firstSpawnPosition.y,
                    firstSpawnPosition.z + (SpaceBetweenCans * i)
                );
                assignedCans[i].MoveForward(endValue, this, i == 0, i == assignedCans.Count - 1);
            }
        }

        internal Can GetFirstCan()
        {
            return assignedCans[0].isFirstCan ? assignedCans[0] : null;
        }

        public void RemoveCan(Can can)
        {
            if (can == null)
            {
                Debug.LogWarning("The provided can is null.");
                return;
            }

            assignedCans.Remove(can);
//            Debug.Log("Can be removed from assigned cans.");
        }

        public void UpdatePositions()
        {
            if (assignedCans.Count == 0)
            {
                Debug.LogWarning($"No cans to update positions for. {this}", this);
                return;
            }

            AssignPositions();
 //           Debug.Log("Positions updated for remaining cans.");
        }
    }
}