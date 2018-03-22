namespace BankingSystem.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BankAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountNumber = c.String(maxLength: 20, unicode: false),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        CurrencyId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currencies", t => t.CurrencyId, cascadeDelete: true)
                .Index(t => t.CurrencyId);
            
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
            
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Action = c.Int(nullable: false),
                        OldAmount = c.Decimal(nullable: false, storeType: "money"),
                        NewAmount = c.Decimal(nullable: false, storeType: "money"),
                        Details = c.String(),
                        AccountId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAccounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionHistories", "AccountId", "dbo.BankAccounts");
            DropForeignKey("dbo.BankAccounts", "CurrencyId", "dbo.Currencies");
            DropIndex("dbo.TransactionHistories", new[] { "AccountId" });
            DropIndex("dbo.BankAccounts", new[] { "CurrencyId" });
            DropTable("dbo.TransactionHistories");
            DropTable("dbo.Currencies");
            DropTable("dbo.BankAccounts");
        }
    }
}
