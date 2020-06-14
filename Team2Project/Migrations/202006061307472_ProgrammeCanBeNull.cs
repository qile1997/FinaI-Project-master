namespace Team2Project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProgrammeCanBeNull : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attendances", "programme", c => c.Int(nullable: false));
            AddColumn("dbo.Courses", "Programme", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "Programme", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "Programme", c => c.Int(nullable: false));
            DropColumn("dbo.Courses", "Programme");
            DropColumn("dbo.Attendances", "programme");
        }
    }
}
