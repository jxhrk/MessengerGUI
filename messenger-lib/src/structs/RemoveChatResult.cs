public class RemoveChatResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public RemoveChatResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}