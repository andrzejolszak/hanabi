﻿using Hanabi;

namespace Agents
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var randomizer = new Randomizer();
            Console.WriteLine($"Random seed: {randomizer.Seed}");

            int numPlayers = 4;

            var agents = new List<BayesianAgent>();
            var game = new Game(numPlayers, Deck.Random(randomizer));
            for (int i = 0; i < numPlayers; i++)
            {
                var view = new GameView(i, game);
                var agent = new BayesianAgent(i, view);
                game.RegisterAgent(i, agent);
                agents.Add(agent);
            }

            while (!game.IsOver)
            {
                agents[game.CurrentPlayer].TakeTurn(randomizer);
            }

            Console.WriteLine($"Game over! Final score: {game.Score()}");
        }
    }
}
