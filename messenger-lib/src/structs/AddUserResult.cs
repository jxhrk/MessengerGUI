public class AddUserResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public AddUserResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}