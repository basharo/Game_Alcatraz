using System;
using System.Collections.Generic;
using System.Text;

namespace ServerService.Client
{
    public class ClientResponse
    {
        public List<string> ClientAddresses { get; set; }

        public int GroupSize { get; set; }

        public string Error { get; set; }

        public ClientResponse(string error)
        {
            ClientAddresses = new List<string>();
            GroupSize = 0;
            Error = error;
        }

        public ClientResponse()
        {
            ClientAddresses = new List<string>();
            GroupSize = 0;
            Error = string.Empty;
        }
    }
}
