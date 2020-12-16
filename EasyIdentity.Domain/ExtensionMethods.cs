using EasyIdentity.Domain;
using LanguageExt;
using static LanguageExt.Prelude;

public static class ExtensionMethods
{
    public static Either<DomainError, TR> CastDomainError<TL,TR>(this Either<TL, TR> either) where TL : DomainError  
         => either.MapLeft<DomainError>(err => err); 
} 