using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YouTube_Backend.Data;
using YouTube_Backend.Models;

namespace YouTube_Backend.Controllers
{
    [ApiController]
    [Route("api/AllOtherFeatures")]
    public class AllOtherFeaturesController : ControllerBase
    {
        private readonly DataContext context;
        public AllOtherFeaturesController(DataContext ctx){
            context = ctx;
        }

        Constants constants = new Constants();

        [HttpPost("SearchVideo")]
        public async Task<IActionResult> SearchVideo(string searchTerm){
            var searchResult = context.VideoModels.ToList().Where(v=>v.Title != null && v.Title.Contains(searchTerm,StringComparison.OrdinalIgnoreCase)).ToList();
            if(searchResult.Count()==0){
                return NotFound("No Result Found");
            }
            return Ok(searchResult);
        
        }

        [HttpPost("Messages")]
        public async Task<IActionResult> SendMessage([FromBody]Message request){
            var chat = new Message{
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                UserMessage = request.UserMessage,
                DateOfMessage = DateTime.Today.Date.ToString("dd MMMM, yyyy")
            };
            context.Messages.Add(chat);
            await context.SaveChangesAsync();

            try
        {
            if (chat.FullName == null || chat.Email == null|| chat.PhoneNumber == null || chat.UserMessage == null){
                return BadRequest("All the fields are required");
            }
            await SendMessageEmail(constants.AdminEmail, chat.FullName, chat.Email, chat.PhoneNumber, chat.UserMessage,chat.DateOfMessage);

        }
        catch (Exception)
        {  return BadRequest("Failed to send messages. Please try again later.");}

        return Ok("Chat successfully sent");

        }


    [HttpGet("GetAllMessages")]
    public async Task<IActionResult> GetAllMessages(string AdminId){
        var isAdmin = context.AdminAccounts.FirstOrDefault(a=>a.AdminId == AdminId);
        if (isAdmin == null){
            return BadRequest("UnAuthorized");
        }
        var messages = context.Messages.ToList();
        return Ok(messages);
    }

    











        private async Task SendMessageEmail(string email, string FullName, string UserEmail, string PhoneNumber,string Message, string DateSent)
{
     EmailRequest mail = new EmailRequest();
    string subject = "New Message";
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
        <div class='header'>New Message</div>
        <div class='text'>Full Name: {FullName}</div>
        <div class='text'>User Email: {UserEmail}</div>
        <div class='text'>Phone Number: {PhoneNumber}</div>
        <div class='text'>{Message}</div>
        <div class='text'>Date Sent: {DateSent}</div>
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

    }
}