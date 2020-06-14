namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class s : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "Programme", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Programme", c => c.Int());
        }
    }
}
