namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseTimetables",
                c => new
                    {
                        CourseTimetableId = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        CoursesID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourseTimetableId)
                .ForeignKey("dbo.Courses", t => t.CoursesID, cascadeDelete: true)
                .Index(t => t.CoursesID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseTimetables", "CoursesID", "dbo.Courses");
            DropIndex("dbo.CourseTimetables", new[] { "CoursesID" });
            DropTable("dbo.CourseTimetables");
        }
    }
}
