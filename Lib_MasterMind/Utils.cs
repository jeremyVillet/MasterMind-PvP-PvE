using System;
using System.Collections.Generic;

namespace Lib_MasterMind
{
    static public class Utils
    {
        public static Dictionary<int, ConsoleColor> colorPawns = new Dictionary<int, ConsoleColor>()
            {
                 {-1,ConsoleColor.White},
                {0,ConsoleColor.White},
                {1,ConsoleColor.Red },
                {2,ConsoleColor.Blue},
                {3,ConsoleColor.Yellow },
                {4,ConsoleColor.Green },
            };
        public struct DataClient
        {
            public int Column;
            public int AttempsRemaining;
            public int[] WinningCombination;
            public int[] CombinationToTest;
            public int[] CurrentGuessChecked;
            public string StateGame;
            public int GameMode;
        }
        public static void AllocateArrays(ref DataClient dataClient)
        {
            dataClient.CombinationToTest = new int[dataClient.Column];
            dataClient.CurrentGuessChecked = new int[dataClient.Column];
            dataClient.WinningCombination = new int[dataClient.Column];
            for(int x = 0; x < dataClient.Column; x++)
            {
                dataClient.CurrentGuessChecked[x] = -1;
            }
        }

    }
}
