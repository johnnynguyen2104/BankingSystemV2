using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Common.Models
{
    /// <summary>
    /// This model only use to return exception response in explicitly way
    /// </summary>
    public class ResponseExceptionModel
    {
        public ResponseExceptionModel()
        {
            AccountNumber = 0;
            Sucessful = false;
            Balance = null;
            Currency = null;
        }

        public int AccountNumber { get;}

        public bool Sucessful { get;}

        public decimal? Balance { get;}

        public string Currency { get;}

        public string Message { get; set; }
    }
}
