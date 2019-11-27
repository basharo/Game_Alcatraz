using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerService.Configuration;
using ServerService.Interface;
using ServerService.Utils;
using spread;

namespace ServerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IServerService _serverService;
        private ISpreadService _spreadService;
        private ConfigurationManager _configurationManager;
        private bool isRecovered = false;
        private delegate string InformClientOfChange(string ipAddress, int port);
        private InformClientOfChange _delegate;
        private bool isDisconnected = false;
        private bool isInitState = false;
        private static IActorRef localChatActor;
        private string path = "C:/temp/";
        private string fileName = "game.txt";
        List<ClientData> exisitingClients;

        public Worker(ILogger<Worker> logger, IServerService serverService, ISpreadService spreadService)
        {
            isInitState = true;
            _configurationManager = new ConfigurationManager();
            _logger = logger;
            _spreadService = spreadService;
            exisitingClients = new List<ClientData>();

            var connected = _spreadService.ConnectToSpread();
            if (connected)
            {
                var spreadGroupName = _spreadService.JoinSpreadGroup();
                Console.WriteLine($"Joined the group {spreadGroupName} ...");

                Console.WriteLine("Preparing message listener\'s ...");
                _spreadService._spreadConnection.OnMembershipMessage += _spreadConnection_OnMembershipMessage;
                _spreadService._spreadConnection.OnRegularMessage += _spreadConnection_OnRegularMessage;
            }
            else
            {
                Console.WriteLine("Cannot connect to the deamon ....");
            }

            string actorName = "server";
            Globals.groupSize = 2;
            Console.Title = actorName;

            try
            {

                
                localChatActor = Globals.mainActorSystem.ActorOf(Props.Create<RegisterActor>(_spreadService), "RegisterActor");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }


       

        private void _spreadConnection_OnRegularMessage(SpreadMessage msg)
        {
            DisplayMessage(msg);

            if (ServerOptions.IsPrimary)
            {
                //primary server can only receive ack regular messages

                var data = Encoding.ASCII.GetString(msg.Data, 0, msg.Data.Length);

                if (data.Equals("Received"))
                {
                    //we got the ack -> respond to client (back to ExecuteTaksAsync)
                    return;
                }
            }
            else
            {
                var deserializedMessage = msg.ParseRegularSpreadMessage();
                //save to File
                _spreadService.SendACK();
            }
        }

        private void _spreadConnection_OnMembershipMessage(SpreadMessage msg)
        {
            MembershipInfo info = msg.MembershipInfo;
            SpreadGroup group = info.Group;

            if (info.IsRegularMembership)
            {
                SpreadGroup[] members = info.Members;
                Console.WriteLine($"Regular membership for {group.ToString()} with {members.Length} members:");
                for (int i = 0; i < members.Length; i++)
                {
                    Console.WriteLine($"\t {members[i]}");
                }

                Console.WriteLine($"GroupID: {info.GroupID}");
            }

            SpreadGroup Sender;

            if (info.IsCausedByDisconnect)
            {
                Sender = info.Disconnected;
                Console.WriteLine($"{Sender} has disconnected ...");
                isDisconnected = true;
                _delegate = new InformClientOfChange(NotifyClient);
                _delegate.Invoke(ServerOptions.IPAddress, ServerOptions.Port);
            }
            if (info.IsCausedByLeave)
            {
                Sender = info.Left;
                Console.WriteLine($"{Sender} has left ...");
                isDisconnected = true;
                _delegate = new InformClientOfChange(NotifyClient);
                _delegate.Invoke(ServerOptions.IPAddress, ServerOptions.Port);
            }
            if (info.IsCausedByJoin)
            {
                Sender = info.Joined;
                Console.WriteLine($"{Sender} has joined ...");

                if (isInitState)
                {
                    #region Set Primary
                    Console.WriteLine("Setting Primary Server ...");

                    int highestPort = _configurationManager.ServerGroups.ToList().Max();

                    if (ServerOptions.Port.Equals(highestPort))
                    {
                        ServerOptions.IsPrimary = true;
                        Console.WriteLine("I am the primary server ...");
                    }
                    else
                    {
                        ServerOptions.IsPrimary = false;
                        Console.WriteLine("I am the backup server ...");
                    }
                    #endregion
                }
                else
                {
                    //receive full state
                }

                //_delegate = new InformClientOfChange(NotifyClient);
                //_delegate.Invoke(ServerOptions.IPAddress, ServerOptions.Port);
            }
        }

        private void NotifyConsole(string message)
        {
            var filePath = FileOperations.GetFileFullPath(string.Format("data-{0}.txt", DateTime.Today.ToShortDateString().Replace("/", string.Empty)));

            using (StreamWriter file = new StreamWriter(filePath, true))
            {
                if (message.Equals("done"))
                {
                    file.WriteLine("|");
                }
                if (!message.Equals("done"))
                {
                    file.WriteLine(message);
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Server running at: {DateTimeOffset.Now}");

            #region comments
            //while (true)
            //{
            //    if (ServerOptions.IsPrimary)
            //    {
            //        //init heartbeat

            //        //start receiving data from the clients
            //        //store data in text file

            //        //send SpreadMessage to the group
            //        while (true)
            //        {
            //            Console.WriteLine("Sending data");
            //            //var data = new { Id = 1, address = "akka.net.address", name = "uniqueName" };
            //            var data = $"test";

            //            //byte[] dataForSpreadMessage = Encoding.ASCII.GetBytes($"{data.Id}|{data.address}|{data.name}");
            //            _spreadService.SendSpreadMessage(data);
            //            Thread.Sleep(2000);
            //        }



            //        //await and ACK
            //        _spreadService._spreadConnection.OnRegularMessage += _spreadConnection_OnRegularMessage;
            //        _spreadService._spreadConnection.OnMembershipMessage += _spreadConnection_OnMembershipMessage;

            //        //reply to client

            //        await Task.Delay(1000, stoppingToken);
            //    }
            //    else
            //    {
            //        //init hearbeat
            //        Console.WriteLine("Listening for messages...");

            //        if(_spreadService._spreadConnection.Poll())
            //        {
            //            var msg = _spreadService.ReceiveSpreadMessage();

            //            if (msg.MembershipInfo.IsCausedByLeave || msg.MembershipInfo.IsCausedByDisconnect)
            //            {
            //                ServerOptions.IsPrimary = true;
            //                NotifyChangeToClients();
            //            }
            //            else
            //            {
            //                if (_spreadService._spreadConnection.Poll())
            //                {
            //                    //update local storage

            //                    //send ack
            //                    _spreadService.SendACK();
            //                }
            //            }
            //        }
            //        else
            //        {
            //            await Task.Delay(2000, stoppingToken);
            //            continue;
            //        }
            //    }
            //}
            #endregion

                if (isDisconnected)
                {
                    _delegate = new InformClientOfChange(NotifyClient);
                    _delegate = NotifyClient;
                }

                await Task.Delay(2000, stoppingToken);


        }

        private string NotifyClient(string ipAddress, int port)
        {

            if (!File.Exists(path + fileName))
            {
                var myFile = File.Create(path + fileName);
                myFile.Close();
            }
            string content = File.ReadAllText(path + fileName);
            exisitingClients = JsonConvert.DeserializeObject<List<ClientData>>(content);

            if(exisitingClients == null)
            {
                exisitingClients = new List<ClientData>();
            }

            foreach (var item in exisitingClients)
            {
                string clientAdress = $"{item.protocol}://{item.system}@{item.host}:{item.port}/user/{item.actorName}";
                var remoteChatActorClient = Globals.mainActorSystem.ActorSelection(clientAdress);

                if (remoteChatActorClient != null)
                {
                    remoteChatActorClient.Tell(ipAddress + port, localChatActor);
                }
            }

            return "success";
        }

        private void DisplayMessage(SpreadMessage msg)
        {
            try
            {
                if (msg.IsRegular)
                {
                    Console.Write("Received a ");
                    if (msg.IsUnreliable)
                        Console.Write("UNRELIABLE");
                    else if (msg.IsReliable)
                        Console.Write("RELIABLE");
                    else if (msg.IsFifo)
                        Console.Write("FIFO");
                    else if (msg.IsCausal)
                        Console.Write("CAUSAL");
                    else if (msg.IsAgreed)
                        Console.Write("AGREED");
                    else if (msg.IsSafe)
                        Console.Write("SAFE");
                    Console.WriteLine(" message.");

                    Console.WriteLine("Sent by  " + msg.Sender + ".");

                    Console.WriteLine("Type is " + msg.Type + ".");

                    if (msg.EndianMismatch == true)
                        Console.WriteLine("There is an endian mismatch.");
                    else
                        Console.WriteLine("There is no endian mismatch.");

                    SpreadGroup[] groups = msg.Groups;
                    Console.WriteLine("To " + groups.Length + " groups.");

                    byte[] data = msg.Data;
                    Console.WriteLine("The data is " + data.Length + " bytes.");

                    Console.WriteLine("The message is: " + System.Text.Encoding.ASCII.GetString(data));
                }
                else if (msg.IsMembership)
                {
                    MembershipInfo info = msg.MembershipInfo;

                    if (info.IsRegularMembership)
                    {
                        SpreadGroup[] groups = msg.Groups;

                        Console.WriteLine("Received a REGULAR membership.");
                        Console.WriteLine("For group " + info.Group + ".");
                        Console.WriteLine("With " + groups.Length + " members.");
                        Console.WriteLine("I am member " + msg.Type + ".");
                        for (int i = 0; i < groups.Length; i++)
                            Console.WriteLine("  " + groups[i]);

                        Console.WriteLine("Group ID is " + info.GroupID);

                        Console.Write("Due to ");
                        if (info.IsCausedByJoin)
                        {
                            Console.WriteLine("the JOIN of " + info.Joined + ".");
                        }
                        else if (info.IsCausedByLeave)
                        {
                            Console.WriteLine("the LEAVE of " + info.Left + ".");
                        }
                        else if (info.IsCausedByDisconnect)
                        {
                            Console.WriteLine("the DISCONNECT of " + info.Disconnected + ".");
                        }
                        else if (info.IsCausedByNetwork)
                        {
                            SpreadGroup[] stayed = info.Stayed;
                            Console.WriteLine("NETWORK change.");
                            Console.WriteLine("VS set has " + stayed.Length + " members:");
                            for (int i = 0; i < stayed.Length; i++)
                                Console.WriteLine("  " + stayed[i]);
                        }
                    }
                    else if (info.IsTransition)
                    {
                        Console.WriteLine("Received a TRANSITIONAL membership for group " + info.Group);
                    }
                    else if (info.IsSelfLeave)
                    {
                        Console.WriteLine("Received a SELF-LEAVE message for group " + info.Group);
                    }
                }
                else if (msg.IsReject)
                {
                    // Received a Reject message 
                    Console.Write("Received a ");
                    if (msg.IsUnreliable)
                        Console.Write("UNRELIABLE");
                    else if (msg.IsReliable)
                        Console.Write("RELIABLE");
                    else if (msg.IsFifo)
                        Console.Write("FIFO");
                    else if (msg.IsCausal)
                        Console.Write("CAUSAL");
                    else if (msg.IsAgreed)
                        Console.Write("AGREED");
                    else if (msg.IsSafe)
                        Console.Write("SAFE");
                    Console.WriteLine(" REJECTED message.");

                    Console.WriteLine("Sent by  " + msg.Sender + ".");

                    Console.WriteLine("Type is " + msg.Type + ".");

                    if (msg.EndianMismatch == true)
                        Console.WriteLine("There is an endian mismatch.");
                    else
                        Console.WriteLine("There is no endian mismatch.");

                    SpreadGroup[] groups = msg.Groups;
                    Console.WriteLine("To " + groups.Length + " groups.");

                    byte[] data = msg.Data;
                    Console.WriteLine("The data is " + data.Length + " bytes.");

                    Console.WriteLine("The message is: " + System.Text.Encoding.ASCII.GetString(data));
                }
                else
                {
                    Console.WriteLine("Message is of unknown type: " + msg.ServiceType);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.Exit(1);
            }
        }
    }
}
