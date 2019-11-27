using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Alcatraz;
using Akka.Configuration;
using Akka;
using Akka.Remote;
using Akka.Actor;
using System.Configuration;
using Newtonsoft.Json;
using Akka.Event;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Alcatraz
{

    public class Game : MoveListener
    {
        private Alcatraz[] other = new Alcatraz[4];
        private int numPlayer = Globals.AllPlayers.Count;
        private static string playerName = "";
        private static string line = string.Empty;
        class Move
        {
            public int playerId;
            public int prisonerId;
            public int rowOrCol;
            public int row;
            public int col;
        }


        //++++++++++++++++++++++++++++++++
        //+++++ ANBINDUNG ZUM SERVER +++++
        //++++++++++++++++++++++++++++++++
        
 
        public class RegisterActor : UntypedActor
        {

            public RegisterActor()
            {


            }

            protected override void OnReceive(object message)
            {
                var messageString = message.ToString();

                if (messageString.Contains("exists"))
                {
                    Console.WriteLine(message.ToString());
                    string playerName = Console.ReadLine();

                    Sender.Tell(playerName, Self);

                }

                if (messageString.Contains("successfully registered"))
                {
                    Console.WriteLine(messageString);

                    line = Console.ReadLine();
                }

                if (messageString.Contains("deleted"))
                {
                    Console.WriteLine(messageString);
                }


                if (messageString.StartsWith("[{\"protocol"))
                {
                    List<ClientData> exisitingClients = JsonConvert.DeserializeObject<List<ClientData>>(messageString);

                    Globals.AllPlayers = exisitingClients;
                }
            }
        }

        public class DeadletterMonitor : ReceiveActor
        {
            public DeadletterMonitor()
            {
                Receive<DeadLetter>(dl => HandleDeadletter(dl));
            }
            private void HandleDeadletter(DeadLetter dl)
            {
                Console.WriteLine("Something getting Worng.. Please restart your Program");
                Console.WriteLine($"DeadLetter captured: {dl.Message}, sender: {dl.Sender}, recipient: {dl.Recipient}");
                Context.Stop(Self);
                startgame();
            }
        }


        class ReceivingActor : UntypedActor
        {
            Alcatraz myGame;
            public ReceivingActor(Alcatraz localAlcatraz)
            {
                myGame = localAlcatraz;
            }
            protected override void OnReceive(object message)
            {

                Move receivedMove = JsonConvert.DeserializeObject<Move>(message.ToString());
                myGame.doMove(myGame.getPlayer(receivedMove.playerId), myGame.getPrisoner(receivedMove.prisonerId), receivedMove.rowOrCol, receivedMove.row, receivedMove.col);
           
            }
        }

        class SendingActor : UntypedActor
        {
            protected override void OnReceive(object message)
            {
            }
        }

        public static class Globals
        {
            public static ActorSystem ActSys { get; set; }
            //public static string[] remoteActorAddresses;
            public static List<ClientData> AllPlayers;
            public static List<string> remoteActorAddresses;
            public static int myPlayerId;
            public static string myName;


            static Globals()
            {
                //string actorSystemName = ConfigurationManager.AppSettings["actorSystemName"];
                ActSys = ActorSystem.Create("alcatraz");
                remoteActorAddresses = new List<string>();
                //string akkaAddress;
            }
        }
        /*public class akkaAddress
        {
            public string address;
        }*/


        public Game()
        {
        }
        public static bool startgame()
        {
            
            int gamesize = 0;
            string temp = "";
            Console.WriteLine("To cancel the registration enter 'delete'");
            Console.WriteLine("Please choose game Size max Size 4:");
            temp = Console.ReadLine();
            try
            {
                int.TryParse(temp, out gamesize);
                while (String.IsNullOrEmpty(temp) || gamesize == 0 || gamesize > 4 || gamesize < 2)
                {
                    Console.WriteLine("GameSize cannot be empty or non Number. Please choose a player name:");
                    temp = Console.ReadLine();

                    int.TryParse(temp, out gamesize);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Please choose game Size:");
            }
            Console.WriteLine("Please choose a player name:");
            playerName = Console.ReadLine();

            while (playerName == "")
            {
                Console.WriteLine("Your name cannot be empty. Please choose a player name:");
                playerName = Console.ReadLine();
            }
            try
            {
                //startActorSystem("alcatraz");
                string unique = DateTime.Now.ToString().Replace("\\", "").Replace("/", "").Replace(":", "").Replace(" ", "");
                // Setup an actor that will handle deadletter type messages
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                
                var deadletterWatchActorRef =   Globals.ActSys.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor"+ unique);

                // subscribe to the event stream for messages of type "DeadLetter"
                Globals.ActSys.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                var localChatActor = Globals.ActSys.ActorOf(Props.Create<RegisterActor>(), "RegisterActor"+ unique);

                string remoteActorAddressClient1 = "akka.tcp://alcatraz@localhost:5555/user/RegisterActor";
                var remoteChatActorClient1 = Globals.ActSys.ActorSelection(remoteActorAddressClient1);
                try
                {
                    if (remoteChatActorClient1 != null)
                    {

                        remoteChatActorClient1.Tell("register" + playerName + "|" + gamesize, localChatActor);


                        while (line != "gamestart")
                        {

                            if (line == "delete")
                            {
                                remoteChatActorClient1.Tell("delete|" + playerName, localChatActor);
                                line = string.Empty;
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine("Could not get remote actor ref");
                        Console.ReadLine();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            //ANBINDUNG ZUM SERRVER - end

            Globals.myName = playerName;

            /*
            string testJSON = @"[
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5248,""actorName"":""ReceivingActor"",""playerId"":0,""playerName"":""Franz""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5249,""actorName"":""ReceivingActor"",""playerId"":1,""playerName"":""Bashar""}
            ]";


            string testJSON3 = @"[
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5248,""actorName"":""ReceivingActor"",""playerId"":0,""playerName"":""Franz""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5249,""actorName"":""ReceivingActor"",""playerId"":1,""playerName"":""Bashar""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5250,""actorName"":""ReceivingActor"",""playerId"":2,""playerName"":""Indrit""}
            ]";

            string testJSON4 = @"[
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5248,""actorName"":""ReceivingActor"",""playerId"":0,""playerName"":""Franz""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5249,""actorName"":""ReceivingActor"",""playerId"":1,""playerName"":""Bashar""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5250,""actorName"":""ReceivingActor"",""playerId"":2,""playerName"":""Indrit""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":5251,""actorName"":""ReceivingActor"",""playerId"":3,""playerName"":""Palo""}
            ]";
            */

            //Console.WriteLine(Globals.remoteActorAddresses.ElementAt(0));

            foreach (var item in Globals.AllPlayers)
            {
                //Console.WriteLine(item.ToString());

                //Globals.remoteActorAddresses.Add(item.ToString());

                if (item.playerName == Globals.myName)
                {
                    Globals.myPlayerId = item.playerId;
                    //break;
                }
                else
                {
                    //Console.WriteLine(item.ToString());
                    Globals.remoteActorAddresses.Add(item.ToString());
                }
            }

            if (Globals.AllPlayers.Count == 2)
            {
                Game t1 = new Game();
                Game t2 = new Game();
                Alcatraz a1 = new Alcatraz();
                Alcatraz a2 = new Alcatraz();
                t1.setNumPlayer(Globals.AllPlayers.Count);
                t2.setNumPlayer(Globals.AllPlayers.Count);
                a1.init(2, 0);
                a2.init(2, 1);
                a1.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a1.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a2.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a2.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                t1.setOther(0, a2);
                t2.setOther(0, a1);
                if (Globals.myPlayerId == 0)
                {
                    a1.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a1), "ReceivingActor");
                }
                else if (Globals.myPlayerId == 1)
                {
                    a2.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a2), "ReceivingActor");
                }
                a1.addMoveListener(t1);
                a2.addMoveListener(t2);
                a1.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a2.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a1.start();
                a2.start();
            }
            else if (Globals.AllPlayers.Count == 3)
            {
                Game t1 = new Game();
                Game t2 = new Game();
                Game t3 = new Game();
                Alcatraz a1 = new Alcatraz();
                Alcatraz a2 = new Alcatraz();
                Alcatraz a3 = new Alcatraz();
                t1.setNumPlayer(Globals.AllPlayers.Count);
                t2.setNumPlayer(Globals.AllPlayers.Count);
                t3.setNumPlayer(Globals.AllPlayers.Count);
                a1.init(3, 0);
                a2.init(3, 1);
                a3.init(3, 2);
                a1.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a1.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a1.getPlayer(2).Name = Globals.AllPlayers.ElementAt(2).playerName;
                a2.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a2.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a2.getPlayer(2).Name = Globals.AllPlayers.ElementAt(2).playerName;
                a3.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a3.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a3.getPlayer(2).Name = Globals.AllPlayers.ElementAt(2).playerName;
                t1.setOther(0, a2);
                t1.setOther(1, a3);
                t2.setOther(0, a1);
                t2.setOther(1, a3);
                t3.setOther(0, a1);
                t3.setOther(1, a2);
                if (Globals.myPlayerId == 0)
                {
                    a1.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a1), "ReceivingActor");
                }
                else if (Globals.myPlayerId == 1)
                {
                    a2.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a2), "ReceivingActor");
                }
                else if (Globals.myPlayerId == 2)
                {
                    a3.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a3), "ReceivingActor");
                }
                a1.addMoveListener(t1);
                a2.addMoveListener(t2);
                a3.addMoveListener(t3);
                a1.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a2.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a3.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a1.start();
                a2.start();
                a3.start();
            }
            else if (Globals.AllPlayers.Count == 4)
            {
                Game t1 = new Game();
                Game t2 = new Game();
                Game t3 = new Game();
                Game t4 = new Game();
                Alcatraz a1 = new Alcatraz();
                Alcatraz a2 = new Alcatraz();
                Alcatraz a3 = new Alcatraz();
                Alcatraz a4 = new Alcatraz();
                t1.setNumPlayer(Globals.AllPlayers.Count);
                t2.setNumPlayer(Globals.AllPlayers.Count);
                t3.setNumPlayer(Globals.AllPlayers.Count);
                t4.setNumPlayer(Globals.AllPlayers.Count);
                a1.init(4, 0);
                a2.init(4, 1);
                a3.init(4, 2);
                a4.init(4, 3);
                a1.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a1.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a1.getPlayer(2).Name = Globals.AllPlayers.ElementAt(2).playerName;
                a1.getPlayer(3).Name = Globals.AllPlayers.ElementAt(3).playerName;
                a2.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a2.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a2.getPlayer(2).Name = Globals.AllPlayers.ElementAt(2).playerName;
                a2.getPlayer(3).Name = Globals.AllPlayers.ElementAt(3).playerName;
                a3.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a3.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a3.getPlayer(2).Name = Globals.AllPlayers.ElementAt(2).playerName;
                a3.getPlayer(3).Name = Globals.AllPlayers.ElementAt(3).playerName;
                a4.getPlayer(0).Name = Globals.AllPlayers.ElementAt(0).playerName;
                a4.getPlayer(1).Name = Globals.AllPlayers.ElementAt(1).playerName;
                a4.getPlayer(2).Name = Globals.AllPlayers.ElementAt(2).playerName;
                a4.getPlayer(3).Name = Globals.AllPlayers.ElementAt(3).playerName;
                t1.setOther(0, a2);
                t1.setOther(1, a3);
                t1.setOther(2, a4);
                t2.setOther(0, a1);
                t2.setOther(1, a3);
                t2.setOther(2, a4);
                t3.setOther(0, a1);
                t3.setOther(1, a2);
                t3.setOther(2, a4);
                t4.setOther(0, a1);
                t4.setOther(1, a2);
                t4.setOther(2, a3);
                if (Globals.myPlayerId == 0)
                {
                    a1.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a1), "ReceivingActor");
                }
                else if (Globals.myPlayerId == 1)
                {
                    a2.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a2), "ReceivingActor");
                }
                else if (Globals.myPlayerId == 2)
                {
                    a3.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a3), "ReceivingActor");
                }
                else if (Globals.myPlayerId == 3)
                {
                    a4.showWindow();
                    var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a4), "ReceivingActor");
                }
                a1.addMoveListener(t1);
                a2.addMoveListener(t2);
                a3.addMoveListener(t3);
                a4.addMoveListener(t4);
                a1.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a2.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a3.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a4.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a1.start();
                a2.start();
                a3.start();
                a4.start();
            }

            Application.Run();
            return true;
        }
        [STAThread]
        static void Main()
        {
            //++++++++++++++++++++++++++++++++
            //+++++ ANBINDUNG ZUM SERVER +++++
            //++++++++++++++++++++++++++++++++
            startgame();
        }

        public static void Test_FormClosed(object sender, FormClosedEventArgs args)
        {
            Environment.Exit(0);
        }

        public void setOther(int i, Alcatraz t)
        {
            this.other[i] = t;
        }

        public int getNumPlayer()
        {
            return numPlayer;
        }

        public void setNumPlayer(int numPlayer)
        {
            this.numPlayer = numPlayer;
        }

        public void doMove(Player player, Prisoner prisoner, int rowOrCol, int row, int col)
        {
            

            Move lastMove = new Move();
            lastMove.playerId = player.Id;
            lastMove.prisonerId = prisoner.Id;
            lastMove.rowOrCol = rowOrCol;
            lastMove.row = row;
            lastMove.col = col;

            string lastMoveJson = JsonConvert.SerializeObject(lastMove);

            //string currentAddress;
            foreach (var item in Globals.remoteActorAddresses)
            {
                Globals.ActSys.ActorSelection(item).Tell(lastMoveJson);
            }


            for (int i = 0; i < getNumPlayer()-1; i++)
            {
                other[i].doMove(other[i].getPlayer(player.Id), other[i].getPrisoner(prisoner.Id), rowOrCol, row, col);
                Console.WriteLine("Player " + other[i].getPlayer(player.Id) + "Prisoner " + prisoner + rowOrCol + row + "col" + col);
            }
        }

        public void undoMove()
        {
            Console.WriteLine("Undoing move");
        }
            
        public void gameWon(Player player)
        {
            Console.WriteLine("Player " + player.Id + " wins.");
        }
    }
}
