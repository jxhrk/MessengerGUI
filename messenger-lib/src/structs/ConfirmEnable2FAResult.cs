public class ConfirmEnable2FAResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public ConfirmEnable2FAResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}