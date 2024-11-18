using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerTip : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if(other.name == "Sphere")
        {
            Debug.Log(transform.name);
        }
    }
}
