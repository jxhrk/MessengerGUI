public class SetChatInfoResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public SetChatInfoResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}