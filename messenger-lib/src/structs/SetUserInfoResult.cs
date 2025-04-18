public class SetUserInfoResult {
    public bool ok { get; set; }
    public int status { get; set; }

    public SetUserInfoResult(bool ok, int status) {
        this.ok = ok;
        this.status = status;
    }
}

