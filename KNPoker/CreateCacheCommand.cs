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
                    FirstRange = "22-AA,A2s-AKs,K2s-KQs,Q2s-QJs,J2s-JTs,T2s-T9s,92s-98s,82s-87s,72s-76s,62s-65s,52s-54s,42s-43s,32s,A2o-AKo,K2o-KQo,Q2o-QJo,J2o-JTo,T2o-T9o,92o-98o,82o-87o,72o-76o,62o-65o,52o-54o,42o-43o,32o",
                    SecondRange = "22-AA,A2s-AKs,K2s-KQs,Q2s-QJs,J2s-JTs,T2s-T9s,92s-98s,82s-87s,72s-76s,62s-65s,52s-54s,42s-43s,32s,A2o-AKo,K2o-KQo,Q2o-QJo,J2o-JTo,T2o-T9o,92o-98o,82o-87o,72o-76o,62o-65o,52o-54o,42o-43o,32o"
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
