using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RestCli
{
    class Program
    {
        static async Task<int> Main( string[] args )
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
                
            var builder = new HostBuilder()
                .ConfigureServices( ( hostContext, services ) =>
                {
                    services.AddRestClient();
                } );

            try
            {
                return await builder.RunCommandLineApplicationAsync<RestCommand>( args );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.Message );
                
                return 1;
            }
        }
    }
}
