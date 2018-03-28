namespace BankingSystem.DAL.Migrations
{
    using BankingSystem.DAL.DomainModels;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BankingSystem.DAL.DbContexts.BankingSystemDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(BankingSystem.DAL.DbContexts.BankingSystemDb context)
        {
            //  This method will be called after migrating to the latest version.

            //You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            context.Set<BankAccount>().AddOrUpdate(
                new BankAccount() { AccountNumber = 1, Amount = 100, CreatedDate = DateTime.Now, Currency = "USD", IsActive = true },
                new BankAccount() { AccountNumber = 1, Amount = 100, CreatedDate = DateTime.Now, Currency = "THB", IsActive = true },
                new BankAccount() { AccountNumber = 1, Amount = 100, CreatedDate = DateTime.Now, Currency = "USD", IsActive = true }
                );
        }
    }
}
