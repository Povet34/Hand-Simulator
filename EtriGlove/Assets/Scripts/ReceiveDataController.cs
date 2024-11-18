using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveDataController : MonoBehaviour
{
    public List<ReceiveDataColumn> dataColumns = new List<ReceiveDataColumn>();

    public void Init()
    {

    }

    public void ReceiveData(List<Vector3> angles)
    {
        if (dataColumns.Count > 0 && angles.Count > 0)
        {
            for (int i = 0; i < dataColumns.Count; i++)
            {
                dataColumns[i].UpdateData(angles[i]);
            }
        }
    }
}
