using Dfc.DiscoverSkillsAndCareers.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Dfc.UnitTests
{
    public class SessionIdTests
    {
        private readonly ITestOutputHelper Output;

        public SessionIdTests(ITestOutputHelper output)
        {
            Output = output;
        }
        
        
    }
    
    public class ReloadCodeGenerationTests
    {
        private readonly ITestOutputHelper testOutput;

        public ReloadCodeGenerationTests(ITestOutputHelper testOutput)
        {
            this.testOutput = testOutput;
        }

        [Fact]
        public void CanGenerate()
        {
            var idGen = new ReloadCodeGenerator(new [] { "cat", "sat", "mat" });
            var result = idGen.Generate();
            
            testOutput.WriteLine(result);
        }

        [Fact]
        public void CanGenerateUniqueSetsInASingleThread()
        {
            
            var idGen = new ReloadCodeGenerator(File.OpenRead("data\\dictionary.txt"));
            var list = new List<string>();
            
            for(var i = 0; i < 100000; i++)
            {
                list.Add(idGen.Generate());
            }

            Assert.Equal(100000, list.Distinct().Count());
        }

        [Fact]
        public async void CanGenerateAppropriatelyUniqueSetsInMultipleThreads()
        {
            
            var idGen = new ReloadCodeGenerator(File.OpenRead("data\\dictionary.txt"));
            var list = new System.Collections.Concurrent.ConcurrentDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            
            var taskCollection = new Task[Environment.ProcessorCount];
            var perThreadIterations = 2500000;
            
            var dupes = 0.0;
            for(var t = 0; t < Environment.ProcessorCount; t++) 
            {
                taskCollection[t] = Task.Run(() => {    
                    for(var i = 0; i < perThreadIterations; i++)
                    {
                        var x = idGen.Generate();
                        if(!list.TryAdd(x, null))
                        {
                            dupes++;
                        }
                    }
                });
            }

            Task.WaitAll(taskCollection);

            using(var fs = File.OpenWrite(@"D:\example3words.txt"))
            using(var sw = new StreamWriter(fs))
            {
                foreach(var kvp in list)
                {
                    sw.WriteLine(kvp.Key);
                }
            }

            testOutput.WriteLine($"Duplicates: {dupes} with dictionary size {idGen.DictionarySize}");

            Assert.InRange(dupes, 0.0, 10.0);
        }
    }
}
