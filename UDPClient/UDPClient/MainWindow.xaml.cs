using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Microsoft.Win32;

namespace UDPClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private long onBlockSize;
        public MainWindow()
        {
            InitializeComponent();

            onBlockSize = 1500;// MTU = 1500 не больше 64 т.
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter= "Text files(*.txt)|*.txt|All files(*.*)|*.*";

            if (openFileDialog.ShowDialog()==true)
            {
                FileNameTextBox.Text = openFileDialog.FileName;
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            EndPoint serverEndPoint = new IPEndPoint
                (IPAddress.Parse(ServerIpTextBox.Text), 
                int.Parse(PortTextBox.Text));//Чтение файла

            using (FileStream inputFile = new FileStream(FileNameTextBox.Text, FileMode.Open))
            {
                using (Socket serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram, ProtocolType.Udp))
                {
                    long fileSize = inputFile.Length;
                    long countRead = fileSize / onBlockSize;
                    long RemaindSize = fileSize % onBlockSize;
                    byte[] buf = new byte[onBlockSize];

                    //уведомить получателя о количестве частей файла
                    int countSend = (int)countRead;

                    if (RemaindSize > 0)
                        RemaindSize++;

                    serverSocket.SendTo(BitConverter.GetBytes(countSend), serverEndPoint);

                    for (long i = 0; i < countRead; i++)
                    {
                        inputFile.Read(buf, 0, (int)onBlockSize);
                        serverSocket.SendTo(buf, 0, serverEndPoint);
                        Thread.Sleep(10);
                    }

                    if (RemaindSize != 0)
                    {
                        inputFile.Read(buf, 0, (int)RemaindSize);
                        serverSocket.SendTo(buf, 0, (int)RemaindSize, SocketFlags.None, serverEndPoint);
                    }
                }
            }
        }

    }
}
