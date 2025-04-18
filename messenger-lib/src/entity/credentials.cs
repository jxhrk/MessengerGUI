

public struct Credentials {
    public string username { get; set; }
    public string password { get; set; }

    public Credentials(string username, string password) {
        this.username = username;
        this.password = password;
    }
}