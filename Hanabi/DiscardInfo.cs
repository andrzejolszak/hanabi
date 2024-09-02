
namespace Hanabi
{
    public class DiscardInfo : IMoveInfo
    {
        public int PlayerIndex { get; init; }
        public int HandIndex { get; init; }
        public Guid CardId { get; init; }
        public Color CardColor { get; init; }
        public int CardNumber { get; init; }

        public static string FormatMoveText(int handIndex) => $"discard {handIndex}";

        public static bool IsDiscard(string move) => move.StartsWith("discard ");
    }
}
