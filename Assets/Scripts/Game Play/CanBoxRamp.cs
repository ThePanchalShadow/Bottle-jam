using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Game_Play
{
    public class CanBoxRamp : MonoBehaviour
    {
        public List<CanBox> assignedBoxCans = new();
        public Vector3 firstSpawnPosition;
        public CanBox currentAvailableBoxCan;

        public void FirstPositionAssign()
        {
            firstSpawnPosition = transform.position;
            for (var i = 0; i < assignedBoxCans.Count; i++)
            {
                assignedBoxCans[i].transform.position = new Vector3(firstSpawnPosition.x,
                    firstSpawnPosition.y,
                    firstSpawnPosition.z - i * 2 - assignedBoxCans.Count - 10);
            }
            AssignPositions();
        }

        private void AssignPositions()
        {
            for (var i = 0; i < assignedBoxCans.Count; i++)
            {
                var targetPosition = new Vector3(firstSpawnPosition.x, firstSpawnPosition.y, firstSpawnPosition.z - i * 2);
                assignedBoxCans[i].MoveForward(targetPosition);
                assignedBoxCans[i].AssignRamp(this);
            }

            currentAvailableBoxCan = assignedBoxCans.Count > 0 ? assignedBoxCans[0] : null;

            if (!currentAvailableBoxCan) return;
            currentAvailableBoxCan.transform.DOKill();
            currentAvailableBoxCan.transform.DOLocalMove(Vector3.forward, 0.2f);
            currentAvailableBoxCan.capOfCanBox.SetActive(false);
        }
        public void UpdatePositionAndRemove(CanBox box)
        {
            if (assignedBoxCans.Count == 0 || box == null)
            {
                Debug.LogWarning("No box cans to remove or the box is null.", gameObject);
                return;
            }

            assignedBoxCans.Remove(box);
            AssignPositions();
        }
    }
}