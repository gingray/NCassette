NCassette
=======

NCassette its a tool specially for debugging purposes but u can use it how ever u want.
It can helps to 'mock' or 'caching' heavy parts of ur algorithms during debugging.

An attempt to create some kind of VCR in Ruby or Betamax in Java, but main idea not quite the same
lib record the answer from service not trying to intercept the connection or something like that.
Thats mean that ucan record not only some network manipulation but what ever u want and its very simple
and light weigth, no additional dependencies.
Example below:


						
		static void Main(string[] args)
		{
			var url = "http://google.com";
			var result = NCassette.Record(() =>
			{
			//some very heavy manipulation here
			var networkRespone = NetworkResponse();
			var someClass = new CustomClass {SomeStringProperty = networkRespone};
			return someClass;
			})
			.SerializeWayJson()
			.WorkInReleaseMode()
			.DependsOn(url)
			.StorageInTempFiles()
			.Execute();
			Console.WriteLine("{0}", result.SomeStringProperty);
			Console.WriteLine("Done");
			Console.ReadLine();
		}

The code in lambda wil execute only once all other calls will be cached.

