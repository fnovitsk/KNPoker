using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console.Cli;

namespace KNPoker
{
    public class CreateCacheCommand : Command<CreateCacheCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<filePath>")]
            public required string FilePath { get; set; }
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var filePath = settings.FilePath;

            // Check if the provided path is a directory
            if (Directory.Exists(filePath) || filePath.EndsWith(Path.DirectorySeparatorChar.ToString()) || filePath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                filePath = Path.Combine(filePath, "cache.csv");
            }

            try
            {
                var equityCommand = new EquityCommand();
                var equitySettings = new EquityCommand.Settings
                {
                    FirstRange = "AA-KK",
                    SecondRange = "JTs"
                };
                var equityResultsTask = equityCommand.GetEquityResults(context, equitySettings);
                equityResultsTask.Wait();
                var equityResults = equityResultsTask.Result;

                File.WriteAllLines(filePath, equityResults);
                Console.WriteLine($"Cache file created at {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating cache file: {ex.Message}");
                return -1;
            }
            return 0;
        }
    }
}
