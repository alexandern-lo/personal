using System;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.ViewModels
{
    public class AttendeeInfoItemViewModel
    {

        public string Key { get; set; }
        public string Value { get; set; }
        public AttendeeDetailsViewModel.InfoType Type { get; set; }

        public AttendeeInfoItemViewModel( AttendeeCategoryValueDTO categoryValue, AttendeeDetailsViewModel.InfoType type )
        {
            Key = categoryValue.CategoryName;
            Value = categoryValue.OptionName;
            Type = type;
        }

        public AttendeeInfoItemViewModel(string key, string value, AttendeeDetailsViewModel.InfoType type)
        {
            Key = key;
            Value = value;
            Type = type;
        }
    }
}

