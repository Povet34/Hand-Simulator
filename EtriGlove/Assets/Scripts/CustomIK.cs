using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomIK : MonoBehaviour
{
    [Serializable]
    public class FingerJoint
    {
        public string ID;
        public List<Transform> joints;

        [Range(0, 1)] public List<float> weights = new List<float>() { 0.4f, 0.7f, 0.9f };

        public void SetJointQuaternion(Quaternion q, Quaternion wristQ)
        {
            var first = joints[0];
            var second = joints[1];
            var third = joints[2];

            var flr = Quaternion.Lerp(wristQ, q, weights[0]);
            var slr = Quaternion.Lerp(flr, q, weights[1]);
            var tlr = Quaternion.Lerp(slr, q, weights[2]);

            first.rotation  = flr;
            second.rotation = slr;
            third.rotation  = tlr;
        }
    }

    [SerializeField] List<FingerJoint> fingerParts;

    public void SetFingerEndPosition_Euler(int index, Quaternion q, Quaternion wristQ)
    {
        fingerParts[index].SetJointQuaternion(q, wristQ);
    }
}
