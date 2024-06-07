using Microsoft.AspNetCore.SignalR;

namespace Together.Service;

public class ChatHub : Hub
{
    private readonly ChatService _chatService;

    public ChatHub(ChatService chatService)
    {
        _chatService = chatService;
    }
/*
    public async Task SendMessage(int chatRoomId, string userId, string message)
    {
        var chatMessage = await _chatService.SaveMessage(chatRoomId, userId, message);
        await Clients.Group(chatRoomId.ToString()).SendAsync("ReceiveMessage", chatMessage);
    }
*/
    public async Task JoinRoom(int chatRoomId, string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId.ToString());
        await _chatService.AddUserToRoom(chatRoomId, userId);
    }
}