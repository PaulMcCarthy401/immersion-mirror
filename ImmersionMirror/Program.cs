using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImmersionMirror
{
    class Program
    {
        const bool dryRun = false;
        const string inDir = @"./InDir/";
        const string outDir = @"./OutDir/";

        static void Main(string[] args)
        {
            Console.WriteLine("Creating initial directories...");
            CreateInitialDirectories();

            Console.WriteLine($"Input directory: {Path.GetFullPath(inDir)}");
            Console.WriteLine($"Output directory: {Path.GetFullPath(outDir)}");

            var watchTask = Task.Run(async () => await Watch(inDir));

            Console.WriteLine($"Listening for file changes in {inDir}");

            watchTask.Wait();
        }

        private static async Task Watch(string inDir)
        {
            while (true)
            {
                if (!IsDirectoryEmpty(inDir))
                {
                    var files = Directory.GetFiles(inDir, "*", SearchOption.AllDirectories);

                    foreach (var file in files)
                    {
                        Console.WriteLine("Processing " + file);
                        OnFileChanged(file);
                    }
                }

                await Task.Delay(5000);
            }
        }

        private static bool IsFileReady(string filename)
        {
            // If the file can be opened for exclusive access it means that the file
            // is no longer locked by another process.
            try
            {
                using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                    return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void CreateInitialDirectories()
        {
            Directory.CreateDirectory(inDir);
            Directory.CreateDirectory(outDir);
        }

        private static bool IsDirectoryEmpty(string directory)
        {
            return Directory.GetFiles(inDir, "*", SearchOption.AllDirectories).Length == 0;
        }

        private static void OnFileChanged(string file)
        {
            string outputExtension = ".mp3";

            string inputPath = file;

            string relativeInputPath = inputPath.Replace(inDir, "");
            string outputPath = Path.ChangeExtension(Path.Combine(outDir, relativeInputPath), outputExtension);

            Console.WriteLine($"Extracting audio from {inputPath} and saving it to {outputPath}");

            while (!IsFileReady(inputPath))
            {
                Console.WriteLine($"Waiting to consume {inputPath} because file is busy...");
                Thread.Sleep(500);
            };

            ConvertFileMirrored(inputPath, outputPath);

            File.Delete(file);

            if (IsDirectoryEmpty(Path.GetDirectoryName(file)))
            {
                Directory.Delete(Path.GetDirectoryName(file));
            }
        }

        private static void ConvertFileMirrored(string inputPath, string outputPath)
        {
            // Create mirrored directory structure
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            // Run ffmpeg conversion
            Console.WriteLine($"ffmpeg -i \"{inputPath}\" -codec libmp3lame -qscale:a 2 -vn \"{outputPath}\"".Bash(dryRun));
        }
    }
}
