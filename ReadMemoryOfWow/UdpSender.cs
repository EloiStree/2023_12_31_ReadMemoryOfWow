using Microsoft.VisualBasic;
using System.Net.Sockets;
using System.Text;

partial class Program
{
    public class UdpSender
    {
        public static string dateFormat = "yyyy:MM:dd: HH:mm:ss ffff";

        public static void SendUdpMessageText(string targetIp, int targetPort, string text) {
            SendUdpMessageLines(targetIp, targetPort, text.Split(new char[] { '\n', '\r' }));
        }
        public static void SendUdpMessageLines(string targetIp, int targetPort, params string[] lines)
        {
            // Create a UDP client
            using (UdpClient udpClient = new UdpClient())
            {
                try
                {
                    SendLine(targetIp, targetPort, udpClient, "CHUNK_START:" + DateTime.Now.ToString(dateFormat));
                    foreach (string line in lines)
                    {
                        SendLine(targetIp, targetPort, udpClient, line);

                    }
                    SendLine(targetIp, targetPort, udpClient, "CHUNK_END:" + DateTime.Now.ToString(dateFormat));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message: {ex.Message}");
                }
            }
        }

        private static void SendLine(string targetIp, int targetPort, UdpClient udpClient, string line)
        {
            byte[] data = Encoding.UTF8.GetBytes(line);
            udpClient.Send(data, data.Length, targetIp, targetPort);
        }

       
    }
}
 
