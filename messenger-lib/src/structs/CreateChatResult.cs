namespace messenger_lib;

public class CreateChatResult
{
    public bool ok { get; set; }
    public int status { get; set; }
    public Chat chat { get; set; }

    public CreateChatResult(bool ok, int status, Chat chat = null)
    {
        this.ok = ok;
        this.status = status;
        this.chat = chat;
    }
}