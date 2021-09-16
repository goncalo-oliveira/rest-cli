using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli.Auth
{
    [Command( Name = "auth", Description = "Authentication commands" )]
    [Subcommand( 
        typeof( BasicCommand ),
        typeof( BearerCommand )
    )]
    public class AuthCommand
    {
        [Option( "--clear", Description = "Clears any stored credentials" )]
        public bool Clear { get; set; }

        protected Task<int> OnExecute( CommandLineApplication app )
        {
            if ( Clear )
            {
                LocalConfiguration.Authorization.Clear();

                Console.WriteLine( "Credentials cleared." );

                return Task.FromResult( 0 );
            }

            app.ShowHelp();

            return Task.FromResult( 0 );
        }
    }
}
