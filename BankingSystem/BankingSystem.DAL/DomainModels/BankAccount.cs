using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.DomainModels
{
    public class BankAccount : BaseEntity<int>
    {
        public string AccountNumber { get; set; }

        public decimal Amount { get; set; } 

        public int CurrencyId { get; set; }

        public bool IsActive { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual ICollection<TransactionHistory> Histories { get; set; }
    }
}
