using Comdirect.Auth.CSharp.Dtos;
using System.Threading.Tasks;

namespace Comdirect.Auth.CSharp.Services
{
    public interface IComdirectAuthService
    {
        Task<ValidComdirectToken> RunInitial();
        Task<ValidComdirectToken> RunRefreshTokenFlow(string refreshToken);
    }
}