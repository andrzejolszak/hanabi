namespace Hanabi
{
    public interface IPlayer
    {
        void Init(int playerIndex, PrivateGameView privateGameView);

        string TakeTurn();

        void ObserveMove(IMoveInfo moveInfo);
    }
}
