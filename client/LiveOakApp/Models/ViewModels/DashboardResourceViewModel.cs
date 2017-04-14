using System;
using LiveOakApp.Models.Data.NetworkDTO;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class DashboardResourceViewModel : DataContext
    {
        Field<Guid> _uid;
        Field<string> _name;
        Field<string> _type;
        Field<string> _url;
        Field<int> _sentCount;
        Field<int> _openedCount;

        public Guid Uid
        {
            get
            {
                return _uid.Value;
            }
            set
            {
                _uid.SetValue(value);
            }
        }
        public string Name
        {
            get
            {
                return _name.Value;
            }
            set
            {
                _name.SetValue(value);
            }
        }
        public string Type
        {
            get
            {
                return _type.Value;
            }
            set
            {
                _type.SetValue(value);
                RaisePropertyChanged(() => ResourceTypeImageName);
            }
        }
        public string Url
        {
            get
            {
                return _url.Value;
            }
            set
            {
                _url.SetValue(value);
            }
        }
        public int SentCount
        {
            get
            {
                return _sentCount.Value;
            }
            set
            {
                _sentCount.SetValue(value);
            }
        }
        public int OpenedCount
        {
            get
            {
                return _openedCount.Value;
            }
            set
            {
                _openedCount.SetValue(value);
            }
        }

        public DashboardResourceViewModel(DashboardResourceDTO dashboardResourceDTO)
        {
            _uid = Value(dashboardResourceDTO.Uid);
            _name = Value(dashboardResourceDTO.Name);
            _url = Value(dashboardResourceDTO.Url);
            _type = Value(dashboardResourceDTO.Type);
            _sentCount = Value(dashboardResourceDTO.SentCount);
            _openedCount = Value(dashboardResourceDTO.OpenedCount);
        }

        public string ResourceTypeImageName
        {
            get
            {
                switch (Type)
                {
                    case "application/pdf":
                        return "resources_pdf";
                    case "url":
                        return "resources_link";
                    case "application/vnd.ms-powerpoint":
                    case "application/vnd.openxmlformats-officedocument.presentationml.presentation":
                        return "resources_ppt";
                    case "application/vnd.ms-excel":
                    case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                        return "resources_xls";
                    case "application/msword":
                    case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                        return "resources_doc";
                    case "application/zip":
                    case "application/rar":
                        return "resources_archive";
                    case "image/jpeg":
                    case "image/png":
                    case "image/gif":
                    case "image/tiff":
                    case "image/pjpeg":
                    case "image/svg+xml":
                        return "resources_image";
                    default:
                        return "resources_unknown";
                }
            }
        }

    }
}
