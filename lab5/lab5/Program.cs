using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace lab5
{
	class Program
	{
		static void Main(string[] args)
		{
			Thread thread;
			startSwitch:
			Console.WriteLine("1-Server, 2-Client");
			switch(Console.ReadLine())
			{
				case "1":
					Server server = new Server();
					thread = new Thread(server.Run);
					thread.Start();
					break;
				case "2":
					Client client = new Client();
					thread = new Thread(client.Run);
					thread.Start();
					break;
				default:
					Console.WriteLine("Uncorrect number");
					goto startSwitch;
			}
			thread.Join();
		}
	}
	class Client
	{
		public string id;
		TcpClient client;
		NetworkStream stream;
		public void ReadMessage()
		{
			try
			{
				while(true)
				{
					Thread.Sleep(50);
					if(stream.DataAvailable)
					{
						byte[] message = new byte[64];
						string data = null;
						while (stream.DataAvailable)
						{
							stream.Read(message, 0, message.Length);
							data += Encoding.ASCII.GetString(message);
						}
						Console.WriteLine("Message from server: "+data);
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				if (stream != null)
					stream.Close();
				if (client != null)
					client.Close();
			}
		}
		public void Run()
		{
			client = new TcpClient("localhost",11000);
			try
			{
				id = Guid.NewGuid().ToString();
				Console.WriteLine(id);
				stream = client.GetStream();
				byte[] message = Encoding.ASCII.GetBytes(id);
				stream.Write(message, 0, message.Length);
				Task.Factory.StartNew(ReadMessage);
				while(Console.ReadLine()!="end")
					Console.WriteLine("Entry 'end' to exit");
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				if(stream!=null)
					stream.Close();
				if (client != null)
					client.Close();
			}
		}
	}
	class ClientObject
	{
		public string id;
		NetworkStream stream;
		public ClientObject(NetworkStream stream)
		{
			this.stream = stream;
		}
		public void Run()
		{
			id = null;
			byte[] buffer = new byte[32];
			stream.Read(buffer, 0, buffer.Length);
			id += Encoding.ASCII.GetString(buffer);
			Console.WriteLine(id);
			WriteId(id);
		}
		public void SendMessage(string message)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(message);
			stream.Write(buffer,0,buffer.Length);
		}
		void WriteId(string id)
		{
			using (StreamWriter streamWriter = new StreamWriter(@"C:\Users\brija\Desktop\'.net'\Parallel\lab5\lab5\id.txt",true))
			{
				streamWriter.Write(id+" ");
			}
		}
	}
	class Server
	{
		List<ClientObject> clientObjects;
		NetworkStream stream;
		TcpListener tcpListener;
		TcpClient client;
		public void Run()
		{
			try
			{
				clientObjects = new List<ClientObject>();
				tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 11000);
				tcpListener.Start();
				ListenerAsync();
				while(true)
				{
					client=tcpListener.AcceptTcpClient();
					stream = client.GetStream();
					ClientObject clientObject = new ClientObject(stream);
					clientObjects.Add(clientObject);
					Task.Factory.StartNew(clientObject.Run);
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				if (stream != null)
					stream.Close();
				if (client != null)
					client.Close();
			}

		}
		async void ListenerAsync()
		{
			await Task.Factory.StartNew(Listener);
		}
		void Listener()
		{
			while(true)
			{
				Console.WriteLine("Entry message: ");
				string message = Console.ReadLine();
				string data = null;
				using (StreamReader streamReader = new StreamReader(@"C:\Users\brija\Desktop\'.net'\Parallel\lab5\lab5\id.txt"))
				{
					while(!streamReader.EndOfStream)
					{
						data=streamReader.ReadToEnd();
					}
				}
				foreach (var clientObject in clientObjects)
				{
					if(data.Contains(clientObject.id))
						clientObject.SendMessage(message);
				}
			}
		}
	}
}
