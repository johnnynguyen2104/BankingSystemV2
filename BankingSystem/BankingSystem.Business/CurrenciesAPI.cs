using BankingSystem.Business.DTOs;
using BankingSystem.Common;
using BankingSystem.Common.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Business
{
    public interface ICurrenciesTestAPI {

        Task<CurrenciesResponse> RequestCurrenciesAsyn(string inputCurrency);
    }

    public class CurrenciesTestAPI : ICurrenciesTestAPI
    {
        private List<string> SupportedCurrencies = new List<string>() { "THB", "USD" };

        public Task<CurrenciesResponse> RequestCurrenciesAsyn(string inputCurrency)
        {
            return Task.Run(() =>
            {

                if (!SupportedCurrencies.Any(a => a == inputCurrency))
                {
                    throw new BusinessServerErrorException(string.Format(AppMessages.UnsupportedCurrency, inputCurrency));
                }

                CurrenciesResponse result = new CurrenciesResponse()
                {
                    BaseCurrency = inputCurrency
                };

                if (inputCurrency == "THB")
                {
                    result.Rates.Add("USD", 0.032);
                }
                else if (inputCurrency == "USD")
                {
                    result.Rates.Add("THB", 31.21);
                }

                return result;
            });
        }

    }
}
