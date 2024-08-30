using Hanabi;

namespace Agents
{
    public class GameRunner
    {
        private Game _game;
        private IList<IPlayer> _agents;
        private Randomizer _randomizer;

        public GameRunner(Game game, IList<IPlayer> agents, Randomizer randomizer)
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
                int player = this._game.CurrentPlayer;
                string details = this.MakeMove(move);
                Console.WriteLine($"Player {player}: {move} [{details}]");
            }

            Console.WriteLine($"Game over: {this._game.GameOverStatus}! Final score: {_game.Score()}");
        }

        public string MakeMove(string move)
        {
            string[] tokens = move.Split();

            if (tokens[0] == "tell")
            {
                Tell(tokens);
                return string.Empty;
            }

            switch (tokens[0])
            {
                case "play":
                    Card card = this._game.PlayerHands[this._game.CurrentPlayer][int.Parse(tokens[1])];
                    bool success = _game.PlayCard(int.Parse(tokens[1]));

                    return $"{card.Color}:{card.Number}{(success ? " -> OK" : " -> *** FUSE ***")}";
                case "discard":
                    card = this._game.PlayerHands[this._game.CurrentPlayer][int.Parse(tokens[1])];
                    _game.Discard(int.Parse(tokens[1]));
                    return $"{card.Color}:{card.Number}";

                default:
                    throw new InvalidOperationException(tokens[0]);
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
            }

        }
    }
}
