using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.DomainModels
{
    public class BankAccount : BaseEntity<int>
    {
        public int AccountNumber { get; set; }

        public decimal Amount { get; set; } 

        public bool IsActive { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public string Currency { get; set; }

        public virtual ICollection<TransactionHistory> Histories { get; set; }
    }
}
