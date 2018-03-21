using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.DomainModels
{
    public abstract class BaseEntity<TKey> where TKey: struct
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Nullable<DateTime> UpdatedDate { get; set; }
    }
}
