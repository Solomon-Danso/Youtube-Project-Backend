using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTube_Backend.Data;
using YouTube_Backend.Models;

namespace YouTube_Backend.Controllers
{
    [ApiController]
    [Route("api/Auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly DataContext context;
        public AuthenticationController(DataContext ctx){
            context = ctx;
        }

        Roles role = new Roles();
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromForm] AdminAccountDto request){
        
           

        if (request.File == null || request.File.Length == 0)
    {
        return BadRequest("Invalid file");
    }

    // Create the uploads directory if it doesn't exist
    var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Admin", "ProfilePicture");
    if (!Directory.Exists(uploadsDirectory))
    {
        Directory.CreateDirectory(uploadsDirectory);
    }

    // Get the original file extension
    var fileExtension = Path.GetExtension(request.File.FileName);

    // Generate a unique file name
    var fileName = Guid.NewGuid().ToString() + fileExtension;

    // Save the uploaded file to the uploads directory
    var filePath = Path.Combine(uploadsDirectory, fileName);
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        await request.File.CopyToAsync(stream);
    }

    var admin = new AdminAccount{
        ProfilePicture = Path.Combine("Admin/ProfilePicture", fileName),
        FullName = request.FullName,
        EmailAddress = request.EmailAddress,
        PhoneNumber = request.PhoneNumber,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
       // AdminId = IdGenerator(),
        Role = role.Admin

    };
var checker =  context.AdminAccounts.Count();
if(checker>0){
    return BadRequest("You cannot have more than one administrator account");
}
 
    context.AdminAccounts.Add(admin);
    await context.SaveChangesAsync();
    return Ok("Admin Account Created Successfully");

        }


[HttpPost("RegisterUser")]
public async Task<IActionResult> RegisterUser([FromBody] UserAccount request){
    var newUser = new UserAccount{
        FullName = request.FullName,
        EmailAddress = request.EmailAddress,
        PhoneNumber = request.PhoneNumber,
        Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
        //UserId = IdGenerator(),
        Role = role.NormalUser,

    };
    bool checker = await context.UserAccounts.AnyAsync(x=>x.UserId == newUser.UserId||x.EmailAddress == newUser.EmailAddress||x.PhoneNumber == newUser.PhoneNumber);
    if(checker){
        return BadRequest("User already registered");
    }
    context.UserAccounts.Add(newUser);
    await context.SaveChangesAsync();
    return Ok("User registered successfully");
}


[HttpPost("AdminLogin")]
public async Task<IActionResult>AdminLogin([FromBody]LoginForm request){

var UserEmail = context.AdminAccounts.FirstOrDefault(a=>a.EmailAddress == request.UserEmail);
if(UserEmail == null){
    return BadRequest("User Email Is Incorrect");
}
bool Password =  BCrypt.Net.BCrypt.Verify(request.Password, UserEmail.Password);
if(!Password){
    return BadRequest("Password Is Incorrect");
}


UserEmail.TwoStepsVerification = IdGenerator();
UserEmail.TwoStepsVerificationExpireDate = DateTime.Now.AddMinutes(10);

await context.SaveChangesAsync();
 try
        {
            if (UserEmail.EmailAddress==null|| UserEmail.FullName == null|| UserEmail.TwoStepsVerification == null){
                return BadRequest("All the fields are required");
            }
            await LoginEmail(UserEmail.EmailAddress, UserEmail.FullName, UserEmail.TwoStepsVerification);

        }
        catch (Exception)
        {  return BadRequest("Failed to send messages. Please try again later.");}

    
    return Ok("Verify Your Login Before We Proceed");


}


[HttpPost("AdminVerifyLogin")]
public async Task<IActionResult>AdminVerifyLogin(string Token){
    var admin = context.AdminAccounts.FirstOrDefault(a => a.TwoStepsVerification==Token);
    if (admin == null){
        return BadRequest("Invalid Token");
    }
    if(DateTime.Now>admin.TwoStepsVerificationExpireDate){
        return BadRequest("Token Has Expired ");
    }
    admin.TwoStepsVerification = null;
    admin.TwoStepsVerificationExpireDate = null;
    await context.SaveChangesAsync();
    return Ok("Login was Successful");
}

[HttpPost("AdminChangePasswordToken")]
public async Task<IActionResult>ChangePasswordToken(string Email){
    var user = context.AdminAccounts.FirstOrDefault(a => a.EmailAddress == Email);
    if (user == null){
        return BadRequest("User does not exist");
    }
    user.PasswordResetToken = IdGenerator();
    user.PasswordResetTokenExpireDate = DateTime.Now.AddMinutes(10);
    await context.SaveChangesAsync();

try
        {
            if (user.EmailAddress==null|| user.FullName == null|| user.PasswordResetToken == null){
                return BadRequest("All the fields are required");
            }
            await PasswordResetEmail(user.EmailAddress, user.FullName, user.PasswordResetToken);

        }
        catch (Exception)
        {  return BadRequest("Failed to send messages. Please try again later.");}

            return Ok("Password reset token sent successfully");

}

[HttpPost("AdminChangePassword")]
public async Task<IActionResult> AdminChangePassword(string Token,[FromBody]LoginForm request){
    var user = context.AdminAccounts.FirstOrDefault(a=>a.PasswordResetToken==Token);
    if (user == null){
        return BadRequest("Incorrect password reset token");
    }
    if(DateTime.Now>user.PasswordResetTokenExpireDate){
        return BadRequest("Expired password reset token");
    }
    user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
    await context.SaveChangesAsync();
    return Ok("Password reset was successfull");

}

[HttpPost("UserLogin")]
public async Task<IActionResult>UserLogin([FromBody]LoginForm request){

var UserEmail = context.UserAccounts.FirstOrDefault(a=>a.EmailAddress == request.UserEmail);
if(UserEmail == null){
    return BadRequest("User Email Is Incorrect");
}
bool Password =  BCrypt.Net.BCrypt.Verify(request.Password, UserEmail.Password);
if(!Password){
    return BadRequest("Password Is Incorrect");
}


UserEmail.TwoStepsVerification = IdGenerator();
UserEmail.TwoStepsVerificationExpireDate = DateTime.Now.AddMinutes(10);

await context.SaveChangesAsync();
 try
        {
            if (UserEmail.EmailAddress==null|| UserEmail.FullName == null|| UserEmail.TwoStepsVerification == null){
                return BadRequest("All the fields are required");
            }
            await LoginEmail(UserEmail.EmailAddress, UserEmail.FullName, UserEmail.TwoStepsVerification);

        }
        catch (Exception)
        {  return BadRequest("Failed to send messages. Please try again later.");}

    
    return Ok("Verify Your Login Before We Proceed");


}


[HttpPost("UserVerifyLogin")]
public async Task<IActionResult>UserVerifyLogin(string Token){
    var admin = context.UserAccounts.FirstOrDefault(a => a.TwoStepsVerification==Token);
    if (admin == null){
        return BadRequest("Invalid Token");
    }
    if(DateTime.Now>admin.TwoStepsVerificationExpireDate){
        return BadRequest("Token Has Expired ");
    }
    admin.TwoStepsVerification = null;
    admin.TwoStepsVerificationExpireDate = null;
    await context.SaveChangesAsync();
    return Ok("Login was Successful");
}

[HttpPost("UserChangePasswordToken")]
public async Task<IActionResult>UserChangePasswordToken(string Email){
    var user = context.UserAccounts.FirstOrDefault(a => a.EmailAddress == Email);
    if (user == null){
        return BadRequest("User does not exist");
    }
    user.PasswordResetToken = IdGenerator();
    user.PasswordResetTokenExpireDate = DateTime.Now.AddMinutes(10);
    await context.SaveChangesAsync();

try
        {
            if (user.EmailAddress==null|| user.FullName == null|| user.PasswordResetToken == null){
                return BadRequest("All the fields are required");
            }
            await PasswordResetEmail(user.EmailAddress, user.FullName, user.PasswordResetToken);

        }
        catch (Exception)
        {  return BadRequest("Failed to send messages. Please try again later.");}

            return Ok("Password reset token sent successfully");

}

[HttpPost("UserChangePassword")]
public async Task<IActionResult> UserChangePassword(string Token,[FromBody]LoginForm request){
    var user = context.UserAccounts.FirstOrDefault(a=>a.PasswordResetToken==Token);
    if (user == null){
        return BadRequest("Incorrect password reset token");
    }
    if(DateTime.Now>user.PasswordResetTokenExpireDate){
        return BadRequest("Expired password reset token");
    }
    user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
    await context.SaveChangesAsync();
    return Ok("Password reset was successfull");

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

        private async Task LoginEmail(string email, string FullName, string Token)
{
     EmailRequest mail = new EmailRequest();
    string subject = "Two Factor Authentication";
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
        <div class='text'>Dear {FullName}</div>
        <div class='text'>You have requested to Login into you account and the numbers below is your login code</div>
        <div class='token'>{Token}</div>
        <div class='text'>This code expires in 10 minutes, after that period, you cannot login with this code unless you generate a new one</div>
        <div class='text'>Under NO circumstances should you share this code with any other person</div>
        <div class='text'>When you share this code with any person, you have lost your account to the person</div>
       
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

        private async Task PasswordResetEmail(string email, string FullName, string Token)
{
     EmailRequest mail = new EmailRequest();
    string subject = "Password Reset Authentication";
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
        <div class='text'>Dear {FullName}</div>
        <div class='text'>You have requested to Change your account password and the numbers below is your password reset code</div>
        <div class='token'>{Token}</div>
        <div class='text'>This code expires in 10 minutes, after that period, you cannot change your password with this code unless you generate a new one</div>
        <div class='text'>Under NO circumstances should you share this code with any other person</div>
        <div class='text'>When you share this code with any person, you have lost your account to the person</div>
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