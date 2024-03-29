﻿using System.IO.Compression;
using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Strategy;

namespace Testar.ChangeDetection.BlazarApp;

public class FileHandler : IFileOutputHandler
{
    public FileHandler(Model control, Model test)
    {
        var folderName = $"{control.Name}_{control.Version}_Diff_{test.Name}_{test.Version}";

        RootFolder = Path.Combine("out", folderName);

        if (Directory.Exists(RootFolder))
        {
            Directory.Delete(RootFolder, recursive: true);
        }

        Directory.CreateDirectory(RootFolder);
    }

    public HashSet<string> UsedPaths { get; } = new();
    public string RootFolder { get; }

    public string GetFilePath(string fileName)
    {
        var path = Path.Combine(RootFolder, fileName);
        UsedPaths.Add(path);
        return path;
    }

    public Stream DownloadOutputAsZip()
    {
        if (!File.Exists(@"out\out.zip"))
        {
            ZipFile.CreateFromDirectory(RootFolder, @"out\out.zip", CompressionLevel.Fastest, true);
        }
        var zipBytes = File.ReadAllBytes(@"out\out.zip");

        return new MemoryStream(zipBytes);
    }
}