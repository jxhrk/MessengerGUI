using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using messenger_lib;

namespace messenger_gui
{
    public class Messenger
    {
        public enum ApiRequest
        {
            OpenSession, GetChats, Auth, Register, GetUserInfo, Pulse, Push, Pull, GetChat, CreateChat
        }

        public MessengerApi messengerApi = new MessengerApi();

        private const string ServerAddress = "https://___/api/v0/";

        public string session;
        public string token;
        public string nickname { get; set; }

        Dictionary<string,string> SessionHeader = new();
        Dictionary<string,string> TokenSessionHeader = new();

        private string ApiReqToURL(ApiRequest apir)
        {
            switch (apir)
            {
                case ApiRequest.OpenSession: return "session/open";
                case ApiRequest.Auth: return "user/auth";
                case ApiRequest.Register: return "user/register";
                case ApiRequest.GetUserInfo: return "user/info";
                case ApiRequest.GetChats: return "client/chat/get";
                case ApiRequest.Pulse: return "session/pulse";
                case ApiRequest.Push: return "client/messages/push";
                case ApiRequest.Pull: return "client/messages/pull";
                case ApiRequest.GetChat: return "client/chat/get";
                case ApiRequest.CreateChat: return "client/chat/create";
                default: return "";
            }
        }

        private void CheckStatusCode(int code)
        {
            //int code = int.Parse(requestResponse.GetValue("status").ToString());
            switch (code)
            {
                case 100: // Success
                    break;
                case 101: // NullParameter
                case 102: // UsernameFormat
                case 103: // PasswordFormat
                    throw new Exception("Поля заполнены неверно");
                case 104: // UserNotExists
                case 105: // PasswordIncorrect
                    throw new Exception("Неверный логин или пароль");
                case 109: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 110: // Success
                    break;
                case 111: // NullParameter
                case 112: // UsernameFormat
                case 113: // PasswordFormat
                    throw new Exception("Поля заполнены неверно");
                case 114: // UserAlreadyExists
                    throw new Exception("Пользователь уже существует");
                case 115: // NicknameFormat
                    throw new Exception("Формат ника");
                case 116:
                    break; // need 2FA
                case 119: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 120: // Success
                    break;
                case 121: // NullParameter
                    throw new Exception("Нулевой параметр");
                case 122: // NoAuth
                    throw new Exception("Не авторизован");
                case 129: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 130: // Success
                    break;
                case 131: // NullParameter
                    throw new Exception("Нулевой параметр");
                case 132: // NoAuth
                    throw new Exception("Не авторизован");
                case 133: // NicknameFormat
                    throw new Exception("Неправильно указан никнейм");
                case 134: // NicknameBusy
                    throw new Exception("Никнейм занят");
                case 139: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 140: // Success
                    break;
                case 141: // NullParameter
                    throw new Exception("Нулевой параметр");
                case 142: // NoAuth
                    throw new Exception("Не авторизован");
                case 143: // TOTPIncorrect
                    throw new Exception("Код неверный");
                case 144: // TOTPNotEnabled
                    throw new Exception("2FA не подключен");
                case 149: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 150: // Success
                    break;
                case 151: // NullParameter
                    throw new Exception("Нулевой параметр");
                case 152: // NoAuth
                    throw new Exception("Не авторизован");
                case 153: // TOTPIncorrect
                    throw new Exception("Код неверный");
                case 154: // TOTPNotEnabled
                    throw new Exception("2FA не подключен");
                case 159: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 160: // Success
                    break;
                case 161: // NullParameter
                    throw new Exception("Нулевой параметр");
                case 162: // NoAuth
                    throw new Exception("Не авторизован");
                case 163: // TOTPIncorrect
                    throw new Exception("Код неверный");
                case 164: // TOTPNotEnabled
                    throw new Exception("2FA не подключен");
                case 169: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 200: // Success
                    break;
                case 201: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 202: //NoAuth
                    throw new Exception($"Не авторизован");
                case 203: //TextLenght
                    break;
                    //throw new Exception($"Некорректная длина сообщения");
                case 204: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 205: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 206: // NoPermission
                    throw new Exception($"Недостаточно прав");
                case 209: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 210: // Success
                    break;
                case 211: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 212: // NoAuth
                    throw new Exception($"Не авторизован");
                case 213: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 214: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 219: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 220: // Success
                    break;
                case 221: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 222: // NoAuth
                    throw new Exception($"Не авторизован");
                case 223: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 224: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 225: // MessageNotFound
                    throw new Exception($"Сообщение не найдено");
                case 226: // NoPermission
                    throw new Exception($"Нет прав");
                case 229: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 230: // Success
                    break;
                case 231: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 232: // NoAuth
                    throw new Exception($"Не авторизован");
                case 233: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 234: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 235: // MessageNotFound
                    throw new Exception($"Сообщение не найдено");
                case 236: // NoPermission
                    throw new Exception($"Нет прав");
                case 237: // TextLenght
                    throw new Exception($"Некорректная длина сообщения");
                case 239: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 300: // Success
                    break;
                case 301: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 302: // NoAuth
                    throw new Exception($"Не авторизован");
                case 309: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 310: // Success
                    break;
                case 311: // NullParameter
                    throw new Exception("Нулевой параметр");
                case 312: // NoAuth
                    throw new Exception("Пользователь не авторизован");
                case 313: // TitleFormat
                    throw new Exception("Неверный формат названия");
                case 314: // DescriptionFormat
                    throw new Exception("Неверный формат описания");
                case 319: // InternalError
                    throw new Exception("Внутренняя ошибка");

                case 320: // Success
                    break;
                case 321: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 322: // NoAuth
                    throw new Exception($"Не авторизован");
                case 323: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 324: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 325: // NoPermission
                    throw new Exception($"Нет прав");
                case 329: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 330: // Success
                    break;
                case 331: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 332: // NoAuth
                    throw new Exception($"Не авторизован");
                case 333: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 334: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 335: // NoPermission
                    throw new Exception($"Нет прав");
                case 339: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 340: // Success
                    break;
                case 341: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 342: // NoAuth
                    throw new Exception($"Не авторизован");
                case 343: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 344: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 349: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 350: // Success
                    break;
                case 351: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 352: // NoAuth
                    throw new Exception($"Не авторизован");
                case 353: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 354: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 355: // NoPermission
                    throw new Exception($"Нет прав");
                case 357: // TitleFormat
                    throw new Exception($"Неправильно указано название");
                case 358: // DescriptionFormat
                    throw new Exception($"Неправильно указано описание");
                case 359: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                case 360: // Success
                    break;
                case 361: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 362: // NoAuth
                    throw new Exception($"Не авторизован");
                case 363: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 364: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 365: // UserNotFound
                    throw new Exception($"Пользователь не существует");
                case 367: // AlradyInGroup
                    throw new Exception($"Пользователь уже есть в чате");
                case 368: // NoPermission
                    throw new Exception($"Нет прав на добавление пользователей");
                case 369: // InternalError
                    throw new Exception($"Внутренняя ошибка");


                case 370: // Success
                    break;
                case 371: // NullParameter
                    throw new Exception($"Нулевой параметр");
                case 372: // NoAuth
                    throw new Exception($"Не авторизован");
                case 373: // ChatNotExist
                    throw new Exception($"Чат не существует");
                case 374: // ChatNoAccess
                    throw new Exception($"Нет доступа к чату");
                case 375: // UserNotFound
                    throw new Exception($"Пользователь не существует");
                case 377: // UserNotInGroup
                    throw new Exception($"Пользователя нет в чате");
                case 378: // NoPermission
                    throw new Exception($"Нет прав на добавление пользователей");
                case 379: // InternalError
                    throw new Exception($"Внутренняя ошибка");

                default:
                    throw new Exception($"Неизвестная ошибка ({code})");
            }
        }

        private JObject ToJson(string jsonString)
        {
            return (JObject)JsonConvert.DeserializeObject(jsonString);
        }

        public async Task SessionOpen()
        {
            messengerApi.InitConnection();
            //session = await RequestApiString(ApiRequest.OpenSession, "{}", new Dictionary<string, string>(), Method.Get);
            //SessionHeader = new() {{ "session", session }};
        }

        public async Task Registration(string username, string nick, string password)
        {

            RegisterResult result = await messengerApi.Register(new Credentials(username, password));
            CheckStatusCode(result.status);

            await SetUserInfo(nick);
            await GetUserInfo();
        }


        public async Task GetUserInfo()
        {
            UserInfoResult result = await messengerApi.GetUserInfo();

            CheckStatusCode(result.status);

            UserInfoObj user = result.info;

            nickname = user.nickname;
        }

        public async Task SetUserInfo(string nick)
        {
            UserInfoObj user = new UserInfoObj();
            user.nickname = nick;
            SetUserInfoResult result = await messengerApi.SetUserInfo(user);

            CheckStatusCode(result.status);
        }


        public async Task<bool> Authorization(string username, string password)
        {
            AuthResult result = await messengerApi.Auth(new Credentials(username, password));

            if (result.status == 106)
            {
                return false;
                //Console.Write("Enter code: ");
                //var code = Console.ReadLine();
                //var twfaauth = api.Auth2FA(code);
                //Console.WriteLine($"[CLI] TOTP: {twfaauth.status} Token: {twfaauth.token}");
            }

            CheckStatusCode(result.status);

            await GetUserInfo();

            return true;
        }

        public async Task Authorization2FA(string code)
        {
            ConfirmAuth2FAResult result = await messengerApi.Auth2FA(code);

            CheckStatusCode(result.status);

            await GetUserInfo();
        }
        public async Task CreateNewChat(string title, string description, string secondNick)
        {
            CreateChatResult result = await messengerApi.CreateChat(title, description, new string[] { secondNick });
            CheckStatusCode(result.status);
        }
        public async Task<List<Chat>> GetAllChats()
        {
            GetUserChatsResult result = await messengerApi.GetChats();
            CheckStatusCode(result.status);
            List<Chat> chats = result.chats.ToList();

            foreach (Chat chat in chats)
            {
                List<Message> chatMessages = await GetChatMessages(chat.chatid, 1);
                if (chatMessages.Count > 0)
                {
                    Message lastMsg = chatMessages.First();
                    chat.ChatPreviewMsg = lastMsg.text;
                    chat.ChatLastMessageTime = lastMsg.TimeStr;
                    chat.LastMsgTime = lastMsg.sentdate;
                }
                else
                {
                    chat.ChatPreviewMsg = "(Чат создан)";
                    chat.LastMsgTime = chat.creationdate;
                    chat.ChatLastMessageTime = chat.creationdate.ToLocalTime().ToShortTimeString();
                }
            }

            return chats;
        }

        public async Task<List<Message>> GetChatMessages(int chatid, int count = 100)
        {
            MessagePullResult result = await messengerApi.PullMessages(chatid, count);
            CheckStatusCode(result.status);

            List<Message> messages = result.messages.ToList();

            foreach (Message msg in messages)
            {
                msg.IsYourMessage = msg.sender == nickname;
                msg.TimeStr = msg.sentdate.ToLocalTime().ToShortTimeString();
            }

            return messages;
        }

        public async Task<ChatInfoObj> GetChatInfo(int chatid)
        {
            ChatInfoResult result = await messengerApi.GetChatInfo(chatid);
            CheckStatusCode(result.status);
            return result.info;
        }

        public async Task SetChatInfo(int chatid, string title, string desc)
        {
            SetChatInfoResult result = await messengerApi.SetChatInfo(chatid, title, desc);
            CheckStatusCode(result.status);
        }

        public async Task AddChatUser(int chatid, string user)
        {
            AddUserResult result = await messengerApi.AddUserChat(chatid, user);
            CheckStatusCode(result.status);
        }

        public async Task RemoveChatUser(int chatid, string user)
        {
            RemoveUserResult result = await messengerApi.RemoveUserChat(chatid, user);
            CheckStatusCode(result.status);
        }

        public async Task PushMessage(string text, int id)
        {
            MessagePushResult result = await messengerApi.PushMessage(id, text);
            CheckStatusCode(result.status);
        }

        public async Task DeleteMessage(int chatid, int messageid)
        {
            RemoveMessageResult result = await messengerApi.RemoveMessage(chatid, messageid);
            CheckStatusCode(result.status);
        }

        public async Task EditMessage(int chatid, int messageid, string text)
        {
            EditMessageResult result = await messengerApi.EditMessage(chatid, messageid, text);
            CheckStatusCode(result.status);
        }

        public async Task ClearChat(int chatid)
        {
            ClearChatResult result = await messengerApi.ClearChat(chatid);
            CheckStatusCode(result.status);
        }

        public async Task DeleteChat(int chatid)
        {
            RemoveChatResult result = await messengerApi.RemoveChat(chatid);
            CheckStatusCode(result.status);
        }

    }
}
