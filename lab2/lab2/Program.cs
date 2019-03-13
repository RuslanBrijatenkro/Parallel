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
		static void Main(string[] args)
		{

		}
	}
	class CPUProcess
	{
		Random random = new Random();
		CPUQueue queue;
		int processCount;
		int generateDelay;
		CPUProcess(CPUQueue queue, int processCount)
		{
			this.queue = queue;
			this.processCount = processCount;
		}
		public void Run()
		{
			for(int i=0;i<processCount;i++)
			{
				generateDelay = random.Next(1000, 4000);
				Thread.Sleep(generateDelay);
				Console.WriteLine("");
			}
		}
	}
	class CPU
	{

	}
	class CPUQueue
	{
		AutoResetEvent fullQueueEvent = new AutoResetEvent(false);
		public List<string> queue = new List<string>();
		int capacity;
		int maxSize = 0;
		CPUQueue(int capasity)
		{
			this.capacity = capacity;
		}
		public void Get()
		{
			if(queue.Count==0)
			{
				Console.WriteLine("Queue is empty");
			}
			
		}
		public void Put(string thread)
		{
			if (queue.Count == capacity)
			{
				Console.WriteLine($"Queue is full. Thread {thread} in waiting");
				fullQueueEvent.WaitOne();
			}
			if (queue.Count < capacity)
				queue.Add(thread);
		}
	}
}
