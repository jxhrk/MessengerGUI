namespace messenger_lib;

public class UserInfoResult {
    public bool ok { get; set; }
    public int status { get; set; }
    public UserInfoObj info { get; set; }

    public UserInfoResult(bool ok, int status, UserInfoObj info = null)
    {
        this.ok = ok;
        this.status = status;
        this.info = info;
    }
}

