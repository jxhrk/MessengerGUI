using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace messenger_gui
{
    public class MessageOld
    {

        public bool IsYourMessage { get; set; } = false;

        public string TimeStr { get; set; } = "14:32";
        public int messageid { get; set; }
        public int localmessageid { get; set; }
        public int chatid { get; set; }

        public string text { get; set; }
        public string sender { get; set; }
        public DateTime sentdate { get; set; }
        public int replyid { get; set; }
        public int flag { get; set; }

        public MessageOld(string msg, bool isYourMessage)
        {
            text = msg;
            //IsYourMessage = isYourMessage;
            //TimeStr = "14:32";

        }
    }
}
