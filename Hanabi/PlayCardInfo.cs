namespace Hanabi
{
    /// <summary>
    /// Contains information visible to ALL players after a card is played
    /// </summary>
    public class PlayCardInfo : IMoveInfo
    {
        public int PlayerIndex { get; set; }
        public int HandPosition { get; set; }
        public Color CardColor { get; set; }
        public int CardNumber { get; set; }
    }
}
