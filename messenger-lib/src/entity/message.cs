using System;

namespace messenger_lib;

public class Message {
    public int localmessageid { get; set; }
    [Obsolete("Not used in client! Always zero")]
    public int messageid { get; set; }

    public int chatid { get; set; }
    public MessageFlag flag { get; set; }
    public string text { get; set; }
    public DateTime sentdate { get; set; }
    
    [Obsolete("Not used in client! Always zero")]
    public int senderid { get; set; }
    public string sender { get; set; }

    [Obsolete("Not used in client! Always -1")]
    public int replyid { get; set; }

    // UI

    public bool IsYourMessage { get; set; } = false;
    
    public string TimeStr { get; set; } = "14:32";

    public override string ToString()
    {
        return $"{localmessageid} {text} {sentdate}";
    }

    public Message()
    {
        
    }

    public Message(Message msg)
    {
        localmessageid = msg.localmessageid;
        messageid = msg.messageid;
        chatid = msg.chatid;
        flag = msg.flag;
        text = msg.text;
        sentdate = msg.sentdate;
        senderid = msg.senderid;
        sender = msg.sender;
        replyid = msg.replyid;
        IsYourMessage = msg.IsYourMessage;
        TimeStr = msg.TimeStr;
    }

}