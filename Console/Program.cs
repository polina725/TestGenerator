using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ConsoleApp
{
    class Program
    {
        private static string outputFolderPath = "..\\..\\..\\Output";
        private static string inputFolderParh = "..\\..\\..\\Input";

        static void Main(string[] args)
        {
            Generator.Generator gen = new Generator.Generator(outputFolderPath);
            Generator.Generator gen1 = new Generator.Generator(outputFolderPath);
            ExecutionDataflowBlockOptions options = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 };
            TransformBlock<string,string> getInputCode = new TransformBlock<string, string>
            (
                async filePath => await File.ReadAllTextAsync(filePath),
                options
            );            
            
            TransformManyBlock<string,Generator.GeneratedFileInfo> createTests = new TransformManyBlock<string, Generator.GeneratedFileInfo>
            (
                async sourceCode => await Task.Run( ()=>gen.AnalyseFile(sourceCode).ToArray()),
                options
            );
            
            ActionBlock<Generator.GeneratedFileInfo> saveTests = new ActionBlock<Generator.GeneratedFileInfo>
            (
                async testsFile => { await File.WriteAllTextAsync(testsFile.FullFileName, testsFile.SourceCode); },
                options
            );

            DataflowLinkOptions linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            getInputCode.LinkTo(createTests, linkOptions);
            createTests.LinkTo(saveTests, linkOptions);

            string[] filePaths = Directory.GetFiles(inputFolderParh);
           
            foreach(string filePath in filePaths)
            {
                if (filePath.EndsWith(".cs"))
                    getInputCode.Post(filePath);
            }
            getInputCode.Complete();
           saveTests.Completion.Wait();
        }
    }
}
