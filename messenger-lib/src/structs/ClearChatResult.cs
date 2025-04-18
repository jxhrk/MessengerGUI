public class ClearChatResult
{
    public bool ok { get; set; }
    public int status { get; set; }
    public int? affected { get; set; }

    public ClearChatResult(bool ok, int status, int? affected = null)
    {
        this.ok = ok;
        this.status = status;
        this.affected = affected;
    }
}