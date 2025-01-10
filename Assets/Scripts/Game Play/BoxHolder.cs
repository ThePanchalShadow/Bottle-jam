using UnityEngine;
using UnityEngine.Serialization;

namespace Game_Play
{
    public class BoxHolder : MonoBehaviour
    {
        public CanBox HeldCanBox { get; private set; }

        [FormerlySerializedAs("isAssigningAnimating")] [FormerlySerializedAs("isAnimating")] 
        public bool isAssigning = false;
        public bool isDetachingBox = false;
        private BoxHolderState boxHolderState = BoxHolderState.Nothing;

        public BoxHolderState BoxHolderState
        {
            get => boxHolderState;
            set
            {
                Log($"boxHolderState changed from {boxHolderState} to {value}");
                boxHolderState = value;
            }
        }
        public void AttachCanBox(CanBox canBox)
        {
            if (HeldCanBox)
            {
                LogWarning("BoxHolder is already occupied.");
                return;
            }

            HeldCanBox = canBox;

            // Set the target position for the jump, and adjust the y-axis to a fixed value
            var targetPosition = transform.position;
            BoxHolderState = BoxHolderState.AssigningBox;
            isAssigning = true;
            HeldCanBox.JumpToBoxHolder(targetPosition, this);
        }

        public void DetachCanBox()
        {
            if (!HeldCanBox) return;
            HeldCanBox = null;
            BoxHolderState = BoxHolderState.DetachingBox;
            isDetachingBox = true;
        }

        public bool IsOccupied()
        {
            return HeldCanBox;
        }
        private void Log(object message)
        {
//            Debug.Log($"<b><color=yellow>{gameObject.name}</color></b> {message}", gameObject);
        }
        private void LogWarning(object message)
        { 
//            Debug.LogWarning($"<b><color=yellow>{gameObject.name}</color></b> {message}", gameObject);
        }
    }
    public enum BoxHolderState
    {
        Nothing,
        DetachingBox,
        AssigningBox,
        HoldingBox
    }
}
