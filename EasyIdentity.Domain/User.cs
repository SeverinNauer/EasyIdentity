﻿using LanguageExt;
using System;
using static LanguageExt.Prelude;

namespace EasyIdentity.Domain
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
            EmailAddress Email,
            Password Password,
            Option<Username> Username,
            EmailVerificationState EmailVerificationState);

        private readonly UserRecord userRecord;
        public UserId UserId => userRecord.UserId;
        public EmailAddress Email => userRecord.Email;
        public Password Password => userRecord.Password;
        public Option<Username> Username => userRecord.Username;
        public bool EmailIsVerified => userRecord.EmailVerificationState == EmailVerificationState.Verified;

        public User(UserId userId, EmailAddress email, Password password, Option<Username> username, EmailVerificationState emailVerification)
        {
            userRecord = new UserRecord(userId, email, password, username, emailVerification);
        }

        private User(UserRecord userRecord)
        {
            this.userRecord = userRecord;
        }

        public static Either<DomainError, User> TryCreateNew(string email, string password, Option<string> username)
        {
            var userId = UserId.Create(Guid.NewGuid());

            Either<DomainError, User> createUser(Option<Username> username) =>
                from emailAddr in EmailAddress.TryCreate(email).CastDomainError()
                from password in Password.TryCreate(password).CastDomainError()
                select new User(userId, emailAddr, password, username, EmailVerificationState.Unverified);

            return username
                .Map(username => EasyIdentity.Domain.Username.TryCreate(username).CastDomainError())
                .Match(
                    usnameEither => usnameEither.Bind(usernameValue => createUser(usernameValue)),
                    () => createUser(None)
                );
        }

        public User ChangePassword(Password newPassword)
        {
            return new User(userRecord with { Password = newPassword });
        }

        public bool IsValidLoginUsername(string username, string password)
        {
            return Username.Match(usernameUnwrapped => usernameUnwrapped == username && Password == password, () => false);
        }

        public bool IsValidLoginEmail(string email, string password)
        {
            return Email == email && Password == password;
        }

        public User VerifyEmail()
        {
            return new User(userRecord with { EmailVerificationState = EmailVerificationState.Verified });
        }

        public User ChangeEmailAddress(EmailAddress emailAddress)
        {
            return new User(userRecord with { Email = emailAddress, EmailVerificationState = EmailVerificationState.Unverified });
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
        public abstract class UsernameError : DomainError
        {
            public override ErrorSection Section => ErrorSection.Username;
        }

        public class UsernameTooShort : UsernameError
        {
            public override int ErrorCode => 10;
            public override string Message => "The username must contain at least 8 characters";
        }

        private Username(string value) : base(value)
        {
        }

        public static Either<UsernameError, Username> TryCreate(string username)
        {
            if (username.Length < 8)
            {
                return Left<UsernameError>(new UsernameTooShort());
            }
            return Right(new Username(username));
        }
    }

    public sealed class Password : TypeWrapper<string>
    {
        public abstract class PasswordError : DomainError
        {
            public override ErrorSection Section => ErrorSection.Password;
        }

        public class NoEmptyPassword : PasswordError
        {
            public override int ErrorCode => 10;

            public override string Message => "The provided email can not be empty";
        }

        private Password(string value) : base(value)
        {
        }

        public static Either<PasswordError, Password> TryCreate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return Left<PasswordError>(new NoEmptyPassword());
            }
            return Right(new Password(password));
        }
    }
}