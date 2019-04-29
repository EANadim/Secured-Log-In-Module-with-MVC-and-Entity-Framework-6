namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "Type");
        }
    }
}
