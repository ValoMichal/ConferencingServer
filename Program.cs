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
        public class User
        {
            int id { get; set; }
            string name { get; set; }
            string ip { get; set; }
            public User(int id, string name, string ip)
            {
                this.id = id;
                this.name = name;
                this.ip = ip;
            }
        }
        private static UdpClient comms = new UdpClient(5000);
        private static IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
        private static List<User> users = new List<User>();
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
                        List<string> input = new List<string>(Encoding.ASCII.GetString(comms.Receive(ref client)).Split(new[] { Environment.NewLine }, StringSplitOptions.None));
                        switch (input[0])
                        {
                            case "connect":
                                comms.Send(Encoding.ASCII.GetBytes(users.Count.ToString()),users.Count.ToString().Length,client.Address.ToString(),5000);
                                users.Add(new User(users.Count, input[1], client.Address.ToString()));
                                //add new upstream/downstream channels
                                break;
                            default:
                                break;
                        }
                        users.Add(client.Address.ToString());
                        users.
                        //string sendText = string.Join(Environment.NewLine, output);
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
