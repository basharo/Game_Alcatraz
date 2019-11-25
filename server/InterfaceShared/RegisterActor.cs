using Akka.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz
{
    public class RegisterActor : ReceiveActor
    {

        private ICancelable _helloTask;
        public RegisterActor()
        {

            string path = @"c:\temp\";
            string fileName = "game.txt";


            Receive<Hello>(hello =>
            {
                Console.WriteLine("[{0}]: {1}", Sender, hello.Message);
                Sender.Tell(hello);
            });

            Receive<Client>(client =>
            {
                Console.WriteLine("[{0}]: {1}", Sender, client.clientData.playerID + "--" + client.clientData.address + "--" /*+ client.port*/);
                if (!File.Exists(path + fileName))
                    File.WriteAllText(path + fileName, "name:" + client.clientData.uniqueName + "ip:" + client.clientData.address + "port:" + client.clientData.port);
                else
                    Sender.Tell("already registered");

                Sender.Tell("already registered", ActorRefs.NoSender);
                //Sender.Tell("already registered", Self);
                this.Self.Tell("already registered");
                //Self.Tell("Self send");
                Sender.Tell("Server" + client.clientData.uniqueName);

            });

            Receive<Terminated>(terminated =>
            {
                Console.WriteLine(terminated.ActorRef);
                Console.WriteLine("Was address terminated? {0}", terminated.AddressTerminated);

            });

            Receive<string>(text =>
            {
                Console.WriteLine(text);
            });
        }

        protected override void PreStart()
        {
            //_helloTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
            //    TimeSpan.FromSeconds(1), Context.Self, new Hello("hi"), ActorRefs.NoSender);
        }

        protected override void PostStop()
        {
            _helloTask.Cancel();
        }

    }
}
