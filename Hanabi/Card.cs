namespace Hanabi
{
    public class Card
    {
        public Color Color { get; }
        public bool ColorKnown { get; private set; }
        public int Number { get; }
        public bool NumberKnown { get; private set; }
        public Guid CardId { get; }

        public Card(Color color, int number)
        {
            Color = color;
            Number = number;
            CardId = Guid.NewGuid();
        }

        public override string ToString()
        {
            return $"{Enum.GetName(typeof(Color), Color)[0]}{(this.ColorKnown ? "+" : string.Empty)}{this.Number}{(this.NumberKnown ? "+" : string.Empty)}";
        }

        public bool Equals(Card other)
        {
            return Color == other.Color && Number == other.Number;
        }

        internal void SetNumberKnown()
        {
            NumberKnown = true;
        }

        internal void SetColorKnown()
        {
            ColorKnown = true;
        }
    }
}
