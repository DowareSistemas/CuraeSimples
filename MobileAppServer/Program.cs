using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;
using MobileAppServer.Model;
using Newtonsoft.Json;

namespace MobileAppServer
{
    public class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 4096 * 2;
        private static int PORT = 14555;
        private static readonly byte[] buffer = new byte[BUFFER_SIZE];
        private static int requests = 0;
        static void Main(string[] args)
        {
            Console.Title = "Doware Mobile App Server - " + Version;

            StartupServer();
        }

        private static void StartupServer()
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);

            Console.WriteLine("Server Port: " + PORT);
            Console.ReadKey();
        }


        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket;

            try
            {
                socket = serverSocket.EndAccept(AR);

                clientSockets.Add(socket);
                socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (Exception ex) // I cannot seem to avoid this (on exit when properly closing sockets)
            {
            }
        }

        private static bool isBusy = false;
        private static bool busyNotified = false;

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Stopwatch stopW = new Stopwatch();
            stopW.Start();
            try
            {
                Socket current = (Socket)AR.AsyncState;
                int received;

                try
                {
                    received = current.EndReceive(AR);
                }
                catch (SocketException)
                {
                    // Don't shutdown because the socket may be disposed and its disconnected anyway.
                    current.Close();
                    clientSockets.Remove(current);
                    return;
                }

                byte[] recBuf = new byte[received];
                Array.Copy(buffer, recBuf, received);
                string commandText = Encoding.Default.GetString(recBuf);
                string resultText = string.Empty;
                if (string.IsNullOrEmpty(commandText))
                {
                    current.Close();
                    clientSockets.Remove(current);
                    return;
                }

                Teste teste = new Teste()
                {
                    Id = 10,
                    Nome = "Testando resposta"
                };

                ResponseService response = new ResponseService();
                response.entity = JsonConvert.SerializeObject(teste);
                response.status = 600;
                response.message = "Servidor respondendo OK";

                resultText = JsonConvert.SerializeObject(response);
                Console.WriteLine(resultText);
                byte[] resultData = null;

                resultData = Encoding.UTF8.GetBytes(resultText);
                current.Send(resultData);
                current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);

            }
            catch (Exception ex)
            {

            }
        }

        public static string Version
        {
            get
            {
                return "1.1.10 - Build 170120";
            }
        }
    }

    public static class Compactor
    {
        public static string ToCompact(this string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        public static string FromCompact(this string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
