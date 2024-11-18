using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FingerEndCollision;

public class HapticContol : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fingerName;
    [SerializeField] Slider timeSlider;
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] Slider powerSlider;
    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] Toggle activateToggle;

    HapticData data;
    Action<int, float> onTimeChange;
    Action<int, int> onPowerChange;
    Action<int, bool, Action> onActivate;

    public void Init(HapticData data, Action<int, float> onTimeChange, Action<int, int> onPowerChange, Action<int, bool, Action> onActivate)
    {
        fingerName.text = data.id;

        timeSlider.value = data.time;
        timeText.text = data.time.ToString("N2");

        powerSlider.value = data.power;
        powerText.text = data.power.ToString();

        this.data = data;
        this.onTimeChange = onTimeChange;
        this.onPowerChange = onPowerChange;
        this.onActivate = onActivate;
    }

    private void Awake()
    {
        timeSlider.onValueChanged.AddListener(UpdateTimeText);
        powerSlider.onValueChanged.AddListener(UpdatePowerText);
        activateToggle.onValueChanged.AddListener(ActivateHaptic);
    }

    private void UpdateTimeText(float value)
    {
        onTimeChange?.Invoke(data.index, value);
        timeText.text = value.ToString("N2");
    }

    private void UpdatePowerText(float value)
    {
        onPowerChange?.Invoke(data.index, (int)value);
        powerText.text = value.ToString();
    }

    private void ActivateHaptic(bool isOn)
    {
        onActivate?.Invoke(data.index, isOn, 
            ()=> 
            {
                activateToggle.isOn = false;
            });
    }
} 
