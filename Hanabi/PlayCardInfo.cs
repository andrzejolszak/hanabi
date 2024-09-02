
namespace Hanabi
{
    /// <summary>
    /// Contains information visible to ALL players after a card is played
    /// </summary>
    public class PlayCardInfo : IMoveInfo
    {
        public int PlayerIndex { get; init; }
        public int HandIndex { get; init; }
        public Guid CardId { get; init; }
        public Color CardColor { get; init; }
        public int CardNumber { get; init; }
        public bool WasSuccess { get; init; }

        public static string FormatMoveText(int handIndex) => $"play {handIndex}";

        public static bool IsPlay(string move) => move.StartsWith("play ");
    }
}
