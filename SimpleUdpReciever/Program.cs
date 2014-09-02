using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace SimpleUdpReciever
{
    class Program
    {
        static void Main(string[] args)
        {
            //int localPort = 4444;
            IPEndPoint remoteSender = new IPEndPoint(IPAddress.Any, 0);
           // bool flag = false;
            Console.WindowWidth = 150;
            Console.Title = "Andys UDP onsole Application";
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("***************************************************");
            Console.WriteLine("*   Welcome all to Andys UDP console application  *");
            Console.WriteLine("*                                                 *");
            Console.WriteLine("***************************************************");
            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine("Your Local IP Address are: ");
            foreach (string add in LocalIPAddress())
            {
                Console.WriteLine("- "+add);
            }
            //Console.WriteLine("Local IP Address: " + LocalIPAddress());
           
            // Console.WriteLine("Google IP:" + GetIPAddress("google.com"));
            //Console.ReadLine();

            Console.WriteLine("Enter Remote IP address:");
            string value = Console.ReadLine();
            Console.WriteLine("Enter Recive Port (4444):");
            int localPort = Convert.ToInt32( Console.ReadLine());
            Console.WriteLine("Enter Remote Port (ENTER for any):");
            string remotePortString = Console.ReadLine();
            int remotePort = 0;
            if (remotePortString != "")
            {
                remotePort = Convert.ToInt32(remotePortString);
            }
            else
            {
                remotePort = 0;
            }
            int tempInt;
            IPAddress tempAddress;
                
          
                        //value = GetValue(args, ref i);
                if (IPAddress.TryParse(value, out tempAddress))
                {
                    remoteSender.Address = tempAddress;
                    remoteSender.Port = remotePort;  
                }
                else if (int.TryParse(value, out tempInt) && tempInt == 0)
                    remoteSender.Address = IPAddress.Any;
      

            // Display some information
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("***************************************************");
            Console.WriteLine("Local port: " + localPort+"                                  *");
            Console.WriteLine("Remote ip: " + remoteSender.Address.ToString() + " : " + remoteSender.Port+"                          *");
            Console.WriteLine("Starting Upd receiving. (Press any key to quit)   *");
            Console.WriteLine("***************************************************");
            Console.ResetColor();
            

            // Create UDP client
            UdpClient client = new UdpClient(localPort);
            UdpState state = new UdpState(client, remoteSender);
            // Start async receiving
            client.BeginReceive(new AsyncCallback(DataReceived), state);

            // Wait for any key to terminate application
            Console.ReadKey();
            client.Close();
        }


        private static void DataReceived(IAsyncResult ar)
        {
            UdpClient c = (UdpClient)((UdpState)ar.AsyncState).c;
            IPEndPoint wantedIpEndPoint = (IPEndPoint)((UdpState)(ar.AsyncState)).e;
            IPEndPoint receivedIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Byte[] receiveBytes = c.EndReceive(ar, ref receivedIpEndPoint);

            // Check sender
            bool isRightHost = (wantedIpEndPoint.Address.Equals(receivedIpEndPoint.Address)) || wantedIpEndPoint.Address.Equals(IPAddress.Any);
            bool isRightPort = (wantedIpEndPoint.Port == receivedIpEndPoint.Port) || wantedIpEndPoint.Port == 0;
            if (isRightHost && isRightPort)
            {
                // Convert data to ASCII and print in console
              //  string receivedText = ASCIIEncoding.ASCII.GetString(receiveBytes);
                
                string receivedText ="";
                int lData = receiveBytes.Length;
                for (int i = 0; i < lData; i++)
                {
                    if (lData > 1)
                    {
                        //if (receiveBytes[i] == 0x2)
                        //{
                        //    if (receiveBytes[i - 1] == 0x10)
                        //    {
                        //        receivedText += "\n";
                        //    }
                        //}

                        if (receiveBytes[i] == 0x10)
                        {
                            if ((i + 1) < lData)
                            {
                                if (receiveBytes[i + 1] == 0x02)
                                {
                                    receivedText += "\n";
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                    receivedText += string.Format("{0:X2}", receiveBytes[i]);
                }
                
                Console.Write(receivedText);
            }

            // Restart listening for udp data packages
            c.BeginReceive(new AsyncCallback(DataReceived), ar.AsyncState);

        }



        public static string[] LocalIPAddress()
        {
            IPHostEntry host;
            string[] localIP;
            host = Dns.GetHostEntry(Dns.GetHostName());
            localIP = new string[host.AddressList.Length];
            int i = 0;
            foreach (IPAddress ip in host.AddressList)
            { 
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP[i++] = ip.ToString();
                }
            }
            return localIP;
        }
   
    }
}
