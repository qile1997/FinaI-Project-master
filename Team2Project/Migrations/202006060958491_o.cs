namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class o : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attendances",
                c => new
                    {
                        AttendanceID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CourseName = c.String(),
                        AttendanceTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AttendanceID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Attendances");
        }
    }
}
