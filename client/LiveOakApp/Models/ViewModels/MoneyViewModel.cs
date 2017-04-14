using LiveOakApp.Models.Data.NetworkDTO;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class MoneyViewModel : DataContext
    {
        public enum MoneyCurrency
        {
            USD,
            None
        }
        Field<decimal> _amount;
        public decimal Amount 
        { 
            get
            {
                return _amount.Value;
            }
            set
            {
                _amount.SetValue(value);
            }
        }
        Field<MoneyCurrency> _currency;
        public MoneyCurrency Currency 
        {
            get
            {
                return _currency.Value;
            }
            set
            {
                _currency.SetValue(value);
            }
        }

        public MoneyViewModel()
        {
            _amount = Value((decimal)0);
            _currency = Value(MoneyCurrency.USD);
        }

        public MoneyViewModel(MoneyDTO dto)
        {
            _amount = Value(dto.Amount);
            _currency = Value(CurrencyFromDTOToViewModel(dto.CurrencyEnum));
        }

        public MoneyDTO MoneyDTO 
        { 
            get
            {
                return new MoneyDTO
                {
                    Amount = Amount,
                    CurrencyEnum = CurrencyFromViewModelToDTO(Currency)
                };
            }
        }

        public MoneyViewModel(decimal amount, MoneyCurrency code)
        {
            _amount = Value(amount);
            _currency = Value(code);
        }

        public void UpdateFromDTO(MoneyDTO dto) 
        {
            if(dto == null)
            {
                Amount = 0;
                Currency = MoneyCurrency.USD;
                return;
            }
            Currency = CurrencyFromDTOToViewModel(dto.CurrencyEnum);
            Amount = dto.Amount;
        }

        public string GetCurrencySymbol()
        {
            switch (Currency)
            {
                case MoneyCurrency.USD: return "$";
                case MoneyCurrency.None: return " ";
                default: return " ";
            }
        }

        static MoneyCurrency CurrencyFromDTOToViewModel(MoneyDTO.MoneyCurrency currencyDTO)
        {
            switch (currencyDTO)
            {
                case MoneyDTO.MoneyCurrency.USD:
                    return MoneyCurrency.USD;
                default:
                    return MoneyCurrency.None;
            }
        }

        static MoneyDTO.MoneyCurrency CurrencyFromViewModelToDTO(MoneyCurrency currency)
        {
            switch (currency)
            {
                case MoneyCurrency.USD:
                    return MoneyDTO.MoneyCurrency.USD;
                default:
                    return MoneyDTO.MoneyCurrency.USD;
            }
        }
    }
}
