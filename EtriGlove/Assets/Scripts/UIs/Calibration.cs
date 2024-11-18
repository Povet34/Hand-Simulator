using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibration : MonoBehaviour
{
    static Calibration instance;

    public static Calibration Instance
    {
        get
        {
            if(instance == null)
                instance = new Calibration();

            return instance;
        }
    }


    GloveCommunicator gloveCommunicator;

    private void Awake()
    {
        gloveCommunicator = FindObjectOfType<GloveCommunicator>();
    }
}
