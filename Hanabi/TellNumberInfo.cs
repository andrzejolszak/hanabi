namespace Hanabi
{
    public class TellNumberInfo : IMoveInfo
    {
        public int PlayerIndex { get; set; }
        public int RecipientIndex { get; set; }
        public int Number { get; set; }
        public List<int> HandIndexes { get; set; } = new List<int>();

        public static string FormatMoveText(int playerIndex, int number) => $"tell player {playerIndex} about number {number}";
    }
}
