public class TaggedMemoryDoubleFetcher {
    public MemoryDoubleFetcher m_linkedFecher;
    public int m_flooredType;
    public int m_flooredIndex;
    public double m_value;
    
    
    // I need to do an array that is append N time on 1 seconds. THen use it to check that memory did not randomly changed accidently.
    // Value can be dealed by the developer behind, but type and index must be exact.
    // To do later when I am not a sleep.
    //public List<double> m_antiFlickeringType=new List<double>();
    //public List<double> m_antiFlickeringIndex = new List<double>();





    public TaggedMemoryDoubleFetcher(MemoryDoubleFetcher linkedFecher)
    {
        m_linkedFecher = linkedFecher;
    }

    public void SetType(int value) => m_flooredType =( value);
    public void SetIndex(int value) => m_flooredIndex = (value);
    public void SetValue(double value) => m_value = value;

    public void SetTypeFromLastRead()
    {
        m_linkedFecher.GetLastFetch(out bool found, out double value);
        m_flooredType = (int)value;
        if (!found)
            m_flooredType = 0;
    }

    public void SetIndexFromLastRead()
    {
        m_linkedFecher.GetLastFetch(out bool found, out double value);
        m_flooredIndex = (int)value;
        if (!found)
            m_flooredIndex = 0;
    }

    public void SetValueFromLastRead()
    {
        m_linkedFecher.GetLastFetch(out bool found, out m_value);
        if (!found)
            m_value = 0;
    }

    public string GetAsChar(string defaultIfError="_")
    {
        if (m_linkedFecher != null && m_linkedFecher.IsReadWasWithoutError())
        {
            double value = m_linkedFecher.GetCurrentValue();
            if (value >= 0 && value <= 255)
            {
                try
                {
                    return ""+(char)value;
                }
                catch (Exception) { return (defaultIfError); }

            }
        }
         return (defaultIfError);
    }
}

