using System;
using System.Net.Sockets;
using System.Text;
using Lib_MasterMind;
using Newtonsoft.Json;
using System.Threading;


namespace Client_MasterMind
{
    public class UtilsClientTCP
    {
        private const string SERVER_ADDRESS = "127.0.0.1";
        private const int SERVER_PORT = 13000;


        public static void RequestServer(ref Utils.DataClient dataCurrentRound)
        {
            Thread.Sleep(500); // On limite le nombre possible de requete au serveur quand le client est en attente d'un autre client
            TcpClient client = null;

            try
            {
                client = new TcpClient(SERVER_ADDRESS, SERVER_PORT);

                string dataCurrentRound_Raw = JsonConvert.SerializeObject(dataCurrentRound);
                byte[] buffer = Encoding.ASCII.GetBytes(dataCurrentRound_Raw);

                NetworkStream stream = client.GetStream();

                // Envoie du message
                stream.Write(buffer, 0, buffer.Length);

                // Reception message
                buffer = new byte[1024];
                dataCurrentRound_Raw = string.Empty;

                stream.Read(buffer, 0, buffer.Length);
                dataCurrentRound_Raw = Encoding.ASCII.GetString(buffer);
                dataCurrentRound = JsonConvert.DeserializeObject<Utils.DataClient>(dataCurrentRound_Raw);

                stream.Close();
                client.Close();
            }
            catch (Exception)
            {
                dataCurrentRound.StateGame = "The connection with the server have been lost";
            }

        }
    }
}
