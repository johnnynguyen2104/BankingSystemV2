using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Business.DTOs
{
    public class ResponseAccountInfo
    {
        public int AccountId { get; set; }

        public int AccountNumber { get; set; }

        public bool Sucessful { get;}

        public decimal Balance { get; set; }

        public string Currency { get; set; }

        public string Message { get; set; }

        public ResponseAccountInfo()
        {
            Sucessful = true;
        }
    }
}
