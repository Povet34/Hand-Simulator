using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollsionSpawnUI : MonoBehaviour
{
    public GameObject sphereObject;

    public void ShowSphere(bool isOn)
    {
        sphereObject.SetActive(isOn);
    }
}
