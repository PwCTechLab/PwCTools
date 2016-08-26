namespace PwCTools.BoardContextMigrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PwCTools.DAL.BoardContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"BoardContextMigrations";
        }

        protected override void Seed(PwCTools.DAL.BoardContext context)
        {
            DAL.BoardInitializer.InitializeBoardForEF(context);
        }
    }
}
