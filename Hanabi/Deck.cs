namespace Hanabi
{
    public class Deck : ICloneable
    {
        private int _index = 0;
        internal List<Card> _cards;

        public int NumCardsRemaining => _cards.Count - _index;

        public Deck(List<Card> cards)
        {
            _cards = cards;
        }

        internal Card? DrawCard()
        {
            if (_index >= _cards.Count)
                return null;

            _index++;
            return _cards[_index - 1];
        }

        public static Deck Random(Randomizer randomizer)
        {
            var cards = new List<Card>();
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                cards.Add(new Card(color, 1));
                cards.Add(new Card(color, 1));
                cards.Add(new Card(color, 1));

                for (int number = 2; number < 5; number++)
                {
                    cards.Add(new Card(color, number));
                    cards.Add(new Card(color, number));
                }

                cards.Add(new Card(color, 5));
            }

            Shuffle(cards, randomizer);
            return new Deck(cards);
        }

        static void Shuffle<T>(List<T> list, Randomizer randomizer)
        {
            int n = list.Count;
            for (int i = 0; i < n - 1; i++)
            {
                int j = randomizer.Next(i, n - 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        public object Clone()
        {
            return new Deck(new List<Card>(_cards));
        }

        public static int NumInstances(int number) => number == 5 ? 1 : (number == 1 ? 3 : 2);
    }
}
