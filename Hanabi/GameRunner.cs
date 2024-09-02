using Hanabi;

namespace Agents
{
    public class GameRunner
    {
        private Game _game;
        private IList<IPlayer> _agents;

        public GameRunner(Game game, IList<IPlayer> agents)
        {
            _game = game;
            _agents = agents;
        }

        public void Run(Queue<string>? imposedMoves, int? stopAfterTurns)
        {
            int turn = 0;

            this.PrintStatus();

            while (!_game.IsOver)
            {
                if (stopAfterTurns is not null && turn >= stopAfterTurns)
                {
                    Console.WriteLine($"[{turn}] Stopping early...");
                    break;
                }

                string move = imposedMoves?.Count > 0 ? imposedMoves.Dequeue() : _agents[_game.CurrentPlayer].TakeTurn();

                int player = this._game.CurrentPlayer;
                string details = this.MakeMove(move);
                Console.WriteLine($"[{turn}] Player {player}: {move} [{details}]");
                turn++;
            }

            Console.WriteLine($"Game over: {this._game.GameOverStatus}! Final score: {_game.Score()}");

            this.PrintStatus();
        }

        public void PrintStatus()
        {
            Console.WriteLine(string.Empty);
            Console.WriteLine($"==================================================================");
            Console.WriteLine($"=  Game state:");
            Console.WriteLine($"=* Score: {this._game.Score()}");
            Console.WriteLine($"=* Tokens: {this._game.NumTokens}");
            Console.WriteLine($"=* Lives: {this._game.NumLives}");
            Console.WriteLine($"=* Stacks: | R{this._game.Stacks[Color.Red]} | G{this._game.Stacks[Color.Green]} | B{this._game.Stacks[Color.Blue]} | W{this._game.Stacks[Color.White]} | Y{this._game.Stacks[Color.Yellow]} |");
            Console.WriteLine($"=* Discard pile: <bottom>, {string.Join(", ", this._game.DiscardPile.Select(x => "" + x.Color.ToString()[0] + x.Number))}");
            for (int i = 0; i < this._game.NumPlayers; i++)
            {
                Console.WriteLine($"=* Player {i} hand: {string.Join(", ", this._game.PlayerHands[i])}");
            }

            Console.WriteLine($"==================================================================");
            Console.WriteLine(string.Empty);
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

                    return $"{card}{(success ? " -> OK" : " -> *** FUSE ***")}";
                case "discard":
                    card = this._game.PlayerHands[this._game.CurrentPlayer][int.Parse(tokens[1])];
                    _game.Discard(int.Parse(tokens[1]));
                    return $"{card}";

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
    }
}
