using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReceiveDataColumn : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI indexText;
    [SerializeField] TextMeshProUGUI rangeText;
    [SerializeField] Scrollbar rangeBar;

    [SerializeField] Vector3 TestInput;
    [SerializeField] bool isTest;

    float min = 0;
    float max = 360;

    private void Update()
    {
        if (isTest)
        {
            UpdateData(TestInput);
        }
    }

    public void Init(int index, float min, float max)
    {
        this.min = min;
        this.max = max;

        indexText.text = index.ToString();
    }

    public void UpdateData(Vector3 value)
    {
        var normal = Normalize(value.magnitude / 3);

        rangeText.text =
            $"{value.x.ToString("N2")}\n {value.y.ToString("N2")}\n {value.z.ToString("N2")}";

        rangeBar.size = normal;
    }

    private float Normalize(float value)
    {
        return Mathf.InverseLerp(min, max, value);
    }
}
