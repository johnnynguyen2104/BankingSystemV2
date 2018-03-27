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
        private IRepository<int, BankAccount> accountRepo;
        private IRepository<int, TransactionHistory> transactionHistoryRepo;

        public IRepository<int, BankAccount> AccountRepo
        {
            get
            {

                if (accountRepo == null)
                {
                    accountRepo = new Repository<int, BankAccount>(_dbContext);
                }
                return accountRepo;
            }
        }

        public IRepository<int, TransactionHistory> TransactionHistoryRepo
        {
            get
            {
                if (transactionHistoryRepo == null)
                {
                    transactionHistoryRepo = new Repository<int, TransactionHistory>(_dbContext);
                }

                return transactionHistoryRepo;
            }
        }
    }
}
