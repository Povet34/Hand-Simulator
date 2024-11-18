using System;
using UnityEngine;

namespace AdaptiveHands.Demo
{
    /// <summary>
    /// A simple script that sets all fingers' bend every frame while enabled based on their distance from some object.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class BendDistanceRange : MonoBehaviour
    {
        // FloatMinMax.
        [Serializable]
        public struct FloatMinMax
        {
            /// <summary>The minimum value.</summary>
            public float minimum;
            /// <summary>The maximum value.</summary>
            public float maximum;
        }

        // BendDistanceRange.
        [Header("Settings")]
        [Tooltip("The KinematicHand whose finger bend will be controlled by this component.")]
        public KinematicHand hand;
        [Tooltip("The target Transform the hands distance is checked against.")]
        public Transform target;
        [Tooltip("When the hands distance from target is greater than bendDistance.maximum the bend for all fingers is set to 0. When the distance is less than or equal to bendDistance.minimum the bend value is set to an interpolated value between the range.")]
        public FloatMinMax bendDistance = new FloatMinMax() { minimum = 0.05f, maximum = 0.4f };

        // Unity callback(s).
        void Update()
        {
            // Ensure there is a valid 'hand' and 'target' reference.
            if (hand != null && target != null)
            {
                // Check if the distance between 'hand' and 'target' is <= bendDistance.
                float handDistance = Vector3.Distance(hand.transform.position, target.position);
                if (handDistance <= bendDistance.maximum)
                {
                    // Set the finger bend to some interpolated value.
                    hand.SetAllFingerBend(Mathf.Lerp(0f, 1f, 1f - ((handDistance - bendDistance.minimum) / (bendDistance.maximum - bendDistance.minimum))));
                }
                // Otherwise simply unbend the hand.
                else
                {
                    // Set all fingers bend values to 0.
                    hand.SetAllFingerBend(0f);
                }
            }
        }
    }
}
