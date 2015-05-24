using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AskIt.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.SignalR;


namespace AskIt.Hubs
{
    public class ChatHub : Hub
    {
        static List<ChatUser> ChatUsers = new List<ChatUser>();

        // Отправка сообщений
        public void Send(string name, string message)
        {
            name = Context.User.Identity.Name;

            Clients.All.addMessage(name, message);
        }

        // Подключение нового пользователя
        public void Connect(string userName)
        {
            var id = Context.ConnectionId;


            if (ChatUsers.Count(x => x.connectionId == id) == 0)
            {
                ChatUsers.Add(new ChatUser { connectionId = id, userLogin = userName });

                // Посылаем сообщение текущему пользователю
                Clients.Caller.onConnected(id, userName, ChatUsers);

                // Посылаем сообщение всем пользователям, кроме текущего
                Clients.AllExcept(id).onNewUserConnected(id, userName);
            }
        }

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled)
        {
            var item = ChatUsers.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
            if (item != null)
            {
                ChatUsers.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.userLogin);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}