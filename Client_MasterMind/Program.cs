using System;

namespace Client_MasterMind
{
    class Program
    {

        static void Main(string[] args)
        {
            // Initialisation de la partie 

            Console.Title = "Client MasterMind";
            
            bool gameInProcess = true;
            while (gameInProcess)
            {
                Console.WriteLine("########   Welcome to the Master Mind !   ######## \n\n Please select a game mode : \n\n1.Player vs Player : Press key 1 \n2.Player vs Computeur : Press key 2  \n\nPress key Escape to leave the game !");
                bool validCommand = false;
                while (!validCommand)
                {
                    Game CurrentGame;
                    ConsoleKeyInfo actionKey = Console.ReadKey(true);
                    switch (actionKey.Key)
                    {
                        // Joueur vs Joueur 
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                            Console.Clear();
                            CurrentGame = new GamePVP();
                            validCommand = true;
                            break;

                        // Joueur vs Ordinateur 
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            Console.Clear();
                            CurrentGame = new GamePVE();
                            validCommand = true;
                            break;
                        case ConsoleKey.Escape:
                            Console.Clear();
                            Console.WriteLine("See you soon :-) \n Press Enter to close the game");
                            Console.ReadLine();
                            validCommand = true;
                            gameInProcess = false;
                            break;
                        default:
                            break;

                    }

                }
            }
           
        }

    }
}
