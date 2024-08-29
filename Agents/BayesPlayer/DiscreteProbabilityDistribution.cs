using Hanabi;

namespace Agents.BayesPlayer
{
    public class DiscreteProbabilityDistribution<T> where T : notnull
    {
        private T[] _items;
        private double[] _lowerLimits;
        private Randomizer _random;
        public DiscreteProbabilityDistribution(Dictionary<T, double> probabilities, Randomizer random)
        {
            _items = probabilities.Keys.ToArray();
            _lowerLimits = new double[_items.Count()];

            for (int i = 1; i < _items.Count(); i++)
            {
                _lowerLimits[i] = _lowerLimits[i - 1] + probabilities[_items[i - 1]];
            }

            foreach (double d in _lowerLimits)
            {
                if (double.IsNaN(d))
                {
                    throw new Exception();
                }
            }

            _random = random;
        }

        public T GetNext()
        {
            double rand = _random.NextDouble();
            for (int iValue = 0; iValue < _items.Length; iValue++)
            {
                double lowerLimit = _lowerLimits[iValue];
                double upperLimit = iValue == _lowerLimits.Length - 1 ? 1 : _lowerLimits[iValue + 1];
                if (rand >= lowerLimit && rand <= upperLimit)
                    return _items[iValue];
            }

            throw new Exception("Implementation error in DiscreteProbabilityDistribution");
        }
    }
}
