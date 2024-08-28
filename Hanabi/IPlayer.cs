namespace Hanabi
{
    public interface IPlayer
    {
        void Init(int playerIndex, PrivateGameView gameAdapter);

        string TakeTurn();

        void ObserveMove(IMoveInfo moveInfo);
    }
}
