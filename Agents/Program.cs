using Hanabi;
using CommandLine;
using System.Collections.Concurrent;
using Agents.BayesPlayer;

namespace Agents
{
    public class Program
    {
        public static readonly ConcurrentBag<IPlayer> PlayerRegistrations = new ConcurrentBag<IPlayer>() { new BayesianPlayer(), new BayesianPlayer(), new BayesianPlayer(), new BayesianPlayer() };

        public static void Main(string[] args)
        {
            Queue<string>? imposedMoves = null;
            int seed = Guid.NewGuid().GetHashCode();

            // Run config:
            // imposedMoves = new Queue<string>(new []{ "tell player 1 about number 1", "play 0", "discard 2" });
            // seed = 3239583;

            var randomizer = new Randomizer(seed);
            Console.WriteLine($"Random seed: {randomizer.Seed}");

            int numPlayers = 4;

            var agents = Program.PlayerRegistrations.ToList();
            var game = new Game(numPlayers, Deck.Random(randomizer));
            for (int i = 0; i < numPlayers; i++)
            {
                game.RegisterAgent(i, agents[i]);
            }

            var runner = new GameRunner(game, agents);
            runner.Run(imposedMoves, imposedMoves?.Count);
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
