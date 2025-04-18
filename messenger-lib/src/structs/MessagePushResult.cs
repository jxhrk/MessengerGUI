public class MessagePushResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public MessagePushResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}