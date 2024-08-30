using System.Drawing;

namespace Hanabi
{
    public class TellColorInfo : IMoveInfo
    {
        public int PlayerIndex { get; init; }
        public int RecipientIndex { get; init; }
        public Color Color { get; init; }
        public List<(int HandIndex, Guid CardId, bool NewKnowledge)> HandIndexes { get; init; }

        public static string FormatMoveText(int playerIndex, Color color) => $"tell player {playerIndex} about color {PrivateGameView.GetColorName(color)}";
    }
}
