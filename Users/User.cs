﻿using EasyIdentity.Core;
using LanguageExt;
using System;
using static LanguageExt.Prelude;

namespace Users
{
    public enum EmailVerificationState
    {
        Verified,
        Unverified,
    }

    public sealed class User
    {
        private record UserRecord(
            UserId UserId,
            EmailAddress EmailAddress,
            Password Password,
            Option<Username> Username,
            EmailVerificationState EmailVerificationState);

        private readonly UserRecord userRecord;
        public UserId UserId => userRecord.UserId;
        public EmailAddress EmailAddress => userRecord.EmailAddress;
        public Password Password => userRecord.Password;
        public Option<Username> Username => userRecord.Username;
        public EmailVerificationState EmailVerificationState => userRecord.EmailVerificationState;
        public bool EmailIsVerified => userRecord.EmailVerificationState == EmailVerificationState.Verified;

        public User(UserId userId, EmailAddress email, Password password, Option<Username> username, EmailVerificationState emailVerification)
        {
            userRecord = new UserRecord(userId, email, password, username, emailVerification);
        }

        private User(UserRecord userRecord)
        {
            this.userRecord = userRecord;
        }

        public static Either<DomainError, User> TrySignUp(string email, string password, Option<string> username)
        {
            return username
                .Map(username => Users.Username.TryCreate(username).CastDomainError())
                .Match(
                    usnameEither => usnameEither.Bind(usernameValue => createUser(usernameValue)),
                    () => createUser(None)
                );

            Either<DomainError, User> createUser(Option<Username> username) =>
              from emailAddr in EmailAddress.TryCreate(email).CastDomainError()
              from password in Password.TryCreate(password).CastDomainError()
              select new User(UserId.Create(Guid.NewGuid()), emailAddr, password, username, EmailVerificationState.Unverified);
        }

        public User ChangePassword(Password newPassword)
        {
            return new User(userRecord with { Password = newPassword });
        }

        public User VerifyEmail()
        {
            return new User(userRecord with { EmailVerificationState = EmailVerificationState.Verified });
        }

        public User ChangeEmailAddress(EmailAddress emailAddress)
        {
            return new User(userRecord with { EmailAddress = emailAddress, EmailVerificationState = EmailVerificationState.Unverified });
        }
    }

    public sealed class UserId : TypeWrapper<Guid>
    {
        private UserId(Guid id) : base(id)
        {
        }

        public static UserId Create(Guid id)
        {
            return new UserId(id);
        }
    }

    public sealed class Username : TypeWrapper<string>
    {
        public const int MaxLength = 100;
        public const int MinLength = 4;
        public abstract class InvalidUsernameError : DomainError
        {
            public override ErrorSection Section => ErrorSection.Username;
        }

        public class UsernameTooShort : InvalidUsernameError
        {
            public override int ErrorCode => 10;
            public override string Message => "The username must contain at least 8 characters";
        }

        public class UsernameTooLong : InvalidUsernameError
        {
            public override int ErrorCode => 20;
            public override string Message => "The username cannot be longer than 100 characters";
        }

        private Username(string value) : base(value)
        {
        }

        public static Either<InvalidUsernameError, Username> TryCreate(string username)
        {
            return username.Length switch
            {
                < MinLength => new UsernameTooShort(),
                > MaxLength => new UsernameTooLong(),
                _ => new Username(username)
            };
        }
    }

    public sealed class Password : TypeWrapper<string>
    {
        private Password(string value) : base(value)
        {
        }

        public static Either<PasswordError, Password> TryCreate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return new NoEmptyPassword();
            }
            return new Password(password);
        }

        public abstract class PasswordError : DomainError
        {
            public override ErrorSection Section => ErrorSection.Password;
        }

        public class NoEmptyPassword : PasswordError
        {
            public override int ErrorCode => 10;

            public override string Message => "The provided password cannot be empty";
        }
    }
}