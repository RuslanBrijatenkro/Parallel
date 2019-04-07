using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace lab5
{
	class Program
	{
		static void Main(string[] args)
		{
			startSwitch:
			Console.WriteLine("1-Server, 2-Client");
			switch(Console.ReadLine())
			{
				case "1":
					Server server = new Server();
					server.Run();
					break;
				case "2":
					Client client = new Client();
					client.Run();
					break;
				default:
					Console.WriteLine("Uncorrect number");
					goto startSwitch;
			}
			Console.ReadKey();
		}
	}
	class Client
	{
		public void Run()
		{
			IPHostEntry ipHost = Dns.GetHostEntry("localhost");
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
			try
			{
				TcpListener listener = new TcpListener(ipEndPoint);
				listener.Start(1);

				while (true)
				{
					Console.WriteLine("Client waiting: {0}", listener.LocalEndpoint);
					TcpClient client = listener.AcceptTcpClient();
					NetworkStream io = client.GetStream();
					byte[] bytes = new byte[1024];
					io.Read(bytes,0,bytes.Length);
					string data = null;
					data += Encoding.ASCII.GetString(bytes);
					Console.WriteLine(data);
					Console.WriteLine("End");
					client.Close();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Произошла ошибка {0}", e.Message);
			}
		}
	}
	class Server
	{
		static IPHostEntry ipHost = Dns.GetHostEntry("localhost");
		static IPAddress ipAddr = ipHost.AddressList[0];
		IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
		public void Run()
		{

			Socket serverSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				serverSocket.Bind(ipEndPoint);
				serverSocket.Listen(10);
				while(true)
				{
					Console.WriteLine("Waiting for client...");
					Socket listener = serverSocket.Accept();
					byte[] message = Encoding.ASCII.GetBytes("Connection successful!");
					listener.Send(message);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
		async void SendMessageAsync()
		{
			await Task.Run(() => SendMessage());
		}
		void SendMessage()
		{
			TcpClient server = new TcpClient(ipEndPoint);
			server.Connect(ipAddr,11000);
			while(true)
			{
				NetworkStream write = server.GetStream();
				Console.WriteLine("Enter message:");
				string msg = Console.ReadLine();
				byte[] message = Encoding.ASCII.GetBytes(msg);
				write.Write(message,0,message.Length);

			}
		}
	}
}
