namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcourseStartAndEndTIme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "StartTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.Courses", "EndTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Courses", "CourseTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Courses", "CourseTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Courses", "EndTime");
            DropColumn("dbo.Courses", "StartTime");
        }
    }
}
