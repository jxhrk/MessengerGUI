namespace messenger_lib;

public class GetUserChatsResult
{
    public bool ok { get; set; }
    public int status { get; set; }
    public Chat[] chats { get; set; }

    public GetUserChatsResult(bool ok, int status, Chat[] chats = null)
    {
        this.ok = ok;
        this.status = status;
        this.chats = chats;
    }
}