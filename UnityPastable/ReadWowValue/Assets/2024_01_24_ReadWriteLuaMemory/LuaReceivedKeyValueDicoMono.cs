using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class LuaReceivedKeyValueDicoMono : MonoBehaviour
{


    public Dictionary<string, string> m_keyValueReceived = new Dictionary<string, string>();

    public bool m_useDebugger;
    [TextArea(0,30)]
    public string m_debugText;
    public UnityEventDebugText m_onUpdateDebugText;

    [System.Serializable]
    public class UnityEventDebugText : UnityEvent<string> { }

    public void PushIn(string key, string value) {

        if (!m_keyValueReceived.ContainsKey(key))
            m_keyValueReceived.Add(key, value);
        m_keyValueReceived[key] = value;

    }
    private string m_spliter=":";
    private string m_lineReturn="\n";
    void Update()
    {
        if (!m_useDebugger) return;
        StringBuilder sb = new StringBuilder();
        foreach (var key in m_keyValueReceived.Keys)
        {
            sb.Append(key);
            sb.Append(m_spliter);
            sb.Append(m_keyValueReceived[key]);
            sb.Append(m_lineReturn);

        }
        m_debugText = sb.ToString();
        m_onUpdateDebugText.Invoke(m_debugText);
    }
}
