using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using webapi.Models;

namespace webapi.Hubs
{
    public class ChatHub : Hub
    {
        private static int OnlineAdminCount { get; set; } = 0;
        private readonly BumpContext _context;

        public ChatHub(BumpContext context)
        {
            _context = context;
        }

        public async Task FetchHistoryMessage(string username)
        {
            var messages = await _context.Conversations
                .Include(c => c.Member)
                .Where(c => c.Member.Account == username)
                .OrderBy(c => c.SendTime)
                .Select(m => new
                {
                    m.Message,
                    m.IsSelf,
                    m.SendTime
                })
                .ToListAsync();
                

            var json = JsonSerializer.Serialize(messages, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

            await Clients.Caller.SendAsync("HistoryMessagesReceived", json, username);
        }

        public async Task SendMessageToAdmin(string username, string message)
        {
            var entity = await _context.Conversations.AddAsync(new Conversation
            {
                MemberId = int.Parse(Context.User.FindFirst("memberId").Value),
                Message = message,
                IsSelf = true
            });
            await _context.SaveChangesAsync();

            await Clients.Caller.SendAsync("messageReceived", username, message, entity.Entity.SendTime);
            await Clients.Group("Admin").SendAsync("messageReceivedFromUser", username, message, Context.ConnectionId, entity.Entity.SendTime);
        }

        public async Task SendMessageToUser(string connectionId, string message, string username)
        {
            var userId = (await _context.Members.AsQueryable().SingleAsync(m => m.Account == username)).Id;
            var entity = await _context.Conversations.AddAsync(new Conversation
            {
                MemberId = userId,
                Message = message,
                IsSelf = false
            });
            await _context.SaveChangesAsync();
            
            await Clients.Group("Admin").SendAsync("messageReceivedFromAdmin", connectionId, message, entity.Entity.SendTime);
            await Clients.Group(connectionId).SendAsync("messageReceived", Guid.NewGuid().ToString(), message, entity.Entity.SendTime);
        }

        public override async Task OnConnectedAsync()
        {
            if (Context?.User?.Identity?.IsAuthenticated == false)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admin");
                OnlineAdminCount++;
            }
            else
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Context.ConnectionId);
            }

            if (OnlineAdminCount == 0)
            {
                await Clients.Caller.SendAsync("NoAdminAlive");
            }
            else
            {
                await Clients.All.SendAsync("AdminAlive");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context?.User?.Identity?.IsAuthenticated == false)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admin");
                OnlineAdminCount--;
            }
            else
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.ConnectionId);
            }

            if (OnlineAdminCount == 0)
            {
                await Clients.All.SendAsync("NoAdminAlive");
            }

            await base.OnDisconnectedAsync(exception);
        }

    }
}
