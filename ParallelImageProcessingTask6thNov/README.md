# Dotnet Assignment 18 — Image Processing in Parallel

Goal:
- Resize all images in a folder to thumbnails simultaneously using `Parallel.ForEach`.

Requirements:
- .NET 7.0 SDK (project targets `net7.0`)
- Uses `SixLabors.ImageSharp` for cross-platform image processing.

Files:
- `DotnetAssignment18.csproj` — project file with dependency
- `Program.cs` — console program that reads files, resizes in parallel, and saves thumbnails

Build and run:
1. Restore & build:
   - dotnet restore
   - dotnet build

2. Run:
   - dotnet run -- <input-folder> <output-folder> [width] [height] [maxParallelism]
   Example:
   - dotnet run -- ./images ./thumbs 150 150 0

Parameters:
- input-folder: folder that contains source images
- output-folder: folder to write thumbnails (is created if missing)
- width (optional, default 150): max width of thumbnail
- height (optional, default 150): max height of thumbnail
- maxParallelism (optional, default 0 uses Environment.ProcessorCount): limit parallel threads

Notes:
- ImageSharp is cross-platform and recommended over System.Drawing for non-Windows environments.
- The code keeps the aspect ratio and fits the image into the width×height box.
- Errors per-file are caught and logged; processing continues for other files.
- You can tune `MaxDegreeOfParallelism` to match your CPU and I/O capabilities.

Alternatives:
- If you are on Windows and prefer `System.Drawing.Common`, you can replace ImageSharp calls with `Bitmap`/`Graphics` resizing, but System.Drawing is not recommended on non-Windows platforms.

License:
- None — sample code for learning/demo purposes.