using UnityEngine;

namespace PinnedTransforms.Demo
{
    /// <summary>
    /// A component that provides public methods for rotating a Transform on each axis at a given rate.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class TransformRotator : MonoBehaviour
    {
        [Header("Settings - Rotation")]
        [Tooltip("The rate of rotation around any axis in degrees per second. (degs/sec)")]
        public float rotationRate = 120f;
        [Tooltip("The rotation axis to use.")]
        public Vector3 rotationAxis = Vector3.right;

        // Unity callback(s).
        void Update()
        {
            // Rotate around the rotation axis at rotationRate degrees per second.
            transform.Rotate(rotationAxis, rotationRate * Time.deltaTime);
        }
    }
}
