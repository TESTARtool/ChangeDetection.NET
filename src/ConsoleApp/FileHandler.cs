using Testar.ChangeDetection.Core;
using Testar.ChangeDetection.Core.Strategy;

namespace Testar.ChangeDetection.ConsoleApp;

internal sealed partial class ConsoleHostedService
{
    public class FileHandler : IFileOutputHandler
    {
        public FileHandler(Application control, Application test)
        {
            var folderName = $"{control.ApplicationName}_{control.ApplicationVersion}_Diff_{test.ApplicationName}_{test.ApplicationVersion}";

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
    }
}