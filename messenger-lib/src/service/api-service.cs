using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace messenger_lib;

public class MessengerApi {
    public const string SERVER_HOST = "http://127.0.0.1:8080";

    public string session { get; private set; } = "";
    public string token { get; private set; } = "";

    private HttpClient http = new HttpClient();
    private CancellationTokenSource cts;
    private Task loopTask;

    private void _connectionLoop() {
        _openSession();

        while (!cts.IsCancellationRequested) {
            Task.Delay(5000).Wait();
            var ok = _pulseSession();
            if (!ok) _openSession();
        }
    }
    public void InitConnection() {
        cts = new CancellationTokenSource();
        loopTask = Task.Run(_connectionLoop, cts.Token);
    }
    public bool CloseConnection() {
        if (cts == null) return false;
        if (loopTask == null) return false;

        cts.Cancel();
        return true;
    }

    public async Task<AuthResult> Auth(Credentials creds) {
        token = "";
        http.DefaultRequestHeaders.Remove("token");

        string strIn = JsonConvert.SerializeObject(creds);
        string strOut = await _post("/api/v0/user/auth", strIn, MediaTypeNames.Application.Json);
        AuthResult res = JsonConvert.DeserializeObject<AuthResult>(strOut);
        token = res.ok ? res.token : "";
        if (res.ok) http.DefaultRequestHeaders.Add("token", token);
        return res;
    }
    public async Task<RegisterResult> Register(Credentials creds) {
        token = "";
        http.DefaultRequestHeaders.Remove("token");

        string strIn = JsonConvert.SerializeObject(creds);
        string strOut = await _post("/api/v0/user/register", strIn, MediaTypeNames.Application.Json);
        RegisterResult res = JsonConvert.DeserializeObject<RegisterResult>(strOut);
        token = res.ok ? res.token : "";
        if (res.ok) http.DefaultRequestHeaders.Add("token", token);
        return res;
    }
    public async Task<UserInfoResult> GetUserInfo() {
        string strOut = await _post("/api/v0/user/info");
        return JsonConvert.DeserializeObject<UserInfoResult>(strOut);
    }
    public async Task<SetUserInfoResult> SetUserInfo(UserInfoObj info) {
        string strIn = JsonConvert.SerializeObject(info);
        string strOut = await _post("/api/v0/user/setinfo", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<SetUserInfoResult>(strOut);
    }
    public async Task<ConfirmAuth2FAResult> Auth2FA(string code)
    {
        token = "";
        http.DefaultRequestHeaders.Remove("token");

        string strIn = JsonConvert.SerializeObject(new { code });
        string strOut = await _post("/api/v0/user/2fa/auth", strIn, MediaTypeNames.Application.Json);
        var res = JsonConvert.DeserializeObject<ConfirmAuth2FAResult>(strOut);

        token = res.ok ? res.token : "";
        if (res.ok) http.DefaultRequestHeaders.Add("token", token);
        return res;
    }
    public async Task<Enable2FAResult> Enable2FA()
    {
        string strOut = await _post("/api/v0/user/2fa/enable");
        return JsonConvert.DeserializeObject<Enable2FAResult>(strOut);
    }
    public async Task<ConfirmEnable2FAResult> Enable2FAConfirm(string code)
    {
        string strIn = JsonConvert.SerializeObject(new { code });
        string strOut = await _post("/api/v0/user/2fa/confirm", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<ConfirmEnable2FAResult>(strOut);
    }
    public async Task<Disable2FAResult> Disable2FA(string code)
    {
        string strIn = JsonConvert.SerializeObject(new { code });
        string strOut = await _post("/api/v0/user/2fa/disable", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<Disable2FAResult>(strOut);
    }

    public async Task<MessagePullResult> PullMessages(int chatid, int count = 1, int? offset = null) {
        string strIn = JsonConvert.SerializeObject(new { chatid, options = new { offset, count } });
        string strOut = await _post("/api/v0/client/messages/pull", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<MessagePullResult>(strOut);
    }
    public async Task<MessagePushResult> PushMessage(int chatid, string text) {
        string strIn = JsonConvert.SerializeObject(new { chatid, text });
        string strOut = await _post("/api/v0/client/messages/push", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<MessagePushResult>(strOut);
    }
    public async Task<RemoveMessageResult> RemoveMessage(int chatid, int messageid) {
        string strIn = JsonConvert.SerializeObject(new { chatid, messageid });
        string strOut = await _post("/api/v0/client/messages/remove", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<RemoveMessageResult>(strOut);
    }
    public async Task<EditMessageResult> EditMessage(int chatid, int messageid, string text) {
        string strIn = JsonConvert.SerializeObject(new { chatid, messageid, text });
        string strOut = await _post("/api/v0/client/messages/edit", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<EditMessageResult>(strOut);
    }

    public async Task<CreateChatResult> CreateChat(string title, string description, string[] users) {
        string strIn = JsonConvert.SerializeObject(new { title, description, users });
        string strOut = await _post("/api/v0/client/chat/create", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<CreateChatResult>(strOut);
    }
    public async Task<GetUserChatsResult> GetChats() {
        string strOut = await _post("/api/v0/client/chat/get");
        return JsonConvert.DeserializeObject<GetUserChatsResult>(strOut);
    }
    public async Task<ClearChatResult> ClearChat(int chatid) {
        string strIn = JsonConvert.SerializeObject(new { chatid });
        string strOut = await _post("/api/v0/client/chat/clear", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<ClearChatResult>(strOut);
    }
    public async Task<RemoveChatResult> RemoveChat(int chatid) {
        string strIn = JsonConvert.SerializeObject(new { chatid });
        string strOut = await _post("/api/v0/client/chat/remove", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<RemoveChatResult>(strOut);
    }
    public async Task<ChatInfoResult> GetChatInfo(int chatid) {
        string strIn = JsonConvert.SerializeObject(new { chatid });
        string strOut = await _post("/api/v0/client/chat/info", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<ChatInfoResult>(strOut);
    }
    public async Task<SetChatInfoResult> SetChatInfo(int chatid, string title = "", string description = "") {
        string strIn = JsonConvert.SerializeObject(new { chatid, title, description });
        string strOut = await _post("/api/v0/client/chat/setinfo", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<SetChatInfoResult>(strOut);
    }
    public async Task<AddUserResult> AddUserChat(int chatid, string user) {
        string strIn = JsonConvert.SerializeObject(new { chatid, user });
        string strOut = await _post("/api/v0/client/chat/adduser", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<AddUserResult>(strOut);
    }
    public async Task<RemoveUserResult> RemoveUserChat(int chatid, string user) {
        string strIn = JsonConvert.SerializeObject(new { chatid, user });
        string strOut = await _post("/api/v0/client/chat/removeuser", strIn, MediaTypeNames.Application.Json);
        return JsonConvert.DeserializeObject<RemoveUserResult>(strOut);
    }


    private bool _openSession() {
        session = "";
        http.DefaultRequestHeaders.Remove("session");

        session = _get("/api/v0/session/open");
        http.DefaultRequestHeaders.Add("session", session);
        return session != "";
    }
    private bool _pulseSession() {
        var res = _get("/api/v0/session/pulse");
        return res == "1";
    }

    private string _get(string URL, string body = "", string type = "") {
        StringContent content = null;
        if (body != "") content = new StringContent(body, Encoding.UTF8, type);

        var cnt = new HttpRequestMessage {  
            Method = HttpMethod.Get,
            RequestUri = new Uri(SERVER_HOST + URL),
            Content = content
        };

        var res = http.Send(cnt);
        var t = res.Content.ReadAsStringAsync();
        t.Wait();

        if (res.StatusCode != HttpStatusCode.OK) throw new Exception($"[GET] FAULT CODE: {res.StatusCode} URL: {URL}");
        return t.Result;
    }
    private async Task<string> _post(string URL, string body = "", string type = "") {
        StringContent content = null;
        if (body != "") content = new StringContent(body, Encoding.UTF8, type);

        var cnt = new HttpRequestMessage {  
            Method = HttpMethod.Post,
            RequestUri = new Uri(SERVER_HOST + URL),
            Content = content
        };

        var res = await http.SendAsync(cnt);
        var t = await res.Content.ReadAsStringAsync();

        if (res.StatusCode != HttpStatusCode.OK) throw new Exception($"[GET] FAULT CODE: {res.StatusCode} URL: {URL}");
        return t;
    }

    public delegate void ApiEvent(object obj);
    public event ApiEvent OnError;
}
