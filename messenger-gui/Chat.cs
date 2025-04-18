using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection.PortableExecutable;

namespace messenger_gui;

public class ChatOld
{
    public string title { get; set; }
    public string description { get; set; }
    public string ChatPreviewMsg { get; set; }
    public string ChatLastMessageTime { get; set; }
    public DateTime LastMsgTime { get; set; }
    public int chatid { get; set; }
    public int creatorid { get; set; }
    public DateTime creationdate { get; set; }
    public string[] users { get; set; }
    public bool isuser { get; set; }
    public bool isgroup { get; set; }

    public ChatOld(string title, string chatPreviewMsg)
    {
        this.title = title;
        ChatPreviewMsg = chatPreviewMsg;
        ChatLastMessageTime = "14:32";
    }

    //public Chat(string JsonText)
    //{

    //    using (JsonTextReader reader = new JsonTextReader(new StringReader(JsonText)))
    //    {
    //        JObject o2 = (JObject)JToken.ReadFrom(reader);
    //        o2.ToObject<Chat>();
    //    }
    //}
}