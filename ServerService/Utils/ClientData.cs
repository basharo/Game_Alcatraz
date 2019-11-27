using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Utils
{
    public class ClientData
    {

        public string protocol { get; set; }
        public string system { get; set; }
        public string host { get; set; }
        public int? port { get; set; }
        public string actorName { get; set; }
        public int playerId { get; set; }
        public string playerName { get; set; }

        public ClientData() { }

        public ClientData(string protocol, string system, string host, int? port, string actorName, int playerId, string playerName)
        {
            this.protocol = protocol;
            this.system = system;
            this.host = host;
            this.port = port;
            this.actorName = actorName;
            this.playerId = playerId;
            this.playerName = playerName;
        }
    }
}
