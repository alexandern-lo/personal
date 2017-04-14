using System;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.Data.Entities
{
    public class FileResource
    {
        readonly FileResourcesService fileResourcesService;

        public string RelativeLocalPath { get; }
        public string RemoteUrl { get; }

        public FileResource(string relativeLocalPath, string remoteUrl)
        {
            fileResourcesService = ServiceLocator.Instance.FileResourcesService;
            RelativeLocalPath = relativeLocalPath;
            RemoteUrl = remoteUrl;
        }

        public bool isEmpty()
        {
            return string.IsNullOrWhiteSpace(RelativeLocalPath) && string.IsNullOrWhiteSpace(RemoteUrl);
        }

        public string AbsoluteLocalPath
        {
            get
            {
                return fileResourcesService.AbsolutePathForFile(RelativeLocalPath);
            }
        }
    }
}
