using LanguageExt;
using static LanguageExt.Prelude;

namespace EasyIdentity.Domain
{
    public class EmailAddress : TypeWrapper<string>
    {
        private EmailAddress(string value) : base(value)
        {
        }

        public static Either<InvalidEmailError, EmailAddress> TryCreate(string emailaddress)
        {
            if (string.IsNullOrWhiteSpace(emailaddress))
            {
                return Left<InvalidEmailError>(new NoEmptyEmail());
            }
            if (!emailaddress.Contains("@"))
            {
                return Left<InvalidEmailError>(new NoAtProvided());
            }
            return Right(new EmailAddress(emailaddress));
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
    }
}