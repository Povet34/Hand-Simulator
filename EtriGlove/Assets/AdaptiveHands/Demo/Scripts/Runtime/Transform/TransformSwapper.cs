using UnityEngine;

namespace AdaptiveHands.Demo
{
    /// <summary>
    /// A simple component that moves between transform states every set number of seconds.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class TransformSwapper : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The Transform state is changed every stateChangeDelay seconds.")]
        public float stateChangeDelay = 5f;
        [Tooltip("An array of valid Transform references for the states.")]
        public Transform[] states;

        /// <summary>The current state.</summary>
        public int CurrentState { get; private set; } = 0;
        /// <summary>The next Time.time a state change will occur.</summary>
        public float NextStateChange { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Next state change is stateChangeDelay seconds from now.
            NextStateChange = Time.time + stateChangeDelay;
        }

        void Update()
        {
            // Check if it is time to change states.
            if (Time.time >= NextStateChange)
            {
                // State change!
                // Move to next state.
                ++CurrentState;
                if (CurrentState >= states.Length)
                    CurrentState = 0;

                // Move transform to current state.
                transform.SetPositionAndRotation(states[CurrentState].position, states[CurrentState].rotation);

                // Next state change is stateChangeDelay seconds from now.
                NextStateChange = Time.time + stateChangeDelay;
            }
        }
    }
}
