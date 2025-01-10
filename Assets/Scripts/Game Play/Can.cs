using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game_Play
{
    public class Can : MonoBehaviour
    {
        public ColorType color;
        public CanRamp assignedRamp;
        public bool assigned;
        public bool isFirstCan;
        [Header("CanBox Jump Animation Settings")]
        [SerializeField] private float canJumpHeight = 1.5f;          // Jump height for CanBox animations
        [SerializeField] private float canJumpDuration = 0.5f;      // Duration of CanBox jump animations
        [SerializeField] private Ease canJumpEase = Ease.OutSine; // Ease function for CanBox jump animations
        [SerializeField] private float moveForwardAnimationDuration = 0.3f;
        // Scale settings for CanBox
        public float canScaleUp = 1.3f;  // Scale factor
        private void AssignToRamp(CanRamp ramp)
        {
            if (!ramp)
            {
                Debug.LogError("Cannot assign to a null ramp.");
                return;
            }
            assignedRamp = ramp;
        }

        internal bool CheckColorMatch(out CanBox canBox)
        {
            if (assigned)
            {
                Debug.Log($"Already assigned {this}", this);
                canBox = null;
                return true;
            }
           
            foreach (var box in from boxHolder in GameManager.Instance.spawnedBoxHolders
                     where boxHolder.IsOccupied() && boxHolder.BoxHolderState == BoxHolderState.HoldingBox/*!boxHolder.isAssigning*/
                     select boxHolder.HeldCanBox)
            {
//                Debug.Log($"found occupied {box}");
                
                if (box.color != color) continue;
//               Debug.Log($"Assigned {this} to {box}");
                box.AssignCanDataOnly(this);
                canBox = box;
                return true;
            }
            canBox = null;
            return false;
            
            // foreach (var boxHolder in GameManager.Instance.spawnedBoxHolders)
            // {
            //     if (boxHolder.IsOccupied() && !boxHolder.isAssigning)
            //     {
            //         var canBox = boxHolder.HeldCanBox;
            //         Debug.Log($"found occupied box {canBox}");
            //         if (canBox.color == color && canBox.AssignCan(this))
            //         {
            //             Debug.Log($"assigned can {canBox}");
            //             assignedRamp.RemoveCan(this);
            //             assigned = true;
            //             return true;
            //         }
            //     }
            //     //returning true even though can not be assigned because if we do not return true then game loop executes game over
            //     else return true;
            // }
        }

        internal Sequence CanAnimation(Can can, Vector3 targetPosition)
        {
            return AnimateCan(can.transform, targetPosition);
        }

        private Sequence AnimateCan(Transform canTransform, Vector3 targetPosition)
        {

            var initialScale = canTransform.localScale;

            var sequence = DOTween.Sequence();

            sequence.Append(canTransform.DOScaleY(canTransform.localScale.y * 0.7f, 0.2f).SetEase(Ease.InBack));
            sequence.Append(canTransform.DOScaleY(canTransform.localScale.y * 1.2f, 0.2f).SetEase(Ease.OutBack));
            sequence.AppendCallback(() => assignedRamp.UpdatePositions());
            sequence.Insert(0.3f, canTransform.DOJump(targetPosition, canJumpHeight, 1, canJumpDuration).SetEase(canJumpEase)
                .OnComplete(() =>
                {
                    canTransform.DOLocalJump(canTransform.localPosition, 0.2f, 1, 0.2f);
                    _ = GameManager.Instance.CheckAllRampColor();
                }));

            sequence.Join(canTransform.DOScale(initialScale * canScaleUp, canJumpDuration).SetEase(Ease.InOutBack));

            // Return the sequence so it can be further manipulated or chained externally
            return sequence;
        }
        internal void MoveForward(Vector3 endValue, CanRamp canRamp, bool checkColor, bool animationFinished)
        {
            transform.DOMove(endValue, moveForwardAnimationDuration).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                { 
                    assignedRamp.IsMovingForward = !animationFinished;
                    if (!checkColor) return;
                    isFirstCan = true;
//                    Debug.Log($"Moving forward {this} and checking color");
                    /*if (*/CheckColorMatch(out var canBox); /*)
                    {
                        // canBox.AssignCan(this);
                    }*/
                });
            AssignToRamp(canRamp);
        }
    }
}