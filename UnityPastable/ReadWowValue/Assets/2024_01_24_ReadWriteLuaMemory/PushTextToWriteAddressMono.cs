using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushTextToWriteAddressMono : MonoBehaviour
{
    public string m_textToPush;


    public UnityEventText m_onTextToPush;

    [System.Serializable]
    public class UnityEventText : UnityEvent<string> { }

    public void SetText(string text) { m_textToPush = text; }
    public void PushText() {
        m_onTextToPush.Invoke(m_textToPush);
    }
}
