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

namespace Alcatraz
{

    public class Test : MoveListener
    {
        private Alcatraz[] other = new Alcatraz[4];
        private int numPlayer = 2;

        class Move
        {
            public int playerId;
            public int prisonerId;
            public int rowOrCol;
            public int row;
            public int col;
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


            static Globals()
            {
                string actorSystemName = ConfigurationManager.AppSettings["actorSystemName"];
                ActSys = ActorSystem.Create(actorSystemName);
            }
        }
        public Test()
        {
        }

        [STAThread]
        static void Main()
        {

            var localSendingActor = Globals.ActSys.ActorOf(Props.Create<SendingActor>(), "SendingActor");

            string remoteActorAddress = ConfigurationManager.AppSettings["remoteActorAddress"];
            var remoteChatActor = Globals.ActSys.ActorSelection(remoteActorAddress);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //PM: statische init für zwei Spieler - MUSS DYNAMISIERT WERDEN!!!
            Test t1 = new Test();
            Test t2 = new Test();
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
            a2.start();
            

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



            string remoteActorAddress = ConfigurationManager.AppSettings["remoteActorAddress"];
            var remoteChatActor = Globals.ActSys.ActorSelection(remoteActorAddress);

            string lastMoveJson = JsonConvert.SerializeObject(lastMove);
            remoteChatActor.Tell(lastMoveJson);

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
