using ExampleWebServer.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExampleWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Replace HelloWorld.html with HelloWorld.json, it'll read that file and send response.
            ////string s=ReadFile("HelloWorld.html");
            ////Console.WriteLine(s);
            ////Console.Read();

            TcpListener server = null;
            try
            {
                string connetionString = "Data Source=54.213.195.209;Initial Catalog=Example;User ID=example;Password=example";
                SqlConnection cnn = new SqlConnection(connetionString);
                try
                {
                    cnn.Open();
                    cnn.Close();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can not open data connection");
                    //Logging Connection exception
                    LogException(ex, "Can not open data connection", 0);
                }

                Int32 port = 80;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();

                Byte[] bytes = new Byte[1024];
                string data;

                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected");

                    data = null;
                    NetworkStream stream = client.GetStream();

                    int i = stream.Read(bytes, 0, bytes.Length);
                    while (i != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.Write("{0}", data);

                        string body = @"<html><body><b>Hello world</b></body></html>";
                        string response =
                                    @"HTTP/1.1 200 OK
                                    Server: Example
                                    Accept-Ranges: bytes
                                    Content-Length: " + body.Length.ToString() + @"
                                    Content-Type: text/html

                                    " + body;

                        ////if you want to read from file, uncomment the below code
                        //response = ReadFile("HelloWorld.html");

                        byte[] msg = Encoding.ASCII.GetBytes(response);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", response);
                        i = stream.Read(bytes, 0, bytes.Length);
                    }


                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
        public static string DownloadUrl(string url)
        {
            return new WebClient().DownloadString(url);
        }
        public static void LogException(Exception e, string logEvent, int userId)
        {
            BaseLogger.LogException(userId, logEvent, e);
        }
        public static string GetApplicationPath(string fileName)
        {
            string cwd = Directory.GetCurrentDirectory();

            if (cwd.EndsWith("\\bin\\Debug"))
            {
                cwd = cwd.Replace("\\bin\\Debug", "\\MyPage\\" + fileName);
            }
            return cwd;
        }
        public static string ReadFile(string fileName)
        {
            string line;
            string file = GetApplicationPath(fileName);
            using (StreamReader reader = new StreamReader(file))
            {
                line = reader.ReadToEnd();
            }
            return line;
        }
    }
}

