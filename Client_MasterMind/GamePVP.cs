using System;
using Lib_MasterMind;
using System.Collections.Generic;

namespace Client_MasterMind
{
    public class GamePVP : Game
    {
        // En fonction de la clé (dataClient.StateGame) recu par le serveur , On executera la methode associé => plus opti qu'un switch au niveau du cpu 
        private delegate void ActionRef<GamePVP,DataClient>(ref GamePVP gamePvp ,ref DataClient dataClient);
        private Dictionary<string, ActionRef<GamePVP,Utils.DataClient>> dataProcess = new Dictionary<string, ActionRef<GamePVP,Utils.DataClient>>()
            {
                 {"Setting game's parameters", SettingGameParameters},
                {"Choice combination to find",ChoiceCombinationToFind},
                {"Must find the winning combination",  FindWinningCombination},
                {"Waiting opponnant find winning combination", FindWinningCombination },
            };
        public GamePVP() : base() { }

        protected override void DoAGame()
        {
            Utils.DataClient dataCurrentRound = new Utils.DataClient();
            dataCurrentRound.GameMode = 1;
            dataCurrentRound.StateGame = "Initialize";

            while (dataCurrentRound.StateGame != "You have lost" && dataCurrentRound.StateGame != "You have won")
            {
                var currentGamePVP = this;
                if (dataProcess.ContainsKey(dataCurrentRound.StateGame))                
                   this.dataProcess[dataCurrentRound.StateGame](ref currentGamePVP, ref dataCurrentRound); // Traitement des donnée en fonction de la clé dataCurrentRound.StateGame reçue

                // Actualise les données avec le serveur
                UtilsClientTCP.RequestServer(ref dataCurrentRound);
                Console.Clear();
                Console.WriteLine($"Game State : {dataCurrentRound.StateGame}");
                if (dataCurrentRound.StateGame == "Sorry a game have already started between 2 player :(" || dataCurrentRound.StateGame == "The connection with the server have been lost")
                    break;
            }
            Console.WriteLine("\n\n Press enter to back to the menu");
            Console.ReadLine();
            Console.Clear();
        }

        /////////////// Traitement des données  ///////////////
        static private void SettingGameParameters(ref GamePVP gamePvp ,ref Utils.DataClient dataCurrentRound)
        {
            gamePvp.ChooseParameter(gamePvp);
            gamePvp.AllocateArrays();
            dataCurrentRound.Column = gamePvp.Column;
            dataCurrentRound.AttempsRemaining = gamePvp.Attempts;
            Utils.AllocateArrays(ref dataCurrentRound);
            dataCurrentRound.StateGame = "Setting game's parameters Done";
        }
        static private void ChoiceCombinationToFind(ref GamePVP gamePvp, ref Utils.DataClient dataCurrentRound)
        {
            gamePvp.Column = dataCurrentRound.Column;
            gamePvp.Attempts = dataCurrentRound.AttempsRemaining;
            Utils.AllocateArrays(ref dataCurrentRound);
            gamePvp.HumanGiveCombination(dataCurrentRound.WinningCombination,1);
            dataCurrentRound.StateGame = "Choice combination to find Done";
        }
        static private void FindWinningCombination(ref GamePVP gamePvp, ref Utils.DataClient dataCurrentRound)
        {
            if (gamePvp.Board == null)          
                gamePvp.AllocateArrays();
            

            int currentRow = gamePvp.Attempts - dataCurrentRound.AttempsRemaining - 1;
            if (currentRow >= 0)
            {
                for (int i = 0; i < gamePvp.Column; i++)
                {
                    gamePvp.Board[currentRow, i] = dataCurrentRound.CombinationToTest[i];
                    gamePvp.GuessChecked[currentRow, i] = dataCurrentRound.CurrentGuessChecked[i];
                }
            }
            gamePvp.PrintGameBoard();

            if (dataCurrentRound.StateGame == "Must find the winning combination")
            {
                Console.WriteLine("\n\n Press enter to continue");
                Console.ReadLine();
                gamePvp.HumanGiveCombination(dataCurrentRound.CombinationToTest,2);
            }
        }
    }
}
