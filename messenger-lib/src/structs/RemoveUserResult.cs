public class RemoveUserResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public RemoveUserResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}