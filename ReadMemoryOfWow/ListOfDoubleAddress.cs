public class ListOfDoubleAddress
{

    public List<MemoryDoubleFetcher> m_addressFetchers = new List<MemoryDoubleFetcher>();
    public void ClearAddresses()
    {
        m_addressFetchers.Clear();
    }
    public void SetFromAddresses(ProcessOpenHandler process, params string[] addresses)
    {
        ClearAddresses();
        Append(process, addresses);
    }

    public void Append(ProcessOpenHandler process, params string[] addresses)
    {
        foreach (string address in addresses)
        {
            m_addressFetchers.Add(new MemoryDoubleFetcher(process, address));
        }

    }
    public void Append(MemoryDoubleFetcher memoryFetcher)
    {
        m_addressFetchers.Add(memoryFetcher);
    }


    public void ReadThemAll()
    {

        if (m_addressFetchers == null) return;

        bool isFound;
        double value;
        for (int i = m_addressFetchers.Count - 1; i >= 0; i--)
        {
            if (m_addressFetchers[i] == null)
                m_addressFetchers.RemoveAt(i);
            m_addressFetchers[i].FetchDataInMemory();
        }
    }

    public void DisplayTheFirstOne(int count)
    {
        Console.WriteLine($"First {count} addresses:");
        for (int i = 0; i < count; i++)
        {
            if (i < m_addressFetchers.Count)
                Console.WriteLine(string.Format("{0}:{1} {2}",
                     m_addressFetchers[i].GetAddressAsString(),
                      m_addressFetchers[i].IsReadWasWithoutError(),
                       m_addressFetchers[i].GetCurrentValue()));
        }
    }
}

