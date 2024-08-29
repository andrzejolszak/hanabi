
namespace Hanabi
{
    public class DiscardInfo : IMoveInfo
    {
        public int PlayerIndex { get; set; }
        public int HandIndex { get; set; }
        public Color CardColor { get; set; }
        public int CardNumber { get; set; }

        public static string FormatMoveText(int handIndex) => $"discard {handIndex}";
    }
}
