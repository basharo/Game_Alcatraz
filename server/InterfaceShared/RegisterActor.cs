using Akka.Actor;
using Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public class RegisterActor : UntypedActor
    {

        private ICancelable _helloTask;
        ClientData[] players;

        public RegisterActor()
        {

            string path = @"c:\temp\";
            string fileName = "game.txt";

        }

            /*
            Receive<Hello>(hello =>
            {
                Console.WriteLine("[{0}]: {1}", Sender, hello.Message);
                Sender.Tell(hello);
            });

            Receive<ClientData>(client =>
            {


                Console.WriteLine(client);

            });

            Receive<Terminated>(terminated =>
            {
                Console.WriteLine(terminated.ActorRef);
                Console.WriteLine("Was address terminated? {0}", terminated.AddressTerminated);

            });

            Receive<string>(text =>
            {
                if (text == "start")
                {
                    return;
                }

                ClientData clientData = new ClientData();

                string[] slice = text.Split(';');
                int n = 0;

                foreach (string item in slice)
                {
                    if (item.Contains("akka.tcp"))
                    {
                        return;
                    }
                    string[] element = item.Split(',');

                    clientData.address = element[0];
                    clientData.playerID = 1;
                    clientData.playerName = element[1];

                    players[0] = clientData;

                    if (n % 2 == 0 && n != 0)
                    {
                        WriteToFIle("\n", "C:\temp", "game.txt");
                    }

                }

                n++;
            });
      
        }
        */

                
                

                //Sender.Tell("already registered", ActorRefs.NoSender);
                //Sender.Tell("already registered", Self);
                //this.Self.Tell("already registered");
                //Self.Tell("Self send");
                //Sender.Tell("Server" + client.clientData.uniqueName);

                //    Console.WriteLine(text);
                //    Sender.Tell("");
                //    Console.ReadLine();
                //});
            

        protected override void PreStart()
        {
            //_helloTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
            //    TimeSpan.FromSeconds(1), Context.Self, new Hello("hi"), ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            _helloTask.Cancel();
        }


        void WriteToFIle(string line, string path, string fileName)
        {
            if (!File.Exists(path + fileName))
                File.WriteAllText(path + fileName, line);
            else
                Sender.Tell("already registered");
        }

        protected override void OnReceive(object message)
        {
            Console.WriteLine(message);
        }
    }
}
