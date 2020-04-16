using UnityEngine;
using UnityEngine.Events;

namespace Extinction.Utils
{
    public class DistanceDetector : MonoBehaviour
    {
        [SerializeField] GameObject target;

        [SerializeField]
        [Range(10, 100)]
        public float tooFarLimit = 50;

        public UnityEvent onEscape;
        public UnityEvent onReturn;

        void Update()
        {
            if (!target) return;

            CheckEscape();
        }

        void CheckEscape()
        {
            if (!IsTargetTooFar()) return;
            transform.position = TargetPosition();
            onEscape.Invoke();
        }

        float DistanceToTarget() => Vector3.Distance(transform.position, TargetPosition());

        public void UpdateTarget(GameObject newTarget) => target = newTarget;

        public bool IsTargetTooFar() => DistanceToTarget() > tooFarLimit;

        public Vector3 TargetPosition() => target.transform.position;

    }
}