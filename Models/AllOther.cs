using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTube_Backend.Models
{
    public class UploadVideoNotification
    {
        public int Id { get; set; }
        public string? VideoId { get; set; }
        public string? Title { get; set; }
        public string? VideoUrl { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? DateOfNotification { get; set; }

    }
    public class Message{
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserMessage { get; set; }
        public string? DateOfMessage { get; set; }

    }

}