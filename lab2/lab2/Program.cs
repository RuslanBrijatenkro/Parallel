using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lab2
{
	class Program
	{
		static void Main()
		{
			int cpuCount=4;
			CPU cpu;
			int capacity = 2;
			int processCount = 30;
			Queue queue = new Queue(capacity);
			ProcessFactory factory = new ProcessFactory(queue,processCount);
			new Thread(factory.Run).Start();
			for (int i = 0; i < cpuCount; i++)
			{
				cpu = new CPU(queue);
				Thread thread = new Thread(cpu.Run);
				thread.Name = $"{i}";
				thread.Start();
			}
			Console.ReadKey();
		}
	}
	class ProcessFactory
	{
		Queue queue;
		int processCount;
		int generateDelay;
		public ProcessFactory(Queue queue, int processCount)
		{
			this.queue = queue;
			this.processCount = processCount;
		}
		public void Run()
		{
			for(int i=0;i<processCount;i++)
			{
				generateDelay = queue.random.Next(100, 200);
				Thread.Sleep(generateDelay);
				Console.WriteLine($"Process generated with delay {generateDelay}");
				new Thread(()=>queue.Put("Process "+i)).Start();
			}
			queue.job = "done";
			Console.WriteLine("No more processes");
		}
	}
	class CPU
	{
		static List<int> processesDone = new List<int>();
		Queue queue;
		public CPU(Queue queue)
		{
			this.queue = queue;
			processesDone.Add(0);
		}
		public void Run()
		{
			int processTime=queue.random.Next(200,400);
			while(true)
			{
				try
				{
					if (queue.job == "done" && queue.queue.Count == 0)
						break;
					queue.Get(processTime);
					Thread.Sleep(processTime);
					processesDone[Convert.ToInt32(Thread.CurrentThread.Name)]++;
					Console.WriteLine($"CPU {Thread.CurrentThread.Name} time: {processTime}");
				}
				catch(Exception)
				{
					Console.WriteLine("Queue is empty");
				}
			}
			queue.emptyQueueEvent.Set();
			Console.WriteLine("Well done");
			Console.WriteLine($"{Thread.CurrentThread.Name}: "+processesDone[Convert.ToInt32(Thread.CurrentThread.Name)]*100/30d+"%");
		}
	}
	class Queue
	{
		public string job;
		object queueLock = new object();
		volatile bool full = false;
		volatile bool empty = true;
		public Random random = new Random();
		AutoResetEvent fullQueueEvent = new AutoResetEvent(false);
		public AutoResetEvent emptyQueueEvent = new AutoResetEvent(false);
		public List<string> queue= new List<string>();
		int capacity;
		public Queue(int capacity)
		{
			this.capacity = capacity;
		}
		public void Get(int processTime)
		{
			if(empty)
			{
				Console.WriteLine("Queue is empty");
				emptyQueueEvent.WaitOne();
			}
			Console.WriteLine($"CPU {Thread.CurrentThread.Name} get process");
			lock (queueLock)
				queue.RemoveAt(queue.Count-1);
 
			if (queue.Count==0)
				empty = true;

			if (full)
			{
				fullQueueEvent.Set();
				full = false;
			}			
		}
		public void Put(string process)
		{
			if (full)
			{
				Console.WriteLine($"Queue is full. Process {process} in waiting");
				fullQueueEvent.WaitOne();
			}

			lock(queueLock)
				queue.Add(process);
			Console.WriteLine($"{queue[queue.Count-1]} added to queue");

			if (queue.Count==capacity)
				full = true;

			if (empty)
			{
				empty = false;
				emptyQueueEvent.Set();
			}
		}
	}
}
