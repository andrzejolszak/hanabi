using Hanabi;
using CommandLine;

namespace Agents
{
    public class Options
    {
        [Option('s', "seed", Required = false, HelpText = "You can provide a random seed for reproducible RNG")]
        public int? Seed { get; set; } = null;

        [Option('d', "debug", HelpText = "Runs the game in debug mode, in which the user is prompted to enter debug commands after each round")]
        public bool Debug { get; set; } = false;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseError);
        }

        public static void Run(Options options)
        {
            var randomizer = new Randomizer(options.Seed);
            Console.WriteLine($"Random seed: {randomizer.Seed}");

            int numPlayers = 4;

            var agents = new List<BayesianPlayer>();
            var game = new Game(numPlayers, Deck.Random(randomizer));
            for (int i = 0; i < numPlayers; i++)
            {
                var agent = new BayesianPlayer();
                game.RegisterAgent(i, agent);
                agents.Add(agent);
            }

            var runner = new GameRunner(game, agents, randomizer);
            runner.Run(options.Debug);
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
