namespace Hanabi
{
    public class Game
    {
        public static int MAX_TOKENS = 8;

        bool _isLastRound;
        int _playerWhoDrewLastCard = -1;
        Dictionary<int, (IPlayer Player, PrivateGameView PrivateView)> _players = new();

        public int NumPlayers { get; }
        public int NumLives { get; internal set; }
        public int NumTokens { get; internal set; } = MAX_TOKENS;
        public int CurrentPlayer { get; internal set; } = 0;
        public List<List<Card>> PlayerHands { get; internal set; } = new List<List<Card>>();
        public int CardsPerPlayer { get; }
        public Deck Deck { get; set;  }
        public bool IsOver { get; internal set; }
        public Dictionary<Color, int> Stacks { get; set; }
        public List<Card> DiscardPile { get; internal set; } = new List<Card>();

        /// <summary>
        /// Information about the last move that took place and its consequences.
        /// Useful for agents to update their probabilities after a move.
        /// This must be cast to the appropriate MoveInfo subclass depending on the type of move that was made.
        /// </summary>
        public IMoveInfo? LastMoveInfo { get; private set; }

        public Game(int numPlayers, Deck deck, int numStartingLives = 3, bool doSetup = true)
        {
            Deck = deck;
            NumLives = numStartingLives;
            NumPlayers = numPlayers;

            CardsPerPlayer = numPlayers > 3 ? 4 : 5;
            Stacks = new Dictionary<Color, int>();

            if (doSetup)
            {
                for (int i = 0; i < numPlayers; i++)
                {
                    var hand = new List<Card>();
                    for (int j = 0; j < CardsPerPlayer; j++)
                    {
                        Card? card = deck.DrawCard();
                        if (card != null)
                            hand.Add(card);
                    }

                    PlayerHands.Add(hand);
                }

                // Initialise stacks
                Stacks = new Dictionary<Color, int>();
                foreach (var color in Enum.GetValues(typeof(Color)))
                {
                    Stacks[(Color)color] = 0;
                }
            }
        }

        public void RegisterAgent(int playerIndex, IPlayer agent)
        {
            PrivateGameView view = new PrivateGameView(playerIndex, this);
            agent.Init(playerIndex, view);
            _players[playerIndex] = (agent, view);
        }

        public void Discard(int positionInHand, bool informAgents = true)
        {
            if (NumTokens < MAX_TOKENS)
                NumTokens++;

            Card discardedCard = PlayerHands[CurrentPlayer][positionInHand];
            PlayerHands[CurrentPlayer].RemoveAt(positionInHand);
            DiscardPile.Add(discardedCard);

            Card? nextCard = Deck.DrawCard();

            if (nextCard != null)
                PlayerHands[CurrentPlayer].Add(nextCard);

            var moveInfo = new DiscardInfo
            {
                PlayerIndex = CurrentPlayer,
                HandPosition = positionInHand,
                CardColor = discardedCard.Color,
                CardNumber = discardedCard.Number
            };

            if (informAgents)
            {
                foreach (var agent in _players.Values)
                {
                    agent.Player.ObserveMove(moveInfo);
                }
            }

            EndTurn();
        }

        public void TellColor(int player, Color color, bool informAgents = true)
        {
            if (NumTokens <= 0)
                throw new RuleViolationException("At least one token is required to tell anything");

            if (player == CurrentPlayer)
                throw new RuleViolationException("You cannot tell yourself anything");

            if (!PlayerHands[player].Any(card => card.Color == color))
                throw new RuleViolationException("You can only tell a player about a color if they are " +
                    "holding at least one card of that color");

            NumTokens--;

            var moveInfo = new TellColorInfo
            {
                PlayerIndex = CurrentPlayer,
                RecipientIndex = player,
                Color = color,
                HandPositions = Enumerable.Range(0, CardsPerPlayer).Where(i => PlayerHands[player][i].Color == color).ToList()
            };

            if (informAgents)
            {
                foreach (var agent in _players.Values)
                {
                    agent.Player.ObserveMove(moveInfo);
                }
            }
            
            EndTurn();
        }

        public void TellNumber(int player, int number, bool informAgents = true)
        {
            if (NumTokens <= 0)
                throw new RuleViolationException("At least one token is required to tell anything");

            if (player == CurrentPlayer)
                throw new RuleViolationException("You cannot tell yourself anything");

            if (!PlayerHands[player].Any(card => card.Number == number))
                throw new RuleViolationException("You can only tell a player about a number if they are " +
                    "holding at least one card of that number");

            var handPositionsString = Enumerable.Range(0, CardsPerPlayer)
                .Where(i => PlayerHands[player][i].Number == number)
                .Select(i => i.ToString())
                .Aggregate("", (current, next) => current + " " + next);

            NumTokens--;

            var moveInfo = new TellNumberInfo
            {
                PlayerIndex = CurrentPlayer,
                RecipientIndex = player,
                Number = number,
                HandPositions = Enumerable.Range(0, CardsPerPlayer).Where(i => PlayerHands[player][i].Number == number).ToList()
            };

            if (informAgents)
            {
                foreach (var agent in _players.Values)
                {
                    agent.Player.ObserveMove(moveInfo);
                }
            }

            EndTurn();
        }

        public void PlayCard(int positionInHand, bool informAgents = true)
        {
            Card playedCard = PlayerHands[CurrentPlayer][positionInHand];
            
            if (Stacks[playedCard.Color] == playedCard.Number - 1)
            {
                // Card can be played: add it to the stack
                Stacks[playedCard.Color] = playedCard.Number;

                if (playedCard.Number == 5)
                {
                    if (NumTokens < MAX_TOKENS)
                        NumTokens++;

                    IsOver = Stacks.Values.All(x => x == 5);
                }

                if (playedCard.Number == 5 && NumTokens < MAX_TOKENS)
                    NumTokens++;
            } else
            {
                // Card cannot be played: discard it and lose a life
                DiscardPile.Add(playedCard);
                NumLives--;
                if (NumLives == 0)
                    IsOver = true;
            }

            PlayerHands[CurrentPlayer].RemoveAt(positionInHand);

            Card? nextCard = Deck.DrawCard();

            if (nextCard != null)
                PlayerHands[CurrentPlayer].Add(nextCard);

            var moveInfo = new PlayCardInfo
            {
                PlayerIndex = CurrentPlayer,
                HandPosition = positionInHand,
                CardColor = playedCard.Color,
                CardNumber = playedCard.Number,
            };

            if (informAgents)
            {
                foreach (var agent in _players.Values)
                {
                    agent.Player.ObserveMove(moveInfo);
                }
            }

            EndTurn();
        }

        public int Score() => Stacks.Values.Sum();

        public bool IsWinnable()
        {
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                for (int i = 1; i < 6; i++)
                {
                    int numInstances = i == 1 ? 3 : (i == 5 ? 1 : 2);
                    if (DiscardPile.Where(card => card.Color == color && card.Number == i).Count() >= numInstances)
                        return false;
                }
            }

            return true;
        }

        public Game Clone()
        {
            if (Deck.Clone() is not Deck deck) throw new Exception("Cloning deck failed");

            var clonedPlayerHands = new List<List<Card>>();
            foreach (List<Card> hand in PlayerHands)
            {
                clonedPlayerHands.Add(new List<Card>(hand));
            }

            return new Game(NumPlayers, deck, doSetup: false)
            {
                NumLives = NumLives,
                NumTokens = NumTokens,
                CurrentPlayer = CurrentPlayer,
                PlayerHands = clonedPlayerHands,
                _playerWhoDrewLastCard = _playerWhoDrewLastCard,
                _isLastRound = _isLastRound,
                _players = new (_players),
                Stacks = new Dictionary<Color, int>(Stacks),
                DiscardPile = new List<Card>(DiscardPile)
            };
        }

        void EndTurn()
        {
            if (!_isLastRound && Deck.NumCardsRemaining == 0)
            {
                _isLastRound = true;
                _playerWhoDrewLastCard = CurrentPlayer;
            }
            else if (_isLastRound && CurrentPlayer == _playerWhoDrewLastCard)
            {
                IsOver = true;
            }

            CurrentPlayer = (CurrentPlayer + 1) % NumPlayers;
        }
    }
}
