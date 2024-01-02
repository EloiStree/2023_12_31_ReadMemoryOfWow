using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    const int PROCESS_ALL_ACCESS = 0x1F0FFF;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern bool CloseHandle(IntPtr hObject);

    static void Main()
    {

        // Goblin start zone hearthstone:
        // x horizontal 1361,3
        // y Vertical   -8423,876 
        // z height     -8423,876
        //  

        // Replace these values with your actual process ID and memory addresses
        int processId = 0x6EF4; // Replace with your actual process ID
        IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, processId);

        if (processHandle == IntPtr.Zero)
        {
            Console.WriteLine("Failed to open process.");
            return;
        }

        // Memory addresses to read
        IntPtr addressToReadHorizontal = (IntPtr)0x22B2085F8C8; // Replace with your actual memory address for horizontal
        IntPtr addressToReadVertical = (IntPtr)0x22B203E4370; // Replace with your actual memory address for vertical
        IntPtr addressToReadHeight = (IntPtr)0x22B203E4370; // Replace with your actual memory address for height

        // Buffers to store read data
        byte[] bufferHorizontal = new byte[4]; // Assuming a 4-byte float for horizontal
        byte[] bufferVertical = new byte[4]; // Assuming a 4-byte float for vertical
        byte[] bufferHeight = new byte[4]; // Assuming a 4-byte float for height

        float valueHorizontal=0;
        float valueVertical = 0;
        float valueHeight = 0;



        try
        {
            while (true)
            {

                // Read memory at the horizontal address
                int bytesReadHorizontal;


                if (ReadProcessMemory(processHandle, addressToReadHorizontal, bufferHorizontal, (uint)bufferHorizontal.Length, out bytesReadHorizontal))
                {
                    float horizontalValue = BitConverter.ToSingle(bufferHorizontal, 0);
                    valueHorizontal = horizontalValue;
                }
                else
                {

                }



                // Read memory at the vertical address
                int bytesReadVertical;
                if (ReadProcessMemory(processHandle, addressToReadVertical, bufferVertical, (uint)bufferVertical.Length, out bytesReadVertical))
                {
                    float verticalValue = BitConverter.ToSingle(bufferVertical, 0);

                    valueVertical = verticalValue;
                }
                else
                {
                }




                // Read memory at the height address
                int bytesReadHeight;
                if (ReadProcessMemory(processHandle, addressToReadHeight, bufferHeight, (uint)bufferHeight.Length, out bytesReadHeight))
                {
                    float heightValue = BitConverter.ToSingle(bufferHeight, 0);

                    valueHeight = heightValue;
                }
                else
                {
                }
                Console.WriteLine($"Horizontal Value: {valueHorizontal} : {valueVertical} : {valueHeight} ");


                // Wait for 0.1 seconds
                Thread.Sleep(100);

            }
        }
        finally
        {
            // Close the process handle when done
            CloseHandle(processHandle);
        }
    }
}
