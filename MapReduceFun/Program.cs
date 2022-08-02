
using System.Collections;
using MapReduceFun;
using System.Diagnostics;

namespace MapReduceFun
{
	public static class Program
	{

		public static void Main(string[] args)
		{
			User user = new User();
			Master master = new Master();

			//string[] input = new String[5] {"Det er det", "Det er det", "Det er det", "Det er det", "Det er det"};

			RandomText randomText = new RandomText();

			string[] input = randomText.RandomTextArray(10, 5000);

			master.Execute(input, user.MyMapFunction, user.MyReduceFunction, 5);
		}
	}


	public class User
	{
		public List<KeyValuePair<string, int>> MyMapFunction(string input)
		{
			string[] inputMadeIntoArray = input.Split(" ");
			List<KeyValuePair<string, int>> output = new();
			foreach (var word in inputMadeIntoArray)
			{
				KeyValuePair<string, int> res = new KeyValuePair<string, int>(word, 1);
				output.Add(res);
			}

			return output;
		}

		public string MyReduceFunction(string input)
		{
			return "";
		}
	}
	
	

	public class Master
	{
		static Stack<Worker> _workers = new();
		static Queue<String> _unfinishedJobQueue = new();
		static List<KeyValuePair<string, int>> _intermediateResults = new();
		
		public void Execute(string[] input, Func<string, List<KeyValuePair<string, int>>> userMap, Func<string, string> userReduce, int amountOfWorkers)
		{
			_workers = new Stack<Worker>();
			_unfinishedJobQueue = new Queue<String>();

			foreach (var job in input)
			{
				_unfinishedJobQueue.Enqueue(job);
			}

			for (int i = 0; i < amountOfWorkers; i++)
			{
				_workers.Push(new Worker());
			}
			
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			MasterWorker(userMap, userReduce);
			stopwatch.Stop();
			//PrintIntermediateResult();
			Console.WriteLine("Time taken: " + stopwatch.Elapsed + "\n");
		}


		public void MasterWorker(Func<string, List<KeyValuePair<string, int>>> userMap, Func<string, string> userReduce)
		{
			while (_unfinishedJobQueue.Count != 0)
			{
				if (_workers.Count != 0)
				{
					_workers.Pop().Work(userMap);
				}

				//TODO: do reducing properly
				if (_unfinishedJobQueue.Count != 0)
				{
					//_workers.Pop().Work(userReduce);
				}
			}
		}

		public void PrintIntermediateResult()
		{
			foreach (var kvPair in _intermediateResults)
			{
				Console.Out.WriteLine(kvPair.Key + " , " +  kvPair.Value);
			}
		}
			
		
		class Worker
		{
			public void Work(Func<string, List<KeyValuePair<string, int>>> userFunc)
			{
				List<KeyValuePair<string, int>> res = new List<KeyValuePair<string, int>>();
				string job = _unfinishedJobQueue.Dequeue();
				try
				{
					
					Task<List<KeyValuePair<string, int>>> task = new Task<List<KeyValuePair<string, int>>>(() => userFunc(job));
					Console.WriteLine(task.Id + " <--- Task ID");
					task.Start();
					res = task.Result;

					//res = userFunc(job);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					_unfinishedJobQueue.Enqueue(job);
				}

				foreach (var result in res)
				{
					_intermediateResults.Add(result);
				}
				_workers.Push(this);
			}
		}
	}
}