using System;

namespace UserManagement
{
    public sealed class UserEntity
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string EmailAddress { get; set; } = "";
        public string? Username { get; set; } 
        public string Password { get; set; } = "";
        public EmailVerificationState EmailVerificationState { get; set;  } = EmailVerificationState.Unverified;

    }
}