using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace UDPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] buf = new byte[64*1024];//Максимальный размер доступный для UDP
            long fullSize = 0;
            int countFileBlock = 0;
            int port = 12345;
            byte[] countFileBlockBytes = new byte[8];//Получчение количетсва файлов для приема по UDP

            Socket serverSocket = new Socket
                (AddressFamily.InterNetwork, 
                SocketType.Dgram, ProtocolType.Udp);

            EndPoint sourceEndPoint = new IPEndPoint(0, 0);

            FileStream outFile = new FileStream("outputFile.bin", FileMode.Create);

            EndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, port);
            serverSocket.Bind(serverEndPoint);

            int size = serverSocket.ReceiveFrom(countFileBlockBytes, ref sourceEndPoint);
            countFileBlock = BitConverter.ToInt32(countFileBlockBytes, 0);//Чтение частей файла

            Console.WriteLine($"Count file block {countFileBlock}");

            int i = 0;
            while (countFileBlock > 0)
            {
                size = serverSocket.ReceiveFrom(buf, ref sourceEndPoint);
                Console.WriteLine($"Received size = {size}");
                outFile.Write(buf, 0, size);
                fullSize += size;
                Console.WriteLine($"{i}Write size = {fullSize}");
                i++;
                countFileBlock--;
            }
            
        }
    }
}
