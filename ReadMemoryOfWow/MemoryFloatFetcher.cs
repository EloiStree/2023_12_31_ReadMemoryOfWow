public class MemoryFloatFetcher : MemoryDecimalFetcherAbstract<float>
{
    public MemoryFloatFetcher(ProcessOpenHandler processHandle, string mapAddress0x) : base(processHandle, mapAddress0x)
    {
    }

    public override void Compute_ConvertByteToValue()
    {
        m_valueFromBuffer = BitConverter.ToSingle(m_bufferByte, 0);
    }

    public override void GetBufferNeededSize(out uint sizeOfBuffer)
    {
        sizeOfBuffer = 4;
    }

    public override float GetDefaultValue()
    {
        return 0.0f;
    }

    protected override void Compute_ConvertValueToByte()
    {

        m_bufferByte = BitConverter.GetBytes(m_valueFromBuffer);
    }
}

