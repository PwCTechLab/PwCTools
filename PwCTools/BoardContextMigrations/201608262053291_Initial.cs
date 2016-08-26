namespace PwCTools.BoardContextMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoardCommentAttachment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoardTaskCommentId = c.Int(nullable: false),
                        FileName = c.String(),
                        FilePath = c.String(),
                        FileContentType = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BoardTaskComment", t => t.BoardTaskCommentId, cascadeDelete: true)
                .Index(t => t.BoardTaskCommentId);
            
            CreateTable(
                "dbo.BoardTask",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ColumnId = c.Int(),
                        SprintId = c.Int(),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sprint", t => t.SprintId)
                .ForeignKey("dbo.Column", t => t.ColumnId)
                .Index(t => t.ColumnId)
                .Index(t => t.SprintId);
            
            CreateTable(
                "dbo.BoardTaskComment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoardTaskId = c.Int(nullable: false),
                        Comment = c.String(),
                        CreatedById = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BoardTask", t => t.BoardTaskId, cascadeDelete: true)
                .Index(t => t.BoardTaskId);
            
            CreateTable(
                "dbo.Column",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProgramId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Program", t => t.ProgramId, cascadeDelete: true)
                .Index(t => t.ProgramId);
            
            CreateTable(
                "dbo.Program",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sprint",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoardTask", "ColumnId", "dbo.Column");
            DropForeignKey("dbo.Column", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.BoardTask", "SprintId", "dbo.Sprint");
            DropForeignKey("dbo.Sprint", "ProjectId", "dbo.Project");
            DropForeignKey("dbo.Project", "ProgramId", "dbo.Program");
            DropForeignKey("dbo.BoardTaskComment", "BoardTaskId", "dbo.BoardTask");
            DropForeignKey("dbo.BoardCommentAttachment", "BoardTaskCommentId", "dbo.BoardTaskComment");
            DropIndex("dbo.Sprint", new[] { "ProjectId" });
            DropIndex("dbo.Project", new[] { "ProgramId" });
            DropIndex("dbo.Column", new[] { "ProjectId" });
            DropIndex("dbo.BoardTaskComment", new[] { "BoardTaskId" });
            DropIndex("dbo.BoardTask", new[] { "SprintId" });
            DropIndex("dbo.BoardTask", new[] { "ColumnId" });
            DropIndex("dbo.BoardCommentAttachment", new[] { "BoardTaskCommentId" });
            DropTable("dbo.Sprint");
            DropTable("dbo.Program");
            DropTable("dbo.Project");
            DropTable("dbo.Column");
            DropTable("dbo.BoardTaskComment");
            DropTable("dbo.BoardTask");
            DropTable("dbo.BoardCommentAttachment");
        }
    }
}
