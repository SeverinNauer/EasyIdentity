namespace EasyIdentity.Domain
{
    public abstract class DomainError
    {
        public abstract ErrorSection Section { get; }
        public abstract int ErrorCode { get; }
        public abstract string Message { get; }
        public string Error => $"{(int)Section}.{ErrorCode}";
    }

    public enum ErrorSection
    {
        EmailAddress = 1000,
        Username = 1010,
        Password = 1020,
    }
}
