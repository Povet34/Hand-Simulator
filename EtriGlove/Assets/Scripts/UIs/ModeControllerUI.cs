using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FingerEndCollision;

public class ModeControllerUI : MonoBehaviour
{
    [SerializeField] List<HapticContol> viberations;
    [SerializeField] List<HapticContol> reverses;

    CollisionCheckMngr collisionCheckMngr;

    public void Init(List<HapticData> vibeDatas, List<HapticData> reverseDatas) 
    {
        collisionCheckMngr = FindObjectOfType<CollisionCheckMngr>();

        for (int i = 0; i < viberations.Count; i++)
        {
            var vibe = viberations[i];
            vibe.Init(vibeDatas[i], UpdateVibeTime, UpdateVibePower, ActivateVibe);
        }

        for (int i = 0;i < reverses.Count; i++)
        {
            var reverse = reverses[i];
            reverse.Init(reverseDatas[i], UpdateReverseTime, UpdateReversePower, ActivateReverse);
        }
    }

    #region Vibe

    private void UpdateVibeTime(int index, float value)
    {
        collisionCheckMngr.UpdateVibeTime(index, value);
    }

    private void UpdateVibePower(int index, int value)
    {
        collisionCheckMngr.UpdateVibePower(index, value);
    }

    private void ActivateVibe(int index, bool isOn, Action offCallback)
    {
        collisionCheckMngr.ActivateVibe(index, isOn, offCallback);
    }

    #endregion
    private void UpdateReverseTime(int index, float value)
    {
        collisionCheckMngr.UpdateReverseTime(index, value);
    }

    private void UpdateReversePower(int index, int value)
    {
        collisionCheckMngr.UpdateReversePower(index, value);
    }    

    private void ActivateReverse(int index, bool isOn, Action offCallback)
    {
        collisionCheckMngr.ActivateReverse(index, isOn);
    }
}
