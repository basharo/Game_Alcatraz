using Microsoft.Extensions.Logging;
using ServerService.Configuration;
using ServerService.Interface;
using ServerService.Utils;
using spread;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace ServerService.Service
{
    public class SpreadService : ISpreadService
    {
        public SpreadConnection _spreadConnection { get; set; }
        public SpreadGroup _spreadGroup { get; set; }
        ILogger<SpreadService> _logger { get; set; }
        ConfigurationManager _configurationManager { get; set; }
        private bool isConnectedToDeamon { get; set; }

        public SpreadService(ILogger<SpreadService> logger)
        {
            _logger = logger;
            _configurationManager = new ConfigurationManager();
        }

        public string JoinSpreadGroup()
        {
            _spreadGroup = new SpreadGroup();
            _spreadGroup.Join(_spreadConnection, _configurationManager.SpreadGroup);
            return _spreadGroup.ToString();
        }

        public SpreadMessage SendSpreadMessage(string data)
        {
            SpreadMessage msg = new SpreadMessage();

            msg.AddGroup(_spreadGroup);
            msg.Data = Encoding.ASCII.GetBytes(data);

            _spreadConnection.Multicast(msg);

            return msg;
        }

        public bool ConnectToSpread()
        {
            _logger.LogInformation("Initializing Spread connection...");
            _spreadConnection = new SpreadConnection();
            try
            {
                _logger.LogDebug($"Connecting to the Spread Deamon with address: {_configurationManager.SpreadDeamon} and port {_configurationManager.SpreadPort}");
                _spreadConnection.Connect(_configurationManager.SpreadDeamon, _configurationManager.SpreadPort, _configurationManager.SpreadUser, false, true);
                Console.WriteLine($"Connected to the deamon with IP {_configurationManager.SpreadDeamon} and port {_configurationManager.SpreadPort}");
                isConnectedToDeamon = true;

            }
            catch (SpreadException spreadException)
            {
                _logger.LogError($"There was an error connecting to the daemon.");
                _logger.LogError($"{spreadException.Message} : {spreadException.InnerException}");
                isConnectedToDeamon = false;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Can't find the daemon: {_configurationManager.SpreadDeamon}");
                var msg = $"{exception.Message} : {exception.InnerException}";
                _logger.LogError(msg);
                isConnectedToDeamon = false;
            }

            return isConnectedToDeamon;
        }

        public SpreadMessage ReceiveSpreadMessage()
        {
            SpreadMessage message = _spreadConnection.Receive();

            DisplayMessage(message);

            return message;
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

        public SpreadMessage SendACK()
        {
            SpreadMessage msg = new SpreadMessage();
            msg.AddGroup(_spreadGroup);
            msg.Data = Encoding.ASCII.GetBytes($"Received");

            _spreadConnection.Multicast(msg);

            return msg;
        }
    }
}
