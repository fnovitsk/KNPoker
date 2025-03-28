using KNPokerLib;
using Microsoft.AspNetCore.Mvc;
using KNPoker;
using Spectre.Console.Cli;
using System.Collections.Generic;

namespace KNPokerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquityController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetEquity(string firstRange, string secondRange)
        {
            try
            {
                var equityCommand = new EquityCommand();
                var settings = new EquityCommand.Settings
                {
                    FirstRange = firstRange,
                    SecondRange = secondRange
                };

                // Create a proper command context with the required parameters
                var args = new List<string>(); // Empty arguments list
                var remaining = new RemainingArguments(args);
                var commandContext = new CommandContext(args, remaining, "equity", null);

                var results = await equityCommand.GetEquityResults(commandContext, settings);

                // Parse the results
                var parsedResults = new
                {
                    DetailedResults = results.Where(r => !r.StartsWith("Range")).Select(r =>
                    {
                        var parts = r.Split(',');
                        return new
                        {
                            Hand1 = parts[0],
                            Hand1Equity = double.Parse(parts[1]),
                            Hand2 = parts[2],
                            Hand2Equity = double.Parse(parts[3]),
                            TieEquity = double.Parse(parts[4]),
                            Combos = int.Parse(parts[5])
                        };
                    }).ToList(),
                    Summary = results.Where(r => r.StartsWith("Range")).Select(r =>
                    {
                        var parts = r.Split(',');
                        return new
                        {
                            Label = parts[0],
                            Value = double.Parse(parts[1])
                        };
                    }).ToList()
                };

                return Ok(parsedResults);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private class RemainingArguments : IRemainingArguments
        {
            private readonly IReadOnlyList<string> _arguments;

            public RemainingArguments(IReadOnlyList<string> arguments)
            {
                _arguments = arguments ?? new List<string>();
            }

            public string this[int index] => _arguments.Count > index ? _arguments[index] : null;

            public IReadOnlyList<string> Raw => _arguments;

            public int Count => _arguments.Count;

            public ILookup<string, string?> Parsed => throw new NotImplementedException();
        }
    }
}