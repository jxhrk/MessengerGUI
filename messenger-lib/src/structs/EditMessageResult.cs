public class EditMessageResult
{
    public bool ok { get; set; }
    public int status { get; set; }

    public EditMessageResult(bool ok, int status)
    {
        this.ok = ok;
        this.status = status;
    }
}