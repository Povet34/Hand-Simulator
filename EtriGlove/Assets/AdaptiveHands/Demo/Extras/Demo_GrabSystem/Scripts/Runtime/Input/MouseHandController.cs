using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using GrabSystem;

namespace AdaptiveHands.Demo
{
    /// <summary>
    /// A controller that uses the mouse to control a KinematicHand.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    [RequireComponent(typeof(RayGrabber))]
	[RequireComponent(typeof(KinematicHand))]
    public class MouseHandController : MonoBehaviour
    {
        [Header("Settings")]
		[Tooltip("A reference to the Camera to use when raycasting.")]
		[SerializeField] private Camera m_CameraOverride;
        [Tooltip("A layer mask of layers to be ignored when looking for layers raycasting from the mouse pointer.")]
        public LayerMask ignorePointerRaycastLayers;
		[Tooltip("The maximium distance the cursor-to-world raycasts can travel.")]
		public float maxClickDistance = 100f;
		
		[Header("Settings - Hand")]
		[Tooltip("A reference to the Transform that represents the center of the palm of the hand. This is used to position and rotate the hand. This must be a child-object of the 'Hand' reference.")]
		public Transform handPalmTransform;
        [Tooltip("The speed the hand moves in units per second while moving while holding an object. (units/s)")]
        public float handMoveSpeed = 0.5f;
        [Tooltip("The speed the hand moves in units per second while moving towards a position target. (units/s)")]
        public float handSmoothing = 6f;
#if ENABLE_INPUT_SYSTEM
        [Tooltip("A reference to (or settings about) the input action property that handles grabbing.")]
        public InputActionProperty grabAction;
#elif ENABLE_LEGACY_INPUT_MANAGER
        [Tooltip("The KeyCode that is used as the grab input.")]
        public KeyCode grabInput = KeyCode.Mouse0;
#endif

        [Header("Hand Scroll Settings")]
        [Tooltip("The scroll speed to move the hand up and down at.")]
        public float scrollSpeed = 50f;
        [Tooltip("The minimum y axis position for the hand.")]
        public float minScrollY = 0.045f;
        [Tooltip("The maximum y axis position for the hand.")]
        public float maxScrollY = 1f;
#if ENABLE_INPUT_SYSTEM
        [Tooltip("A reference to (or settings about) the input action property that handles moving the hand up and down on the Vector3.up axis.")]
        public InputActionProperty scrollAction;
#endif

        [Header("Events")]
        [Tooltip("An event that is invoked when this component is enabled.")]
        public UnityEvent Enabled;
        [Tooltip("An event that is invoked when this component is disabled.")]
        public UnityEvent Disabled;
		
		/// <summary>A reference to the Camera that is used in raycasting done by this component.</summary>
		public Camera CameraReference { get { return m_CameraOverride != null ? m_CameraOverride : Camera.main; } }
        /// <summary>A reference to the RayGrabber that is being driven by this component.</summary>
        public RayGrabber Grabber { get; private set; }
		/// <summary>A reference to the KinematicHand that is being driven by this component.</summary>
		public KinematicHand Hand { get; private set; }
        /// <summary>An offset to apply to the hand target position.</summary>
        public Vector3 HandTargetOffset { get; private set; }
        /// <summary>The Vector3 Hand.transform.position is targetting.</summary>
        public Vector3 TargetHandPosition { get; private set; }
        /// <summary>Returns the current mouse position.</summary>
        public Vector2 MousePosition
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector3.zero;
#elif ENABLE_LEGACY_INPUT_MANAGER
                return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#endif
            }
        }
        /// <summary>Returns the position of the mouse last frame.</summary>
        public Vector2 LastMousePosition { get; private set; }
        /// <summary>Returns a stored value for MousePosition - LastMousePosition each time before LastMousePosition is about to be updated.</summary>
        public Vector2 DeltaMousePosition { get; private set; }

        // Unity callback(s).
        void Awake()
        {
            // Find KinematicHand reference.
			Hand = GetComponent<KinematicHand>();

            // Find RayGrabber reference.
            Grabber = GetComponent<RayGrabber>();

#if ENABLE_INPUT_SYSTEM
            // On the new input system ensure 'grabAction' is set.
            if (grabAction == null || grabAction.action == null)
                Debug.LogWarning("No 'grabAction' set for MouseHandConroller component! This must be manually set when using the new input system, no default value is provided.", gameObject);
#endif
        }

        void OnEnable()
        {
#if ENABLE_INPUT_SYSTEM
			// Bind grab action.
            if (grabAction != null && grabAction.action != null)
                grabAction.action.Enable();

            // Bind scroll action.
            if (scrollAction != null && scrollAction.action != null)
            {
                scrollAction.action.Enable();
                scrollAction.action.performed += OnScroll;
            }
#endif

            // Invoke the 'Enabled' unity event.
            Enabled?.Invoke();
        }

        void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            // Unbind scroll action.
            if (scrollAction != null && scrollAction.action != null)
                scrollAction.action.performed -= OnScroll;
#endif

            // Invoke the 'Disabled' unity event.
            Disabled?.Invoke();
        }

        void Update()
        {
            // Look for grab input and something to grab if nothing is being grabbed currently...
            if (Grabber.Grabbing == null)
            {
                // Check if something was hit in a non-ignored layer.
                Ray ray = CameraReference.ScreenPointToRay(MousePosition);
                bool rayHit = Physics.Raycast(ray, out RaycastHit hitInfo, maxClickDistance, ~ignorePointerRaycastLayers, QueryTriggerInteraction.Ignore);
                if (rayHit)
                {
                    // Get the offset of handPalmTransform from Hand.transform by first converting handPalmTransform.position to the Hand.transforms local space.
                    Vector3 handSpaceOffset;
                    if (handPalmTransform != null)
                    {
                        handSpaceOffset = Hand.transform.InverseTransformPoint(handPalmTransform.position);
                    }
                    else { handSpaceOffset = Vector3.zero; }

                    // Set the target hand position.
                    TargetHandPosition = hitInfo.point - Hand.transform.TransformVector(handSpaceOffset);
                }

                // Check if grab input is pressed, if it is try a grab and bend the fingers fully.
                if (IsGrabInputDown())
                {
                    // Attempt a grab.
                    GrabbableObject grabbed = Grabber.GrabByPalmTrace();
                    if (grabbed == null)
                    {
                        // Bend all the fingers fully while attempting a grab.
                        Hand.SetAllFingerBend(1f);
                    }
                }
                else { Hand.SetAllFingerBend(0f); } // No finger bend.
            }
            // Otherwise check if the grab key was released and handle a release.
            else
            {
                // Check if the object has been released.
                if (!IsGrabInputDown())
                {
                    // Release the grabbed object and unbend all fingers.
                    Grabber.Release();
                    Hand.SetAllFingerBend(0f);

                    // Zero hand offset.
                    HandTargetOffset = Vector3.zero;
                }
                // Otherwise the object has not been released.
                else
                {
                    // Handle hand movement.
                    Quaternion cameraRotation = Quaternion.Euler(0, CameraReference.transform.rotation.eulerAngles.y, 0);
                    Vector3 movement = cameraRotation * new Vector3(DeltaMousePosition.x, 0, DeltaMousePosition.y);
                    TargetHandPosition += handMoveSpeed * Time.deltaTime * movement;
                }
            }

#if !ENABLE_INPUT_SYSTEM && ENABLE_LEGACY_INPUT_MANAGER
            // Legacy scroll input handling.
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseScroll != 0)
                ScrollHand(mouseScroll);
#endif

            // Move the hand towards the target position.
            Hand.transform.position = Vector3.MoveTowards(Hand.transform.position, TargetHandPosition + HandTargetOffset, handSmoothing * Time.deltaTime);

            // Update last mouse position after calculating delta.
            DeltaMousePosition = MousePosition - LastMousePosition;
            LastMousePosition = MousePosition;
        }

        // Public method(s).
        /// <summary>
        /// Scrolls the Hand.transforms position up or down along the Vector3.up axis at scrollUnits * pUnits speed.
        /// </summary>
        /// <param name="pUnits">-1f <= pUnits <= 1f</param>
        public void ScrollHand(float pUnits)
        {
            // Determine scroll direction.
            float scrollAmount = pUnits * (scrollSpeed * Time.deltaTime);
            if (pUnits >= 0)
            {
                // Don't scroll up if too high
                if ((TargetHandPosition.y + HandTargetOffset.y) + scrollAmount <= maxScrollY)
                {
                    // Move the Hand.transform up.
                    HandTargetOffset += Vector3.up * scrollAmount;
                }
                else { HandTargetOffset = new Vector3(HandTargetOffset.x, maxScrollY - TargetHandPosition.y, HandTargetOffset.z); }
            }
            else
            {
                // Don't scroll down if too low.
                if ((TargetHandPosition.y + HandTargetOffset.y) - scrollAmount >= minScrollY)
                {
                    // Move the Hand.transform towards the relative transform.
                    HandTargetOffset += Vector3.up * scrollAmount;
                }
                else { HandTargetOffset = new Vector3(HandTargetOffset.x, Mathf.Max(0f, minScrollY - TargetHandPosition.y), HandTargetOffset.z); }
            }
        }

        /// <summary>
        /// Returns true when the grab input is down, otherwise false.
        /// Implemented to work with both the new and legacy Unity input systems.
        /// </summary>
        /// <returns>true when the grab input is down, otherwise false.</returns>
        public bool IsGrabInputDown()
        {
#if ENABLE_INPUT_SYSTEM
            return grabAction != null && grabAction.action != null ? grabAction.action.ReadValue<float>() > 0 : false;
#elif ENABLE_LEGACY_INPUT_MANAGER
            return grabInput != KeyCode.None ? Input.GetKey(grabInput) : false;
#endif
        }

        #region New Input System - Exclusive
#if ENABLE_INPUT_SYSTEM
        /// <summary>An event that is invoked when the new input system scroll action is fired.</summary>
        /// <param name="pContext"></param>
        void OnScroll(InputAction.CallbackContext pContext)
        {
            // When the scroll input is performed (which should be always since we only subscribed to performed)...
            if (pContext.performed)
            {
                float mouseScroll = pContext.ReadValue<float>();
                if (mouseScroll != 0)
                    ScrollHand(mouseScroll);
            }
        }
#endif
        #endregion
    }
}
