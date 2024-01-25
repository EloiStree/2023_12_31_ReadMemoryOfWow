public class NamedMemoryFloatFetcher
{
    public MemoryFloatFetcher m_linkedFecher;
    public string m_labelName;

    public float m_value;

    public NamedMemoryFloatFetcher(string labelName, MemoryFloatFetcher linkedFecher)
    {
        m_labelName = labelName;
        m_linkedFecher = linkedFecher;
    }

    public void SetName(string value) => m_labelName = (value);
    public void SetValue(float value) => m_value = value;

    

    public void SetValueFromLastRead()
    {
        m_linkedFecher.GetLastFetch(out bool found, out m_value);
        if (!found)
            m_value = 0;
    }

}

