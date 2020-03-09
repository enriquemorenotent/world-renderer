using UnityEngine;
using UnityEngine.Events;

namespace Extinction.Utils
{
    public class DistanceDetector : MonoBehaviour
    {
        private bool tooFar;
        [SerializeField] GameObject target;

        [SerializeField]
        [Range(10, 100)]
        public float tooFarLimit = 50;

        public UnityEvent onEscape;
        public UnityEvent onReturn;

        void Update()
        {
            if (!target) return;

            if (tooFar && !IsTargetTooFar())
            {
                tooFar = false;
                onReturn.Invoke();
            }

            if (!tooFar && IsTargetTooFar())
            {
                tooFar = true;
                onEscape.Invoke();
            }
        }

        float GetDistance()
        {
            Vector3 position = target.transform.position;
            position.y = 0;
            return Vector3.Distance(transform.position, position);
        }

        public void Reset() => tooFar = false;

        public void UpdateTarget(GameObject newTarget) => target = newTarget;

        public bool IsTargetTooFar() => GetDistance() > tooFarLimit;

        public Vector3 TargetPosition() => target.transform.position;

    }
}