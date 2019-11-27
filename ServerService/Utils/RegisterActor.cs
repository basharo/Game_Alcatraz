using Akka.Actor;
using Newtonsoft.Json;
using ServerService.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerService.Utils
{
    public class RegisterActor : UntypedActor
    {

        private ICancelable _helloTask;
        private string path = "C:/temp/";
        private string fileName = "game.txt";
        string playerName;
        List<ClientData> exisitingClients = new List<ClientData>();
        private ISpreadService spreadService;

        public RegisterActor(ISpreadService service)
        {
            spreadService = service;
        }


        protected override void PreStart()
        {
            //_helloTask = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
            //    TimeSpan.FromSeconds(1), Context.Self, new Hello("hi"), ActorRefs.NoSender);
            File.Delete(path + fileName);
        }

        protected override void PostStop()
        {
            _helloTask.Cancel();
        }




        protected override void OnReceive(object message)
        {
            string messageString = message.ToString();
            if (messageString.StartsWith("delete"))
            {
                string playerNameToDelete = messageString.Split('|')[1];

                string fileContent = File.ReadAllText(path + fileName);
                exisitingClients = JsonConvert.DeserializeObject<List<ClientData>>(fileContent);
                var item = exisitingClients.SingleOrDefault(x => x.playerName == playerNameToDelete);
                if (item != null)
                    exisitingClients.Remove(item);

                WriteToFile(JsonConvert.SerializeObject(exisitingClients));
                string clientAdress = $"{item.protocol}://{item.system}@{item.host}:{item.port}/user/{item.actorName}";
                var remoteChatActorClient = Globals.mainActorSystem.ActorSelection(clientAdress);

                if (remoteChatActorClient != null)
                {
                    remoteChatActorClient.Tell(item.playerName + " was successfully deleted.", Self);
                }

                return;

            }

            var temp = Sender.Path.Address;
            string playerName = messageString;

            if (!File.Exists(path + fileName))
            {
                var myFile = File.Create(path + fileName);
                myFile.Close();
            }

            string content = File.ReadAllText(path + fileName);
            if (content != "")
            {
                exisitingClients = JsonConvert.DeserializeObject<List<ClientData>>(content);

                foreach (var item in exisitingClients)
                {
                    if (item.playerName == playerName)
                    {
                        Sender.Tell(playerName + " already exists. Please choose another name:");
                        return;
                    }
                }
            }

            ClientData clientToAdd = new ClientData(temp.Protocol, temp.System, temp.Host, temp.Port, Sender.Path.Name, exisitingClients.Count, playerName);
            exisitingClients.Add(clientToAdd);

            string spreadData = JsonConvert.SerializeObject(exisitingClients);
            this.WriteToFile(spreadData);
            spreadService.SendSpreadMessage(spreadData);

            Sender.Tell(playerName + " was successfully registered on server.");

            if (exisitingClients.Count == Globals.groupSize)
            {
                string allPlayerContent = File.ReadAllText(path + fileName);
                foreach (var item in exisitingClients)
                {
                    string clientAdress = $"{item.protocol}://{item.system}@{item.host}:{item.port}/user/{item.actorName}";
                    var remoteChatActorClient = Globals.mainActorSystem.ActorSelection(clientAdress);

                    if (remoteChatActorClient != null)
                    {
                        remoteChatActorClient.Tell(allPlayerContent, Self);
                    }
                }
            }

            return;
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
