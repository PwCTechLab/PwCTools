namespace PwCTools.ApplicationContextMigrations
{
    using DAL;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<PwCTools.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"ApplicationContextMigrations";
        }

        protected override void Seed(PwCTools.Models.ApplicationDbContext context)
        {
            IdentityInitializer.InitializeIdentityForEF(context);
        }
    }
}
