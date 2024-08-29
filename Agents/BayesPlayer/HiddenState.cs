using Hanabi;

namespace Agents.BayesPlayer
{
    public record HiddenState
    {
        public IList<(Color, int)> Hand { get; init; } = new List<(Color, int)>();
        public (Color, int)? NextCard { get; init; }
    }
}
