namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class l : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Programme", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Programme", c => c.Int(nullable: false));
        }
    }
}
