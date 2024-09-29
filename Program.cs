using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using System;
using System.Net.Sockets;
using System.Net;

namespace ConferencingServer
{
    internal class Program
    {
        UdpClient comms = new UdpClient(5000);
        private static IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
        private static Dictionary<int,string> users = new Dictionary<int,string>();
        private static Dictionary<int, string> names = new Dictionary<int, string>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Host();
        }
        private static void Host()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] data = connect.Receive(ref peer);
                        string dataText = Encoding.ASCII.GetString(data);
                        users.Add(peer.Address.ToString());
                        //connect.Connect(users[users.Count - 1], 5001 + users.Count * 3);
                        string sendThis = (5001 + users.Count * 3).ToString();
                        byte[] output = Encoding.ASCII.GetBytes(sendThis);
                        connect.Send(output, output.Length, users[users.Count - 1], 5000);
                        this.Dispatcher.Invoke(() =>
                        {
                            ChatAdd(sender, e, dataText + " connected.");
                        });
                        Task.Run(() => VideoDown(null, null));
                        Task.Run(() => AudioDown(null, null));
                        Task.Run(() => ChatDown(null, null));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            });
        }
    }
}
