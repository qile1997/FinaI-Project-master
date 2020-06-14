namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeProp : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Courses", "StartTime");
            DropColumn("dbo.Courses", "EndTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "EndTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "StartTime", c => c.DateTime(nullable: false));
        }
    }
}
