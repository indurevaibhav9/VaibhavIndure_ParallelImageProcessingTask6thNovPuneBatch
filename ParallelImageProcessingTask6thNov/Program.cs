using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace ParallelImageProcessingTask6thNov
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: <inputImagePath> <outputImagePath> [height] [width] [maxDegreeOfParallelism]");
                Console.WriteLine("Example: ParallelImageProcessingTask6thNov.exe ./Images ./OutputThumbnails 150 150 4");
                return;
            }

            string inputDir = args[0];
            string outputDir = args[1];
            int height = args.Length > 2 && int.TryParse(args[2], out int h) ? h : 150;
            int width = args.Length > 3 && int.TryParse(args[3], out int w) ? w : 150;
            int maxDegreeOfParallelism = args.Length > 4 && int.TryParse(args[4], out int mdp) ? mdp : Environment.ProcessorCount;

            if (!Directory.Exists(inputDir))
            {
                Console.WriteLine($"Input directory '{inputDir}' does not exist.");
                return;
            }

            Directory.CreateDirectory(outputDir);

            var imageFiles = Directory.GetFiles(inputDir, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (imageFiles.Length == 0)
            {
                Console.WriteLine($"No image files found in '{inputDir}'.");
                return;
            }

            Console.WriteLine($"Processing {imageFiles.Length} files with parallelism = {maxDegreeOfParallelism}...");
            Console.WriteLine($"Target size: {width}x{height}\n");

            var po = new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism };
            int processedCount = 0, totalCount = imageFiles.Length;

            Parallel.ForEach(imageFiles, po, (file) =>
            {
                try
                {
                    ProcessFile(file, outputDir, width, height);
                    int done = Interlocked.Increment(ref processedCount);
                    Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Processed {done}/{totalCount}: {Path.GetFileName(file)}");
                }
                catch (Exception e)
                {
                    int done = Interlocked.Increment(ref processedCount);
                    Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] Error {done}/{totalCount}: {Path.GetFileName(file)} -> {e.Message}");
                }
            });

            Console.WriteLine("\n All images processed successfully!");
        }

        public static void ProcessFile(string filePath, string outDir, int width, int height)
        {
            string fileName = Path.GetFileName(filePath);
            string outPath = Path.Combine(outDir, fileName);

            using var image = Image.Load(filePath);

            //  Corrected resize logic
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(width, height),
                Mode = ResizeMode.Max
            }));

            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    image.Save(outPath, new JpegEncoder());
                    break;
                case ".png":
                    image.Save(outPath, new PngEncoder());
                    break;
                case ".bmp":
                    image.Save(outPath, new BmpEncoder());
                    break;
                case ".gif":
                    image.Save(outPath, new GifEncoder());
                    break;
                default:
                    throw new NotSupportedException($"File extension '{extension}' is not supported.");
            }
        }
    }
}
