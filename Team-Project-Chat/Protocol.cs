namespace Protocol
{
    public enum MessageType
    {
        Login,
        Register,
        Chat,
        UserList,
        Disconnect,
        PrivateMessage,
        UserListRequest,
        MessageHistory,
        MessageHistoryRequest
    }

    public class NetworkMessage
    {
        public MessageType Type { get; set; }
        public string Data { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
    }

    public class ChatMessage
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public string Timestamp { get; set; }
        public string Recipient { get; set; }
    }

    public class UserListResponse
    {
        public List<string> Users { get; set; } = new List<string>();
    }

    public class MessageHistoryRequest
    {
        public string Username1 { get; set; }
        public string Username2 { get; set; }
    }

    public class MessageHistoryResponse
    {
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}