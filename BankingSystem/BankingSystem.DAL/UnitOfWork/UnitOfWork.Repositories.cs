using BankingSystem.DAL.DomainModels;
using BankingSystem.DAL.Interfaces;
using BankingSystem.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingSystem.DAL.UnitOfWork
{
    public partial class UnitOfWork
    {
        public IRepository<int, BankAccount> AccountRepo => this.AccountRepo ?? new Repository<int, BankAccount>(_dbContext);

        public IRepository<int, TransactionHistory> TransactionHistoryRepo => this.TransactionHistoryRepo ?? new Repository<int, TransactionHistory>(_dbContext);
    }
}
