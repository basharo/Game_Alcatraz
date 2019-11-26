using Alcatraz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_client_logic_akka
{
    public class Client
    {

        public Alcatraz.Alcatraz alcatraz { get; set; }
        public ClientData clientData { get; set; }

        public Client() { }

        public Client(Alcatraz.Alcatraz alcatraz, ClientData clientData)
        {
            this.alcatraz = alcatraz;
            this.clientData = clientData;

        }



    }
}
