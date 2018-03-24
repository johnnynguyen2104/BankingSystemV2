using BankingSystem.DAL.DomainModels;
using BankingSystem.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.DAL.DbContexts
{
    public class BankingSystemDb : DbContext, IDbContext
    {

        public BankingSystemDb(): base("BankingSystemDb")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BankingSystemDb, Migrations.Configuration>());
        }

        public BankingSystemDb(string connectionString) : base(connectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BankingSystemDb, Migrations.Configuration>());
        }

        public int CommitChanges()
        {
            return this.SaveChanges();
        }

        public Task<int> CommitChangesAsyn()
        {
            return this.SaveChangesAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BankAccount>()
                .HasMany(a => a.Histories)
                .WithRequired(a => a.Account)
                .HasForeignKey(a => a.AccountId);

            //Type Def

            modelBuilder.Entity<BankAccount>()
                .Property(p => p.Amount)
                .HasColumnType("MONEY");

            modelBuilder.Entity<BankAccount>()
                .Property(p => p.RowVersion)
                .IsConcurrencyToken();

            modelBuilder.Entity<TransactionHistory>()
                .Property(a => a.NewAmount)
                .HasColumnType("MONEY");

            modelBuilder.Entity<TransactionHistory>()
               .Property(a => a.OldAmount)
               .HasColumnType("MONEY");

        }
    }
}
