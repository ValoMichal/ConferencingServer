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
            public int id { get; set; }
            public string name { get; set; }
            public string ip { get; set; }
            public UdpClient portChatDown { get; set; }
            public UdpClient portAudioDown { get; set; }
            public UdpClient portVideoDown { get; set; }
            public UdpClient portChatUp { get; set; }
            public UdpClient portAudioUp { get; set; }
            public UdpClient portVideoUp { get; set; }
            public User(int id, string name, string ip)
            {
                this.id = id;
                this.name = name;
                this.ip = ip;
                this.portChatDown = new UdpClient(5001+6*id);
                this.portAudioDown = new UdpClient(5002+6*id);
                this.portVideoDown = new UdpClient(5003+6*id);
                this.portChatUp = new UdpClient(5004+6*id);
                this.portAudioUp = new UdpClient(5005+6*id);
                this.portVideoUp = new UdpClient(5006+6*id);
            }
        }
        private static UdpClient comms = new UdpClient(5000);
        private static IPEndPoint client = new IPEndPoint(IPAddress.Any, 0);
        private static List<User> users = new List<User>();
        private static List<string> banned = new List<string>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            Host();
            Cli();
            Start();
            //turn on upstream and downstream
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
                                if (!banned.Contains(client.Address.ToString()))
                                {
                                    comms.Send(Encoding.ASCII.GetBytes(users.Count.ToString()), users.Count.ToString().Length, client.Address.ToString(), 5000);
                                    users.Add(new User(users.Count, input[1], client.Address.ToString()));
                                }
                                break;
                            case "changeName":
                                foreach (User user in users)
                                {
                                    if (user.ip.Equals(client.Address.ToString()))
                                    {
                                        user.name = input[1];
                                    }
                                }
                                break;
                            default:
                                Debug.WriteLine("Unknown command: "+input[0]);
                                break;
                        }
                        //string sendText = string.Join(Environment.NewLine, output);
                        //string sendThis = (5001 + users.Count * 3).ToString();
                        //byte[] output = Encoding.ASCII.GetBytes(sendThis);
                        //connect.Send(output, output.Length, users[users.Count - 1], 5000);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
            });
        }
        private static void Cli()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    List<string> input = Console.ReadLine().Split(" ").ToList();
                    switch(input[0])
                    {
                        case "kick":
                            foreach (User user in users)
                            {
                                if (user.name.Equals(input[1]))
                                {
                                    users.Remove(user);
                                }
                            }
                            break;
                        case "ban":
                            foreach (User user in users)
                            {
                                if (user.name.Equals(input[1]))
                                {
                                    banned.Add(user.ip);
                                    users.Remove(user);
                                }
                            }
                            break;
                        case "close":
                            Environment.Exit(Environment.ExitCode);
                            break;
                        default:
                            Console.WriteLine("Unknown command. Try kick, ban or close");
                            break;
                    }
                }
            });
        }
        private static void Start()
        {
            while (true)
            {

            }
        }
    }
}
