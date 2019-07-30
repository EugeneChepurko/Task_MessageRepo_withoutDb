namespace Task_MessageRepo_withoutDb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migrat11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        Mess = c.String(),
                        UserName = c.String(),
                        DateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Messages", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "ApplicationUserId" });
            DropTable("dbo.Messages");
        }
    }
}
