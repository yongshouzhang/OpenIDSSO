using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenIDSSO.Models
{
    [Serializable]
    public class UserModel
    {
        public static implicit operator Dictionary<string,string> (UserModel user)
        {
            return new Dictionary<string, string>()
            {
                {"id" ,user.ID.ToString()},
                {"username",user.UserName},
                {"email",user.Email },
                {"password",user.Password },
                {"gender", user.Gender.ToString() },
                {"birthday",user.Birthday.ToString() },
                {"regdate",user.RegDate.ToString() },
            };
        }

        public static explicit operator UserModel (Dictionary<string,string> dic)
        {
            dic = dic.ToDictionary(obj => obj.Key.ToLower(), obj => obj.Value);
            string[] keys = new string[] { "id", "username","password", "email", "gender", "birthday", "regdate" };
            if(dic.Keys.Select(obj=>obj.ToLower()).Intersect(keys).Count() != keys.Length)
            {
                return null;
            }
            return new UserModel
            {
                ID = int.Parse(dic["id"]),
                UserName = dic["username"],
                Email = dic["username"],
                RegDate =  DateTime.Parse(dic["regdate"]),
                Birthday =  DateTime.Parse(dic["birthday"]),
                Password = dic["password"],
                Gender = char.Parse(dic["gender"])
            };
        }
        public int ID { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public DateTime Birthday { get; set; }

        public char Gender { get; set; }

        public DateTime RegDate { get; set; }
    }
}