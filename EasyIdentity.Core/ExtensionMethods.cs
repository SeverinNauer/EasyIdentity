using LanguageExt;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace EasyIdentity.Core
{
    public static class ExtensionMethods
    {
        public static Either<DomainError, TR> CastDomainError<TL, TR>(this Either<TL, TR> either) where TL : DomainError
             => either.MapLeft<DomainError>(err => err);

        public static Task<Either<DomainError, TR>> CastDomainError<TL, TR>(this Task<Either<TL, TR>> either) where TL : DomainError 
            => either.Map(et => et.CastDomainError());

        public static Option<T> AsOption<T>(this T? data) => data is null ? Option<T>.None : Some(data);
    }
}