using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_client_logic_akka
{
    public class ClientData
    {

        public string protocol { get; set; }
        public string system { get; set; }
        public string host { get; set; }
        public int? port { get; set; }
        public string pathString { get; set; }
        public int playerId { get; set; }
        public string playerName { get; set; }

        public ClientData () { }

        public ClientData(string protocol, string system, string host, int? port, string pathString, int playerId, string playerName)
        {
            this.protocol = protocol;
            this.system = system;
            this.host = host;
            this.port = port;
            this.pathString = pathString;
            this.playerId = playerId;
            this.playerName = playerName;

            //akka.tcp://server@localhost:5555/user/RegisterActor
        }

        public override string ToString()
        {
            return $"{protocol}://{system}@{host}:{port}{pathString},{playerName};";
        }
    }
}
