using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace messenger_gui
{
    class User
    {
        public string username;
        public string password;
        public string nickname = "";

        public User(string username, string password, string nickname)
        {
            this.username = username;
            this.password = password;
            this.nickname = nickname;
        }

        public string GetJson()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}
