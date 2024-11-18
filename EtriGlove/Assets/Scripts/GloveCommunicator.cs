using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System.Collections.Generic;
using System;

public class GloveCommunicator : MonoBehaviour
{
    private SerialPort serialPort;
    private byte[] buffer;
    private Coroutine handStuckCoroutine;
    private Coroutine handStuckStopCoroutine;

    [SerializeField] private Transform backHand;
    [SerializeField] private CustomIK customIK;

    public static bool isOnCom8 = false;

    byte[] curVibes = new byte[9] { 0xfa, 0x02, 0xff, 0xff, 0xff, 0xff, 0xff, 0x55, 0xa9 };

    void Start()
    {
        buffer = new byte[1024];

        try
        {
            serialPort = new SerialPort("COM6")
            {
                BaudRate = 115200,         // BaudRate 설정
                DataBits = 8,              // DataBits 설정
                StopBits = StopBits.One,   // StopBits 설정
                Parity = Parity.None       // Parity 설정
            };

            if (null == serialPort)
            {
                Debug.Log("serialPort is null");
            }
            else
            {
                serialPort.Open();
                StartCoroutine(ReadSerialData());

                isOnCom8 = false;
            }
        }
        catch
        {
            serialPort = new SerialPort("COM8")
            {
                BaudRate = 115200,         // BaudRate 설정
                DataBits = 8,              // DataBits 설정
                StopBits = StopBits.One,   // StopBits 설정
                Parity = Parity.None       // Parity 설정
            };

            if (null == serialPort)
            {
                Debug.Log("serialPort is null");
            }
            else
            {
                serialPort.Open();
                StartCoroutine(ReadSerialData());

                isOnCom8 = true;
            }
        }
    }

    public void StartHandStuck()
    {
        if (handStuckCoroutine != null)
        {
            StopCoroutine(handStuckCoroutine);
            handStuckCoroutine = null;
        }
        if (handStuckCoroutine == null)
        {
            handStuckCoroutine = StartCoroutine(SendHandStuck());
        }
    }
    public void StopHandStuck()
    {
        if (handStuckStopCoroutine != null)
        {
            StopCoroutine(handStuckStopCoroutine);
            handStuckStopCoroutine = null;
        }
        if (handStuckStopCoroutine == null)
        {
            handStuckStopCoroutine = StartCoroutine(SendStopHandStuck());
        }
    }

    public void ControlHandStuck(int fingerIndex, int power, int isOn)
    {
        byte[] stopHandStuck = new byte[] { 0xfa, 0x03, (byte)fingerIndex, (byte)isOn, (byte)power, 0x55, 0xa9 };
        serialPort.Write(stopHandStuck, 0, stopHandStuck.Length);
    }

    
    public void ControlVibe(int index, int isOn, int power, float time, Action offCallback = null)
    {
        switch (index)
        {
            case 0:
                StartOneZindong(isOn, power);
                break;
            case 1:
                StartTwoZindong(isOn, power);
                break;
            case 2:
                StartThreeZindong(isOn, power);
                break;
            case 3:
                StartFourZindong(isOn, power);
                break;
            case 4:
                StartFiveZindong(isOn, power);
                break;

        }

        if (time != 0)
        {
            StartCoroutine(OffVibeByTime(time, index, offCallback));
        }
    }

    private IEnumerator OffVibeByTime(float time, int index, Action offCallback)
    {
        yield return new WaitForSeconds(time);
        ControlVibe(index, 0, 0, 0);
        offCallback?.Invoke();
    }

    public void UpdateVibeData(int index, int power)
    {
        var vibe = GetVibePower(1, power);
        curVibes[index + 2] = vibe;
    }

    public void OnOffZindong(bool isOn)
    {
        byte[] vibes;
        if (isOn)
        {
            vibes = new byte[] { 
                0xfa, 0x02,
                curVibes[2],
                curVibes[3],
                curVibes[4],
                curVibes[5],
                curVibes[6], 
                0x55, 0xa9
            };
        }
        else
        {
            vibes = new byte[] { 0xfa, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xa9 };
        }

        serialPort.Write(vibes, 0, vibes.Length);
    }

    public void StartOneZindong(int isOn, int power)
    {
        int vibe = isOn == 1 ? power: 0x00;
        byte[] allZindong = new byte[] { 0xfa, 0x02, GetVibePower(isOn, power), 0x00, 0x00, 0x00, 0x00, 0x55, 0xa9 };
        serialPort.Write(allZindong, 0, allZindong.Length);
    }
    public void StartTwoZindong(int isOn, int power)
    {
        int vibe = isOn == 1 ? power : 0x00;
        byte[] allZindong = new byte[] { 0xfa, 0x02, 0x00, GetVibePower(isOn, power), 0x00, 0x00, 0x00, 0x55, 0xa9 };
        serialPort.Write(allZindong, 0, allZindong.Length);
    }
    public void StartThreeZindong(int isOn, int power)
    {
        int vibe = isOn == 1 ? power : 0x00;
        byte[] allZindong = new byte[] { 0xfa, 0x02, 0x00, 0x00, GetVibePower(isOn, power), 0x00, 0x00, 0x55, 0xa9 };
        serialPort.Write(allZindong, 0, allZindong.Length);
    }
    public void StartFourZindong(int isOn, int power)
    {
        int vibe = isOn == 1 ? power : 0x00;
        byte[] allZindong = new byte[] { 0xfa, 0x02, 0x00, 0x00, 0x00, GetVibePower(isOn, power), 0x00, 0x55, 0xa9 };
        serialPort.Write(allZindong, 0, allZindong.Length);
    }
    public void StartFiveZindong(int isOn, int power)
    {
        byte[] allZindong = new byte[] { 0xfa, 0x02, 0x00, 0x00, 0x00, 0x00, GetVibePower(isOn, power), 0x55, 0xa9 };
        serialPort.Write(allZindong, 0, allZindong.Length);
    }

    private byte GetVibePower(int isOn, int power)
    {
        int vibe = isOn == 1 ? power : 0x00;
        return (byte)vibe;
    }

    public void StopZindong()
    {
        byte[] stopZindong = new byte[] { 0xfa, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x55, 0xa9 };
        serialPort.Write(stopZindong, 0, stopZindong.Length);
    }
    public void PowerOff()
    {
        byte[] powerOff = new byte[] { 0xfa, 0x03, 0x05, 0x01, 0x00, 0x55, 0xa9 };
        serialPort.Write(powerOff, 0, powerOff.Length);
    }

    public void ContorlAllHandStuck(bool isOn)
    {
        if (isOn)
            StartCoroutine(SendHandStuck());
        else
            StartCoroutine(SendStopHandStuck());
    }

    private IEnumerator SendHandStuck()
    {
        for (int i = 0; i < 5; i++)
        {
            byte[] handStuck = new byte[] { 0xfa, 0x03, (byte)i, 0x01, 0xff, 0x55, 0xa9 };
            serialPort.Write(handStuck, 0, handStuck.Length);
            yield return new WaitForSeconds(0.02f);
        }
    }
    private IEnumerator SendStopHandStuck()
    {
        for (int i = 0; i < 5; i++)
        {
            StartHandStuckPerFinger(i);
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void StartHandStuckPerFinger(int i)
    {
        byte[] handStuck = new byte[] { 0xfa, 0x03, (byte)i, 0x00, 0xFF, 0x55, 0xa9 };
        serialPort.Write(handStuck, 0, handStuck.Length);
    }

    private IEnumerator ReadSerialData()
    {
        while (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                if (serialPort.BytesToRead > 0)
                {
                    int bytesRead = serialPort.Read(buffer, 0, buffer.Length);
                    byte[] receivedData = new byte[bytesRead];
                    Array.Copy(buffer, receivedData, bytesRead);

                    List<Vector3> angles = new List<Vector3>();
                    for (int i = 0; i < 7; i++)
                    {
                        short w, x, y, z;
                        int j = i * 9;
                        w = (short)(receivedData[j + 1] * 256 + receivedData[j + 2]);
                        x = (short)(receivedData[j + 3] * 256 + receivedData[j + 4]);
                        y = (short)(receivedData[j + 5] * 256 + receivedData[j + 6]);
                        z = (short)(receivedData[j + 7] * 256 + receivedData[j + 8]);

                        float fw = w / Definitions.QuaternionDivideValue;
                        float fx = x / Definitions.QuaternionDivideValue;
                        float fy = y / Definitions.QuaternionDivideValue;
                        float fz = z / Definitions.QuaternionDivideValue;

                        var q = new Quaternion(fx, -fy, fz, -fw).normalized;

                        if (i == 0)
                        {
                            backHand.rotation = q;
                        }
                        else if (i >= 1 && i <= 5)
                        {
                            customIK.SetFingerEndPosition_Euler(i - 1, q, backHand.rotation);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Read timeout or error: " + e.Message);
            }

            yield return null;
        }
    }
}