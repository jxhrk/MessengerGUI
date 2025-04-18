public class RemoveMessageResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public RemoveMessageResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}
