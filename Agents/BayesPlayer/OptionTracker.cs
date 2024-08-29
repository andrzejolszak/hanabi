using Hanabi;

namespace Agents.BayesPlayer
{
    public class OptionTracker
    {
        Dictionary<(Color, int), int> _numLeftByType = new Dictionary<(Color, int), int>();

        public OptionTracker(Dictionary<(Color, int), int> numLeftByType)
        {
            _numLeftByType = numLeftByType;
        }

        public int Get(Card card) => Get(card.Color, card.Number);

        public int Get(Color color, int number)
        {
            var key = (color, number);
            return _numLeftByType.ContainsKey(key) ? _numLeftByType[key] : 0;
        }

        public void ColorIs(Color color)
        {
            foreach (Color otherColor in Enum.GetValues<Color>())
            {
                if (otherColor == color)
                    continue;

                for (int i = 1; i < 6; i++)
                {
                    _numLeftByType[(otherColor, i)] = 0;
                }
            }
        }

        public void ColorIsNot(Color color)
        {
            for (int i = 1; i < 6; i++)
            {
                _numLeftByType[(color, i)] = 0;
            }
        }

        public void NumberIs(int number)
        {
            foreach (Color color in Enum.GetValues<Color>())
            {
                for (int otherNumber = 0; otherNumber < 5; otherNumber++)
                {
                    if (otherNumber == number)
                        continue;

                    _numLeftByType[(color, otherNumber)] = 0;
                }
            }
        }

        public void NumberIsNot(int number)
        {
            foreach (Color color in Enum.GetValues<Color>())
            {
                _numLeftByType[(color, number)] = 0;
            }
        }

        public void RemoveInstance(Color color, int number)
        {
            if (_numLeftByType[(color, number)] > 0)
                _numLeftByType[(color, number)]--;
        }

        public Dictionary<(Color, int), double> GetProbabilities()
        {
            int numLeftTotal = _numLeftByType.Values.Sum();
            return _numLeftByType.ToDictionary(kv => kv.Key, kv => (double)kv.Value / numLeftTotal);
        }

        public OptionTracker Clone()
        {
            return new OptionTracker(new Dictionary<(Color, int), int>(_numLeftByType));
        }

        /// <summary>
        /// Returns a tabular representation of the options as a list of lines
        /// </summary>
        public IEnumerable<string> TableRepresentation()
        {
            var colorAbbrevs = new Dictionary<Color, string>
            {
                { Color.Red, "R" },
                { Color.Green, "G" },
                { Color.Blue, "B" },
                { Color.Yellow, "Y" },
                { Color.White, "W" }
            };

            var lines = new List<string>();
            string numberHeader = "  1  2  3  4  5  ";
            lines.Add(numberHeader);
            foreach (Color color in colorAbbrevs.Keys)
            {
                string line = colorAbbrevs[color] + " ";
                foreach (int number in Enumerable.Range(1, 5))
                {
                    int numRemaining = Get(color, number);
                    string numRemainingRep = numRemaining == 0 ? " " : numRemaining.ToString();
                    line += numRemainingRep + "  ";
                }
                lines.Add(line);
            }

            return lines;
        }

        public bool HasOptions() => _numLeftByType.Values.Max() > 0;
    }
}
