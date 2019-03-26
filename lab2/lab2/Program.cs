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
			int processCount = 30;
			Queue queue = new Queue(capacity);
			ProcessFactory factory = new ProcessFactory(queue,processCount);
			CPU cpu1 = new CPU(queue);
			CPU cpu2 = new CPU(queue);
			CPU cpu3 = new CPU(queue);
			CPU cpu4 = new CPU(queue);
			new Thread(factory.Run).Start();
			//for(int i=0;i<4;i++)
			//{
			//	Thread thread = new Thread(new CPU(queue).Run);
			//	thread.Name = $"{i}";
			//	thread.Start(
			//}
			Thread thread1 = new Thread(cpu1.Run);
			Thread thread2=new Thread(cpu2.Run);
			Thread thread3=new Thread(cpu3.Run);
			Thread thread4=new Thread(cpu4.Run);
			thread1.Name = "1";
			thread2.Name = "2";
			thread3.Name = "3";
			thread4.Name = "4";
			thread1.Start();
			thread2.Start();
			thread3.Start();
			thread4.Start();
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
					processesDone[Convert.ToInt32(Thread.CurrentThread.Name) - 1]++;
					Console.WriteLine($"{Thread.CurrentThread.Name}: {processTime}");
				}
				catch(Exception)
				{
					Console.WriteLine("Exception");
				}
			}
			queue.emptyQueueEvent.Set();
			Console.WriteLine("Well done");
			Console.WriteLine($"{Thread.CurrentThread.Name}: "+processesDone[Convert.ToInt32(Thread.CurrentThread.Name) - 1]*100/30d+"%");
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
