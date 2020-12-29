using LanguageExt;

namespace EasyIdentity.Core
{
    public sealed class EmailAddress : TypeWrapper<string>
    {
        public const int MaxLength = 255;

        private EmailAddress(string value) : base(value)
        {
        }

        public static Either<InvalidEmailError, EmailAddress> TryCreate(string emailaddress)
        {
            if (string.IsNullOrWhiteSpace(emailaddress))
                return new NoEmptyEmail();

            if (!emailaddress.Contains("@"))
                return new NoAtProvided();

            if(emailaddress.Length > MaxLength)
                return new EmailTooLong();
            
            return new EmailAddress(emailaddress);
        }

        public abstract class InvalidEmailError : DomainError
        {
            public override ErrorSection Section => ErrorSection.EmailAddress;
        }

        public class NoAtProvided : InvalidEmailError
        {
            public override int ErrorCode => 10;
            public override string Message => "An @ sign must be provided to create a valid email address";
        }

        public class NoEmptyEmail : InvalidEmailError
        {
            public override int ErrorCode => 20;
            public override string Message => "The email can not be empty";
        }

        public class EmailTooLong : InvalidEmailError
        {
            public override int ErrorCode => 30;
            public override string Message => "The email is too long";
        }
    }
}