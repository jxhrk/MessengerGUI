namespace messenger_lib;
public class MessagePullResult
{
    public bool ok { get; set; }
    public int status { get; set; }
    public Message[] messages { get; set; }

    public MessagePullResult(bool ok, int status, Message[] messages = null)
    {
        this.ok = ok;
        this.status = status;
        this.messages = messages;
    }
}