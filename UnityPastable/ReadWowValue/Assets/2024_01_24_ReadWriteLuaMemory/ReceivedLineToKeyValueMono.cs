using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ReceivedLineToKeyValueMono : MonoBehaviour
{

    public UnityEventSimpleLine m_onLineReceived;
    public UnityEventKeyValue m_onKeyValueReceived;

    public char m_spliter = ':';
    [System.Serializable]
    public class UnityEventSimpleLine : UnityEvent<string> { }
    [System.Serializable]
    public class UnityEventKeyValue : UnityEvent<string, string> { }

    public void PushIn(string text) {

        string[] lineTokens = text.Split("\n");
        foreach (var line in lineTokens)
        {
            string[] keyValue = line.Split(m_spliter);
            if (keyValue.Length == 2)
                m_onKeyValueReceived.Invoke(keyValue[0].Trim(), keyValue[1].Trim());
            else m_onLineReceived.Invoke(line);
        }
    }
}
