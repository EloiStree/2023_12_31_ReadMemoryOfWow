using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using static Program;


[System.Serializable]
public class WowAddressOfPositionString
{
    public string m_localAliasName = "";
    public string m_processId = "0x1F0FFF";
    public string m_xHorizontalMiniMapAddress = "0x22B2085F8C8";
    public string m_yVerticalMiniMapAddress = "0x22B2085F8C8";
    public string m_zHeightAddress = "0x22B2085F8C8";
}





public class MemoryReadSharpUtility {

    public const int PROCESS_ALL_ACCESS = 0x1F0FFF;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);


    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);

}



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
    public void FetchDataInMemory() {
        m_wasReach = false;
        m_valueFromBuffer = GetDefaultValue();

        if (MemoryReadSharpUtility.ReadProcessMemory(m_process.GetHandlerPointer(), m_mapAddress, m_bufferByte, m_bufferSize, out int intByteValue))
        {
            Compute_ConvertByteToValue();
            m_wasReach = true;
        }
        else m_valueFromBuffer = GetDefaultValue();

    }

    public void OverrideDataInMemory() { 
    
    }

    public bool IsReadWasWithoutError() { return m_wasReach; }
    public T GetCurrentValue() { return m_valueFromBuffer; }
}



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

    
}
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

    
}


[System.Serializable]
public class MemoryVector3Fetcher
{
    public MemoryFloatFetcher m_xHorizontalMiniMapAddress;
    public MemoryFloatFetcher m_yVerticalMiniMapAddress;
    public MemoryFloatFetcher m_zHeightAddress;
}

[System.Serializable]
public class NamedMemoryFloatFetcherVector3
{
    public string m_localAliasName = "";
    public MemoryVector3Fetcher m_vector3MapAddress;
}

[System.Serializable]
public class NamedMemoryFloatFetcherFloat

{
    public string m_localAliasName = "";
    public MemoryFloatFetcher m_floatMapAddress;
}


[System.Serializable]
public class GateUDPOut
{
    public string m_address = "127.0.0.1";
    public int m_port = 4501;
}
[System.Serializable]
public class GateUDPIn
{
    public int m_port = 4501;
}





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

public class ListOfDoubleAddress
{

    public List<MemoryDoubleFetcher> m_addressFetchers = new List<MemoryDoubleFetcher>();
    public void SetFromAddresses(ProcessOpenHandler process, params string[] addresses)
    {
        foreach (string address in addresses)
        {
            m_addressFetchers.Add(new MemoryDoubleFetcher(process, address));
        }
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

class Program
{

    public static string m_relativePathToTextAddressDouble = "TextAddress.txt";


    // TAG Index Type Value

    // Tyoe=0 Reserver for later
    // Tyoe=1 Reserver for later
    // Tyoe=2 Reserver for later
    // Type=3 OnChange RandomValue 54645  // This address change every time a value changed. 
    // Type=4 Addons State Value  99990000012  // Value that allow 
    // Type=5 Double  546541681814684618
    // Type=6 DummyBoolean // True or false store in a double losing lot's of space 000000000001
    // Type=7 BitBoolean // A double where every 1 0 are boolean  10100101
    // Type=8 BitByte // A double where every 0-9 are short value between 0-9   21646103
    // Type=9 Double Five Char 0.1234567812345678
    //                  0.255255255255255 
    // Type=10N = Custom value in double to be parse by other developers


    static void Main()
    {
        //Float 0.12345678
        //Double 0.1234567812345678
        //Double 0.1234567812345678
        //5Char in double "0.250251252253254" 
        //NOTE OF IMPROVEMENT        
        // Stupide idea but I could on a double store 5 char of 0-255 and use array index mode know here it going.
        // Only code that if all works and I want to improve the speed and decrease the time reading memory by adding complexity in the storage vs simplicity
        //Tag inex 450 type "0.250 251 252 253 254" 
        //450= 250
        //450+1=251
        //450+2=252
        //450+3=253
        //450+4=254


        double maxDoubleValue = double.MaxValue;
        Console.WriteLine($"Maximum value of a double: {maxDoubleValue}");

        if (!File.Exists(m_relativePathToTextAddressDouble))
            File.WriteAllText(m_relativePathToTextAddressDouble, "");
        string[] addresses = File.ReadAllLines(m_relativePathToTextAddressDouble);
        addresses = addresses.Where(k => k.Trim().Length > 2).ToArray();

        Console.WriteLine("Number of address loaded:" + addresses.Length);
        Console.WriteLine(string.Join(", ", addresses));

        // WE READ THE ADDRESS AND CONVER THE TO READBLE ONE
        List<MemoryDoubleFetcher> m_luaDoubleAddress = new List<MemoryDoubleFetcher>();
        ProcessOpenHandler handler = new ProcessOpenHandler("0x6D38");
        ListOfDoubleAddress doubleHolder = new ListOfDoubleAddress();
        doubleHolder.SetFromAddresses(handler, addresses);
        doubleHolder.ReadThemAll();
        doubleHolder.DisplayTheFirstOne(30);


        string key = "";

        Console.WriteLine("Setup: Press any key, when your game is in Type mode ?");
         key = Console.ReadLine();




        Console.WriteLine("Setup: Press any key, when your game is in Index mode ?");

         key = Console.ReadLine();




        // Goblin start zone hearthstone:
        // x horizontal 1361,3
        // y Vertical   -8423,876 
        // z height     -8423,876
        //  

        MemoryFloatFetcher floatXP = new MemoryFloatFetcher(handler, "0x29D8777A50C");
        MemoryDoubleFetcher wowAddonState = new MemoryDoubleFetcher(handler, "0x29E7573D4C0");

        MemoryFloatFetcher floatX = new MemoryFloatFetcher(handler, "0x2131FF3F374");
        MemoryFloatFetcher floatZ = new MemoryFloatFetcher(handler, "0x2131FDC2D30");
        MemoryFloatFetcher floatY = new MemoryFloatFetcher(handler, "0x2131FDC2D38");
        LuaAddonModeInformation addonMode = new LuaAddonModeInformation();

        while (true)
        {
            bool found;

            floatXP.FetchDataInMemory();
            wowAddonState.FetchDataInMemory();

            addonMode.SetAddonState(wowAddonState.GetCurrentValue());


            floatXP.GetLastFetch(out found, out float xp);
            floatX.GetLastFetch(out found, out float x);
            floatZ.GetLastFetch(out found, out float z);
            floatY.GetLastFetch(out found, out float y);

            wowAddonState.GetLastFetch(out found, out double addonStateDouble);
            Console.WriteLine(string.Format("XP{0}  H{1} V{2} Top{3} Wow Addon {4} ", xp, x, z, y, addonStateDouble));
            Console.WriteLine(string.Format("Addon Mode: Display {0} Mode {1} Changed{2} ",
                addonMode.IsAddonDisplayIngame(), 
                addonMode.GetModeAsString(),
                addonMode.IsHadAddonUpdateMemory()
                ));

            //if (addonMode.IsAddonInValueMode()) {
            doubleHolder.ReadThemAll();
            doubleHolder.DisplayTheFirstOne(30);
            Console.WriteLine(GetListOfAddressAsCharValue(doubleHolder));
            //}
            // Wait for 0.1 seconds
            Thread.Sleep(100);
        }
    }

    private static string GetListOfAddressAsCharValue(ListOfDoubleAddress doubleHolder)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var fetcher in doubleHolder.m_addressFetchers)
        {
            if (fetcher != null && fetcher.IsReadWasWithoutError())
            {
                double value = fetcher.GetCurrentValue();
                if (value >= 0 && value <= 255) {
                    try {
                        sb.Append((char)value);
                    }
                    catch(Exception) { sb.Append("_"); }

                }
                else sb.Append("_");
            }
            else sb.Append("_");
        }
        return sb.ToString();
    }
}

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

