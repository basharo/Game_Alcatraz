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

namespace Alcatraz
{

    public class Test : MoveListener
    {
        private Alcatraz[] other = new Alcatraz[4];
        private int numPlayer = 2;

        class ReceivingActor : UntypedActor
        {
            protected override void OnReceive(object message)
            {
                //siehe working code bei Client2!
            }
        }

        class SendingActor : UntypedActor
        {
            protected override void OnReceive(object message)
            {
            }
        }


        public Test()
        {
        }

        [STAThread]
        static void Main()
        {

            //string actorSystemName = ConfigurationManager.AppSettings["actorSystemName"];
            //var actorSystem = ActorSystem.Create(actorSystemName);
            //var localReceivingActor = actorSystem.ActorOf(Props.Create<ReceivingActor>(), "ReceivingActor");
            //var localSendingActor = actorSystem.ActorOf(Props.Create<SendingActor>(), "SendingActor");

            //string remoteActorAddress = ConfigurationManager.AppSettings["remoteActorAddress"];
            //var remoteChatActor = actorSystem.ActorSelection(remoteActorAddress);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Test t1 = new Test();
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
            }
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
            //Console.WriteLine("moving " + prisoner + " to " + (rowOrCol == Alcatraz.ROW ? "row" : "col") + " " + (rowOrCol == Alcatraz.ROW ? row : col));
            //Console.WriteLine(player + ", "+ prisoner + ", "+ rowOrCol+ ", " +row+ ", "+  col);

            string actorSystemName = ConfigurationManager.AppSettings["actorSystemName"];
            var actorSystem = ActorSystem.Create(actorSystemName);
            string remoteActorAddress = ConfigurationManager.AppSettings["remoteActorAddress"];
            var remoteChatActor = actorSystem.ActorSelection(remoteActorAddress);
            remoteChatActor.Tell("irgendwas");

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
