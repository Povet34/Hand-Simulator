using UnityEngine;
using AdaptiveHands.Poser;

namespace AdaptiveHands.Demo
{
    /// <summary>
    /// A simple script that sets a hand's pose when a certain distance from some target Transform has been reached.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PoseUnderDistance : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The HandPoser that will be controlled by this component.")]
        public HandPoser handPoser;
        [Tooltip("The target Transform the hands distance is checked against.")]
        public Transform target;
        [Tooltip("The distance at or under which the hand will enter a pose.")]
        public float poseDistance = 0.1f;
        [Tooltip("The case-sensitive name of the pose to load when hnadPoser.Hand is within poseDistance units of target.")]
        public string poseName;

        /// <summary>Returns true if the hand's fingers are currently set to bent by this component, otherwise false.</summary>
        public bool IsPosedByComponent { get; private set; }

        // Unity callback(s).
        void Update()
        {
            // Ensure there is a valid 'handPoser', a valid 'handPoser.Hand', and a valid 'target' reference.
            if (handPoser != null && handPoser.Hand != null && target != null)
            {
                // Check if the distance between 'hand' and 'target' is <= poseDistance.
                if (Vector3.Distance(handPoser.Hand.transform.position, target.position) <= poseDistance)
                {
                    // Enter the set pose if it exists and the handPoser is not already in that pose.
                    int poseIndex = handPoser.GetPoseIndexByName(poseName);
                    if (handPoser.CurrentPoseIndex != poseIndex)
                    {
                        if (poseIndex != -1)
                        {
                            handPoser.SetPoseByIndex(poseIndex);
                        }
                        else { Debug.LogWarning("PoseUnderDistance attempted to load invalid pose '" + poseName + "'!", handPoser.gameObject); }
                    }

                    // Bent by component.
                    IsPosedByComponent = true;
                }
                // Otherwiase check if the fingers need to be un-posed.
                else if (IsPosedByComponent)
                {
                    // Clear the pose.
                    handPoser.ClearPose();

                    // Not bent by component.
                    IsPosedByComponent = false;
                }
            }
        }
    }
}
