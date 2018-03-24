using BankingSystem.DAL.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BankingSystem.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        int CommitChanges();

        IRepository<int, BankAccount> AccountRepo { get;}

        IRepository<int, TransactionHistory> TransactionHistoryRepo { get;}
    }
}
