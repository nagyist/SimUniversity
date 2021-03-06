﻿using System;
using System.Collections.Generic;
using MingStar.SimUniversity.AI.Evaluation;
using MingStar.SimUniversity.AI.Learning;
using MingStar.SimUniversity.AI.Player;
using MingStar.SimUniversity.Board.Constructor;
using MingStar.SimUniversity.Contract;
using MingStar.SimUniversity.Game;
using MingStar.SimUniversity.Game.Random;
using MingStar.Utilities;
using MingStar.Utilities.Linq;
using log4net;

namespace MingStar.SimUniversity.ConsoleUI
{
    internal class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (Program));

        //private static IPlayer _randomAIPlayer = new Random();
        //private static IPlayer _smarterAIPlayer = new Smarter();
        private static readonly ConsoleHumanPlayer _humanConsolePlayer = new ConsoleHumanPlayer();
        //private static IPlayer _expetiMiniMaxAIPlayer = new ExpectiMaxN();

        private static void Main()
        {
            RunMain();
            try
            {
                RunMain();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void RunMain()
        {
            ColorConsole.Write(ConsoleColor.Green, "Learning (L), AI touranament (A) or Play a game (Enter)? ");
            var key = Console.ReadKey().Key;
            Console.WriteLine();
            switch (key)
            {
                case ConsoleKey.A:
                    _log.Info("start ai tournament");
                    Game.Game.RandomEventChance = new DiceCardRandomEvent();
                    RunAITournament(2, 200);
                    break;
                case ConsoleKey.L:
                    _log.Info("start ai learning");
                    Game.Game.RandomEventChance = new DiceCardRandomEvent();
                    var learning = new SimplexLearning(new ConsoleViewer(), new SettlerBoardConstructor());
                    ColorConsole.Write(ConsoleColor.Yellow, "\nHow many evaluations do you want to perform (Enter = 30)?");
                    var num = Console.ReadLine();
                    if (num == "")
                    {
                        num = "30";
                    }
                    ColorConsole.Write(ConsoleColor.Yellow, "\nHow many rounds do you want to run in each tournament (Enter = 21)?");
                    var num2 = Console.ReadLine();
                    if (num2 == "")
                    {
                        num2 = "21";
                    }
                    learning.Learn(int.Parse(num), int.Parse(num2));
                    break;
                default:
                    _log.Info("start human vs. ai");
                    Game.Game.RandomEventChance = new DiceRollRandomEvent();
                    PlayGame();
                    break;
            }
        }

        public static void PlayGame()
        {
            while (true)
            {
                var game = new Game.Game((new SettlerBoardConstructor()).ConstructBoard(), 4);
                IPlayer improvedEMM_AIPlayer = new ImprovedEMN(new GameScores());
                var players = new IPlayer[4];
                players.Fill(improvedEMM_AIPlayer);
                players[RandomGenerator.Next(4)] = _humanConsolePlayer;
                var controller = new GameController(new ConsoleViewer(), game, true, players);
                _humanConsolePlayer.GameController = controller;
                controller.Run();
                Console.WriteLine("Try again? y/n");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();
                if (key.KeyChar == 'n')
                {
                    break;
                }
            }
        }

        public static void RunAITournament(int numPlayers, int round)
        {
            DateTime startTime = DateTime.Now;
            var stats = new Dictionary<string, TournamentPlayerStats>();
            var simplexLearnedScores = SimplexLearnedScores.Load(SimplexLearning.FileName);
            ColorConsole.WriteLine(ConsoleColor.Cyan, "Loaded parameters: {0}", simplexLearnedScores);
            for (int i = 1; i <= round; ++i)
            {
                var game = new Game.Game((new SettlerBoardConstructor()).ConstructBoard(), numPlayers);
                game.Round = i;
                var improvedEmmAiPlayerNormal = new ImprovedEMN(new GameScores());
                var improvedEmmAiPlayerExpansion = new ImprovedEMN(simplexLearnedScores);
                var players = new IPlayer[numPlayers];
                players.Fill(improvedEmmAiPlayerNormal);
                players[RandomGenerator.Next(numPlayers)] = improvedEmmAiPlayerExpansion;
                for (int j = 0; j < numPlayers; ++j)
                {
                    string name = players[j].Name;
                    if (!stats.ContainsKey(name))
                    {
                        stats[name] = new TournamentPlayerStats
                                          {
                                              PlayerName = name
                                          };
                    }
                }
                var controller = new GameController(new ConsoleViewer(), game, false, players);
                int winnerIndex = controller.Run();
                TournamentPlayerStats stat = stats[players[winnerIndex].Name];
                ColorConsole.WriteLine(ConsoleColor.Yellow,
                                       ">>> University {0}, AI player '{1}' has won. <<<",
                                       controller.Game.GetUniversityByIndex(winnerIndex).Color,
                                       stat.PlayerName
                    );
                stat.HasWon();
                foreach (TournamentPlayerStats statForPrint in stats.Values)
                {
                    statForPrint.PrintToConsole();
                }
                ColorConsole.WriteLine(ConsoleColor.Green, "Total time taken: " + (DateTime.Now - startTime));
            }
        }
    }
}