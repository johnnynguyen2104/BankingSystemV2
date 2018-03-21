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

            modelBuilder.Entity<Currency>()
                .HasMany(a => a.Accounts)
                .WithRequired(a => a.Currency)
                .HasForeignKey(a => a.CurrencyId);

            //Type Def
            modelBuilder.Entity<BankAccount>()
                  .Property(p => p.AccountNumber)
                  .HasColumnType("VARCHAR")
                  .HasMaxLength(20);

            modelBuilder.Entity<BankAccount>()
                .Property(p => p.Amount)
                .HasColumnType("MONEY");

            modelBuilder.Entity<Currency>()
                .Property(p => p.CurrencyCode)
                .HasColumnType("VARCHAR")
                .HasMaxLength(20);

            modelBuilder.Entity<Currency>()
               .Property(p => p.CurrencyName)
               .HasColumnType("VARCHAR")
               .HasMaxLength(100);

            modelBuilder.Entity<Currency>()
               .Property(p => p.Country)
               .HasColumnType("VARCHAR")
               .HasMaxLength(100);

            modelBuilder.Entity<TransactionHistory>()
                .Property(a => a.NewAmount)
                .HasColumnType("MONEY");

            modelBuilder.Entity<TransactionHistory>()
               .Property(a => a.OldAmount)
               .HasColumnType("MONEY");

        }
    }
}
