using Akka.Actor;
using Interface;
using Newtonsoft.Json;
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
        private string path = "C:/temp/";
        private string fileName = "game.txt";
        string playerName;
        List<ClientData> exisitingClients = new List<ClientData>();

        public RegisterActor()
        {
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


        

        protected override void OnReceive(object message)
        {

           
                
            var temp = Sender.Path.Address;
            string playerName = message.ToString();

            if (!File.Exists(path + fileName))
            {
                var myFile = File.Create(path + fileName);
                myFile.Close();
            }

            string content = File.ReadAllText(path + fileName);
            if (content != "")
            {
                exisitingClients = JsonConvert.DeserializeObject<List<ClientData>>(content);

                if (exisitingClients.Count >= Globals.groupSize)
                {
                    foreach (var item in exisitingClients)
                    {
                        string clientAdress = $"{item.protocol}://{item.system}@{item.host}:{item.port}/user/{item.actorName}";
                        var remoteChatActorClient = Globals.mainActorSystem.ActorSelection(clientAdress);

                        if (remoteChatActorClient != null)
                        {
                            remoteChatActorClient.Tell(content, Self);
                        }
                    }
                }
                else
                {
                    foreach (var item in exisitingClients)
                    {
                        if (item.playerName == playerName)
                        {
                            Sender.Tell(playerName + " already exists. Please choose another name:");
                            return;
                        }
                    }
                }
            }

            ClientData clientToAdd = new ClientData(temp.Protocol, temp.System, temp.Host, temp.Port, Sender.Path.Name, exisitingClients.Count + 1, playerName);
            exisitingClients.Add(clientToAdd);

            this.WriteToFile(JsonConvert.SerializeObject(exisitingClients));

            Sender.Tell(playerName + " was successfully registered on server.");

            
        }


        private void WriteToFile(string line)
        {
            if (File.Exists(path + fileName))
            {
                File.Delete(path + fileName);
            }
                
            File.WriteAllText(path + fileName, line);
            
        }
    }
}
