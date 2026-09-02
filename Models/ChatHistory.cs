using System;

namespace PortfoyTakipAPI.Models
{
    public class ChatHistory
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}