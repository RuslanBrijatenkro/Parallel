using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace lab4
{
	class Algorithms
	{
		int listsCount = 10;
		Random random = new Random();
		List<int> collection1 = new List<int>();
		List<int> collection2 = new List<int>();
		List<int> collection3 = new List<int>();
		async public void RunAsync()
		{
			Task t1 = Task.Run(() => FillList(collection1));
			Task t2 = Task.Run(() => FillList(collection2));
			Task t3 = Task.Run(() => FillList(collection3));
			await Task.WhenAll(new[] { t1, t2, t3 });

			Console.WriteLine("Collection1: ");
			await Task.Run(() => GetArray(collection1));
			Console.WriteLine("Collection2: ");
			await Task.Run(() => GetArray(collection2));
			Console.WriteLine("Collection3: ");
			await Task.Run(() => GetArray(collection3));

			t1 = Task.Run(() => Multiplication());
			t2 = Task.Run(() => LeaveEven());
			t3 = Task.Run(() => AvarageValues());
			await Task.WhenAll(new[] { t1, t2, t3 });

			Console.WriteLine("Collection1: ");
			await Task.Run(() => GetArray(collection1));
			Console.WriteLine("Collection2: ");
			await Task.Run(() => GetArray(collection2));
			Console.WriteLine("Collection3: ");
			await Task.Run(() => GetArray(collection3));

			t1 = Task.Run(() => collection1.Sort());
			t2 = Task.Run(() => collection2.Sort());
			t3 = Task.Run(() => collection3.Sort());
			await Task.WhenAll(new[] { t1, t2, t3 });
		}
		void GetArray(List<int> collection)
		{
			for(int i=0;i<collection.Count;i++)
			{
				Console.Write(collection[i] + " ");
			}
			Console.WriteLine();
		}
		void FillList(List<int> collection)
		{
			Thread.Sleep(100);
			for(int i=0;i<listsCount;i++)
				collection.Add(random.Next(1,100));
		}
		void Multiplication()
		{
			for(int i=0;i<collection1.Count;i++)
			{
				collection1[i]*=3;
			}
		}
		void LeaveEven()
		{
			int collectionCount = collection2.Count;
			for(int i=0;i<collectionCount;)
			{
				if (collection2[i] % 2 != 0)
				{
					collection2.RemoveAt(i);
					collectionCount--;
					continue;
				}
				i++;
			}
		}
		void AvarageValues()
		{
			double avarage=collection3.Sum()/(double)collection3.Count;
			Console.WriteLine("Avarage: "+avarage);
			int collectionCount = collection3.Count;
			for (int i = 0; i < collectionCount;)
			{
				if (collection3[i] < 0.8 * avarage || collection3[i] > 1.2 * avarage)
				{
					collection3.RemoveAt(i);
					collectionCount--;
					continue;
				}
				i++;
			}
		}
	}
}
