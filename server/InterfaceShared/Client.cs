using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alcatraz;
using Newtonsoft.Json;

namespace Interface
{
    public class Client
    {
       
        public Alcatraz.Alcatraz alcatraz { get; set; }
        public int playerID { get; set; }
        public ClientData clientData { get; set; }

        public Client() { }

        public Client(Alcatraz.Alcatraz alcatraz, ClientData clientData)
        {
            this.alcatraz = alcatraz;
            this.playerID = clientData.playerID;
            this.clientData = clientData;
           
        }


    }
}
