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
using Interface;
using System.Linq;

namespace Alcatraz
{

    public class Game : MoveListener
    {
        private Alcatraz[] other = new Alcatraz[4];
        private int numPlayer = 2;
        private static string playerName;

        class Move
        {
            public int playerId;
            public int prisonerId;
            public int rowOrCol;
            public int row;
            public int col;
        }
        /*
        public class GameActor : UntypedActor
        {
            public GameActor()
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
                }


                if (messageString == "start")
                {
                    return;
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
                Console.WriteLine($"DeadLetter captured: {dl.Message}, sender: {dl.Sender}, recipient: {dl.Recipient}");
            }
        }*/


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
                string actorSystemName = ConfigurationManager.AppSettings["actorSystemName"];
                ActSys = ActorSystem.Create(actorSystemName);
            }
        }
        public Game()
        {
        }

        [STAThread]
        static void Main()
        {
            //start
            /*
            Console.WriteLine("To cancel the registration enter 'delete'");
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

                // Setup an actor that will handle deadletter type messages
                var deadletterWatchMonitorProps = Props.Create(() => new DeadletterMonitor());
                var deadletterWatchActorRef = Globals.ActSys.ActorOf(deadletterWatchMonitorProps, "DeadLetterMonitoringActor");

                // subscribe to the event stream for messages of type "DeadLetter"
                Globals.ActSys.EventStream.Subscribe(deadletterWatchActorRef, typeof(DeadLetter));

                var localChatActor = Globals.ActSys.ActorOf(Props.Create<GameActor>(), "GameActor");

                
                string remoteActorAddressClient1 = "akka.tcp://alcatraz@192.168.43.249:5555/user/RegisterActor";
                var remoteChatActorClient1 = Globals.ActSys.ActorSelection(remoteActorAddressClient1);

                if (remoteChatActorClient1 != null)
                {
                        
                    remoteChatActorClient1.Tell(playerName, localChatActor);
                    
                    string line = string.Empty;
                        while (line != null) {
                            line = Console.ReadLine();
                            
                            if(line == "delete")
                        {
                            remoteChatActorClient1.Tell("delete|" + playerName, localChatActor);
                        }


                    }

                    } else {
                    Console.WriteLine("Could not get remote actor ref");
                    Console.ReadLine();
                }  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            */
            //end

            Globals.myName = "Franz";

            string testJSON = @"[
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":1111,""actorName"":""ReceivingActor"",""playerId"":0,""playerName"":""Franz""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":2222,""actorName"":""ReceivingActor"",""playerId"":1,""playerName"":""Bashar""}
            ]";





            Globals.AllPlayers = JsonConvert.DeserializeObject<List<ClientData>>(testJSON);
            foreach (var item in Globals.AllPlayers)
            {
                if (item.playerName == Globals.myName)
                {
                    Globals.myPlayerId = item.playerId;
                    break;
                }
                    

                Globals.remoteActorAddresses.Add(item.ToString());
            }



            //JArray ClientDataArray = JArray.Parse(testJSON);

            //Console.WriteLine(ClientDataArray.Count);
            //Console.WriteLine(ClientDataArray[0]["protocol"]);




            /*for (int count = 0; count< ClientDataArray.Count; count++)
            {
                string protocol =

                //akka.tcp://Client2@localhost:5249/user/ReceivingActor

                Globals.remoteActorAddresses[count] = ClientDataArray[count]["protocol"].ToString() + "://ActorSystem@" +
                                                ClientDataArray[count]["host"] + ":" +
                                                ClientDataArray[count]["port"] + "/user/" +
                                                ClientDataArray[count]["actorName"];
                if (ClientDataArray[count]["playerName"].ToString() == Globals.myName)
                {
                    Globals.myPlayerId = ClientDataArray[count]["playerId"].ToString();
                }
            }
            var localSendingActor = Globals.ActSys.ActorOf(Props.Create<SendingActor>(), "SendingActor");

            string remoteActorAddress = ConfigurationManager.AppSettings["remoteActorAddress"];
            var remoteChatActor = Globals.ActSys.ActorSelection(remoteActorAddress);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //PM: statische init für zwei Spieler - MUSS DYNAMISIERT WERDEN!!!

            int numberOfPlayers = ClientDataArray.Count;
            


           /* myId

            numberOf Players

                   string testJSON = @"[
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":1111,""actorName"":""ReceivingActor"",""playerId"":1,""playerName"":""Franz""},
                {""protocol"":""akka.tcp"",""system"":""alcatraz"",""host"":""localhost"",""port"":2222,""actorName"":""ReceivingActor"",""playerId"":2,""playerName"":""Bashar""}
            ]";

            int numberofPlayers = testJSON.Replace.Split(':').Length / 15;
            var player1 = testJSON.Split(':')[14];
            */


            if (Globals.AllPlayers.Count == 2)
            {
                Game t1 = new Game();
                Game t2 = new Game();
                Alcatraz a1 = new Alcatraz();
                Alcatraz a2 = new Alcatraz();
                t1.setNumPlayer(2);
                t2.setNumPlayer(2);
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
                }
                else if (Globals.myPlayerId == 1)
                {
                    a2.showWindow();
                }
                a1.addMoveListener(t1);
                a2.addMoveListener(t2);
                a1.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a2.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a1.start();
                a2.start();
            } else if (Globals.AllPlayers.Count == 3) {
                Game t1 = new Game();
                Game t2 = new Game();
                Alcatraz a1 = new Alcatraz();
                Alcatraz a2 = new Alcatraz();
                t1.setNumPlayer(2);
                t2.setNumPlayer(2);
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
                }
                else if (Globals.myPlayerId == 1)
                {
                    a2.showWindow();
                }
                a1.addMoveListener(t1);
                a2.addMoveListener(t2);
                a1.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a2.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a1.start();
                a2.start();
            }
            else if (Globals.AllPlayers.Count == 4)
            {
                Game t1 = new Game();
                Game t2 = new Game();
                Alcatraz a1 = new Alcatraz();
                Alcatraz a2 = new Alcatraz();
                t1.setNumPlayer(2);
                t2.setNumPlayer(2);
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
                }
                else if (Globals.myPlayerId == 1)
                {
                    a2.showWindow();
                }
                a1.addMoveListener(t1);
                a2.addMoveListener(t2);
                a1.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a2.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                a1.start();
                a2.start();
            }



           /* for (int gameInstanceNo = 0; gameInstanceNo < Globals.AllPlayers.Count; gameInstanceNo++)
            {
                Game Game = new Game();
                Alcatraz Alcatraz = new Alcatraz();
                Game.setNumPlayer(Globals.AllPlayers.Count);
                Alcatraz.init(Globals.AllPlayers.Count, gameInstanceNo);

                for (int playerNameNo = 0; playerNameNo < Globals.AllPlayers.Count; playerNameNo++)
                {
                    Alcatraz.getPlayer(playerNameNo).Name = Globals.AllPlayers.ElementAt(playerNameNo).playerName;
                }

            }*/

            /*Game t1 = new Game();
            Game t2 = new Game();
            Alcatraz a1 = new Alcatraz();
            Alcatraz a2 = new Alcatraz();
            t1.setNumPlayer(2);
            t2.setNumPlayer(2);
            a1.init(2, 0);
            a2.init(2, 1);
            a1.getPlayer(0).Name = "Player 1";
            a1.getPlayer(1).Name = "Player 2";
            a2.getPlayer(0).Name = "Player 1";
            a2.getPlayer(1).Name = "Player 2";
            t1.setOther(0, a2);
            t2.setOther(0, a1);
            a1.showWindow();
            a1.addMoveListener(t1);
            //a2.showWindow();
            a2.addMoveListener(t2);
            a1.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
            a2.getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
            a1.start();
            a2.start();*/
            








            //PM: Dynamisierung ist leider nicht richtig gewesen:
            
            /*Test t1 = new Test();
            Alcatraz a1 = new Alcatraz();
            int players = 2;
          
            List<Test> testList = new List<Test>();
            List<Alcatraz> alcatrazList = new List<Alcatraz>();

            for (int i = 0; i < players; i++)
            {

                t1 = new Test();
                a1 = new Alcatraz();
   
                t1.setNumPlayer(players);
                //i is in this case the playersID
                a1.init(players, i);
                Console.WriteLine("a1.init("+players+" , "+i);

                for (int j = 1; j < players + 1; j++)
                {
                    int help = j - 1;
                    a1.getPlayer(help).Name = "Player " + j;

                    Console.WriteLine("getPlayer("+help+").Name = Player "+j);
                }
                
                alcatrazList.Add(a1);
                testList.Add(t1);
            }
            int counterForEachTest = 0;
            foreach (Test testListItem in testList)
            {
                int counterForEachAlcatraz = 0;
                int iterator = 0;
                foreach(Alcatraz alcatrazItem in alcatrazList)
                {
                    if (counterForEachTest != counterForEachAlcatraz)
                    {
                        testListItem.setOther(iterator, alcatrazItem);
                        Console.WriteLine("t" + counterForEachTest + ".setOther( " + iterator + "," + counterForEachAlcatraz);
                        iterator++;
                    }
                    counterForEachAlcatraz++;
                }
                counterForEachTest++;
            }
            for (int ii = 0; ii < players; ii++)
            {
                alcatrazList[0].showWindow();
                //alcatrazList[ii].showWindow();
                alcatrazList[ii].addMoveListener(testList[0]);
                alcatrazList[ii].getWindow().FormClosed += new FormClosedEventHandler(Test_FormClosed);
                alcatrazList[ii].start();
            }*/

            var localReceivingActor = Globals.ActSys.ActorOf(Props.Create<ReceivingActor>(a1), "ReceivingActor");

            Application.Run();
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

            //Console.WriteLine(Globals.remoteActorAddresses[0]);

            //string remoteActorAddress = ConfigurationManager.AppSettings["remoteActorAddress"];
            //var remoteChatActor = Globals.ActSys.ActorSelection(Globals.remoteActorAddresses[0]);

            string lastMoveJson = JsonConvert.SerializeObject(lastMove);
            //remoteChatActor.Tell(lastMoveJson);


            //forea
            //remoteChatActor.Tell();

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
