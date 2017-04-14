using System;
using StudioMobile;
using LiveOakApp.Models.Data.NetworkDTO;
using LiveOakApp.Models.Data.Entities;
using System.Text.RegularExpressions;

namespace LiveOakApp.Models.ViewModels
{
    public class LeadDetailsEmailViewModel : DataContext
    {
        Field<string> _email;
        Field<EmailDTO.EmailType> _emailType;

        LeadDetailsViewModel ParentViewModel;

        public LeadDetailsEmailViewModel(LeadDetailsViewModel parentViewModel, EmailDTO emailDTO)
        {
            ParentViewModel = parentViewModel;
            _email = Value(emailDTO.Email);
            _emailType = Value(emailDTO.TypeEnum);

            Initialize();
        }

        public LeadDetailsEmailViewModel(LeadDetailsViewModel parentViewModel, string email)
        {
            ParentViewModel = parentViewModel;
            _email = Value(email);
            _emailType = Value(EmailDTO.EmailType.Work);

            Initialize();
        }

        public LeadDetailsEmailViewModel(LeadDetailsViewModel parentViewModel, CardEmail cardEmail)
        {
            ParentViewModel = parentViewModel;
            _email = Value(cardEmail.Email);
            _emailType = Value(EmailTypeFromCardType(cardEmail.Type));

            Initialize();
        }

        public LeadDetailsEmailViewModel(LeadDetailsViewModel parentViewModel)
        {
            ParentViewModel = parentViewModel;
            _email = Value("");
            _emailType = Value(EmailDTO.EmailType.Work);

            Initialize();
        }

        void Initialize()
        {
            RemoveEmailCommand = new Command()
            {
                Action = RemoveEmailAction
            };
        }

        public EmailDTO EmailDTO
        {
            get
            {
                return new EmailDTO()
                {
                    Email = _email.Value,
                    TypeEnum = _emailType.Value
                };
            }
        }

        public string Email
        {
            get
            {
                return _email.Value;
            }
            set
            {
                _email.SetValue(value);
                RaisePropertyChanged(() => IsEmailValid);
                ParentViewModel.RaiseHasValidFieldsChanged();
                ParentViewModel.PerformSave();
            }
        }

        public bool IsEmailValid 
        {
            get 
            {
                return Email != null && Regex.IsMatch(Email, ".+@.+\\..+");
            }
        }

        public string TypeString
        {
            get
            {
                return _emailType.Value.ToString();
            }
            set
            {
                var newType = (EmailDTO.EmailType)Enum.Parse(typeof(EmailDTO.EmailType), value);
                if (newType.Equals(_emailType.Value))
                    return;
                _emailType.SetValue(newType);
                ParentViewModel.PerformSave();
            }
        }

        ObservableList<string> emailTypes;
        public ObservableList<string> EmailTypes
        {
            get
            {
                if (emailTypes == null)
                {
                    emailTypes = new ObservableList<string>(new string[] {
                        EmailDTO.EmailType.Home.ToString(),
                        EmailDTO.EmailType.Work.ToString(),
                        EmailDTO.EmailType.Other.ToString()
                    });
                }
                return emailTypes;
            }
        }

        public Command RemoveEmailCommand { get; private set; }

        void RemoveEmailAction(object obj)
        {
            ParentViewModel.RemoveEmailVM(this);
        }

        EmailDTO.EmailType EmailTypeFromCardType(CardEmail.EmailType type)
        {
            switch (type)
            {
                case CardEmail.EmailType.Home:
                    return EmailDTO.EmailType.Home;
                case CardEmail.EmailType.Work:
                    return EmailDTO.EmailType.Work;
                case CardEmail.EmailType.Other:
                    return EmailDTO.EmailType.Other;
                default:
                    return EmailDTO.EmailType.Other;
            }
        }
    }
}
