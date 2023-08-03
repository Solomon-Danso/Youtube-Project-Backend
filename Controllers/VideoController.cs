using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTube_Backend.Data;
using Microsoft.AspNetCore.Mvc;
using YouTube_Backend.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

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
Constants constant = new Constants();
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
        VideoId = IdGenerator()

    };
    var Link = constant.apiServer+newVideo.VideoPath;
    var users = context.UserAccounts.Where(u=>u.Role == role.NormalUser && u.EmailAddress !=null);
    foreach(var user in users){
        var notify = new UploadVideoNotification{
            VideoId = newVideo.VideoId,
            Title = newVideo.Title,
            VideoUrl = Link,
            UserId = user.UserId,
            UserName = user.FullName,
            DateOfNotification = DateTime.Today.Date.ToString("dd MMMM,yyyy")

        };
        context.UploadVideoNotifications.Add(notify);
    
    /*

        try
        {
            if(user.EmailAddress==null||user.FullName==null||newVideo.Title==null||newVideo.DateUploaded==null|| Link==null||admin.FullName==null){
                return BadRequest("All fields are required");
            }
           await VideoUploadNotification(user.EmailAddress, user.FullName, newVideo.Title, newVideo.DateUploaded,Link, admin.FullName);
 
        }
        catch (Exception)
        {  return BadRequest("Failed to send notification. Please try again later.");}
    */
       
    }


    context.VideoModels.Add(newVideo);
    await context.SaveChangesAsync();


return Ok("Video Uploaded successfully");
    }



        [HttpPost("EditVideo")]
    public async Task<IActionResult> EditVideo([FromForm]VideoModelDto request, string AdminId, string videoId){
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

    var theVideo = context.VideoModels.FirstOrDefault(x=>x.VideoId == videoId);
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
public async Task<IActionResult> ViewVideo(string videoId){
    var video = context.VideoModels.FirstOrDefault(v=>v.VideoId==videoId);
    if(video==null){
        return BadRequest("Video does not exist");
    }

    video.TotalViews = video.TotalViews+1;
    await context.SaveChangesAsync();
    return Ok(video);
}

[HttpGet("ViewAllVideo")]
public async Task<IActionResult> ViewAllVideo(){
    var video = context.VideoModels.OrderByDescending(v=>v.TotalViews).ToList();
    return Ok(video);
}




[HttpPost("Likes")]
public async Task<IActionResult> Likes(string VideoId, string UserId ){

    var theUser = context.UserAccounts.FirstOrDefault(a => a.UserId == UserId);
    if (theUser == null){
        return BadRequest("User Account Not Found");
    }

    var video = context.VideoModels.FirstOrDefault(x => x.VideoId == VideoId);
    if (video == null){
        return BadRequest("Video not found");
    }
    
var theLikes = new Like{
VideoId = video.VideoId,
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
var checker2 = context.DisLikes.FirstOrDefault(x=>x.VideoId==video.VideoId&&x.UserId==theUser.UserId);
if (checker2 != null){
    context.DisLikes.Remove(checker2);
}


    context.Likes.Add(theLikes);
    await context.SaveChangesAsync();
    return Ok("You Liked This Video");
}

[HttpGet("TotalLikes")]
public async Task<IActionResult> GetTotalLikes(string videoId){
    var totalLikes = context.Likes.Where(x=>x.VideoId==videoId).Sum(r=>r.LikedVideo);
    var video = context.VideoModels.FirstOrDefault(x=>x.VideoId==videoId);
    if (video == null){
        return BadRequest("Video Not Found");
    }
    video.Likes = totalLikes;
    await context.SaveChangesAsync();
    return Ok(totalLikes);

}


[HttpPost("DisLikes")]
public async Task<IActionResult> DisLikes(string VideoId, string UserId ){

    var theUser = context.UserAccounts.FirstOrDefault(a => a.UserId == UserId);
    if (theUser == null){
        return BadRequest("User Account Not Found");
    }

    var video = context.VideoModels.FirstOrDefault(x => x.VideoId == VideoId);
    if (video == null){
        return BadRequest("Video not found");
    }
    
var theDisLikes = new DisLike{
VideoId = video.VideoId,
VideoName = video.Title,
UserId = theUser.UserId,
UserName = theUser.FullName,
DisLikedVideo = 1,
DateDisLiked = DateTime.Today.Date.ToString("dd MMMM, yyyy"),

};

var checker = context.DisLikes.FirstOrDefault(x=>x.VideoId==theDisLikes.VideoId&&x.VideoName==theDisLikes.VideoName&&x.UserId==theUser.UserId);
if(checker != null){
    return BadRequest("You have already liked this video");
}
var checker2 = context.Likes.FirstOrDefault(x=>x.VideoId==video.VideoId&&x.UserId==theUser.UserId);
if (checker2 != null){
    context.Likes.Remove(checker2);
}

    context.DisLikes.Add(theDisLikes);
    await context.SaveChangesAsync();
    return Ok("You have disliked this video");
}

[HttpGet("TotalDisLikes")]
public async Task<IActionResult> GetTotalDisLikes(string videoId){
    var totalDisLikes = context.DisLikes.Where(x=>x.VideoId==videoId).Sum(r=>r.DisLikedVideo);
    
    var video = context.VideoModels.FirstOrDefault(x=>x.VideoId==videoId);
    if (video == null){
        return BadRequest("Video Not Found");
    }
    video.Likes = totalDisLikes;
    await context.SaveChangesAsync();
    
    return Ok(totalDisLikes);

}

[HttpPost("AddComments")]
public async Task<IActionResult> AddComment([FromBody]VideoComment request, string videoId, string userId){

var video = context.VideoModels.FirstOrDefault(v => v.VideoId == videoId);
if (video == null){
return BadRequest("Video Not Found");
}
var user = context.UserAccounts.FirstOrDefault(u=> u.UserId == userId);
if (user == null){
    return BadRequest("User Not Found");
}

var comment = new VideoComment{
VideoId = video.VideoId,
VideoName = video.Title,
UserId = user.UserId,
UserName = user.FullName,
UserEmail = user.EmailAddress,
theComments = request.theComments,
DateofComment = DateTime.Today.Date.ToString("dd MMMM, yyyy"),
CommentId = IdGenerator()
};

var counter = context.VideoComments.Where(c => c.VideoId == comment.VideoId).Count();
video.TotalComments = counter+1;

context.VideoComments.Add(comment);
await context.SaveChangesAsync();

return Ok("Comment Sent Successfully");
}

[HttpPost("UpdateComments")]
public async Task<IActionResult> UpdateComment([FromBody]VideoComment request, string videoId, string userId, string commentId){

var video = context.VideoModels.FirstOrDefault(v => v.VideoId == videoId);
if (video == null){
return BadRequest("Video Not Found");
}
var user = context.UserAccounts.FirstOrDefault(u=> u.UserId == userId);
if (user == null){
    return BadRequest("User Not Found");
}
var cmt = context.VideoComments.FirstOrDefault(c => c.VideoId == videoId && c.UserId == userId && c.CommentId==commentId);
if (cmt==null){
    return BadRequest("Comment Not Found");
}

cmt.VideoId = video.VideoId;
cmt.VideoName = video.Title;
cmt.UserId = user.UserId;
cmt.UserName = user.FullName;
cmt.UserEmail = user.EmailAddress;
cmt.theComments = request.theComments;
cmt.DateofComment = DateTime.Today.Date.ToString("dd MMMM, yyyy");

await context.SaveChangesAsync();

return Ok("Comment Updated Successfully");
}

[HttpGet("DeleteChecker")]
public async Task<IActionResult> DeleteCommentChecker(string videoId, string userId, string commentId){
bool checker = await context.VideoComments.AnyAsync(c => c.VideoId == videoId && c.UserId == userId && c.CommentId==commentId);

return Ok(checker);
} 

[HttpDelete("DeleteComment")]
public async Task<IActionResult>DeleteComment(string commentId){
var comment = context.VideoComments.FirstOrDefault(c => c.CommentId==commentId);
if (comment == null){
    return BadRequest("Comment not found");
}
context.VideoComments.Remove(comment);
await context.SaveChangesAsync();
return Ok("Comment deleted successfully");
}

[HttpGet("GetVideoComment")]
public async Task<IActionResult>GetVideoComment(string videoId){
    var comments = context.VideoComments.Where(c => c.VideoId==videoId).OrderByDescending(r=>r.Id).ToList();
    return Ok(comments);
}

[HttpPost("AddReply")]
public async Task<IActionResult> AdminReply([FromBody]VideoCommentsReply request, string commentId, string AdminId){

var cmt = context.VideoComments.FirstOrDefault(c => c.CommentId == commentId);
if (cmt == null){
    return BadRequest("Comment Not Found");
}
var Admin = context.AdminAccounts.FirstOrDefault(a => a.AdminId == AdminId);
if (Admin == null){
    return BadRequest("Admin Not Found");
}

var reply = new VideoCommentsReply{
VideoId = cmt.VideoId,
VideoName = cmt.VideoName,
UserId = cmt.UserId,
UserName = cmt.UserName,
UserEmail = cmt.UserEmail,
UserComments = cmt.theComments,
DateofUserComment = cmt.DateofComment,
AdminId = Admin.AdminId,
AdminName = Admin.FullName,
AdminReply = request.AdminReply,
AdminReplyDate = DateTime.Today.Date.ToString("dd MMMM,yyyy"),
ReplyId = IdGenerator(),
};

context.VideoCommentsReplys.Add(reply);
await context.SaveChangesAsync();

try
        {
            if (reply.UserEmail == null || reply.UserName == null|| reply.VideoName == null || reply.DateofUserComment == null || reply.UserComments==null || reply.AdminReply == null ||  reply.AdminName ==null){
                return BadRequest("All the fields are required");
            }
            await SendReplyEmail(reply.UserEmail, reply.UserName, reply.VideoName,reply.DateofUserComment, reply.UserComments,reply.AdminReply,reply.AdminName );

        }
        catch (Exception)
        {  return BadRequest("Failed to send comment reply. Please try again later.");}

return Ok("Admin Reply Sent Successfully");

}

[HttpPost("UpdateReply")]
public async Task<IActionResult> UpdateAdminReply([FromBody]VideoCommentsReply request, int commentId, string AdminId, string replyId){

var cmt = context.VideoComments.FirstOrDefault(c => c.Id == commentId);
if (cmt == null){
    return BadRequest("Comment Not Found");
}
var Admin = context.AdminAccounts.FirstOrDefault(a => a.AdminId == AdminId);
if (Admin == null){
    return BadRequest("Admin Not Found");
}

var rly = context.VideoCommentsReplys.FirstOrDefault(a=>a.ReplyId==replyId);
if (rly ==null){
    return BadRequest("Reply Not Found");
}


rly.VideoId = cmt.VideoId;
rly.VideoName = cmt.VideoName;
rly.UserId = cmt.UserId;
rly.UserName = cmt.UserName;
rly.UserEmail = cmt.UserEmail;
rly.UserComments = cmt.theComments;
rly.DateofUserComment = cmt.DateofComment;
rly.AdminId = Admin.AdminId;
rly.AdminName = Admin.FullName;
rly.AdminReply = request.AdminReply;
rly.AdminReplyDate = DateTime.Today.Date.ToString("dd MMMM,yyyy");

await context.SaveChangesAsync();

try
        {
            if (cmt.UserEmail == null || cmt.UserName == null|| cmt.VideoName == null || cmt.DateofComment == null || cmt.theComments==null || request.AdminReply == null ||  Admin.FullName==null){
                return BadRequest("All the fields are required");
            }
            await SendUpdatedReplyEmail(cmt.UserEmail, cmt.UserName, cmt.VideoName,cmt.DateofComment, cmt.theComments,request.AdminReply,Admin.FullName );

        }
        catch (Exception)
        {  return BadRequest("Failed to send comment reply. Please try again later.");}

return Ok("Admin Reply Sent Successfully");

}

[HttpDelete("DeleteReply")]
public async Task<IActionResult> DeleteReply(string AdminId, string ReplyId){
    bool IsAdmin = await context.AdminAccounts.AnyAsync(a => a.AdminId == AdminId);
    bool IsReply = await context.VideoCommentsReplys.AnyAsync (a => a.ReplyId == ReplyId);
    var reply = context.VideoCommentsReplys.FirstOrDefault(a => a.ReplyId == ReplyId);
    if(!IsReply || !IsAdmin|| reply == null){
        return BadRequest("Failed to delete reply. Please try again later.");
    }
    context.VideoCommentsReplys.Remove(reply);
    await context.SaveChangesAsync();
    return Ok("Reply Deleted Successfully");
}

[HttpGet("VideoReplies")]
public async Task<IActionResult> VideoReplies(string videoId){
    var replies = context.VideoCommentsReplys.Where(a => a.VideoId == videoId).OrderByDescending(r=>r.Id).ToList();
    return Ok(replies);
}



private async Task VideoUploadNotification(string email, string userName, string videoName, string uploadDate,string videoLink, string AdminName)
{
     EmailRequest mail = new EmailRequest();
    string subject = "Video Upload Notification";
string body = $@"<!DOCTYPE html>
<html>
<head>
<style>
    body {{
        font-family: Arial, sans-serif;
        
    }}

    .container {{
        max-width: 600px;
    }}



    .header {{
        font-size: 24px;
       
    }}

    .text {{
        color: #666666;
        margin-bottom: 10px;
    }}

    .token {{
        font-size: 28px;
        font-weight: bold;
    }}

    .footer {{
        color: #999999;
    }}
</style>
</head>
<body>
    <div class='container'>
        <div class='header'>Updated kiComment Reply</div>
        <div class='text'>Dear {userName},</div>
        <div class='text'>I hope this email finds you well. I want to personally thank you for sticking with us</div>
        <div class='text'>A new video have been uploaded today, {uploadDate}</div>
        <div class='text'>The title of the video is <b>{videoName}</b></div>
        <div class='text'>This is the link to the video {videoLink}</div>
        <div class='text'>Regards,</div>
        <div class='text'>{AdminName}</div>
        </div>
</body>
</html>";

    using (SmtpClient smtpClient = new SmtpClient(mail.SmtpHost, mail.SmtpPort))
    {
        
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(mail.SmtpUserName, mail.SmtpPassword);

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(mail.SmtpUserName);
        mailMessage.To.Add(email);
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = true; // Set the email body format to HTML

        await smtpClient.SendMailAsync(mailMessage);
    }
}





private async Task SendReplyEmail(string email, string userName, string videoName, string commentDate, string theComment, string theReply, string AdminName)
{
     EmailRequest mail = new EmailRequest();
    string subject = "Comment Reply";
string body = $@"<!DOCTYPE html>
<html>
<head>
<style>
    body {{
        font-family: Arial, sans-serif;
        
    }}

    .container {{
        max-width: 600px;
    }}



    .header {{
        font-size: 24px;
       
    }}

    .text {{
        color: #666666;
        margin-bottom: 10px;
    }}

    .token {{
        font-size: 28px;
        font-weight: bold;
    }}

    .footer {{
        color: #999999;
    }}
</style>
</head>
<body>
    <div class='container'>
        <div class='header'>Comment Reply</div>
        <div class='text'>Dear {userName},</div>
        <div class='text'>I hope this email finds you well. I want to personally thank you for sharing your thought on, {videoName}</div>
        <div class='text'>On {commentDate} , your comment was <b>{theComment}</b> </div>
        <div class='text'>This is the reply <b>{theReply}</b></div>

        <div class='text'>Regards,</div>
        <div class='text'>{AdminName}</div>
        </div>
</body>
</html>";

    using (SmtpClient smtpClient = new SmtpClient(mail.SmtpHost, mail.SmtpPort))
    {
        
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(mail.SmtpUserName, mail.SmtpPassword);

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(mail.SmtpUserName);
        mailMessage.To.Add(email);
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = true; // Set the email body format to HTML

        await smtpClient.SendMailAsync(mailMessage);
    }
}


private async Task SendUpdatedReplyEmail(string email, string userName, string videoName, string commentDate, string theComment, string theReply, string AdminName)
{
     EmailRequest mail = new EmailRequest();
    string subject = "Updated Reply";
string body = $@"<!DOCTYPE html>
<html>
<head>
<style>
    body {{
        font-family: Arial, sans-serif;
        
    }}

    .container {{
        max-width: 600px;
    }}



    .header {{
        font-size: 24px;
       
    }}

    .text {{
        color: #666666;
        margin-bottom: 10px;
    }}

    .token {{
        font-size: 28px;
        font-weight: bold;
    }}

    .footer {{
        color: #999999;
    }}
</style>
</head>
<body>
    <div class='container'>
        <div class='header'>Updated Comment Reply</div>
        <div class='text'>Dear {userName},</div>
        <div class='text'>I hope this email finds you well. I want to personally thank you for sharing your thought on, {videoName}</div>
        <div class='text'>On {commentDate} , your comment was <b>{theComment}</b> </div>
        <div class='text'>This is the reply <b>{theReply}</b></div>

        <div class='text'>Regards,</div>
        <div class='text'>{AdminName}</div>
        </div>
</body>
</html>";

    using (SmtpClient smtpClient = new SmtpClient(mail.SmtpHost, mail.SmtpPort))
    {
        
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(mail.SmtpUserName, mail.SmtpPassword);

        MailMessage mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(mail.SmtpUserName);
        mailMessage.To.Add(email);
        mailMessage.Subject = subject;
        mailMessage.Body = body;
        mailMessage.IsBodyHtml = true; // Set the email body format to HTML

        await smtpClient.SendMailAsync(mailMessage);
    }
}


 private string IdGenerator()
{
    byte[] randomBytes = new byte[2]; // Increase the array length to 2 for a 4-digit random number
    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(randomBytes);
    }

    ushort randomNumber = BitConverter.ToUInt16(randomBytes, 0);
    int fullNumber = randomNumber; // 109000 is added to ensure the number is 5 digits long

    return fullNumber.ToString("D5");
}






    }
}