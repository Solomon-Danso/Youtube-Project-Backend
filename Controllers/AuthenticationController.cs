using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTube_Backend.Data;
using YouTube_Backend.Models;

namespace YouTube_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
        AdminId = IdGenerator(),
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
        UserId = IdGenerator(),
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