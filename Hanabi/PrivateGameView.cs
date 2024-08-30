using Hanabi;
using System.Collections.Concurrent;
using System.Formats.Asn1;

namespace Hanabi
{
    public class PrivateGameView
    {
        int _playerIndex;
        Game _game;
        private ConcurrentDictionary<object, object> _userDataTags = new ConcurrentDictionary<object, object>();

        public PrivateGameView(int playerIndex, Game game)
        {
            _playerIndex = playerIndex;
            _game = game;
        }

        public void SetTag(Card card, object tag)
        {
            this._userDataTags[card] = tag;
        }

        public void SetTag(int player, object tag)
        {
            this._userDataTags[player] = tag;
        }

        public void SetTag(Guid ownCardId, object tag)
        {
            this._userDataTags[ownCardId] = tag;
        }

        public static string GetColorName(Color color) => Enum.GetName(typeof(Color), color)?.ToLower() ?? "";

        public IEnumerable<string> GetAvailableMoves()
        {
            List<Card> hand = _game.PlayerHands[_game.CurrentPlayer];

            IEnumerable<string> playOptions = hand.Select((_, i) => DiscardInfo.FormatMoveText(i));
            IEnumerable<string> discardOptions = hand.Select((_, i) => PlayCardInfo.FormatMoveText(i));

            if (_game.NumTokens <= 0)
                return playOptions.Concat(discardOptions);

            IEnumerable<string> tellOptions = _game.PlayerHands.SelectMany((otherHand, iPlayer) =>
            {
                if (iPlayer == _game.CurrentPlayer)
                    return new List<string>();

                var colorOptions = otherHand.Select(card => card.Color)
                    .Distinct()
                    .Select(color => TellColorInfo.FormatMoveText(iPlayer, color));

                var numberOptions = otherHand.Select(card => card.Number)
                    .Distinct()
                    .Select(number => TellNumberInfo.FormatMoveText(iPlayer, number));

                return colorOptions.Concat(numberOptions);
            });

            return playOptions.Concat(discardOptions).Concat(tellOptions);
        }

        /// <summary>
        /// Returns the game state that would result if the hidden cards had the provided values and the player made the provided move
        /// </summary>
        public PrivateGameView TestMove(string move, IEnumerable<Card> hypotheticalHand, Card? hypotheticalNextCard)
        {
            Game hypotheticalGame = _game.Clone();
            hypotheticalGame.PlayerHands[_playerIndex] = hypotheticalHand.ToList();

            var hypotheticalDeck = new List<Card>();
            if (hypotheticalNextCard != null)
                hypotheticalDeck.Add(hypotheticalNextCard);

            hypotheticalGame.Deck = new Deck(hypotheticalDeck);
            string[] tokens = move.Split();

            if (tokens[0] == "tell")
            {
                int playerIndex = int.Parse(tokens[2]);

                if (tokens[4] == "color")
                {
                    Color color = Enum.Parse<Color>(tokens[5], ignoreCase: true);
                    hypotheticalGame.TellColor(playerIndex, color, false);
                    return new PrivateGameView(this._playerIndex, hypotheticalGame);
                }
                else
                {
                    int number = int.Parse(tokens[5]);
                    hypotheticalGame.TellNumber(playerIndex, number, false);
                    return new PrivateGameView(this._playerIndex, hypotheticalGame);
                }
            }

            switch (tokens[0])
            {
                case "play":
                    hypotheticalGame.PlayCard(int.Parse(tokens[1]), informAgents: false);
                    break;
                case "discard":
                    hypotheticalGame.Discard(int.Parse(tokens[1]), informAgents: false);
                    break;
            }

            return new PrivateGameView(this._playerIndex, hypotheticalGame);
        }

        public void ReorderHand(List<Guid> reorderedHand)
        {
            if (this.OwnHand.Count != reorderedHand.Count || reorderedHand.Count != reorderedHand.ToHashSet().Count || this.OwnHand.Concat(reorderedHand).ToHashSet().Count != reorderedHand.Count)
            {
                throw new InvalidOperationException("Invalid card reordering");
            }

            List<Card> target = this._game.PlayerHands[this._playerIndex];
            Dictionary<Guid, Card> cardsCopy = target.ToDictionary(x => x.CardId);

            target.Clear();
            foreach(Guid g in reorderedHand)
            {
                target.Add(cardsCopy[g]);
            }
        }

        public int GetOwnCardIndex(Guid cardId)
        {
            return this._game.PlayerHands[this._playerIndex].FindIndex(x => x.CardId == cardId);
        }

        public Guid GetCardId(int handIndex)
        {
            return this._game.PlayerHands[this._playerIndex][handIndex].CardId;
        }

        public int GetPlayerCardIndex(int playerIndex, Card card)
        {
            if (playerIndex == this._playerIndex)
            {
                throw new InvalidOperationException("Invalid other player index");
            }

            return this._game.PlayerHands[playerIndex].IndexOf(card);
        }

        public Card GetPlayerCard(int playerIndex, int handIndex)
        {
            if (playerIndex == this._playerIndex)
            {
                throw new InvalidOperationException("Invalid other player index");
            }

            return this._game.PlayerHands[playerIndex][handIndex];
        }

        public int GetStackValue(Color color) => this._game.Stacks[color];

        public int NumPlayers => _game.NumPlayers;
        public int NumTokens => _game.NumTokens;
        public int NumLives => _game.NumLives;
        public List<Card> DiscardPile => _game.DiscardPile;
        public IMoveInfo? LastMoveInfo => _game.LastMoveInfo;
        public int CardsPerPlayer => _game.CardsPerPlayer;

        public List<(int PlayerIndex, List<Card> Hand)> OtherHands
        {
            get
            {
                List<(int, List<Card>)> hands = new List<(int, List<Card>)>();
                for(int i = 0; i < this._game.PlayerHands.Count; i++)
                {
                    if (i == this._playerIndex)
                    {
                        continue;
                    }

                    hands.Add((i, this._game.PlayerHands[i]));
                }

                return hands;
            }
        }

        public List<Guid> OwnHand => this._game.PlayerHands[this._playerIndex].Select(x => x.CardId).ToList();

        public List<(Guid CardId, Color? Color, int? Number)> OwnHandCardsKnowledge => this._game.PlayerHands[this._playerIndex].Select(x => (x.CardId, x.ColorKnown ? x.Color : (Color?)null, x.NumberKnown ? x.Number : (int?)null)).ToList();

        public bool IsWinnable => this._game.IsWinnable();

        public int Score => this._game.Score();

        public int CardCount => this._game.PlayerHands[this._playerIndex].Count;
    }
}
