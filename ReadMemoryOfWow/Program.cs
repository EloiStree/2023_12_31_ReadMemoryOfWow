using ReadMemoryOfWow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Timers;
using static Program;


public  class UdpWowListener
{
   public UdpClient udpClient;
   public bool isListening = true;
    public int m_port;
   public DateTime lastMessageTime = DateTime.Now;
   public DateTime mainThreadExists = DateTime.Now;
   public Thread listenerThread=null;

    public void NotifyMainThreadExists() {
        mainThreadExists= DateTime.Now; 
    }
    public  void Initiate(int port = 12345)
    {
        m_port = port;
        udpClient = new UdpClient(port);
        listenerThread = new Thread(new ThreadStart(Listen));
        listenerThread.Start();
    }

    public void Listen()
    {
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
        try
        {
            Console.WriteLine($"Start Thread:"+m_port);
            //while ((DateTime.Now- mainThreadExists ).TotalSeconds <5 )
            while (Program.keepThreadAlive!=null)

            {
                byte[] data = udpClient.Receive(ref endPoint);
                string message = System.Text.Encoding.UTF8.GetString(data);
                Console.WriteLine($"Received message: {message}");
                Program.SendMessageToWowMemory(message);

                // Update the last received message time
                lastMessageTime = DateTime.Now;
            }
            Console.WriteLine($"Process Killed");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException: {ex.Message}");
        }
    }

 
}


public class TaggedMemoryFileSave {
    public static void Save(string groupName ,List<TaggedMemoryDoubleFetcher> list ) {
        string fileName = groupName + ".tagmemory";
        if (!File.Exists(fileName))
            File.Create(fileName);
        Thread.Sleep(30);
        StringBuilder sb = new StringBuilder();
        foreach (TaggedMemoryDoubleFetcher entry in list.OrderBy(k => k.m_linkedFecher.m_mapAddress)) {
            sb.AppendLine($"{entry.m_linkedFecher.m_givenAddress}|{entry.m_flooredType}|{entry.m_flooredIndex}");
        }
        File.WriteAllText(fileName,sb.ToString());
    }

    public static void Load(string groupName, ProcessOpenHandler handler , out List<TaggedMemoryDoubleFetcher> list ) {
        string fileName = groupName + ".tagmemory";
        if (!File.Exists(fileName))
            File.Create(fileName);
        list = new List<TaggedMemoryDoubleFetcher>();
        foreach (string line in File.ReadLines(fileName))
        {
            string[] tokens = line.Split("|");
            if (tokens.Length == 3) {
                MemoryDoubleFetcher fetcher = new MemoryDoubleFetcher(handler, tokens[0].Trim()) ;
                TaggedMemoryDoubleFetcher element = new TaggedMemoryDoubleFetcher(fetcher);
                element.SetType((int)(double.Parse(tokens[1].Trim())));
                element.SetIndex((int)(double.Parse(tokens[2].Trim())));
                list.Add(element);
            }
        }
    }
}

public class StaticMemoryManager
{
    public List<TaggedMemoryDoubleFetcher> allTaggedMemory = new List<TaggedMemoryDoubleFetcher>();
    public List<TaggedMemoryDoubleFetcher> luaReadTextOfMemory = new List<TaggedMemoryDoubleFetcher>();
    public List<TaggedMemoryDoubleFetcher> luaWriteTextOfMemory = new List<TaggedMemoryDoubleFetcher>();
    public Dictionary<int, List<TaggedMemoryDoubleFetcher>> luaMemoryPerType = new Dictionary<int, List<TaggedMemoryDoubleFetcher>>();
    internal List<NamedMemoryFloatFetcher> cheateLabeledTable = new List<NamedMemoryFloatFetcher>();
    internal bool useDoublePushUdp=false;
    internal bool useCheatTablePushUDP=true;
    //public List<TaggedMemoryDoubleFetcher> Type_0_Unkown = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_1_Char = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_2_Unkown = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_3_OnChanged = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_4_AddonsState = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_5_DoubleValue = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_6_DummyBoolean = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_7_BitBoolean = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_8_Bit0To9 = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_9_FiveCharInDouble = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_21_WriteChar = new List<TaggedMemoryDoubleFetcher>();
    //public List<TaggedMemoryDoubleFetcher> Type_22_WriteCallLuaFunction = new List<TaggedMemoryDoubleFetcher>();

}


    public class KeepAliveClass { public bool heyMonAmi = false; }

partial class Program
{
    public static StaticMemoryManager memoryManage = new StaticMemoryManager();

    public static string m_relativePathToTextAddressDouble = "TextAddress.txt";


    public static int m_listenedPort=5431;
    public static int m_redirectionPort = 12345; 

    public DateTime m_keepTreadAlive;
    public float m_maxTimeThreadAlive=10;

    public static KeepAliveClass keepThreadAlive = new KeepAliveClass();

    public static string targetIp = "127.0.0.1";  // Change this to the target IP address
    public static string processID = "8F98";
    public static float secondBetweenPushUDP = 1;




    public class NamedKeyValueFloatAddress {
        public string m_keyName;
        public string m_floatAddress;

        public NamedKeyValueFloatAddress(string keyName, string floatAddress)
        {
            m_keyName = keyName;
            m_floatAddress = floatAddress;
        }
    }

    //SCE Static Cheat Engine Value
    //DCE Dynamic Cheat Engine Value
    //WSCE Weird Static Cheat Engine Value
    public NamedKeyValueFloatAddress[] m_extractFloat = new NamedKeyValueFloatAddress[] {
        new NamedKeyValueFloatAddress("SCE_PlayerHorizontal", ""),
        new NamedKeyValueFloatAddress("SCE_PlayerVertical", ""),
        new NamedKeyValueFloatAddress("SCE_PlayerHeight", ""),
        new NamedKeyValueFloatAddress("SCE_ClickCursorHorizontal", ""),
        new NamedKeyValueFloatAddress("SCE_ClickCursorVertical", ""),
        new NamedKeyValueFloatAddress("SCE_ClickCursorHeight", ""),
        new NamedKeyValueFloatAddress("SCE_CameraX", "826A4FB5A8"),
        new NamedKeyValueFloatAddress("SCE_CameraY", "826A4FBBD8"),
        new NamedKeyValueFloatAddress("SCE_CameraZ", ""),
        new NamedKeyValueFloatAddress("SCE_CameraXRotation", ""),
        new NamedKeyValueFloatAddress("WSCE_CameraYRotation", ""),
        new NamedKeyValueFloatAddress("DCE_PlayerXP", ""),

        // Don't it is better to use Cheat Engine XML to export 
        // Except if you plan to use other software.
        // But have a entry is a must for people that continue using it.

    };

    public static string cheatTableFilePath = "CheatTable.xml";



    public static void ReadAndPushNameFloatMemory(List<NamedMemoryFloatFetcher> memory) {
        ReadFromMemory(memory);
        PushOnUdp(memory);
    }

    public static void ReadFromMemory(List<NamedMemoryFloatFetcher> memory)
    {
        foreach (var item in memory)
        {
            item.m_linkedFecher.FetchDataInMemory();
            item.SetValueFromLastRead();
        }
    }
    public static void PushOnUdp(List<NamedMemoryFloatFetcher> memory)
    {
        foreach (var item in memory)
        {
            UdpSender.SendUdpMessageLines(targetIp, m_redirectionPort, string.Format("{0}:{1}", item.m_labelName, item.m_value));
     
        }
    }

    static void Main()
    {


        bool found;

        if (!File.Exists(cheatTableFilePath))
            File.WriteAllText(cheatTableFilePath,"" +
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                "<CheatTable>\r\n " +
                " <CheatEntries>\r\n  " +
                "  <CheatEntry>\r\n   " +
                "   <ID>0</ID>\r\n     " +
                " <Description>\"Height Camera\"</Description>\r\n " +
                "     <VariableType>Float</VariableType>\r\n   " +
                "   <Address>826A4FBBD8</Address>\r\n    </CheatEntry>\r\n  " +
                "  <CheatEntry>\r\n   " +
                "   <ID>1</ID>\r\n  " +
                "    <Description>\"Vertical Camera\"</Description>\r\n   " +
                "   <VariableType>Float</VariableType>\r\n    " +
                "  <Address>826A4FB5A8</Address>\r\n " +
                "   </CheatEntry>\r\n  " +
                "  <CheatEntry>\r\n   " +
                "   <ID>2</ID>\r\n    " +
                "  <Description>\"Horizontal Camera\"</Description>\r\n " +
                "     <VariableType>Float</VariableType>\r\n " +
                "     <Address>826A4FBBD4</Address>\r\n  " +
                "  </CheatEntry>\r\n " +
                " </CheatEntries>\r\n" +
                "</CheatTable>\r\n" +
                "");
        CheatEngineFile.Load(cheatTableFilePath, out  found, out CheatTable foundTable);

        if (found && foundTable != null)
            CheatEngineFile.DisplayCheatTable(foundTable);

        UdpWowListener listener = new UdpWowListener();
        string taggedMemory = "TaggedMemory";
        listener.Initiate(m_listenedPort);



        TimerLoop loopPush = new TimerLoop(secondBetweenPushUDP, ReadMemoryAndPush, false);

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
        //Console.WriteLine(string.Join(", ", addresses));

        // WE READ THE ADDRESS AND CONVER THE TO READBLE ONE
        List<MemoryDoubleFetcher> m_luaDoubleAddress = new List<MemoryDoubleFetcher>();
        ProcessOpenHandler handler = new ProcessOpenHandler(processID);



        List<NamedMemoryFloatFetcher> floatInMemoryFetcher = new List<NamedMemoryFloatFetcher>();
        foreach (var item in foundTable.CheatEntries.CheatEntry)
        {
            MemoryFloatFetcher fetcher = new MemoryFloatFetcher(handler, item.Address);

            floatInMemoryFetcher.Add(new NamedMemoryFloatFetcher((item.Description.Trim('\"')), fetcher));
        }
        memoryManage.cheateLabeledTable = floatInMemoryFetcher;





        ListOfDoubleAddress doubleHolder = new ListOfDoubleAddress();
        doubleHolder.SetFromAddresses(handler, addresses);
        doubleHolder.ReadThemAll();
        //doubleHolder.DisplayTheFirstOne(30);

        string consoleKey = "";
        Console.WriteLine("Use previous ? (N)o ");
        consoleKey = Console.ReadLine();
        consoleKey = consoleKey.ToUpper().Trim();
        if (consoleKey == "N" || consoleKey == "NO")
        {
            List<TaggedMemoryDoubleFetcher> taggedMemories = new List<TaggedMemoryDoubleFetcher>();
            foreach (var fetch in doubleHolder.m_addressFetchers)
            {
                taggedMemories.Add(new TaggedMemoryDoubleFetcher(fetch));
            }
            memoryManage.allTaggedMemory = taggedMemories;




            Console.WriteLine("Setup: Press any key, when your game is in Type mode ?");
            consoleKey = Console.ReadLine();

            doubleHolder.ReadThemAll();
            foreach (var tFetcher in memoryManage.allTaggedMemory)
            {
                tFetcher.SetTypeFromLastRead();
            }





            Console.WriteLine("Setup: Press any key, when your game is in Index mode ?");

            consoleKey = Console.ReadLine();
            doubleHolder.ReadThemAll();
            foreach (var tFetcher in memoryManage.allTaggedMemory)
            {
                tFetcher.SetIndexFromLastRead();
            }
        }
        else
        {
            TaggedMemoryFileSave.Load(taggedMemory, handler, out memoryManage.allTaggedMemory);

        }


        // Goblin start zone hearthstone:
        // x horizontal 1361,3
        // y Vertical   -8423,876 
        // z height     -8423,876
        //  
        //MemoryDoubleFetcher wowAddonState = new MemoryDoubleFetcher(handler, "0x29E7573D4C0");
        MemoryFloatFetcher floatXP = new MemoryFloatFetcher(handler, "0x29D8777A50C");
        MemoryFloatFetcher floatX = new MemoryFloatFetcher(handler, "0x2131FF3F374");
        MemoryFloatFetcher floatZ = new MemoryFloatFetcher(handler, "0x2131FDC2D30");
        MemoryFloatFetcher floatY = new MemoryFloatFetcher(handler, "0x2131FDC2D38");
        LuaAddonModeInformation addonMode = new LuaAddonModeInformation();

        AllMemoryToCategorizedInMemoryManager();

        foreach (var key in Program.memoryManage.luaMemoryPerType.Keys)
        {
            int count = Program.memoryManage.luaMemoryPerType[key].Count;
            Console.WriteLine("Item:" + key);
            if (count < 10)
            {
                Console.WriteLine(string.Join(" ", Program.memoryManage.luaMemoryPerType[key].Select(k=>k.m_linkedFecher.m_givenAddress)));
            }
            else { 
                Console.WriteLine("Addresses count: "+ Program.memoryManage.luaMemoryPerType[key].Count);
            }
        }
            


        FilterOut(1231231, memoryManage.allTaggedMemory);
        TaggedMemoryFileSave.Save(taggedMemory, memoryManage.allTaggedMemory);
        while (true)
        {

            Console.WriteLine();
            Console.WriteLine("Ready ?");
            consoleKey = Console.ReadLine();


            if (consoleKey == null)
                consoleKey = "";

            if (consoleKey.Length > 0 && (consoleKey[0] == 'r' || consoleKey[0] == 'R'))
            {

                ReadThemAll(memoryManage.luaReadTextOfMemory);
                Console.WriteLine(GetListOfAddressAsCharValue(memoryManage.luaReadTextOfMemory));
            }
            if (consoleKey.Length > 0 && (consoleKey[0] == 'd' || consoleKey[0] == 'D'))
            {
                ReadThemAll(memoryManage.allTaggedMemory);
                Console.WriteLine(SleepyDebug(memoryManage.allTaggedMemory));
            }
            if (consoleKey.Length > 0 && (consoleKey[0] == 'w' || consoleKey[0] == 'W'))
            {
                SleepyDebugWrite(memoryManage.luaWriteTextOfMemory, consoleKey.Substring(1) + "  randomness =" + DateTime.Now.Ticks);
            }
            if (consoleKey.Length > 0 && (consoleKey[0] == 'h' || consoleKey[0] == 'H'))
            {
                SleepyDebugWrite(memoryManage.luaWriteTextOfMemory, $"print(\"Hello World {indexHello++} boysss\")  ToggleBackpack()");
            }
            if (consoleKey.Length > 0 && (consoleKey[0] == 'u' || consoleKey[0] == 'U'))
            {
                ReadMemoryAndPush();
            }
            if (consoleKey.Length > 0 && (consoleKey[0] == 'l' || consoleKey[0] == 'L'))
            {
                loopPush.SetAsActive(consoleKey[0] == 'L');
            }




            //floatXP.FetchDataInMemory();
            //wowAddonState.FetchDataInMemory();
            //addonMode.SetAddonState(wowAddonState.GetCurrentValue());
            //wowAddonState.GetLastFetch(out found, out double addonStateDouble);
            //Console.WriteLine(string.Format("Addon Mode: Display {0} Mode {1} Changed{2} ",
            // addonMode.IsAddonDisplayIngame(),
            //addonMode.GetModeAsString(),
            //addonMode.IsHadAddonUpdateMemory()
            //));

            //floatXP.GetLastFetch(out found, out float xp);
            //floatX.GetLastFetch(out found, out float x);
            //floatZ.GetLastFetch(out found, out float z);
            //floatY.GetLastFetch(out found, out float y);
            //Console.WriteLine(string.Format("HX{0} VZ{1} TY{2} Wow Addon ", x, z, y));




            //if (addonMode.IsAddonInValueMode()) {
            //doubleHolder.DisplayTheFirstOne(30);
            //Console.WriteLine(GetListOfAddressAsCharValue(doubleHolder));
            // Console.WriteLine(SleepyDebug(taggedMemories));
            //Console.WriteLine(SleepyDebugText(taggedMemories));
            //            Console.WriteLine(GetListOfAddressAsCharValue(taggedMemories));
            //}
            // Wait for 0.1 seconds
            Thread.Sleep(100);
        }
    }



    public static bool m_UsePrintForReadAndPush = false;

    
    private static void ReadMemoryAndPush()
    {

        if (memoryManage.useCheatTablePushUDP) {
            ReadAndPushNameFloatMemory(memoryManage.cheateLabeledTable);
        }

        if (memoryManage.useDoublePushUdp) { 
        
            ReadThemAll(memoryManage.luaReadTextOfMemory);
            string t = GetListOfAddressAsCharValue(memoryManage.luaReadTextOfMemory);
            if (m_UsePrintForReadAndPush)
                Console.WriteLine(t);
            UdpSender.SendUdpMessageText(targetIp, m_redirectionPort, t);
        }
    }

    private static void AllMemoryToCategorizedInMemoryManager()
    {

        Program.memoryManage.luaMemoryPerType.Clear();
        Program.memoryManage.luaWriteTextOfMemory = memoryManage.allTaggedMemory.Where(k => k.m_flooredType == 21).OrderBy(k => k.m_flooredIndex).ToList();
        Program.memoryManage.luaReadTextOfMemory = memoryManage.allTaggedMemory.Where(k => k.m_flooredType == 1).OrderBy(k => k.m_flooredIndex).ToList();

        
        foreach (var item in memoryManage.allTaggedMemory)
        {
            int typeOfItem = item.m_flooredType;
            if (!memoryManage.luaMemoryPerType.ContainsKey(typeOfItem))
                memoryManage.luaMemoryPerType.Add(typeOfItem, new List<TaggedMemoryDoubleFetcher>());
            memoryManage.luaMemoryPerType[typeOfItem].Add(item);
        }
        foreach (var item in memoryManage.luaMemoryPerType.Keys)
        {
            memoryManage.luaMemoryPerType[item]= memoryManage.luaMemoryPerType[item].OrderBy(k => k.m_flooredIndex).ToList();
        }
    }

    public static  void FilterOut(int typeToRemove,  List<TaggedMemoryDoubleFetcher> fetchers) {
        bool isFound;
        double value;
        for (int i = fetchers.Count - 1; i >= 0; i--)
        {
            if (fetchers[i] == null || fetchers[i].m_flooredType ==typeToRemove)
                fetchers.RemoveAt(i);
        }
    }

    private static void ReadThemAll(List<TaggedMemoryDoubleFetcher> fetchers)
    {

        bool isFound;
        double value;
        for (int i = fetchers.Count - 1; i >= 0; i--)
        {
            if (fetchers[i] == null)
                fetchers.RemoveAt(i);
            fetchers[i].m_linkedFecher.FetchDataInMemory();
        }
    }

    public static int indexHello = 0;

    private static string SleepyDebug(List<TaggedMemoryDoubleFetcher> listOfFetcher)
    {
        listOfFetcher = listOfFetcher.OrderBy(k => k.m_linkedFecher.GetAddressAsString()).ToList();
        StringBuilder sb = new StringBuilder();
        foreach (var fetcher in listOfFetcher)
        {
            sb.Append($"&{fetcher.m_linkedFecher.m_givenAddress}" +
                $" Type:{fetcher.m_flooredType}" +
                $" Index:{fetcher.m_flooredIndex}" +
                $" Reach:{fetcher.m_linkedFecher.m_wasReach}" +
                $" Value:{fetcher.m_value}" +
                $" Char:{fetcher.GetAsChar("_")} \n");
        }
        return sb.ToString();
    }
    private static void SleepyDebugWrite(List<TaggedMemoryDoubleFetcher> listOfFetcher)
    {
        listOfFetcher = listOfFetcher.OrderBy(k => k.m_linkedFecher.GetAddressAsString()).ToList();
        for (int i = 0; i < listOfFetcher.Count; i++)
        {
            listOfFetcher[i].m_linkedFecher.OverrideDataInMemory(30 + i % (60));
        }
    }
    private static void SleepyDebugWrite(List<TaggedMemoryDoubleFetcher> listOfFetcher, string text)
    {
        listOfFetcher = listOfFetcher.OrderBy(k => k.m_linkedFecher.GetAddressAsString()).ToList();
        for (int i = 0; i < listOfFetcher.Count; i++)
        {
            //int ci = 30 + i % (60);
            int ci = (int) ' ';
            if (i<text.Length  )
            {
                ci = (int)text[i];
                
            
            }
            listOfFetcher[i].m_linkedFecher.OverrideDataInMemory(ci);
        }
    }

    private static string SleepyDebugText(List<TaggedMemoryDoubleFetcher> listOfFetcher)
    {
        listOfFetcher = listOfFetcher.OrderBy(k => k.m_linkedFecher.GetAddressAsString()).ToList();
        StringBuilder sb = new StringBuilder();
        sb.Append("\n");
        foreach (var fetcher in listOfFetcher)
        {
            sb.Append(fetcher.GetAsChar("_"));
        }
        return sb.ToString();
    }

    private static string GetListOfAddressAsCharValue(ListOfDoubleAddress doubleHolder)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var fetcher in doubleHolder.m_addressFetchers)
        {
            if (fetcher != null && fetcher.IsReadWasWithoutError())
            {
                double value = fetcher.GetCurrentValue();
                if (value >= 0 && value <= 255)
                {
                    try
                    {
                        sb.Append((char)value);
                    }
                    catch (Exception) { sb.Append("_"); }

                }
                else sb.Append("_");
            }
            else sb.Append("_");
        }
        return sb.ToString();
    }

    private static string GetListOfAddressAsCharValue(List<TaggedMemoryDoubleFetcher> listOfFetcher)
    {
        listOfFetcher= listOfFetcher.OrderBy(k => k.m_flooredIndex).ToList();
        StringBuilder sb = new StringBuilder();
        foreach (var fetcher in listOfFetcher)
        {
            sb.Append( fetcher.GetAsChar("_") );
        }
        return sb.ToString();
    }

    public  static void SendMessageToWowMemory(string message)
    {

        SleepyDebugWrite(memoryManage.luaWriteTextOfMemory, message+"  randomness =" + DateTime.Now.Ticks);
    }
}
 
