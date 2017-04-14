using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using System;

namespace LiveOakApp.Models.ViewModels
{
    public class ResourceViewModel : DataContext
    {

        public ResourceViewModel(ResourceDTO resource)
        {
            Resource = resource;
        }

        public ResourceDTO Resource { get; private set; }

        public string Title { get { return Resource.Name; } }

        public string Type { get { return Resource.Type; } }

        public string Url { get { return Uri.EscapeUriString(Resource.Url); } }

        public Guid Uid { get { return Resource.Uid.Value; } }

        public string Description { get { return Resource.Description; } }

        public string UploadedAt
        {
            get
            {

                DateTime? updatedAt = Resource.UpdatedAt;
                DateTime dateTime;
                if (updatedAt.HasValue)
                {
                    dateTime = (System.DateTime)updatedAt;
                }
                else
                {
                    dateTime = Resource.CreatedAt;
                }
                return dateTime.ToString("MMMM dd, yyyy");
            }
        }

        public bool Selected { get; set; }

        string resourceTypeImageName;
        public string ResourceTypeImageName
        {
            get
            {
                if (resourceTypeImageName == null)
                {
                    switch (Resource.Type)
                    {
                        case "application/pdf":
                            resourceTypeImageName = "resources_pdf";
                            break;
                        case "url":
                            resourceTypeImageName = "resources_link";
                            break;
                        case "application/vnd.ms-powerpoint":
                        case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                            resourceTypeImageName = "resources_ppt";
                            break;
                        case "application/vnd.ms-excel":
                        case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                            resourceTypeImageName = "resources_xls";
                            break;
                        case "application/msword":
                        case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                            resourceTypeImageName = "resources_doc";
                            break;
                        case "application/zip":
                        case "application/rar":
                            resourceTypeImageName = "resources_archive";
                            break;
                        case "image/jpeg":
                        case "image/png":
                        case "image/gif":
                        case "image/tiff":
                        case "image/pjpeg":
                        case "image/svg+xml":
                            resourceTypeImageName = "resources_image";
                            break;
                        default:
                            resourceTypeImageName = "resources_unknown";
                            break;
                    }
                }
                return resourceTypeImageName;
            }
        }

    }
}
