using System;

public class ChatInfoObj {
    public string Title { get; set; }
    public string Description { get; set; }
    public string[] Users { get; set; }
    public int? Messages { get; set; }
    public int? Creatorid { get; set; }
    public DateTime? Creationdate { get; set; }
}