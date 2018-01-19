using System;
using System.Linq;
using Lib_MasterMind;

namespace Client_MasterMind
{
    public abstract class Game
    {
        
        public int Column { get; set; }
        public int Attempts { get; set; }
        public int GameMode { get; set; }
        public int[,] Board { get; set; }  // Permet de faire la representation des combinaisons testées par le joueur 
        public int[,] GuessChecked { get; set; }  // Permet de faire la representation des pions bien possitionnées ou non ou de bonne couleur ou non pour le joueur 

        public Game()
        {
            this.Column = 3;
            this.Attempts = 3;
            this.DoAGame();
        }

        protected abstract void DoAGame();
        protected  void HumanGiveCombination(int[] combination,int mode)
        {
            Console.Clear();
            int column = combination.Length;
            string pawn;

            bool combinationChoosen = false;
            int currentPawn = 0;
            while (!combinationChoosen)
            {
                string question = mode == 1 ? "Please choose the combination that your opponant will have to guess ! \n\n" : "Please try to guess the winning combination ! \n\n";
                Console.WriteLine($"{question} \n\n Press the following keys to swich your pawns ! \n Z or S: Change the color of the current  pawn \n Q : shift left of the current pawn \n D : shift right of the current pawn \n\n ");
                for (int x = 0; x < column; x++)
                {
                    pawn = combination[x] == 0 ? "  ." : "  O";
                    Console.ForegroundColor = Utils.colorPawns[combination[x]];
                    Console.Write(pawn);
                }
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                string currentPawn_patern;
                for (int x = 0; x < column; x++)
                {
                    currentPawn_patern = x == currentPawn ? "  _" : "   ";
                    Console.Write(currentPawn_patern);
                }
                Console.WriteLine("\n\n\n Press Enter to valid your choice ! \n");

                ConsoleKeyInfo actionKey = Console.ReadKey(true);
                switch (actionKey.Key)
                {
                    case ConsoleKey.Z:
                        combination[currentPawn] = combination[currentPawn] == 4 ? 1 : combination[currentPawn] + 1;
                        break;
                    case ConsoleKey.S:
                        combination[currentPawn] = (combination[currentPawn] == 1 ||  combination[currentPawn] == 0 )? 4 : combination[currentPawn] - 1;
                        break;
                    case ConsoleKey.Q:
                        currentPawn = currentPawn == 0 ? column - 1 : currentPawn - 1;
                        break;
                    case ConsoleKey.D:
                        currentPawn = currentPawn == column - 1 ? 0 : currentPawn + 1;
                        break;

                    case ConsoleKey.Enter:
                        combinationChoosen = true;
                        for (var i = 0; i < column; i++)
                        {
                            if (combination[i] == 0)
                            {
                                combinationChoosen = false;
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
                Console.Clear();
            }
        }


        protected void ChooseParameter(Game game)
        {
            bool chooseParameterDone = false;
            while (!chooseParameterDone)
            {
                Console.Clear();
                Console.WriteLine($"You have to choose the number of pawns and the numbers of attemps for the game ! \n \n Press A or Q to change the pawn's value " +
                    $"\nPress z or s to change the attempt's value \n\n Number of pawns : {game.Column} \n Number of attempts : {game.Attempts} \n\nPress Enter to valid your choose !");
                ConsoleKeyInfo actionKey = Console.ReadKey(true);
                switch (actionKey.Key)
                {
                    case ConsoleKey.A:
                        game.Column = game.Column == 10 ? 3 : game.Column + 1;
                        break;
                    case ConsoleKey.Q:
                        game.Column = game.Column == 3 ? 10 : game.Column - 1;
                        break;
                    case ConsoleKey.Z:
                        game.Attempts = game.Attempts == 20 ? 3 : game.Attempts + 1;
                        break;
                    case ConsoleKey.S:
                        game.Attempts = game.Attempts == 3 ? 20 : game.Attempts - 1;
                        break;

                    case ConsoleKey.Enter:
                        chooseParameterDone = true;
                        break;
                    default:
                        break;
                }
            }
            Console.Clear();
            Console.WriteLine($"You have chosen {game.Column} pawns and you will have {game.Attempts} attempts to find the secret combination ! \n\nPress enter to continue ..");
            Console.Read();
            Console.Clear();

        }
        protected void PrintGameBoard()
        {
            string segment = String.Concat(Enumerable.Repeat(" - ", this.Column));
            string pawn;

            Console.WriteLine("\n\n");
            for (int i = 0; i < this.Attempts; i++)
            {
                for (int j = 0; j < this.Column; j++) // Representation d'une combinaison proposé par le joueur 
                {
                    pawn = this.Board[i, j] == 0 ? "." : "O";
                    Console.ForegroundColor = Utils.colorPawns[this.Board[i, j]];
                    Console.Write($"  {pawn}");
                }
                Console.Write("  ");
                for (int j = 0; j < this.Column; j++) // Representation des pions donnant les indices aux joueurs par rapport a la combinaison testée
                {
                    pawn = this.GuessChecked[i, j] == -1 ? "." : "o";
                    Console.ForegroundColor = Utils.colorPawns[this.GuessChecked[i, j]];
                    Console.Write($"{pawn}");

                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"\n {segment}");
            }
        }
        protected void AllocateArrays()
        {
            this.Board = new int[this.Attempts, this.Column];
            this.GuessChecked = new int[this.Attempts, this.Column];
            for (int x = 0; x < this.Attempts; x++)
            {
                for (int y = 0; y < this.Column; y++)
                {
                    this.GuessChecked[x, y] = -1;
                }
            }
        }
    }
}
