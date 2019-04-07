using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace lab3
{
	class Program
	{
		static void Main(string[] args)
		{
			Algorithms algorithms = new Algorithms();
			algorithms.Run();
			Console.ReadKey();

		}
	}
	class Algorithms
	{
		static int temporaryXOR = 0;
		Random random = new Random();
		static int[] intMas;
		static long[] longMas;

		static long lenght = 0;

		static long min=100000000;
		static long max= -100000;
		static int indexMin;
		static int indexMax;

		public Algorithms()
		{
			intMas = new int[random.Next(50, 100)];
			longMas = new long[random.Next(5000, 5001)];
		}
		public void Run()
		{
			for(int i=0;i<intMas.Length;i++)
			{
				intMas[i] = random.Next(1,5);
			}
			for (int i = 0; i < longMas.Length; i++)
			{
				longMas[i] = random.Next((int)max, (int)min);
			}

			//XOR
			//Parallel.For(0, intMas.Length, ArrayXOR);
			//Console.WriteLine("Result: "+temporaryXOR);
			//foreach (var el in intMas)
			//{
			//	Console.WriteLine(Convert.ToString(el, 2) + " ");
			//}

			////MINMAX
			//Parallel.For(0, longMas.Length, MinMaxValues);
			//Console.WriteLine("Max: " + max + " Index: " + indexMax);
			//Console.WriteLine("Min: " + min + " Index: " + indexMin);
			//Console.WriteLine("Max: " + longMas.Max() + " Min: " + longMas.Min());

			//LEnght
			Parallel.Invoke(ArrayLength);
			Console.WriteLine(lenght);
			Console.WriteLine(intMas.Length);

		}
		static void ArrayLength()
		{
			while(true)
			{
				try
				{
					long oldValue, newValue;
					do
					{
						oldValue = Interlocked.Read(ref lenght);
						newValue = oldValue++;
						intMas[oldValue]++;
					}
					while (oldValue != Interlocked.CompareExchange(ref lenght, newValue, oldValue));
				}
				catch(IndexOutOfRangeException)
				{
					break;
				}
			}
			Console.WriteLine("Thread end work");
		}
		static void ArrayXOR(int x)
		{
			int oldValue, newValue;
			do
			{
				oldValue = temporaryXOR;
				newValue = oldValue ^ intMas[x];
			}
			while (oldValue != Interlocked.CompareExchange(ref temporaryXOR, newValue, oldValue));
		}
		static void MinMaxValues(int x)
		{
			long oldValueMax=0, newValueMax=0, oldValueMin = 0, newValueMin = 0;
			do
			{
				Interlocked.Exchange(ref oldValueMin, min);
				Interlocked.Exchange(ref newValueMin, longMas[x]);
				if (Interlocked.Read(ref newValueMin) >= Interlocked.Read(ref oldValueMin))
					break;
				indexMin = x;
			}
			while (Interlocked.Read(ref oldValueMin) != Interlocked.CompareExchange(ref min, newValueMin, oldValueMin));

			do
			{
				Interlocked.Exchange(ref oldValueMax, max);
				Interlocked.Exchange(ref newValueMax, longMas[x]);
				if (Interlocked.Read(ref newValueMax) <= Interlocked.Read(ref oldValueMax))
					break;
				indexMax = x;
			}
			while (Interlocked.Read(ref oldValueMax) != Interlocked.CompareExchange(ref max, newValueMax, oldValueMax));
		}

	}
}
