using System;
using Akka.Actor;
using Akka.Configuration;
using GameInterface;
using Interface;
using Alcatraz;

namespace ClientPlayer
{
    public class Program 
    {
        int numPlayer;
        Program instance = new Program();
        class ClientActor : ReceiveActor
        {
            private IActorRef _remoteActor;
            private int _gameCapacity;
            private ICancelable _clientTask;

           

            //public int sequenceID { get; set; }              //seqence ID assigned by Server at registration
            //public String unique_name { get; set; }          // unique player name   
            //public int preferred_group_size { get; set; }    //  preferred group size            
            //public String ipAddress { get; set; }            // address of client to contact him back         
            //public int port { get; }                         //port which is used for the
            public ClientActor(IActorRef remoteActor)
            {
                _remoteActor = remoteActor;
                Context.Watch(_remoteActor);
                Receive<Client>(client =>
                {
                    Console.WriteLine("Received {1} from {0}", Sender, client.unique_name);
                });

                Receive<JoinGame>(sayHello =>
                {
                    _remoteActor.Tell(new Client(1, "hello", 2, "localhost", 5522));
                });

                Receive<Terminated>(terminated =>
                {
                    Console.WriteLine(terminated.ActorRef);
                    Console.WriteLine("Was address terminated? {0}", terminated.AddressTerminated);
                    _clientTask.Cancel();
                });

            }

            protected override void Unhandled(object message)
            {
                //Do something with the message.
            }
        }
     
        class JoinGame { }
        class SayHello { }

        class HelloActor : ReceiveActor
        {
            private IActorRef _remoteActor;
            private int _helloCounter;
            private ICancelable _helloTask;

            public HelloActor(IActorRef remoteActor)
            {
                _remoteActor = remoteActor;
                Context.Watch(_remoteActor);
                Receive<Hello>(hello =>
                {
                    Console.WriteLine("Received {1} from {0}", Sender, hello.Message);
                });

                Receive<SayHello>(sayHello =>
                {
                    _remoteActor.Tell(new Hello("hello" + _helloCounter++));
                });

                Receive<Terminated>(terminated =>
                {
                    Console.WriteLine(terminated.ActorRef);
                    Console.WriteLine("Was address terminated? {0}", terminated.AddressTerminated);
                    _helloTask.Cancel();
                });
            }

            protected override void PreStart()
            {
                _helloTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1), Context.Self, new SayHello(), ActorRefs.NoSender);
            }

            protected override void PostStop()
            {
                _helloTask.Cancel();
            }
        }

        //static void Main(string[] args)
        //{
        //    using (var system = ActorSystem.Create("Client", ConfigurationFactory.ParseString(@"
        //        akka {  
        //            actor{
        //                provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        //                deployment {
        //                /localecho {
        //                        local = ""akka.tcp://Server@localhost:555""
        //                    }
        //                    /remoteecho {
        //                        remote = ""akka.tcp://Server@localhost:8090""
        //                    }
        //                }
        //            }
        //            remote {
        //                helios.tcp {
        //              port = 550
        //              hostname = localhost
        //                }
        //            }
        //        }")))
        //    {
        //        var remoteAddress = Address.Parse("akka.tcp://Server@localhost:8090");
        //        var remoteEcho1 = system.ActorOf(Props.Create(() => new EchoActor()), "remoteecho"); //
        //        var local = system.ActorOf(Props.Create(() => new EchoActor()), "localecho"); //
        //        var remoteEcho2 =
        //            system.ActorOf(
        //                Props.Create(() => new EchoActor())
        //                    .WithDeploy(Deploy.None.WithScope(new RemoteScope(remoteAddress))), "coderemoteecho");

        //        //system.ActorOf(Props.Create(() => new HelloActor(remoteEcho1)));
        //        ////system.ActorOf(Props.Create(() => new HelloActor(remoteEcho2)));

        //        //system.ActorSelection("/user/remoteecho").Tell(new Hello("hi from selection!"));

        //        system.ActorOf(Props.Create(() => new ClientActor(remoteEcho2)));
        //        //system.ActorSelection("/user/remoteecho").Ask(new Hello("heloooooo"));
        //        system.ActorSelection("/user/remoteecho").Tell(new Client(1, "22", 2, "", 3333), local);

        //        Console.ReadKey();
        //    }
        //}

        //static void Main(string[] args)
        //{
        //    string actorSystemName = "peer1";
        //    Console.Title = actorSystemName;

        //    try
        //    {
        //        using (var actorSystem = ActorSystem.Create(actorSystemName))
        //        {
        //            var localChatActor = actorSystem.ActorOf(Props.Create<MessagingActor>(), "MessagingActor");
        //            var child = actorSystem.ActorOf(Props.Create<MessagingActor>(), "MessagingActorChild");

        //            string remoteActorAddress = "akka.tcp://peer2@localhost:5249/user/MessagingActor";
        //            var remoteChatActor = actorSystem.ActorSelection(remoteActorAddress);

        //            if (remoteChatActor != null)
        //            {
        //                string line = string.Empty;
        //                while (line != null)
        //                {
        //                    line = Console.ReadLine();
        //                    remoteChatActor.Tell(line, child);
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("Could not get remote actor ref");
        //                Console.ReadLine();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}
       

        static void Main(string[] args)
        {
            string actorSystemName = "client1";
            Console.Title = actorSystemName;
            Alcatraz.Player player = new Alcatraz.Player(1);

            try
            {
                using (var actorSystem = ActorSystem.Create(actorSystemName))
                {
                    var localChatActor = actorSystem.ActorOf(Props.Create<EchoActor>(), "EchoActor");
                    var child = actorSystem.ActorOf(Props.Create<EchoActor>(), "EchoActorClient1Child");
                    Players players = new Players(new string[10,10]);
                    players.players[1, 1] = actorSystemName;
                    string remoteActorAddressClient1 = "akka.tcp://client1@localhost:2222/user/EchoActor";
                    string remoteActorAddressClient2 = "akka.tcp://client2@localhost:3333/user/EchoActor";
                    var remoteChatActorClient1 = actorSystem.ActorSelection(remoteActorAddressClient1);
                    var remoteChatActorClient2 = actorSystem.ActorSelection(remoteActorAddressClient2);
                    if (remoteChatActorClient1 != null && remoteChatActorClient2 != null)
                    {
                        string line = string.Empty;
                        while (line != null)
                        {
                            line = Console.ReadLine();
                            remoteChatActorClient1.Tell(players, child);
                            remoteChatActorClient2.Tell(players, child);

                            //remoteChatActorClient1.Tell(line, child);
                            //remoteChatActorClient2.Tell(line, child);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Could not get remote actor ref");
                        Console.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //static void Main(string[] args)
        //{
        //    string actorSystemName = "peer1";
        //    Console.Title = actorSystemName;

        //    try
        //    {
        //        using (var actorSystem = ActorSystem.Create(actorSystemName))
        //        {

        //            var localChatActor = actorSystem.ActorOf(Props.Create<EchoActor>(), "EchoActor");
        //            var child = actorSystem.ActorOf(Props.Create<EchoActor>(), "EchoActorChild");

        //            string remoteActorAddress = "akka.tcp://peer2@localhost:5555/user/EchoActor";
        //            var remoteChatActor = actorSystem.ActorSelection(remoteActorAddress);


        //            var result = actorSystem.ActorSelection("/user/EchoActor").Ask(new Client(1, "22", 2, "", 3333));
        //            Console.WriteLine(result);

        //            if (remoteChatActor != null)
        //            {
        //                string line = string.Empty;
        //                while (line != null)
        //                {
        //                    line = Console.ReadLine();
        //                    remoteChatActor.Tell(line, child);
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("Could not get remote actor ref");
        //                Console.ReadLine();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}
    

    }

}
