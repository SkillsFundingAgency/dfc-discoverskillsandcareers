using System;
using DbUp;
using System.IO;

namespace Dfc.DiscoverSkillsAndCareers.ChangeFeed.DbDeploy
{
    public class MigrateDatabaseHelper
    {
        public static void Deploy(string connectionString, string scriptPath)
        {
            scriptPath = Path.Combine(Environment.CurrentDirectory, scriptPath);
            if (!Directory.Exists(scriptPath))
            {
                throw new Exception($"Migration directory {scriptPath} does not exist");
            }
            Console.WriteLine("Ensuring database exists.");
            EnsureDatabase.For.SqlDatabase(connectionString);
            var migrationCount = Directory.GetFiles(scriptPath).Length;
            Console.WriteLine($"Migrating database. ({migrationCount} files)");
            var result =
                  DeployChanges.To
                      .SqlDatabase(connectionString)
                      .WithScriptsFromFileSystem(scriptPath)
                      .LogToConsole()
                      .Build()
                      .PerformUpgrade();
            if (result.Successful)
            {
                Console.WriteLine("Database migration complete.");
            }
            else
            {
                throw new Exception($"Database migration failed: {result.Error}");
            }
        }
    }
}
