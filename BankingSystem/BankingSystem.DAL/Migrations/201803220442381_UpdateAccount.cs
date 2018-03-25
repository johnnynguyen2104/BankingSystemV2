namespace BankingSystem.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAccount : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BankAccounts", "CurrencyId", "dbo.Currencies");
            DropIndex("dbo.BankAccounts", new[] { "CurrencyId" });
            AddColumn("dbo.BankAccounts", "Currency", c => c.String());
            AlterColumn("dbo.BankAccounts", "AccountNumber", c => c.Int(nullable: false));
            DropColumn("dbo.BankAccounts", "CurrencyId");
            DropTable("dbo.Currencies");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyCode = c.String(maxLength: 20, unicode: false),
                        CurrencyName = c.String(maxLength: 100, unicode: false),
                        Country = c.String(maxLength: 100, unicode: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.BankAccounts", "CurrencyId", c => c.Int(nullable: false));
            AlterColumn("dbo.BankAccounts", "AccountNumber", c => c.String(maxLength: 20, unicode: false));
            DropColumn("dbo.BankAccounts", "Currency");
            CreateIndex("dbo.BankAccounts", "CurrencyId");
            AddForeignKey("dbo.BankAccounts", "CurrencyId", "dbo.Currencies", "Id", cascadeDelete: true);
        }
    }
}
