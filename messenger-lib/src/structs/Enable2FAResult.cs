public class Enable2FAResult
{
    public bool ok { get; set; }
    public int status { get; set; }
    public string? key { get; set; }

    public Enable2FAResult(bool ok, int status, string? key = null)
    {
        this.ok = ok;
        this.status = status;
        this.key = key;
    }
}