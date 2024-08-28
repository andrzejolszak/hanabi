using Hanabi;

namespace Agents
{
    public class GameRunner
    {
        private Game _game;
        private IList<BayesianPlayer> _agents;
        private Randomizer _randomizer;

        public GameRunner(Game game, IList<BayesianPlayer> agents, Randomizer randomizer)
        {
            _game = game;
            _agents = agents;
            _randomizer = randomizer;
        }

        public void Run(bool debug)
        {
            while (!_game.IsOver)
            {
                if (debug)
                    ReadCommand();

                string move = _agents[_game.CurrentPlayer].TakeTurn();
                Console.WriteLine($"Player {_game.CurrentPlayer}: {move}");
                this.MakeMove(move);
            }

            Console.WriteLine($"Game over! Final score: {_game.Score()}");
        }

        public void MakeMove(string move)
        {
            string[] tokens = move.Split();

            if (tokens[0] == "tell")
            {
                Tell(tokens);
                return;
            }

            switch (tokens[0])
            {
                case "play":
                    _game.PlayCard(int.Parse(tokens[1]));
                    return;
                case "discard":
                    _game.Discard(int.Parse(tokens[1]));
                    return;
            }
        }


        void Tell(string[] moveTokens)
        {
            int playerIndex = int.Parse(moveTokens[2]);

            if (moveTokens[4] == "color")
            {
                Color color = Enum.Parse<Color>(moveTokens[5], ignoreCase: true);
                _game.TellColor(playerIndex, color);
            }
            else
            {
                int number = int.Parse(moveTokens[5]);
                _game.TellNumber(playerIndex, number);
            }
        }

        private void ReadCommand()
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrEmpty(input))
                    return;

                if (input.StartsWith("options"))
                {
                    string[] tokens = input.Split();
                    int playerIndex = int.Parse(tokens[1]);

                    var agent = _agents[playerIndex];

                    var trackerTables = agent.HandOptionTrackers.Select(tracker => tracker.TableRepresentation())
                        .Append(agent.DeckOptionTracker.TableRepresentation());

                    int tableWidth = trackerTables.First().First().Length + 3;

                    // Write headers for tracker tables
                    var tableNames = agent.HandOptionTrackers.Select((tracker, i) => $"Hand Pos {i}")
                        .Append("Deck")
                        .Select(name => name.PadRight(tableWidth));

                    string headerLine = string.Join("", tableNames);

                    Console.WriteLine();
                    Console.WriteLine(headerLine);
                    for (int iLine = 0; iLine < trackerTables.First().Count(); iLine++)
                    {
                        string line = string.Join("   ", trackerTables.Select(table => table.ElementAt(iLine)));
                        Console.WriteLine(line);
                    }
                    Console.WriteLine();
                }
            }

        }
    }
}
