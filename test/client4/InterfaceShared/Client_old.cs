using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface
{
    public class Client_old

    {
        public int sequenceID { get; set; }              //seqence ID assigned by Server at registration
        public String unique_name { get; set; }          // unique player name   
        public int preferred_group_size { get; set; }    //  preferred group size            
        public String ipAddress { get; set; }            // address of client to contact him back         
        public int port { get; }                         //port which is used for the

        public Client_old(int sequenceID, String unique_name, int preferred_group_size, String ipAddress, int port)
        {
            this.sequenceID = sequenceID;
            this.unique_name = unique_name;
            this.preferred_group_size = preferred_group_size;
            this.ipAddress = ipAddress;
            this.port = port;
        }
    }
}
