namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class attendance : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attendances", "CourseTimetableId", c => c.Int(nullable: false));
            AlterColumn("dbo.Attendances", "Programme", c => c.Int());
            DropColumn("dbo.Attendances", "CourseName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Attendances", "CourseName", c => c.String());
            AlterColumn("dbo.Attendances", "Programme", c => c.Int(nullable: false));
            DropColumn("dbo.Attendances", "CourseTimetableId");
        }
    }
}
