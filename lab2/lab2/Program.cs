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
			int capacity = 2;
			int processCount = 10;
			Queue queue = new Queue(capacity);
			ProcessFactory factory = new ProcessFactory(queue,processCount);
			CPU cpu1 = new CPU(queue);
			CPU cpu2 = new CPU(queue);
			new Thread(factory.Run).Start();
			new Thread(cpu1.Run).Start();
			new Thread(cpu2.Run).Start();
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
				generateDelay = queue.random.Next(1000, 2000);
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
		Queue queue;
		public CPU(Queue queue)
		{
			this.queue = queue;
		}
		public void Run()
		{
			int processTime=queue.random.Next(2000,6000);
			while(true)
			{
				try
				{
					if (queue.job == "done" && queue.queue.Count == 0)
						break;
					Console.WriteLine("CPU get process");
					queue.Get(processTime);
					Thread.Sleep(processTime);
					Console.WriteLine($": {processTime}");
				}
				catch(Exception)
				{
					Console.WriteLine("Exception");
				}
			}
			Console.WriteLine("Well done");

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
		AutoResetEvent emptyQueueEvent = new AutoResetEvent(false);
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
