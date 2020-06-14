namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        CoursesID = c.Int(nullable: false, identity: true),
                        CourseName = c.String(),
                        CourseTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CoursesID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UsersID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                        Email = c.String(),
                        Programme = c.Int(nullable: false),
                        Roles = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UsersID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Courses");
        }
    }
}
