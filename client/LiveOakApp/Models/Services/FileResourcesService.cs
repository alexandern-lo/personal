using System;
using System.IO;
using LiveOakApp.Models.Data.Entities;
using SL4N;
using System.Collections.Generic;
using System.Linq;

namespace LiveOakApp.Models.Services
{
    public class FileResourcesService
    {
        static readonly ILogger LOG = LoggerFactory.GetLogger<FileResourcesService>();

        readonly string FilesDirectory;

        public FileResourcesService()
        {
            string documentsPath;
#if __IOS__
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            documentsPath = Path.Combine(documentsPath, "..", "Library", "Application Support");
#endif
#if __ANDROID__
            documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
#endif
            FilesDirectory = Path.Combine(documentsPath, "FileResources");
            if (!Directory.Exists(FilesDirectory))
            {
                Directory.CreateDirectory(FilesDirectory);
            }
            LOG.Debug("Using FilesDirectory: {0}", FilesDirectory);
        }

        public FileResource CopyFileToPermanentPath(string tempAbsLocalPath)
        {
            var relativePermPath = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(tempAbsLocalPath));
            var absPermPath = AbsolutePathForFile(relativePermPath);
            File.Copy(tempAbsLocalPath, absPermPath);
            LOG.Debug("copied file {0} => {1}", tempAbsLocalPath, absPermPath);
            return new FileResource(relativePermPath, null);
        }

        public string AbsolutePathForFile(string relativePermPath)
        {
            if (relativePermPath == null) return null;
            return Path.Combine(FilesDirectory, relativePermPath);
        }

        public void DeleteFile(string relativePermPath)
        {
            var absPath = AbsolutePathForFile(relativePermPath);
            if (absPath == null) return;
            File.Delete(absPath);
            LOG.Debug("deleted file {0}", relativePermPath);
        }

        public void DeleteFileNoThrow(string relativePermPath)
        {
            try
            {
                DeleteFile(relativePermPath);
            }
            catch (Exception error)
            {
                LOG.Warn(string.Format("failed to delete file {0}", relativePermPath), error);
            }
        }

        public void DeleteOldUnusedFiles(IEnumerable<string> preserveRelativePaths)
        {
            var preservePathsDict = preserveRelativePaths.Select(p => AbsolutePathForFile(p)).ToDictionary(p => p);
            var files = Directory.EnumerateFiles(FilesDirectory);
            foreach (var file in files)
            {
                if (preservePathsDict.ContainsKey(file))
                {
                    continue;
                }
                var creationTime = File.GetCreationTime(file);
                if (creationTime > DateTime.Now.AddHours(-4))
                {
                    continue;
                }
                File.Delete(file);
                LOG.Debug("Deleted unused old file: {0}", file);
            }
        }
    }
}
