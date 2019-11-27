using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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


        public Worker(ILogger<Worker> logger, IServerService serverService, ISpreadService spreadService)
        {
            _logger = logger;
            _spreadService = spreadService;

            _spreadService.ConnectToSpread();
            _spreadService.JoinSpreadGroup();
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
            _logger.LogInformation("Server running at: {time}", DateTimeOffset.Now);

            Console.WriteLine($"Server running at: {DateTimeOffset.Now}");

            while (true)
            {
                if (ServerOptions.IsPrimary)
                {
                    //init heartbeat

                    //start receiving data from the clients
                    //store data in text file

                    //send SpreadMessage to the group
                    /**var data = new { Id = 1, address = "akka.net.address", name = "uniqueName" };

                    byte[] dataForSpreadMessage = Encoding.ASCII.GetBytes($"{data.Id}|{data.address}|{data.name}");
                    _spreadService.SendSpreadMessage(new SpreadMessage() { Data = dataForSpreadMessage, IsFifo = true });
                    */

                    //await and ACK
                    _spreadService._spreadConnection.OnRegularMessage += _spreadConnection_OnRegularMessage;
                    _spreadService._spreadConnection.OnMembershipMessage += _spreadConnection_OnMembershipMessage;

                    //reply to client

                    await Task.Delay(1000, stoppingToken);
                }
                else
                {
                    //init hearbeat
                    Console.WriteLine("Listening for messages...");

                    if(_spreadService._spreadConnection.Poll())
                    {
                        var msg = _spreadService.ReceiveSpreadMessage();

                        if (msg.MembershipInfo.IsCausedByLeave || msg.MembershipInfo.IsCausedByDisconnect)
                        {
                            ServerOptions.IsPrimary = true;
                            NotifyChangeToClients();
                        }
                        else
                        {
                            if (_spreadService._spreadConnection.Poll())
                            {
                                //update local storage

                                //send ack
                                _spreadService.SendACK();
                            }
                        }
                    }
                    else
                    {
                        await Task.Delay(2000, stoppingToken);
                        continue;
                    }
                }
            }
        }

        private void NotifyChangeToClients()
        {
            Console.WriteLine($"New Primary Server: {ServerOptions.IPAddress} | {ServerOptions.Port}");
        }

        private void _spreadConnection_OnMembershipMessage(SpreadMessage msg)
        {
            if (msg.MembershipInfo.IsCausedByLeave)
            {
                ServerOptions.IsPrimary = true;
            }
            else if(msg.MembershipInfo.IsCausedByJoin || msg.MembershipInfo.IsRegularMembership)
            {
                return;
            }
        }

        private void _spreadConnection_OnRegularMessage(SpreadMessage msg)
        {
            //receive ACK from backup server

            var data = Encoding.ASCII.GetString(msg.Data, 0, msg.Data.Length);
            var parts = data.Split('|');
            var ACK = parts[0];
            if (ACK.Equals("Received"))
            {
                //we got the ack -> respond to client
                return;
            }
        }
    }
}
