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

            return $"{spreadMessage.Groups[0].ToString()}|{spreadMessage.ServiceType}|{messageData}|{spreadMessage.Sender.ToString()}";
        }
    }
}
