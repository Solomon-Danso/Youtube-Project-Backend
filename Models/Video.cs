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
        public string? Category { get; set; }
        public string? DateEdited { get; set; }

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
        public string? Comments { get; set; }
        public string? Category { get; set; }
        public string? DateEdited { get; set; }

    }

    public class Like{
        public int Id { get; set; }
        public int? VideoId { get; set; }
        public string? VideoName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int? LikedVideo { get; set; }
        public string? DateLiked { get; set; }
    }


    public class DisLike{
        public int Id {get; set;}
        public int? VideoId { get; set; }
        public string? VideoName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int? DisLikedVideo { get; set; }
        public string? DateLiked { get; set; }
    }
    
}