public class AddressStringToPointer
{

    public static void Convert(string hexString, out IntPtr pointer)
    {
        hexString = hexString.Trim();
        pointer = (IntPtr)0;
        if (hexString.Length < 2) return;
        if (hexString[0] == '0' && hexString[1] == 'x')
        {
            hexString = hexString.Substring(2);
        }
        long intValue = long.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
        pointer = new IntPtr(intValue);
    }
}

