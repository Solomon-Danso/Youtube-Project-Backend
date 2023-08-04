using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTube_Backend.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpireDate{ get; set; }
        public string? UserId { get; set; }
        public string? Role { get; set; }
           public string? TwoStepsVerification { get; set; }
        public DateTime? TwoStepsVerificationExpireDate { get; set; }
  
    }

    public class AdminAccount
    {
        public int Id { get; set; }
        public string? ProfilePicture { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? TwoStepsVerification { get; set; }
        public DateTime? TwoStepsVerificationExpireDate { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpireDate{ get; set; }
        public string? AdminId { get; set; }
        public string? Role { get; set; }

    }

    public class AdminAccountDto
    {
        public int Id { get; set; }
        public IFormFile? File { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public string? TwoStepsVerification { get; set; }
        public DateTime? TwoStepsVerificationExpireDate { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpireDate{ get; set; }
        public string? AdminId { get; set; }
        public string? Role { get; set; }

    }

public class LoginForm{
    public string? UserEmail{ get; set; }
    public string? Password{ get; set; }
}


    public class Roles{
        public string Admin = "AdminUser";
        public string NormalUser = "NormalUser";

    }

    public class Constants{
        public string apiServer = "http://localhost:5166/";
        public string AdminEmail = "admin@example.com";
    }
}