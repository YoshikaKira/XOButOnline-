using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace HOHOServ
{
    class Program
    {
        static TcpClient clientX;
        static TcpClient clientO;
        static TcpListener listner;
        static bool isCross = true;
        static int[] Arr;
        static bool endGame = false;
        static int count = 0;
        static void SendMessage(string message, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(message+"\n");
            writer.Flush();
            //writer.Close();
        }
        static bool CheckWinner()
        {
            if (Arr[0] == Arr[1] && Arr[1] == Arr[2] && Arr[0] != 0) return true;
            if (Arr[3] == Arr[4] && Arr[4] == Arr[5] && Arr[3] != 0) return true;
            if (Arr[6] == Arr[7] && Arr[7] == Arr[8] && Arr[6] != 0) return true;

            if (Arr[0] == Arr[3] && Arr[3] == Arr[6] && Arr[0] != 0) return true;
            if (Arr[1] == Arr[4] && Arr[4] == Arr[7] && Arr[1] != 0) return true;
            if (Arr[2] == Arr[5] && Arr[5] == Arr[8] && Arr[2] != 0) return true;

            if (Arr[0] == Arr[4] && Arr[4] == Arr[8] && Arr[0] != 0) return true;
            if (Arr[2] == Arr[4] && Arr[4] == Arr[6] && Arr[2] != 0) return true;

            return false;
        }

        static void ListenClient(TcpClient client)
        {
            while (!endGame)
            {
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                string message = reader.ReadLine();
                if (message != "")
                {
                    int index = Convert.ToInt32(message);
                    if (client == clientX)
                    {
                        if (isCross)
                        {
                            if (Arr[index] == 0)
                            {
                                Arr[index] = 1;
                                SendMessage(message + "X", clientO);
                                SendMessage(message + "X", clientX);
                                isCross = false;
                                count++;
                            }
                        }
                    }
                    else
                    {
                        if (client == clientO)
                        {
                            if (!isCross)
                            {
                                if (Arr[index] == 0)
                                {
                                    Arr[index] = 2;
                                    SendMessage(message + "O", clientX);
                                    SendMessage(message + "O", clientO);
                                    isCross = true;
                                    count++;
                                }
                            }
                        }
                    }
                }
                if (CheckWinner())
                {
                    if (isCross)
                    {
                        SendMessage("chel...", clientX);
                        SendMessage("Good job", clientO);

                    }
                    else if (count >= 9)
                    {
                        SendMessage("Draw", clientX);
                        SendMessage("Draw", clientO);
                    }
                    else
                    {
                        SendMessage("Good job", clientX);
                        SendMessage("chel...", clientO);
                    }
                    endGame = true;
                }
                
            }
        }
        static void Main(string[] args)
        {
             listner = new TcpListener(IPAddress.Any, 8888);
            while (true)
            {
                count = 0;
                Arr = new int[9];             
                listner.Start();
                clientX = listner.AcceptTcpClient();
                SendMessage("You game for X. Ogidaite", clientX);
                clientO = listner.AcceptTcpClient();
                SendMessage("You game for O. GG", clientO);
                SendMessage("GG. X turn", clientX);
                SendMessage("GG. O turn", clientO);
                Task taskX = new Task(() => ListenClient(clientX));
                Task taskO = new Task(() => ListenClient(clientO));
                taskX.Start();
                taskO.Start();
                while (!endGame) ;
                listner.Stop();
                Thread.Sleep(1000);
                endGame = true;
            }
        }
    }
}
