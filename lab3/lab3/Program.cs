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

		static long min;
		static long max;
		static int indexMin;
		static int indexMax;

		public Algorithms()
		{
			intMas = new int[random.Next(5, 10)];
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
				longMas[i] = random.Next(100000, 100000000);
			}
			Console.WriteLine(Interlocked.CompareExchange(ref temporaryXOR, 3, 2));
			
			//XOR
			Parallel.For(0, intMas.Length, ArrayXOR);
			Console.WriteLine("Result: "+temporaryXOR);
			foreach (var el in intMas)
			{
				Console.WriteLine(Convert.ToString(el, 2) + " ");
			}

			////MINMAX
			//Parallel.For(0, longMas.Length, MinMaxValues);
			//Console.WriteLine("Max: " + max + " Index: " + indexMax);
			//Console.WriteLine("Min: " + min + " Index: " + indexMin);
			//Console.WriteLine("Max: " + longMas.Max() + " Min: " + longMas.Min());

			////LEnght
			//Parallel.Invoke(ArrayLenght);
			//Console.WriteLine(lenght);
			//Console.WriteLine(intMas.Length);
			
		}
		static void ArrayLenght()
		{
			while(true)
			{
				try
				{
					long oldValue, newValue;
					do
					{
						oldValue = lenght;
						newValue = Interlocked.Increment(ref lenght);
						intMas[newValue]++;
					}
					while (oldValue != Interlocked.CompareExchange(ref lenght, newValue, oldValue));
				}
				catch(Exception e)
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
			if(x==0)
			{
				Interlocked.Exchange(ref max, longMas[x]);
				indexMax = x;
				Interlocked.Exchange(ref min, longMas[x]);
				indexMin = x;
			}
			if (Interlocked.Read(ref longMas[x]) > Interlocked.Read(ref max))
			{
				Interlocked.Exchange(ref max, longMas[x]);
				indexMax = x;
			}
			if (Interlocked.Read(ref longMas[x]) < Interlocked.Read(ref min))
			{
				Interlocked.Exchange(ref min, longMas[x]);
				indexMin = x;
			}
		}

	}
}
