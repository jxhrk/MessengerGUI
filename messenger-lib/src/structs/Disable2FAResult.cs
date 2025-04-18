public class Disable2FAResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public Disable2FAResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}