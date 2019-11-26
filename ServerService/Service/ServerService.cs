using Microsoft.Extensions.Logging;
using ServerService.Client;
using ServerService.Configuration;
using ServerService.Interface;
using ServerService.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerService.Service
{
    public class ServerService : IServerService
    {
        private ILogger<ServerService> Logger { get; set; }
        private Socket _serverSocket;
        private List<ClientHandler> _clients = new List<ClientHandler>();
        private Action<string> _notifier;
        private Thread _acceptingThread;
        private ClientResponse clientResponse;
        private int counter = 0;

        public ServerService(ILogger<ServerService> logger)
        {
            Logger = logger;
            clientResponse = new ClientResponse();
        }

        public void InitializeServerService(Action<string> notifier)
        {
            _notifier = notifier;
            string _ipAddress = string.Empty;
            if(ServerOptions.IPAddress == null || ServerOptions.IPAddress.Equals(string.Empty))
            {
                _ipAddress = IPAddress.Loopback.ToString();
            }
            else
            {
                _ipAddress = ServerOptions.IPAddress;
            }

            try
            {
                var _ipEndpoint = new IPEndPoint(IPAddress.Parse(_ipAddress), ServerOptions.Port);

                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(_ipEndpoint);
                _serverSocket.Listen(5);
            }
            catch (Exception ex)
            {
                var msg = $"{ex.Message} : {ex.InnerException}";
                Logger.LogError("Could not start TCP communication for the Server. See details below: ");
                Logger.LogError(msg);
                throw ex;
            }
        }

        public void DisconnectClient(string clientName)
        {
            foreach (var item in _clients)
            {
                if (item.Name.Equals(clientName))
                {
                    item.Close();
                    _clients.Remove(item);
                    break;
                }
            }
        }

        public void StartAccepting()
        {
            _acceptingThread = new Thread(new ThreadStart(() =>
            {
                while (_acceptingThread.IsAlive)
                {
                    try
                    {
                        _clients.Add(new ClientHandler(_serverSocket.Accept(), new Action<string, Socket>(NewMessageReceived)));
                    }
                    catch (Exception ex)
                    {
                        var msg = $"{ex.Message} : {ex.InnerException}";
                        Logger.LogError("Failed to register client to start server communication. See details below: ");
                        Logger.LogError(msg);
                    }
                }
            }));

            _acceptingThread.IsBackground = true;
            _acceptingThread.Start();
        }

        private void NewMessageReceived(string message, Socket senderSocket)
        {
            _notifier(message);
        }

        public void SendMessageToClient(string message, string clientName)
        {
            foreach (var item in _clients)
            {
                if (item.Name.Equals(clientName))
                {
                    item.Send(message);
                }
            }
        }

        public ClientResponse Register(ClientDemo clientToRegister)
        {
            var clientNames = FileOperations.GetFileFullPath(string.Format("clientNames.txt", DateTime.Today.ToShortDateString().Replace("/", string.Empty)));
            Logger.LogInformation($"File with client names: {clientNames}");

            #region Client Unique Name
            Logger.LogInformation($"Checking if the client name {clientToRegister.UniqueClientName} is unique.");
            if (!isUniqueName(clientToRegister.UniqueClientName, clientNames))
            {
                Logger.LogInformation($"The provided name is not unique. Returning error message!");
                var errorMessage = $"Sorry! The name \'{clientToRegister.UniqueClientName}\' is already taken. Please specify a different name.";
                Logger.LogInformation(errorMessage);
                return new ClientResponse(errorMessage);
            }
            else
            {
                Logger.LogInformation($"The provided name is unique. Saving to file!");
                Logger.LogInformation($"{clientToRegister.UniqueClientName} has the following address {clientToRegister.ClientAddress} " +
                    $"and {clientToRegister.PreferedGroupSize} preferred group size");

                if(counter == 4)
                {
                    counter = 0;
                }

                Logger.LogInformation($"Providing sequence number for {clientToRegister.UniqueClientName} of {counter}");
                counter++;

                using (StreamWriter writter = new StreamWriter(clientNames, true))
                {
                    writter.WriteLine(clientToRegister.UniqueClientName);
                    writter.WriteLine(";");
                    writter.WriteLine();
                    writter.WriteLine(clientToRegister.ClientAddress);
                    writter.WriteLine(";");
                    writter.WriteLine(clientToRegister.PreferedGroupSize);
                    writter.WriteLine(";");
                }

                return new ClientResponse();
            }
            #endregion

            #region Client other data


            #endregion
        }

        private bool isUniqueName(string uniqueClientName, string clientNamesFile)
        {
            using (StreamReader reader = new StreamReader(clientNamesFile))
            {
                var content = reader.ReadToEnd();
                var lines = content.Split(';');

                foreach (var item in lines)
                {
                    if (item.Equals(uniqueClientName))
                    {

                        return false;
                    }
                }

                return true;
            }
        }

        public string SetPrimaryServer(string ipAddress, int port)
        {
            throw new NotImplementedException();
        }

        public void UpdatePlayerList(ClientDemo[] clients)
        {
            throw new NotImplementedException();
        }

        public void DeletePlayerFromList(ClientDemo clientToRemove)
        {
            throw new NotImplementedException();
        }

        public void Start(ClientDemo[] clients, int gameID)
        {
            throw new NotImplementedException();
        }
    }
}
