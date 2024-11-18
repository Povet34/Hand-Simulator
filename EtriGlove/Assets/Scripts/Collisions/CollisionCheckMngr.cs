using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FingerEndCollision;

public class CollisionCheckMngr : MonoBehaviour
{
    GloveCommunicator gloveCommunicator;
    ModeControllerUI modeControllerUI;

    [SerializeField] List<FingerEndCollision> fingerEndCollisions;

    [SerializeField] List<HapticData> vibeDatas;
    [SerializeField] List<HapticData> reverseDatas;
    
    [SerializeField] bool isShowMesh;

    private void Awake()
    {
        gloveCommunicator = FindObjectOfType<GloveCommunicator>();
        modeControllerUI = FindObjectOfType<ModeControllerUI>();

        for (int i = 0; i < fingerEndCollisions.Count; i++)
        {
            var finger = fingerEndCollisions[i];
            finger.InitReverse(reverseDatas[i], ControlHandStuck);
            finger.InitVibe(vibeDatas[i], ControlVibe, UpdateVibeValue);
        }

        modeControllerUI.Init(vibeDatas, reverseDatas);
    }

    private void ControlHandStuck(int index, int power, int isOn)
    {
        gloveCommunicator.ControlHandStuck(index, power, isOn);
    }

    private void ControlVibe(int index, int power, int isOn, float time, Action offCallback)
    {
        gloveCommunicator.ControlVibe(index, isOn, power, time, offCallback);
    }

    private void UpdateVibeValue(int index, int power)
    {
        gloveCommunicator.UpdateVibeData(index, power);
    }

    private void Update()
    {
        foreach(var finger in fingerEndCollisions)
        {
            finger.ShowMeshRenderer(isShowMesh);
        }
    }

    #region Vibe

    public void UpdateVibeTime(int index, float time)
    {
        fingerEndCollisions[index].UpdateVibeTime(time);
    }

    public void UpdateVibePower(int index, int power)
    {
        fingerEndCollisions[index].UpdateVibePower(power);
    }

    public void ActivateVibe(int index, bool isOn, Action offCallback)
    {
        fingerEndCollisions[index].ActivateVibe(isOn, offCallback);
    }

    #endregion

    #region Reverse

    public void UpdateReversePower(int index, int power)
    {
        fingerEndCollisions[index].UpdateReversePower(power);
    }

    public void UpdateReverseTime(int index, float time)
    {
        fingerEndCollisions[index].UpdateReverseTime(time);
    }

    public void ActivateReverse(int index, bool isOn)
    {
        fingerEndCollisions[index].ActivateReverse(isOn);
    }


    #endregion

    #region Index Change

    void ReorderByIndex(ref List<HapticData> origins, List<int> order)
    {
        if (origins.Count != order.Count)
        {
            throw new ArgumentException("The origins and order lists must have the same number of elements.");
        }

        List<HapticData> reordered = new List<HapticData>(new HapticData[origins.Count]);

        for (int i = 0; i < order.Count; i++)
        {
            reordered[order[i]] = origins[i];
        }

        origins = reordered;
    }

    #endregion
}
