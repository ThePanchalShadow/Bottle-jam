using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game_Play
{
    public class CanBox : MonoBehaviour
    {
        private CanBoxRamp assignedRamp;
        public ColorType color;
        private BoxHolder holderItIsAssignedTo;
        [SerializeField] private Transform[] canPositions;
        public GameObject capOfCanBox;

        private bool removed;
        public readonly List<Can> CanCollected = new();
        private readonly List<Task> taskList = new();
        private int canCountForAnimation;
        
        [Header("BoxHolder Jump Animation Settings")]
        [SerializeField] private float boxJumpHeight = 1f;          // Jump height for BoxHolder animations
        [SerializeField] private float boxJumpDuration = 0.5f;      // Duration of BoxHolder jump animations
        [SerializeField] private Ease boxJumpEase = Ease.OutSine; // Ease function for BoxHolder jump animations
        [SerializeField] private float moveForwardDuration = 2f;
        public bool removeAnimationFinished;

        public void AssignRamp(CanBoxRamp canBoxRamp)
        {
            assignedRamp = canBoxRamp;
            removeAnimationFinished = false;
        }
        private void OnMouseDown()
        {
            if (assignedRamp.currentAvailableBoxCan != this) return;
            if (holderItIsAssignedTo != null) return;

            if (!GameManager.Instance.AssignBoxToHolder(this))
            {
                // _ = GameManager.Instance.CheckAllRampColor();
                return;
            }

            capOfCanBox.SetActive(CanCollected.Count >= 4);
            assignedRamp.UpdatePositionAndRemove(this);
        }

        internal void JumpToBoxHolder(Vector3 targetPosition, BoxHolder holder)
        {
            transform.DOJump(targetPosition, boxJumpHeight, 1, boxJumpDuration)
                .SetEase(boxJumpEase).OnComplete(() =>
                    {
                        holder.isAssigning = false;
                        holder.BoxHolderState = BoxHolderState.HoldingBox;
                        _ = GameManager.Instance.CheckAllRampColor();
                    }
                );
        }
        
        internal void MoveForward(Vector3 targetPosition)
        {
            transform.DOMove(targetPosition, moveForwardDuration).SetEase(Ease.InOutQuad);
        }
        
        private void AssignCan(Can can)
        {
            var targetPosition = can.transform.parent.position;
            var canAnimation = can.CanAnimation(can, targetPosition);
            
            taskList.Add(canAnimation.AsyncWaitForCompletion());

            // Add the callback to handle when the animation finishes
            canAnimation.OnKill(() =>
            {
                // If the box is full, remove this box
                Log($"Animation of {can} and canCount {canCountForAnimation} canCollected Count {CanCollected.Count} finished.");
                 if (canPositions.Length < canCountForAnimation)
                 {
                    // RemoveThisBox();
                 }
                 else
                 {
                     canCountForAnimation++;
                     if (canCountForAnimation >= canPositions.Length)
                     {
                         RemoveThisBox();
                     }
                 }
                
                // if (canCountForAnimation >= canPositions.Length)
                // {
                //     RemoveThisBox();
                // }
                // else
                // {
                //     canCountForAnimation++;
                // }
            });
        }

        internal void AssignCanDataOnly(Can can)
        {
            if (CanCollected.Count >= canPositions.Length)
            {
                LogWarning("Cannot assign more cans to this box.");
                return;
            } 
            // Log($"Assigning {can} from {this}");
            can.assigned = true;
            CanCollected.Add(can);
            can.transform.SetParent(canPositions[CanCollected.IndexOf(can)]);
            can.assignedRamp.RemoveCan(can);
            AssignCan(can);
        }

        private void RemoveThisBox()
        {
            if (removed) return;
            removed = true;
            // After removing, ensure to clean up and trigger the animation completion check
            _ = CompleteAllAnimationsAndRemoveBox();
            holderItIsAssignedTo.DetachCanBox();
        }

        private async Task CompleteAllAnimationsAndRemoveBox()
        {
            // Wait until all can animations are completed
            await Task.WhenAll(taskList);  // Wait for all can (animations) to complete
            holderItIsAssignedTo.isDetachingBox = false;
            holderItIsAssignedTo.BoxHolderState = BoxHolderState.Nothing;
            GameManager.Instance.spawnedCanBoxes.Remove(this);
            
            // All animations are complete, now do the next steps
            if (capOfCanBox) capOfCanBox.SetActive(true);

            // Move the box
            if (transform)
                transform.DOMoveY(transform.position.y + 3, 0.3f).OnComplete(() =>
                {
                    var moveDirection = Random.Range(0, 2) == 0 ? -1f : 1f;
                    const float offScreenDistance = 10f;
                    transform.DOMoveX(transform.position.x + moveDirection * offScreenDistance, 1f)
                        .OnComplete(() =>
                        {
                            Log($"<color=red>DoMoveX {gameObject.name}</color>");
                            removeAnimationFinished = true;
                            GameManager.Instance.CheckWinCondition();
                            Destroy(gameObject);
                        });
                });
        }

        public void AssignTheHolder(BoxHolder holder)
        {
            holderItIsAssignedTo = holder;
        }
        private void Log(object message)
        {
            Debug.Log($"<b><color=yellow>{gameObject.name}</color></b> {message}", gameObject);
        }
        private void LogWarning(object message)
        {
            Debug.LogWarning($"<b><color=yellow>{gameObject.name}</color></b> {message}", gameObject);
        }
    }

}
