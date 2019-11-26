using System;
using System.Collections.Generic;
using System.Text;
using spread;

namespace ServerService.Interface
{
    public interface ISpreadService
    {
        public SpreadConnection _spreadConnection { get; set; }
        public SpreadGroup _spreadGroup { get; set; }

        SpreadConnection ConnectToSpread();

        void JoinSpreadGroup();

        SpreadMessage SendSpreadMessage(string spreadMessage);

        SpreadMessage ReceiveSpreadMessage();

        SpreadMessage SendACK();
    }
}
