public abstract class MemoryDecimalFetcherAbstract<T>
{

    public ProcessOpenHandler m_process;
    public IntPtr m_mapAddress;//= 0x22B2085F8C8;
    public string m_givenAddress;//= 0x22B2085F8C8;

    protected byte[] m_bufferByte=null;
    protected uint m_bufferSize=4;
    protected T m_valueFromBuffer;


    public MemoryDecimalFetcherAbstract(ProcessOpenHandler processHandle, string mapAddress0x)
    {
        m_givenAddress = mapAddress0x;
        m_process = processHandle;
        AddressStringToPointer.Convert(mapAddress0x, out m_mapAddress);
        GetBufferNeededSize(out  m_bufferSize);
        m_bufferByte = new byte[m_bufferSize];
        m_valueFromBuffer = GetDefaultValue();
    }
    public string GetAddressAsString()
    {
        return m_givenAddress;
    }
    public void GetLastFetch(out bool found, out T valueFromBuffer)
    {
        found = m_wasReach;
        valueFromBuffer = m_valueFromBuffer;
    }
    public abstract void GetBufferNeededSize(out uint sizeOfBuffer);
    public abstract void Compute_ConvertByteToValue();
    public abstract T GetDefaultValue();

    public bool m_wasReach;
    public bool m_wasOverride;
    public void FetchDataInMemory()
    {
        m_wasReach = false;
        m_wasOverride = false;
        m_valueFromBuffer = GetDefaultValue();

        if (MemoryReadSharpUtility.ReadProcessMemory(m_process.GetHandlerPointer(), m_mapAddress, m_bufferByte, m_bufferSize, out int _))
        {
            Compute_ConvertByteToValue();
            m_wasReach = true;
        }
        else m_valueFromBuffer = GetDefaultValue();

    }
    public void OverrideDataInMemory(T value )
    {
        m_valueFromBuffer = value;
        Compute_ConvertValueToByte();
        m_wasOverride = false;
        m_wasReach = false;
        if (MemoryReadSharpUtility.WriteProcessMemory(m_process.GetHandlerPointer(), m_mapAddress, m_bufferByte, m_bufferSize, out int _))
        {
            m_wasOverride = true;
        }
    }

    protected abstract void Compute_ConvertValueToByte();

    public bool IsReadWasWithoutError() { return m_wasReach; }
    public T GetCurrentValue() { return m_valueFromBuffer; }
}

