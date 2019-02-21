#define VERSIONING 
#define VERSIONING 
#define VERSIONING 
#define VERBOSE
/*
The MIT License(MIT)
Copyright(c) mxgmn 2016.
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
The software is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the software or the use or other dealings in the software.
*/

using System;
using System.Xml.Linq;
using System.Diagnostics;

static class Program
{
	static void Log(string s)
	{
		#if (VERBOSE)
		Console.WriteLine(s);
		#endif
	}

	static void Main()
	{
#if (VERSIONING)
	Console.WriteLine("Version number: {0}", Versioning.VersionNumber);
#else
	Console.WriteLine("There seems to be no versioning available.");
#endif

		Stopwatch sw = Stopwatch.StartNew();

		Random random = new Random();
		XDocument xdoc = XDocument.Load("samples.xml");

		int counter = 1;
		foreach (XElement xelem in xdoc.Root.Elements("overlapping", "simpletiled"))
		{
			Model model;
			string name = xelem.Get<string>("name");
			Log($"Now processing picture <{name}>");

			if (xelem.Name == "overlapping")
			{
				model = new OverlappingModel(
					name,
					xelem.Get("N", 2),
					xelem.Get("width", 48),
					xelem.Get("height", 48),
					xelem.Get("periodicInput", true),
					xelem.Get("periodic", false),
					xelem.Get("symmetry", 8),
					xelem.Get("ground", 0));
				Log("\tProcessing as overlapping model");
			}

			else if (xelem.Name == "simpletiled")
			{
				model = new SimpleTiledModel(
					name,
					xelem.Get<string>("subset"),
					xelem.Get("width", 10),
					xelem.Get("height", 10),
					xelem.Get("periodic", false),
					xelem.Get("black", false));
				Log("\tProcessing as simpletiled model");
			}
			else
			{
				Log("\tDid not find a valid model. Moving on to the next picture..");
				continue;
			}

			for (int i = 0; i < xelem.Get("screenshots", 2); i++)
			{
				for (int k = 0; k < 10; k++)
				{
					int seed = random.Next();
					Log($"\tFor process #{i+1} the random seed is {seed}");
					bool finished = model.Run(seed, xelem.Get("limit", 0));
					if (finished)
					{
						Log("\t-->SUCCESS<--");

						model.Graphics().Save($"{counter} {name} {i}.png");

						Log($"\tSaving result with name {counter} {name} {i}.png");
						if (model is SimpleTiledModel && xelem.Get("textOutput", false))
						{
							System.IO.File.WriteAllText($"{counter} {name} {i}.txt", (model as SimpleTiledModel).TextOutput());
						}

						break;
					}
					else {
						Log("\t-->CONTRADICTION<--");
						continue;
					}
				}
			}

			counter++;
		}
		Log($"time = {sw.ElapsedMilliseconds}");

        Console.ReadLine();

    }
}
