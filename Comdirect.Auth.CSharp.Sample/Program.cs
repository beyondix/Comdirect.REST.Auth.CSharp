using Comdirect.Auth.CSharp.Services;
using System;
using System.Threading.Tasks;

namespace Comdirect.Auth.CSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var comdirectCredentials = new ComdirectCredentials 
            { 
                ClientId = "User_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                ClientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                Username = "xxxxxxxx",
                Pin = "xxxxxx"
            };

            var comdirectAuthService = new ComdirectAuthService(comdirectCredentials)
            {
                SessionId = Guid.NewGuid().ToString()
            };

            Task.Run(async () =>
            {
               Console.WriteLine("Running initial flow, do not forget to activate the TAN in your photoTAN App");
               var validComdirectToken = await comdirectAuthService.RunInitial();
               Console.WriteLine("Initial flow ran");
               Console.Write(validComdirectToken);

               Console.WriteLine("Running refresh token flow...");
               var validComdirectTokenRefresh = await comdirectAuthService.RunRefreshTokenFlow(validComdirectToken.RefreshToken);
               Console.WriteLine("Refresh flow ran");
               Console.Write(validComdirectToken);
            }).GetAwaiter().GetResult();

            Console.ReadKey();
        }
    }
}
