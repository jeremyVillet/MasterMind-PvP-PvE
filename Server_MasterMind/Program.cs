using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Lib_MasterMind;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;


namespace Server_MasterMind
{
    class Program
    {
        private const int SERVER_PORT = 13000;
        private static IPAddress Server_Address = IPAddress.Parse("127.0.0.1");
        private static int client_Connected_onPVP = 0;
        private static TcpListener server = null;

        private static ContextCurrentGame contextCurrentGame; // Données globale de la partie en cours

        // En fonction de la clé (dataClient.StateGame) recu par le joueur , On executera la methode associé => plus opti qu'un switch au niveau du cpu 
        private delegate void ActionRef<DataClient>(ref DataClient dataClient);
        private static Dictionary<string, ActionRef<Utils.DataClient>> dataProcess = new Dictionary<string, ActionRef<Utils.DataClient>>()
            {
                 {"Initialize", Initialize},
                {"Waiting for a second player",WaitingSecondPlayer },
                {"Setting game's parameters Done", SettingParametersDone},
                {"Waiting opponant set game's parameters",WaitingOpponnantSetParameters },
                {"Choice combination to find Done", ChoiceCombinationToFindDone },
                {"Waiting opponant choice winning combination",   WaitingOpponantChoiceWinningCombination},
                {"Must find the winning combination",  MustFindWinningCombination },
                {"Waiting opponnant find winning combination", WaitingOpponnantFindWinningCombination },
            };

        static void Main(string[] args)
        {
            Console.Title = "Server MasterMind ";

            try
            {
                server = new TcpListener(Server_Address, SERVER_PORT);
                server.Start();
                TcpClient client;
                Console.WriteLine("Server is operational");


                while (true) // boucle d'ecoute
                {
                    client = server.AcceptTcpClient();
                    // Afin de gerer potentiellement deux client a la fois , j'ai mis en place un ensemble de thread permettant de gerer chaque requete des clients de maniere non bloquante
                    ThreadPool.QueueUserWorkItem(ThreadManageClient, client);
                }
            }
            catch (Exception error)
            {
                Console.WriteLine($"An error has occured : {error}");
                Console.Read();
            }
        }
        private static void ThreadManageClient(object obj)
        {
            byte[] buffer = new byte[1024];
            TcpClient client = (TcpClient)obj;
            Console.WriteLine($"Data exchange with client {client.Client.LocalEndPoint}");

            string dataCurrentRound_Raw = string.Empty;
            NetworkStream stream = client.GetStream();

            stream.Read(buffer, 0, buffer.Length);
            dataCurrentRound_Raw = Encoding.ASCII.GetString(buffer);
            Utils.DataClient dataCurrentRound = JsonConvert.DeserializeObject<Utils.DataClient>(dataCurrentRound_Raw); ; // Données du client courant en train de requeter le serveur 

            if(dataProcess.ContainsKey(dataCurrentRound.StateGame)) 
                dataProcess[dataCurrentRound.StateGame](ref dataCurrentRound); // Traitement des donnée en fonction de la clé dataCurrentRound.StateGame reçue

            dataCurrentRound_Raw = JsonConvert.SerializeObject(dataCurrentRound);
            buffer = Encoding.ASCII.GetBytes(dataCurrentRound_Raw);

            stream.Write(buffer, 0, buffer.Length);
            client.Close();
        }

        /////////////// Traitement des données ///////////////

        static private  void Initialize(ref Utils.DataClient dataCurrentRound)
        {
            if (client_Connected_onPVP == 0)
            {
                contextCurrentGame = new ContextCurrentGame(); // Creation d'un nouveau contexte pour la partie en cours 
                dataCurrentRound.StateGame = dataCurrentRound.GameMode == 1 ? "Waiting for a second player" : "Setting game's parameters";
                client_Connected_onPVP++;
            }
            else if (client_Connected_onPVP == 1)
            {
                dataCurrentRound.StateGame = "Waiting opponant set game's parameters";
                client_Connected_onPVP++;
            }
            else if (client_Connected_onPVP == 2)
            {
                dataCurrentRound.StateGame = dataCurrentRound.GameMode == 1 ? "Sorry a game have already started between 2 player :(" : "Setting game's parameters";
            }
        }


        static private void WaitingSecondPlayer(ref Utils.DataClient dataCurrentRound)
        {
            if (client_Connected_onPVP == 2)
                dataCurrentRound.StateGame = "Setting game's parameters";
        }
        static private void SettingParametersDone(ref Utils.DataClient dataCurrentRound)
        {
            if (client_Connected_onPVP == 2 || dataCurrentRound.GameMode == 2)
            {
                contextCurrentGame.Column = dataCurrentRound.Column;
                contextCurrentGame.AttempsRemaining = dataCurrentRound.AttempsRemaining;
                contextCurrentGame.AllocateArrays();

                dataCurrentRound.StateGame = dataCurrentRound.GameMode == 1 ? "Waiting opponant choice winning combination" : "Waiting computeur choice winning combination";
            }
        }
        static private void WaitingOpponnantSetParameters(ref Utils.DataClient dataCurrentRound)
        {
            if (contextCurrentGame.Column != 0)
            {
                dataCurrentRound.Column = contextCurrentGame.Column;
                dataCurrentRound.AttempsRemaining = contextCurrentGame.AttempsRemaining;
                dataCurrentRound.StateGame = "Choice combination to find";
            }
        }
        static private void ChoiceCombinationToFindDone(ref Utils.DataClient dataCurrentRound)
        {
            Array.Copy(dataCurrentRound.WinningCombination, contextCurrentGame.WinningCombination, contextCurrentGame.Column);
            dataCurrentRound.StateGame = dataCurrentRound.GameMode == 1 ? "Waiting opponnant find winning combination" : "Must find the winning combination";
        }
        static private void WaitingOpponantChoiceWinningCombination(ref Utils.DataClient dataCurrentRound)
        {
            if (contextCurrentGame.WinningCombination[0] != 0)
            {
                dataCurrentRound.StateGame = "Must find the winning combination";
            }
        }
        static private void MustFindWinningCombination(ref Utils.DataClient dataCurrentRound)
        {
            contextCurrentGame.AttempsRemaining--;
            dataCurrentRound.AttempsRemaining--;
            Array.Copy(dataCurrentRound.CombinationToTest, contextCurrentGame.CombinationToTest, contextCurrentGame.Column);

            if (contextCurrentGame.TestCombination())
            {
                dataCurrentRound.StateGame = "You have won";
                contextCurrentGame.AttempsRemaining = -1; // Permet d avertir le jouer opposant qu'il a perdu
                client_Connected_onPVP--;
            }
            else if (dataCurrentRound.AttempsRemaining == 0)
            {
                dataCurrentRound.StateGame = "You have lost";
                client_Connected_onPVP--;
            }

            Array.Copy(contextCurrentGame.CurrentGuessChecked, dataCurrentRound.CurrentGuessChecked, contextCurrentGame.Column);
        }
        static private  void WaitingOpponnantFindWinningCombination(ref Utils.DataClient dataCurrentRound)
        {
            dataCurrentRound.AttempsRemaining = contextCurrentGame.AttempsRemaining;
            Array.Copy(contextCurrentGame.CombinationToTest, dataCurrentRound.CombinationToTest, contextCurrentGame.Column);
            Array.Copy(contextCurrentGame.CurrentGuessChecked, dataCurrentRound.CurrentGuessChecked, contextCurrentGame.Column);


            if (dataCurrentRound.AttempsRemaining == 0)
            {
                dataCurrentRound.StateGame = "You have won";
                client_Connected_onPVP--;
            }
            else if (dataCurrentRound.AttempsRemaining == -1)
            {
                dataCurrentRound.StateGame = "You have lost";
                client_Connected_onPVP--;
            }

        }

    }
}
