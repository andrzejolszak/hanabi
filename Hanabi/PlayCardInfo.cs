
namespace Hanabi
{
    /// <summary>
    /// Contains information visible to ALL players after a card is played
    /// </summary>
    public class PlayCardInfo : IMoveInfo
    {
        public int PlayerIndex { get; set; }
        public int HandIndex { get; set; }
        public Color CardColor { get; set; }
        public int CardNumber { get; set; }

        public static string FormatMoveText(int handIndex) => $"play {handIndex}";
    }
}
