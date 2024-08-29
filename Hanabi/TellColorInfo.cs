using System.Drawing;

namespace Hanabi
{
    public class TellColorInfo : IMoveInfo
    {
        public int PlayerIndex { get; set; }
        public int RecipientIndex { get; set; }
        public Color Color { get; set; }
        public List<int> HandIndexes { get; set; } = new List<int>();

        public static string FormatMoveText(int playerIndex, Color color) => $"tell player {playerIndex} about color {PrivateGameView.GetColorName(color)}";
    }
}
