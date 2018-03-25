namespace BankingSystem.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class concurrency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BankAccounts", "RowVersion", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.BankAccounts", "RowVersion");
        }
    }
}
