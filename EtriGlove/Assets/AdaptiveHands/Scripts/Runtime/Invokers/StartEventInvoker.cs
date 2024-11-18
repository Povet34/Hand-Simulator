using UnityEngine;
using UnityEngine.Events;

namespace AdaptiveHands.Invokers
{
    /// <summary>
    /// A component that is triggered immediately at Start().
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class StartEventInvoker : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("An event that is invoked when Start() is called.")]
        public UnityEvent Triggered;

        // Unity callback(s).
        void Start()
        {
			Trigger();
        }

        // Public method(s).
        /// <summary>Invokes the 'Triggered' event of this component.</summary>
        public void Trigger()
        {
			// Invoke the 'Triggered' unity event.
			Triggered?.Invoke();    
        }
    }
}
