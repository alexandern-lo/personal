using System;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Entities;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadDetailsPhoneViewModel : DataContext
    {
        Field<string> _phone;
        Field<PhoneDTO.PhoneType> _phoneType;

        LeadDetailsViewModel ParentViewModel;

        public LeadDetailsPhoneViewModel(LeadDetailsViewModel parentViewModel, PhoneDTO phoneDTO)
        {
            ParentViewModel = parentViewModel;
            _phone = Value(phoneDTO.Phone);
            _phoneType = Value(phoneDTO.TypeEnum);

            Initialize();
        }

        public LeadDetailsPhoneViewModel(LeadDetailsViewModel parentViewModel, string phone)
        {
            ParentViewModel = parentViewModel;
            _phone = Value(phone);
            _phoneType = Value(PhoneDTO.PhoneType.Work);

            Initialize();
        }

        public LeadDetailsPhoneViewModel(LeadDetailsViewModel parentViewModel, CardPhone cardPhone)
        {
            ParentViewModel = parentViewModel;
            _phone = Value(cardPhone.Phone);
            _phoneType = Value(PhoneTypeFromCardType(cardPhone.Type));

            Initialize();
        }

        public LeadDetailsPhoneViewModel(LeadDetailsViewModel parentViewModel)
        {
            ParentViewModel = parentViewModel;
            _phone = Value("");
            _phoneType = Value(PhoneDTO.PhoneType.Work);

            Initialize();
        }

        void Initialize()
        {
            RemovePhoneCommand = new Command
            {
                Action = RemovePhoneAction
            };
        }

        public PhoneDTO PhoneDTO
        {
            get
            {
                return new PhoneDTO()
                {
                    Phone = _phone.Value,
                    TypeEnum = _phoneType.Value
                };
            }
        }

        public string Phone
        {
            get
            {
                return _phone.Value;
            }
            set
            {
                _phone.SetValue(value);
                ParentViewModel.PerformSave();
            }
        }

        public string TypeString
        {
            get
            {
                return _phoneType.Value.ToString();
            }
            set
            {
                var newType = (PhoneDTO.PhoneType)Enum.Parse(typeof(PhoneDTO.PhoneType), value);
                if (newType.Equals(_phoneType.Value))
                    return;
                _phoneType.SetValue(newType);
                ParentViewModel.PerformSave();
            }
        }

        ObservableList<string> phoneTypes;
        public ObservableList<string> PhoneTypes
        {
            get
            {
                if (phoneTypes == null)
                {
                    phoneTypes = new ObservableList<string>(new string[] {
                        PhoneDTO.PhoneType.Home.ToString(),
                        PhoneDTO.PhoneType.Work.ToString(),
                        PhoneDTO.PhoneType.Mobile.ToString()
                    });
                }
                return phoneTypes;
            }
        }

        public Command RemovePhoneCommand { get; private set; }

        void RemovePhoneAction(object obj)
        {
            ParentViewModel.RemovePhoneVM(this);
        }

        PhoneDTO.PhoneType PhoneTypeFromCardType(CardPhone.PhoneType type)
        {
            switch (type)
            {
                case CardPhone.PhoneType.Home:
                    return PhoneDTO.PhoneType.Home;
                case CardPhone.PhoneType.Mobile:
                    return PhoneDTO.PhoneType.Mobile;
                case CardPhone.PhoneType.Work:
                    return PhoneDTO.PhoneType.Work;
                default:
                    return PhoneDTO.PhoneType.Home;
            }
        }
    }
}
