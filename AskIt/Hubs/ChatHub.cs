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
using System.Collections.Concurrent;


namespace AskIt.Hubs
{
    public class ChatHub : Hub
    {
        //static List<ChatUser> ChatUsers = new List<ChatUser>();

        //// Отправка сообщений
        //public void Send(string name, string message)
        //{
        //    name = Context.User.Identity.Name;

        //    Clients.All.addMessage(name, message);
        //}

        //// Подключение нового пользователя
        //public void Connect(string userName)
        //{
        //    var id = Context.ConnectionId;


        //    if (ChatUsers.Count(x => x.connectionId == id) == 0)
        //    {
        //        ChatUsers.Add(new ChatUser { connectionId = id, userLogin = userName });

        //        // Посылаем сообщение текущему пользователю
        //        Clients.Caller.onConnected(id, userName, ChatUsers);

        //        // Посылаем сообщение всем пользователям, кроме текущего
        //        Clients.AllExcept(id).onNewUserConnected(id, userName);
        //    }
        //}

        //// Отключение пользователя
        //public override Task OnDisconnected(bool stopCalled)
        //{
        //    var item = ChatUsers.FirstOrDefault(x => x.connectionId == Context.ConnectionId);
        //    if (item != null)
        //    {
        //        ChatUsers.Remove(item);
        //        var id = Context.ConnectionId;
        //        Clients.All.onUserDisconnected(id, item.userLogin);
        //    }

        //    return base.OnDisconnected(stopCalled);
        //}

        private static readonly ConcurrentDictionary<string, ChatUser> chatUsers
            = new ConcurrentDictionary<string, ChatUser>(StringComparer.InvariantCultureIgnoreCase);

        public void Send(string message)
        {

            string sender = Context.User.Identity.Name;

            // So, broadcast the sender, too.
            Clients.All.received(new { sender = sender, message = message, isPrivate = false });
        }

        public void Send(string message, string to)
        {

            ChatUser receiver;
            if (chatUsers.TryGetValue(to, out receiver))
            {

                ChatUser sender = GetUser(Context.User.Identity.Name);

                IEnumerable<string> allReceivers;
                lock (receiver.ConnectionIds)
                {
                    lock (sender.ConnectionIds)
                    {

                        allReceivers = receiver.ConnectionIds.Concat(sender.ConnectionIds);
                    }
                }

                foreach (var cid in allReceivers)
                {
                    Clients.Client(cid).received(new { sender = sender.userLogin, message = message, isPrivate = true });
                }
            }
        }

        public IEnumerable<string> GetConnectedUsers()
        {

            return chatUsers.Where(x =>
            {

                lock (x.Value.ConnectionIds)
                {

                    return !x.Value.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
                }

            }).Select(x => x.Key);
        }

        public override Task OnConnected()
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            var user = chatUsers.GetOrAdd(userName, _ => new ChatUser
            {
                userLogin = userName,
                ConnectionIds = new HashSet<string>()
            });

            lock (user.ConnectionIds)
            {

                user.ConnectionIds.Add(connectionId);

                // // broadcast this to all clients other than the caller
                // Clients.AllExcept(user.ConnectionIds.ToArray()).userConnected(userName);

                // Or you might want to only broadcast this info if this 
                // is the first connection of the user
                if (user.ConnectionIds.Count == 1)
                {

                    Clients.Others.userConnected(userName);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {

            string userName = Context.User.Identity.Name;
            string connectionId = Context.ConnectionId;

            ChatUser user;
            chatUsers.TryGetValue(userName, out user);

            if (user != null)
            {

                lock (user.ConnectionIds)
                {

                    user.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));

                    if (!user.ConnectionIds.Any())
                    {

                        ChatUser removedUser;
                        chatUsers.TryRemove(userName, out removedUser);

                        // You might want to only broadcast this info if this 
                        // is the last connection of the user and the user actual is 
                        // now disconnected from all connections.
                        Clients.Others.userDisconnected(userName);
                    }
                }
            }

            return base.OnDisconnected(stopCalled);
        }

        private ChatUser GetUser(string username)
        {

            ChatUser user;
            chatUsers.TryGetValue(username, out user);

            return user;
        }
    }
}