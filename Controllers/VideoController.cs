using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTube_Backend.Data;
using Microsoft.AspNetCore.Mvc;
using YouTube_Backend.Models;

namespace YouTube_Backend.Controllers
{
    [ApiController]
    [Route("api/Video")]
    public class VideoController : ControllerBase
    {
        private readonly DataContext context;
        public VideoController(DataContext ctx){
            context = ctx;
        }
Roles role = new Roles();
        [HttpPost("AddVideo")]
    public async Task<IActionResult> AddVideo([FromForm]VideoModelDto request, string AdminId){
        //isAdmin Login Credentials Before you can upload video
        var admin = context.AdminAccounts.FirstOrDefault(a=>a.AdminId == AdminId);
        if(admin == null){
            return BadRequest("You dont have the permission to access this page");
        }

 if (request.VideoFile == null || request.VideoFile.Length == 0)
    {
        return BadRequest("Invalid file");
    }

    // Create the uploads directory if it doesn't exist
    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", "VideoFiles");
    if (!Directory.Exists(uploadsDirectory))
    {
        Directory.CreateDirectory(uploadsDirectory);
    }

    // Get the original file extension
    var fileExtension = Path.GetExtension(request.VideoFile.FileName);

    // Generate a unique file name
    var fileName = Guid.NewGuid().ToString() + fileExtension;

    // Save the uploaded file to the uploads directory
    var filePath = Path.Combine(uploadsDirectory, fileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await request.VideoFile.CopyToAsync(stream);
    }


if (request.VideoPictureFile == null )
    {
        return Ok("Try choosing a picture");
    }

    // Create the uploads directory if it doesn't exist
    var uploadsPictureDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", "VideoPictureFiles");
    if (!Directory.Exists(uploadsPictureDirectory))
    {
        Directory.CreateDirectory(uploadsPictureDirectory);
    }

    // Get the original file extension
    var filePictureExtension = Path.GetExtension(request.VideoPictureFile.FileName);

    // Generate a unique file name
    var filePictureName = Guid.NewGuid().ToString() + filePictureExtension;

    // Save the uploaded file to the uploads directory
    var filePicturePath = Path.Combine(uploadsPictureDirectory, filePictureName);
    using (var stream = new FileStream(filePicturePath, FileMode.Create))
    {
        await request.VideoPictureFile.CopyToAsync(stream);
    }

    var newVideo = new VideoModel{
        VideoPath = Path.Combine("Videos/VideoFiles", fileName),
        VideoPicturePath = Path.Combine("Videos/VideoPictureFiles", fileName),
        Title = request.Title,
        Description = request.Description,
        DateUploaded = DateTime.Today.Date.ToString("dd MMMM,yyyy"),
        TotalViews = 0,
        Likes = 0,
        Dislikes = 0,
        Category = request.Category,
        DateEdited = DateTime.Today.Date.ToString("dd MMMM,yyyy"),

    };

    context.VideoModels.Add(newVideo);
    await context.SaveChangesAsync();


return Ok("Video Uploaded successfully");
    }



        [HttpPost("EditVideo")]
    public async Task<IActionResult> EditVideo([FromForm]VideoModelDto request, string AdminId, int videoId){
        //isAdmin Login Credentials Before you can upload video
        var admin = context.AdminAccounts.FirstOrDefault(a=>a.AdminId == AdminId);
        if(admin == null){
            return BadRequest("You dont have the permission to access this page");
        }

 if (request.VideoFile == null || request.VideoFile.Length == 0)
    {
        return BadRequest("Invalid file");
    }

    // Create the uploads directory if it doesn't exist
    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", "VideoFiles");
    if (!Directory.Exists(uploadsDirectory))
    {
        Directory.CreateDirectory(uploadsDirectory);
    }

    // Get the original file extension
    var fileExtension = Path.GetExtension(request.VideoFile.FileName);

    // Generate a unique file name
    var fileName = Guid.NewGuid().ToString() + fileExtension;

    // Save the uploaded file to the uploads directory
    var filePath = Path.Combine(uploadsDirectory, fileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await request.VideoFile.CopyToAsync(stream);
    }


if (request.VideoPictureFile == null )
    {
        return Ok("Try choosing a picture");
    }

    // Create the uploads directory if it doesn't exist
    var uploadsPictureDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Videos", "VideoPictureFiles");
    if (!Directory.Exists(uploadsPictureDirectory))
    {
        Directory.CreateDirectory(uploadsPictureDirectory);
    }

    // Get the original file extension
    var filePictureExtension = Path.GetExtension(request.VideoPictureFile.FileName);

    // Generate a unique file name
    var filePictureName = Guid.NewGuid().ToString() + filePictureExtension;

    // Save the uploaded file to the uploads directory
    var filePicturePath = Path.Combine(uploadsPictureDirectory, filePictureName);
    using (var stream = new FileStream(filePicturePath, FileMode.Create))
    {
        await request.VideoPictureFile.CopyToAsync(stream);
    }

    var theVideo = context.VideoModels.FirstOrDefault(x=>x.Id == videoId);
    if (theVideo == null){
        return BadRequest("Video not found");
    }

    
        theVideo.VideoPath = Path.Combine("Videos/VideoFiles", fileName);
        theVideo.VideoPicturePath = Path.Combine("Videos/VideoPictureFiles", fileName);
        theVideo.Title = request.Title;
        theVideo.Description = request.Description;
        theVideo.DateUploaded = DateTime.Today.Date.ToString("dd MMMM,yyyy");
        theVideo.Category = request.Category;
        theVideo.DateEdited = DateTime.Today.Date.ToString("dd MMMM,yyyy");

   
    await context.SaveChangesAsync();


return Ok("Video Uploaded successfully");
    }


[HttpGet("ViewVideo")]
public async Task<IActionResult> ViewVideo(int videoId){
    var video = context.VideoModels.FirstOrDefault(v=>v.Id==videoId);
    if(video==null){
        return BadRequest("Video does not exist");
    }

    video.TotalViews = video.TotalViews+1;
    await context.SaveChangesAsync();
    return Ok(video);
}





[HttpPost("Likes")]
public async Task<IActionResult> Likes(int VideoId, string UserId ){

    var theUser = context.UserAccounts.FirstOrDefault(a => a.UserId == UserId);
    if (theUser == null){
        return BadRequest("User Account Not Found");
    }

    var video = context.VideoModels.FirstOrDefault(x => x.Id == VideoId);
    if (video == null){
        return BadRequest("Video not found");
    }
    
var theLikes = new Like{
VideoId = video.Id,
VideoName = video.Title,
UserId = theUser.UserId,
UserName = theUser.FullName,
LikedVideo = 1,
DateLiked = DateTime.Today.Date.ToString("dd MMMM, yyyy"),

};

var checker = context.Likes.FirstOrDefault(x=>x.VideoId==theLikes.VideoId&&x.VideoName==theLikes.VideoName&&x.UserId==theUser.UserId);
if(checker != null){
    return BadRequest("You have already liked this video");
}
var checker2 = context.DisLikes.FirstOrDefault(x=>x.VideoId==video.Id&&x.UserId==theUser.UserId);
if (checker2 != null){
    context.DisLikes.Remove(checker2);
}


    context.Likes.Add(theLikes);
    await context.SaveChangesAsync();
    return Ok("You Liked This Video");
}

[HttpGet("TotalLikes")]
public async Task<IActionResult> GetTotalLikes(int videoId){
    var totalLikes = context.Likes.Where(x=>x.VideoId==videoId).Sum(r=>r.LikedVideo);
    var video = context.VideoModels.FirstOrDefault(x=>x.Id==videoId);
    if (video == null){
        return BadRequest("Video Not Found");
    }
    video.Likes = totalLikes;
    await context.SaveChangesAsync();
    return Ok(totalLikes);

}


[HttpPost("DisLikes")]
public async Task<IActionResult> DisLikes(int VideoId, string UserId ){

    var theUser = context.UserAccounts.FirstOrDefault(a => a.UserId == UserId);
    if (theUser == null){
        return BadRequest("User Account Not Found");
    }

    var video = context.VideoModels.FirstOrDefault(x => x.Id == VideoId);
    if (video == null){
        return BadRequest("Video not found");
    }
    
var theDisLikes = new DisLike{
VideoId = video.Id,
VideoName = video.Title,
UserId = theUser.UserId,
UserName = theUser.FullName,
DisLikedVideo = 1,
DateLiked = DateTime.Today.Date.ToString("dd MMMM, yyyy"),

};

var checker = context.DisLikes.FirstOrDefault(x=>x.VideoId==theDisLikes.VideoId&&x.VideoName==theDisLikes.VideoName&&x.UserId==theUser.UserId);
if(checker != null){
    return BadRequest("You have already liked this video");
}
var checker2 = context.Likes.FirstOrDefault(x=>x.VideoId==video.Id&&x.UserId==theUser.UserId);
if (checker2 != null){
    context.Likes.Remove(checker2);
}

    context.DisLikes.Add(theDisLikes);
    await context.SaveChangesAsync();
    return Ok("You have disliked this video");
}

[HttpGet("TotalDisLikes")]
public async Task<IActionResult> GetTotalDisLikes(int videoId){
    var totalDisLikes = context.DisLikes.Where(x=>x.VideoId==videoId).Sum(r=>r.DisLikedVideo);
    
    var video = context.VideoModels.FirstOrDefault(x=>x.Id==videoId);
    if (video == null){
        return BadRequest("Video Not Found");
    }
    video.Likes = totalDisLikes;
    await context.SaveChangesAsync();
    
    return Ok(totalDisLikes);

}





    }
}