using Hanabi;

namespace Hanabi
{
    public class PrivateGameView
    {
        int _playerIndex;
        Game _game;

        public PrivateGameView(int playerIndex, Game game)
        {
            _playerIndex = playerIndex;
            _game = game;
        }

        string GetColorName(Color color) => Enum.GetName(typeof(Color), color)?.ToLower() ?? "";

        public IEnumerable<string> AvailableMoves()
        {
            List<Card> hand = _game.PlayerHands[_game.CurrentPlayer];

            IEnumerable<string> playOptions = hand.Select((_, i) => $"discard {i}");
            IEnumerable<string> discardOptions = hand.Select((_, i) => $"play {i}");

            if (_game.NumTokens <= 0)
                return playOptions.Concat(discardOptions);

            IEnumerable<string> tellOptions = _game.PlayerHands.SelectMany((otherHand, iPlayer) =>
            {
                if (iPlayer == _game.CurrentPlayer)
                    return new List<string>();

                var colorOptions = otherHand.Select(card => card.Color)
                    .Distinct()
                    .Select(color => $"tell player {iPlayer} about color {GetColorName(color)}");

                var numberOptions = otherHand.Select(card => card.Number)
                    .Distinct()
                    .Select(number => $"tell player {iPlayer} about number {number}");

                return colorOptions.Concat(numberOptions);
            });

            return playOptions.Concat(discardOptions).Concat(tellOptions);
        }

        /// <summary>
        /// Returns the game state that would result if the hidden cards had the provided values and the player made the provided move
        /// </summary>
        public Game TestMove(string move, IEnumerable<Card> hypotheticalHand, Card? hypotheticalNextCard)
        {
            Game hypotheticalGame = GameWithHypothesis(hypotheticalHand, hypotheticalNextCard);

            string[] tokens = move.Split();

            if (tokens[0] == "tell")
                return ApplyTell(hypotheticalGame, tokens, informAgents: false);

            switch (tokens[0])
            {
                case "play":
                    hypotheticalGame.PlayCard(int.Parse(tokens[1]), informAgents: false);
                    break;
                case "discard":
                    hypotheticalGame.Discard(int.Parse(tokens[1]), informAgents: false);
                    break;
            }

            return hypotheticalGame;
        }

        private Game GameWithHypothesis(IEnumerable<Card> hypotheticalHand, Card? hypotheticalNextCard)
        {
            var ret = _game.Clone();
            ret.PlayerHands[_playerIndex] = hypotheticalHand.ToList();

            var hypotheticalDeck = new List<Card>();
            if (hypotheticalNextCard != null)
                hypotheticalDeck.Add(hypotheticalNextCard);

            ret.Deck = new Deck(hypotheticalDeck);
            return ret;
        }

        Game ApplyTell(Game gameClone, string[] moveTokens, bool informAgents = true)
        {
            int playerIndex = int.Parse(moveTokens[2]);

            if (moveTokens[4] == "color")
            {
                Color color = Enum.Parse<Color>(moveTokens[5], ignoreCase: true);
                gameClone.TellColor(playerIndex, color, informAgents);
                return gameClone;
            } else
            {
                int number = int.Parse(moveTokens[5]);
                gameClone.TellNumber(playerIndex, number, informAgents);
                return gameClone;
            }
        }

        public int NumPlayers => _game.NumPlayers;
        public int NumTokens => _game.NumTokens;
        public int NumLives => _game.NumLives;
        public List<Card> DiscardPile => _game.DiscardPile;
        public IMoveInfo LastMoveInfo => _game.LastMoveInfo;
        public int CardsPerPlayer => _game.CardsPerPlayer;

        public List<List<Card>> OtherHands
        {
            get
            {
                var hands = _game.PlayerHands.ToList();
                hands.RemoveAt(_playerIndex);
                return hands;
            }
        }
    }
}
