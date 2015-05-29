using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AskIt.Models
{
    public class ChatUser
    {
        public string userLogin { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
}