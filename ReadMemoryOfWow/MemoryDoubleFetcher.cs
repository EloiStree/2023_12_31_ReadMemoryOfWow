public class MemoryDoubleFetcher : MemoryDecimalFetcherAbstract<double>
{
    public MemoryDoubleFetcher(ProcessOpenHandler processHandle, string mapAddress0x) : base(processHandle, mapAddress0x)
    {
    }

    public override void Compute_ConvertByteToValue()
    {
        m_valueFromBuffer = BitConverter.ToDouble(m_bufferByte, 0);
    }

    public override void GetBufferNeededSize(out uint sizeOfBuffer)
    {
        sizeOfBuffer = 8;
    }

    public override double GetDefaultValue()
    {
        return 0.0;
    }

    protected override void Compute_ConvertValueToByte()
    {
        m_bufferByte = BitConverter.GetBytes(m_valueFromBuffer);
    }
}

