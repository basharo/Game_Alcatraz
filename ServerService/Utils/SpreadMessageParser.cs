using spread;
using System.Text;

namespace ServerService.Utils
{
    public static class SpreadMessageParser
    {
        public static string ParseRegularSpreadMessage(this SpreadMessage spreadRegularMessage)
        {
            return spreadRegularMessage.ToCustomString();
        }

        public static string ParseMembershipMessage(this SpreadMessage spreadMembershipMessage)
        {
            return spreadMembershipMessage.ToCustomString();
        }

        private static string ToCustomString(this SpreadMessage spreadMessage)
        {
            int length = spreadMessage.Data.Length;
            string messageData;
            if (length == 0)
            {
                messageData = "No data available";
            }
            else
            {
                messageData = Encoding.ASCII.GetString(spreadMessage.Data, 0, length);
            }

            MembershipInfo info = spreadMessage.MembershipInfo;

            return $"{info.Group.ToString()}|{info.GroupID}|{messageData}|{spreadMessage.Sender.ToString()}";
        }
    }
}
