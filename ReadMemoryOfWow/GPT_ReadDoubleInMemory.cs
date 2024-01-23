//using System;
//using System.Diagnostics;
//using System.Runtime.InteropServices;

//class Program
//{
//    const int PROCESS_ALL_ACCESS = 0x1F0FFF; // Adjust the access rights accordingly
//    const int SIZE_OF_DOUBLE = 8; // Size of double in bytes

//    [DllImport("kernel32.dll")]
//    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

//    [DllImport("kernel32.dll", SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

//    [DllImport("kernel32.dll", SetLastError = true)]
//    [return: MarshalAs(UnmanagedType.Bool)]
//    public static extern bool CloseHandle(IntPtr hObject);

//    static void MainT()
//    {
//        string processName = "YourProcessName"; // Replace with the name of your target process
//        IntPtr processHandle = IntPtr.Zero;

//        try
//        {
//            // Get the process ID by name
//            Process[] processes = Process.GetProcessesByName(processName);
//            if (processes.Length == 0)
//            {
//                Console.WriteLine("Process not found");
//                return;
//            }

//            int processId = processes[0].Id;

//            // Open the process with desired access rights
//            processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
//            if (processHandle == IntPtr.Zero)
//            {
//                Console.WriteLine("Failed to open process");
//                return;
//            }

//            // Define the address where your double value is located
//            IntPtr address = new IntPtr(0x12345678); // Replace with the actual address

//            // Read the double value from the target process
//            byte[] buffer = new byte[SIZE_OF_DOUBLE];
//            int bytesRead;

//            if (ReadProcessMemory(processHandle, address, buffer, SIZE_OF_DOUBLE, out bytesRead))
//            {
//                double doubleValue = BitConverter.ToDouble(buffer, 0);
//                Console.WriteLine($"Read double value: {doubleValue}");
//            }
//            else
//            {
//                Console.WriteLine("Failed to read process memory");
//            }
//        }
//        finally
//        {
//            // Close the process handle
//            if (processHandle != IntPtr.Zero)
//                CloseHandle(processHandle);
//        }
//    }
//}
