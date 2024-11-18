using UnityEngine;

namespace AdaptiveHands.Demo
{
    /// <summary>
    /// A simple component that translates a component from one transform to another then back infinite times at a given speed.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PingPongTransforms : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The Transform the component's Transform starts at.")]
        public Transform start;
        [Tooltip("The Transform the component's Transform ends at.")]
        public Transform end;
        [Tooltip("The speed this component's Transform moves at in units per second. (units/sec)")]
        public float speed = 1f;

        /// <summary>Returns true if this component is moving towards 'end' Transform otherwise false if moving towards 'start' Transform.</summary>
        public bool MovingToEnd { get; private set; }

        // Update is called once per frame
        void Update()
        {
            // Determine target transform.
            Transform targetTransform = MovingToEnd ? end : start;

            // Move towards the target at a given speed.
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, speed * Time.deltaTime);

            // If the target position has been reached flip 'MovingToEnd'.
            if (Vector3.Distance(transform.position, targetTransform.position) < float.Epsilon)
                MovingToEnd = !MovingToEnd;
        }
    }
}
