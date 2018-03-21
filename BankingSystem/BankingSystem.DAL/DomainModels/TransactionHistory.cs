using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.DomainModels
{
    public class TransactionHistory : BaseEntity<int>
    {
        public ActionCode Action { get; set; }

        public decimal OldAmount { get; set; }

        public decimal NewAmount { get; set; }

        public string Details { get; set; }

        public int AccountId { get; set; }

        public virtual BankAccount Account { get; set; }
    }
}
