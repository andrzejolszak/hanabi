using Hanabi;
using CommandLine;
using Agents.BayesPlayer;
using System.Diagnostics;

namespace Agents
{
    public class Program
    {
        public static readonly Func<List<IPlayer>> Players = () => new List<IPlayer>() { new BayesianPlayer(), new BayesianPlayer(), new BayesianPlayer(), new BayesianPlayer() };

        public static void Main(string[] args)
        {
            int seed = Guid.NewGuid().GetHashCode();
            seed = 12343468;

            int numberGames = 10;
            Queue<string>? imposedMoves = null;

            // Test scenario config:
            // numberGames = 1;
            // imposedMoves = new Queue<string>(new []{ "tell player 1 about number 1", "play 0", "discard 2" });

            var randomizer = new Randomizer(seed);
            Console.WriteLine($"Random seed: {randomizer.Seed}");

            int wins = 0;
            int totalScore = 0;
            int crashes = 0;
            long totalLatency = 0;
            for (int g = 0; g < numberGames; g++)
            {
                Console.WriteLine($"GAME #{g}:");
                List<IPlayer> agents = Program.Players();
                var game = new Game(agents.Count, Deck.Random(randomizer));
                for (int i = 0; i < agents.Count; i++)
                {
                    game.RegisterAgent(i, agents[i]);
                }

                var runner = new GameRunner(game, agents);
                
                long before = Stopwatch.GetTimestamp();
                try
                {
                    runner.Run(imposedMoves, imposedMoves?.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Crash: {ex.Message}");
                    crashes++;
                }
                
                totalLatency = Stopwatch.GetTimestamp() - before;

                totalScore += game.Score();
                wins += game.Stacks.Values.All(x => x == 5) ? 1 : 0;
            }

            Console.WriteLine($"==================================================================");
            Console.WriteLine($"==================================================================");
            Console.WriteLine($" Total results after {numberGames} game(s):");
            Console.WriteLine($" Players: {string.Join(", ", Program.Players().Select(x => x.GetType().Name))}");
            Console.WriteLine($" Wins: {wins}");
            Console.WriteLine($" Total score: {totalScore}");
            Console.WriteLine($" Crashes: {crashes}");
            Console.WriteLine($" Single game latency: {Math.Round((totalLatency / (float)numberGames)/Stopwatch.Frequency, 3)}s");
        }

        public static void HandleParseError(IEnumerable<Error> errors)
        {
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
        }
    }
}
