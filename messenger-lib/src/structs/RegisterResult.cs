public class RegisterResult {
    public bool ok { get; set; }
    public int status { get; set; }
    public string token { get; set; }

    public RegisterResult(bool ok, int status, string token = null) {
        this.ok = ok;
        this.status = status;
        this.token = token;
    }
}
