public class LuaAddonModeInformation {


    public void SetAddonState(double addonState)
    {
        SetAddonState(Math.Floor(addonState).ToString());
    }
    public void SetAddonState(string addonState)
    {
        m_rawValue = addonState.Trim();
        int arraylenght = m_rawValue.Length;
        int endStartIndex = m_rawValue.Length-1;
        m_rawValue = addonState;
        m_isAddonWindowOpen = m_rawValue.Length >= 2 && m_rawValue[endStartIndex - 1] == '1';
        m_currentAddonMod = m_rawValue.Length >= 2? m_rawValue[endStartIndex]:'0';
        m_onChangeNumberPrevious = m_onChangeNumberCurrent;
        m_onChangeNumberCurrent = m_rawValue.Length >= 4 ? int.Parse(m_rawValue.Substring(0, 4)):0;


    }

    public string m_rawValue="";
    public bool m_isAddonWindowOpen;
    public char m_currentAddonMod;
    public int m_onChangeNumberPrevious;
    public int m_onChangeNumberCurrent;

    public bool IsHadAddonUpdateMemory() { return m_onChangeNumberCurrent != m_onChangeNumberPrevious; }
    public bool IsAddonDisplayIngame() { return m_isAddonWindowOpen; }
    public bool IsAddonInTagMode() { return m_currentAddonMod=='0'; }
    public bool IsAddonInTypeMode() { return m_currentAddonMod == '1'; }
    public bool IsAddonInIndexMode() { return m_currentAddonMod == '2'; }   
    public bool IsAddonInValueMode() { return m_currentAddonMod == '3'; }

    public string GetModeAsString()
    {

        switch (m_currentAddonMod)
        {
            case '0': return "Tag";
            case '1': return "Type";
            case '2': return "Index";
            case '3': return "Value";
            default: return "Unkown";
        }
    }
}

