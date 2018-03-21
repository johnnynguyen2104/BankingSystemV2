using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.DomainModels
{
    public class Currency : BaseEntity<int>
    {
        public string CurrencyCode { get; set; }

        public string CurrencyName { get; set; }

        public string Country { get; set; }

        public virtual ICollection<BankAccount> Accounts { get; set; }
    }
}
