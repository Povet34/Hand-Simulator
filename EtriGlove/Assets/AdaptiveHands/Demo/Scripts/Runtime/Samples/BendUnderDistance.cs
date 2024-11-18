using UnityEngine;

namespace AdaptiveHands.Demo
{
    /// <summary>
    /// A simple script that sets all fingers' bend values on the hand when a certain distance from some target Transform has been reached.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class BendUnderDistance : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The KinematicHand whose finger bend will be controlled by this component.")]
        public KinematicHand hand;
        [Tooltip("The target Transform the hands distance is checked against.")]
        public Transform target;
        [Tooltip("The distance at or under which the hands fingers will bend.")]
        public float bendDistance = 0.1f;
        [Range(0f, 1f)]
        [Tooltip("The finger bend factor to target when bent by the component.")]
        public float bendTarget = 1f;

        /// <summary>Returns true if the hand's fingers are currently set to bent by this component, otherwise false.</summary>
        public bool IsBentByComponent { get; private set; }

        // Unity callback(s).
        void Update()
        {
            // Ensure there is a valid 'hand' and 'target' reference.
            if (hand != null && target != null)
            {
                // Check if the distance between 'hand' and 'target' is <= bendDistance.
                if (Vector3.Distance(hand.transform.position, target.position) <= bendDistance)
                {
                    // Bend the fingers fully.
                    hand.SetAllFingerBend(bendTarget);

                    // Bent by component.
                    IsBentByComponent = true;
                }
                // Otherwise check if the fingers need to be unbent.
                else if (IsBentByComponent)
                {
                    // Unbend the fingers.
                    hand.SetAllFingerBend(0f);

                    // Not bent by component.
                    IsBentByComponent = false;
                }
            }
        }
    }
}
