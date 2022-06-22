using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XOXOXOXOXOXO
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        char[] _arr;
        TcpClient client;
        string _ip;
        public string IP
        {
            get { return _ip; }
            set
            {
                _ip = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IP"));
            }
        }
        void ListenServer()
        {
            while (client.Connected)
            {
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                string message = reader.ReadLine();
                if (message != "")
                {

                    if (message[0] >= '0' && message[0] <= '9')
                    {
                        int index = message[0] - 48;
                        Arr[index] = message[1];
                        Arr = new char[9]
                        {
                            Arr[0], Arr[1],
                            Arr[2], Arr[3],
                            Arr[4], Arr[5],
                            Arr[6], Arr[7],
                            Arr[8]
                        };
                    }
                    else
                        MessageBox.Show(message);
                }
            }
        }
        public ButtonCommand ConnectClick
        {
            get
            {
                return new ButtonCommand(new Action<object>((obj)
              =>
                  {
                      client = new TcpClient(_ip, 8888);
                      Task task = new Task(() => ListenServer());
                      task.Start();
                  }));
            }
        }
        void SendMessage(object message)
        {
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine((string)message + "\n");
            writer.Flush();
        }
        public ButtonCommand Click
        {
            get { return new ButtonCommand((obj) => SendMessage(obj)); }
        }
        public ViewModel()
        {
            Arr = new char[9];
            IP = "127.0.0.1";
        }
        public char[] Arr
        {
            get { return _arr; }
            set
            {
                _arr = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Arr"));
            }
        }
    }
}
