using System;
using Lib_MasterMind;
using System.Collections.Generic;

namespace Client_MasterMind
{
    public class GamePVE : Game
    {
        // En fonction de la clé (dataClient.StateGame) recu par le serveur , On executera la methode associé => plus opti qu'un switch au niveau du cpu     
        private delegate void ActionRef<GamePVE, DataClient>(ref GamePVE gamePvp, ref DataClient dataClient);
        private Dictionary<string, ActionRef<GamePVE, Utils.DataClient>> dataProcess = new Dictionary<string, ActionRef<GamePVE, Utils.DataClient>>()
            {
                 {"Setting game's parameters", SettingGameParameters},
                {"Waiting computeur choice winning combination",WaitingComputeurChoiceWinningCombination},
                {"Must find the winning combination",  FindWinningCombination},
            };

        public GamePVE() : base() { }

        protected override void DoAGame()
        {
            Utils.DataClient dataCurrentRound = new Utils.DataClient();
            dataCurrentRound.GameMode = 2;
            dataCurrentRound.StateGame = "Initialize";
           
            while (dataCurrentRound.StateGame != "You have lost" && dataCurrentRound.StateGame != "You have won")
            {
                var currentGamePVE = this;
                if (dataProcess.ContainsKey(dataCurrentRound.StateGame))
                   this.dataProcess[dataCurrentRound.StateGame](ref currentGamePVE, ref dataCurrentRound); // Traitement des donnée en fonction de la clé dataCurrentRound.StateGame reçue

                // Actualise les données avec le serveur
                UtilsClientTCP.RequestServer(ref dataCurrentRound);
                Console.Clear();
                Console.WriteLine($"Game State : {dataCurrentRound.StateGame}");

            }
            Console.WriteLine("\n\n Press enter to back to the menu");
            Console.ReadLine();
            Console.Clear();
        }

        private void ComputeurGiveCombination(int[] combination)
        {
            int column = combination.Length;
            Random random = new Random();
            for (int x = 0; x < column; x++)
            {
                combination[x] = random.Next(1, 5);
            }

        }

        /////////////// Traitement des données  //////////////
        static private void SettingGameParameters(ref GamePVE gamePvE, ref Utils.DataClient dataCurrentRound)
        {
            gamePvE.ChooseParameter(gamePvE);
            gamePvE.AllocateArrays();
            dataCurrentRound.Column = gamePvE.Column;
            dataCurrentRound.AttempsRemaining = gamePvE.Attempts;
            Utils.AllocateArrays(ref dataCurrentRound);
            dataCurrentRound.StateGame = "Setting game's parameters Done";
        }
        static private void WaitingComputeurChoiceWinningCombination(ref GamePVE gamePvE, ref Utils.DataClient dataCurrentRound)
        {
            Utils.AllocateArrays(ref dataCurrentRound);
            gamePvE.ComputeurGiveCombination(dataCurrentRound.WinningCombination);
            dataCurrentRound.StateGame = "Choice combination to find Done";
        }
        static private void FindWinningCombination(ref GamePVE gamePvE, ref Utils.DataClient dataCurrentRound)
        {
            int currentRow = gamePvE.Attempts - dataCurrentRound.AttempsRemaining - 1;
            if (currentRow >= 0)
            {
                for (int i = 0; i < gamePvE.Column; i++)
                {
                    gamePvE.Board[currentRow, i] = dataCurrentRound.CombinationToTest[i];
                    gamePvE.GuessChecked[currentRow, i] = dataCurrentRound.CurrentGuessChecked[i];
                }
            }


            gamePvE.PrintGameBoard();

            Console.WriteLine("\n\n Press enter to continue");
            Console.ReadLine();
           gamePvE.HumanGiveCombination(dataCurrentRound.CombinationToTest,2);
        }

    }
}
