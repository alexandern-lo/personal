using System;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Resources;

namespace LiveOakApp.Models.ViewModels
{
    public class AttendeeDetailsViewModel : DataContext
    {
        public enum InfoType
        {
            Phone,
            Email,
            Other
        }

        private Field<AttendeeViewModel> attendeeViewModel;
        private Field<string> attendeeName;
        private Field<string> attendeeTitle;
        private Field<string> attendeeCompany;
        private Field<string> attendeePhone;
        private Field<string> attendeeEmail;
        public EventViewModel Event { get; private set; }

        public Field<ObservableList<AttendeeInfoItemViewModel>> attendeeInfoList;
        private Field<RemoteImage> attendeeAvatar;

        public AttendeeDetailsViewModel(AttendeeViewModel attendee, EventViewModel eventViewModel)
        {
            attendeeViewModel = Value(attendee);
            attendeeName = Value(attendee.FullName);
            attendeeTitle = Value(attendee.Title);
            attendeeCompany = Value(attendee.Company);
            attendeePhone = Value(attendee.Phone);
            attendeeEmail = Value(attendee.Email);
            Event = eventViewModel;

            var _imageUrl = attendee.AvatarUrl.TryParseWebsiteUri();
            if (_imageUrl != null)
                attendeeAvatar = Value(new RemoteImage(_imageUrl));
            else 
                attendeeAvatar = Value<RemoteImage>(null);
            
            attendeeInfoList = Value(new ObservableList<AttendeeInfoItemViewModel>());
            attendeeInfoList.Value.Add(new AttendeeInfoItemViewModel(L10n.Localize("AttendeePhone", "Phone Number"), attendee.Phone, InfoType.Phone));
            attendeeInfoList.Value.Add(new AttendeeInfoItemViewModel(L10n.Localize("AttendeeEmail", "Email"), attendee.Email, InfoType.Email));
            foreach (AttendeeCategoryValueDTO categoryValue in attendee.CategoryValues)
            {
                attendeeInfoList.Value.Add(new AttendeeInfoItemViewModel(categoryValue, InfoType.Other));
            }

        }

        public AttendeeViewModel AttendeeViewModel
        {
            get
            {
                return attendeeViewModel.Value;
            }
            set
            {
                attendeeViewModel.SetValue(value);
            }
        }

        public RemoteImage AttendeeAvatar
        {
            get
            {
                return attendeeAvatar.Value;
            }
            set
            {
                attendeeAvatar.SetValue(value);
            }
        }

        public string AttendeeName
        {
            get
            {
                return attendeeName.Value;
            }
            set
            {
                attendeeName.SetValue(value);
            }
        }
        public string AttendeeTitle
        {
            get
            {
                return attendeeTitle.Value;
            }
            set
            {
                attendeeTitle.SetValue(value);
            }
        }
        public string AttendeeCompany
        {
            get
            {
                return attendeeCompany.Value;
            }
            set
            {
                attendeeCompany.SetValue(value);
            }
        }
        public string AttendeePhone
        {
            get
            {
                return attendeePhone.Value;
            }
            set
            {
                attendeePhone.SetValue(value);
            }
        }
        public string AttendeeEmail
        {
            get
            {
                return attendeeEmail.Value;
            }
            set
            {
                attendeeEmail.SetValue(value);
            }
        }
        public ObservableList<AttendeeInfoItemViewModel> AttendeeInfoList
        {
            get
            {
                return attendeeInfoList.Value;
            }
            private set
            {
                attendeeInfoList.SetValue(value);
            }
        }

    }
}

