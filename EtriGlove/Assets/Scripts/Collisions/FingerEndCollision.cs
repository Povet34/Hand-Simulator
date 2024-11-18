using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FingerEndCollision : MonoBehaviour
{
    [Serializable]
    public class HapticData
    {
        public string id;
        public int index;
        [Range(0, 255)] public int power;
        [Range(0, 1)]   public float time;

        public HapticData(string id, int index, int power, float time)
        {
            this.id = id;
            this.index = index;
            this.power = power;
            this.time = time;
        }
    }

    [SerializeField] InputField tactileForceInterval;

    HapticData reverseData;
    HapticData viberationData;

    Rigidbody rb;
    MeshRenderer meshRenderer;

    Action<int, int, int> onReverseStuck;
    Action<int, int, int, float, Action> onVibe;
    Action<int, int> onUpdateVibeValue;

    public void InitReverse(HapticData reverseData, Action<int, int, int> onReverseStuck)
    {
        this.reverseData = reverseData;
        this.onReverseStuck = onReverseStuck;
    }

    public void InitVibe(HapticData viberationData, Action<int, int, int, float, Action> onVibe, Action<int, int> onUpdateVibeValue)
    {
        this.viberationData = viberationData;
        this.onVibe = onVibe;
        this.onUpdateVibeValue = onUpdateVibeValue;
    }

    public void ShowMeshRenderer(bool isShow)
    {
        meshRenderer.enabled = isShow;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Definitions.COLLIDER_LAYER)
        {
            ActivateVibe(true, null);
            StartCoroutine(DelayActivateCollision(float.Parse(tactileForceInterval.text)));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Definitions.COLLIDER_LAYER)
        {
            ActivateReverse(false);
        }
    }

    #region Reverse

    public void UpdateReverseTime(float time)
    {
        reverseData.time = time;
    }

    public void UpdateReversePower(int power)
    {
        reverseData.power = power;
    }

    public void ActivateReverse(bool isOn)
    {
        int isOnInt = isOn ? Definitions.ON : Definitions.OFF;
        onReverseStuck?.Invoke(reverseData.index, reverseData.power, isOnInt);
    }

    #endregion

    #region Vibe

    public void UpdateVibeTime(float time)
    {
        viberationData.time = time;
    }

    public void UpdateVibePower(int power)
    {
        viberationData.power = power;
        onUpdateVibeValue?.Invoke(viberationData.index, power);
    }

    public void ActivateVibe(bool isOn, Action offCallback)
    {
        int isOnInt = isOn ? Definitions.ON : Definitions.OFF;

        onVibe?.Invoke(viberationData.index, viberationData.power, isOnInt, viberationData.time, offCallback);
    }

    #endregion


    private IEnumerator DelayActivateCollision(float time)
    {
        yield return new WaitForSeconds(time);
        ActivateReverse(true);
    }
}
