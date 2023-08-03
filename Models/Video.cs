using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTube_Backend.Models
{
    public class VideoModel
    {
        public int Id { get; set; }
        public string? VideoPath { get; set; }
        public string? VideoPicturePath { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? DateUploaded { get; set; }
        public int? TotalViews { get; set; }
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }
        public int? TotalComments { get; set; }
        public string? Category { get; set; }
        public string? DateEdited { get; set; }
        public string? VideoId { get; set; }

    }


        public class VideoModelDto
    {
        public int Id { get; set; }
        public IFormFile? VideoFile { get; set; }
        public IFormFile? VideoPictureFile { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? DateUploaded { get; set; }
        public int? TotalViews { get; set; }
        public int? Likes { get; set; }
        public int? Dislikes { get; set; }
        public int? TotalReactions { get; set; }
        public int? TotalComments { get; set; }
        public string? Category { get; set; }
        public string? DateEdited { get; set; }

    }

    public class Like{
        public int Id { get; set; }
        public string? VideoId { get; set; }
        public string? VideoName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int? LikedVideo { get; set; }
        public string? DateLiked { get; set; }
    }


    public class DisLike{
        public int Id {get; set;}
        public string? VideoId { get; set; }
        public string? VideoName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int? DisLikedVideo { get; set; }
        public string? DateDisLiked { get; set; }
    }
    public class VideoComment{
        public int Id {get; set;}
        public string? VideoId { get; set; }
         public string? VideoName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? theComments { get; set; }
        public string? DateofComment { get; set; }
        public string? CommentId { get; set; }

    }

    public class VideoCommentsReply{
        public int Id {get; set;}
        public string? VideoId { get; set; }
         public string? VideoName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserComments { get; set; }
        public string? DateofUserComment { get; set; }
        public string? AdminId { get; set; }
        public string? AdminName { get; set; }
        public string? AdminReply { get; set; }
        public string? AdminReplyDate { get; set; }
        public string? ReplyId { get; set; }

    }

    public class EmailRequest{
        public string SmtpHost = "localhost";
        public int SmtpPort = 8000;
        public string SmtpUserName = "Solomon Danso";
        public string SmtpPassword = "The Password";

    }
    
}