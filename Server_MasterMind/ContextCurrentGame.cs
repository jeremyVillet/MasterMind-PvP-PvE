using System.Collections.Generic;

namespace Server_MasterMind
{

    public class ContextCurrentGame
    {
        public int GameMode { get; set; }
        public int[] WinningCombination { get; set; }
        public int[] CombinationToTest { get; set; }
        public int[] CurrentGuessChecked { get; set; }
        public int AttempsRemaining { get; set; }
        public int Column { get; set; }

        public void AllocateArrays()
        {
            this.WinningCombination = new int[this.Column];
            this.CombinationToTest = new int[this.Column];
            this.CurrentGuessChecked = new int[this.Column];
            for(int x=0; x<this.Column; x++)
            {
                this.CurrentGuessChecked[x] = -1;
            }
        }
        public bool TestCombination() // J'ai choisi les regles du mastermind où l'on peut reutiliser plusieurs fois la meme couleur de pion pour faire la combinaison d ou un algo adapté ci dessous 
        {
            int pawnsFound = 0;
            this.CurrentGuessChecked = new int[CombinationToTest.Length];
            for(int i = 0; i< CombinationToTest.Length; i++)
            {
                this.CurrentGuessChecked[i] = -1;
            }

            List<int> ignorePawns = new List<int>();

            for (int x = 0; x < this.CombinationToTest.Length; x++)
            {
                if (this.CombinationToTest[x] == this.WinningCombination[x])
                {
                    this.CurrentGuessChecked[x] = 1;
                    pawnsFound++;
                    ignorePawns.Add(x);
                }
            }

            for (int x = 0; x < this.CombinationToTest.Length; x++)
            {
                if (!ignorePawns.Contains(x))
                {
                    for (int y = 0; y < this.CombinationToTest.Length; y++)
                    {
                        if (!ignorePawns.Contains(y) && this.CombinationToTest[y] == this.WinningCombination[x])
                        {
                            this.CurrentGuessChecked[y] = 0;
                            break;
                        }
                       
                    }
                }
            }

            return pawnsFound == this.CombinationToTest.Length;
        }
    }

}

