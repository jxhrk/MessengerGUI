public class ChatInfoResult
{
    public bool ok { get; set; }
    public int status { get; set; }
    public ChatInfoObj info { get; set; }

    public ChatInfoResult(bool ok, int status, ChatInfoObj info = null)
    {
        this.ok = ok;
        this.status = status;
        this.info = info;
    }
}