using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskIt.Models
{
    public class ChatUser
    {
        public ChatUser(string group)
        {
            CurrentGroup = group;
        }

        public ChatUser()
            : this("Lobby")
        {

        }

        public string inputGroups { get; set; }

        public List<string> chatGroup { get; set; }
        public string CurrentGroup { get; set; }

        public string userLogin { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}