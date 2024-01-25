public class ProcessOpenHandler
{

    public IntPtr m_processHandle = IntPtr.Zero;
    public ProcessOpenHandler(string processId0x)
    {
        AddressStringToPointer.Convert(processId0x, out m_processHandle);
        m_processHandle =  MemoryReadSharpUtility. OpenProcess(MemoryReadSharpUtility.PROCESS_ALL_ACCESS, false, m_processHandle.ToInt32());
    }
    public bool IsProcessOpen() { return m_processHandle == IntPtr.Zero; }
    public IntPtr GetHandlerPointer() { return m_processHandle; }

}

