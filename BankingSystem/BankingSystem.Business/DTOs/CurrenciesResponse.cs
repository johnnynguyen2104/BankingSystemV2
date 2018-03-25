using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Business.DTOs
{
    public class CurrenciesResponse
    {

        public CurrenciesResponse()
        {
            Date = DateTime.Now;
            Rates = new Dictionary<string, double>();
        }
        public string BaseCurrency { get; set; }

        public DateTime Date { get; set; }

        public Dictionary<string, double> Rates { get; set; }
    }
}
