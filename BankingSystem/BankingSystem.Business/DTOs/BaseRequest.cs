using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Business.DTOs
{
    public class BaseRequest
    {
        public int AccountNumber { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }
    }
}
