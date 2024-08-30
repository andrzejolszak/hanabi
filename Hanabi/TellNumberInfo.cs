namespace Hanabi
{
    public class TellNumberInfo : IMoveInfo
    {
        public int PlayerIndex { get; init; }
        public int RecipientIndex { get; init; }
        public int Number { get; init; }
        public List<(int HandIndex, Guid CardId, bool NewKnowledge)> HandIndexes { get; init; }

        public static string FormatMoveText(int playerIndex, int number) => $"tell player {playerIndex} about number {number}";
    }
}
