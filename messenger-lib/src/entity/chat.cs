using System;

namespace messenger_lib;

public class Chat {
    public int chatid { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public DateTime creationdate { get; set; }
    public int creatorid { get; set; }

    public bool isuser { get; set; }
    public bool isgroup { get; set; }
    public string[] users { get; set; }

    // UI

    public string ChatPreviewMsg { get; set; }
    public string ChatLastMessageTime { get; set; }
    public DateTime LastMsgTime { get; set; }
}