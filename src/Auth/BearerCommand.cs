using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli.Auth
{
    [Command( Name = "bearer", Description = "Stores Bearer token credentials" )]
    public class BearerCommand
    {

        [Option( "--token|-t", Description = "The token" )]
        public string Token { get; set; }

        [Option( "--token-stdin|-ts", Description = "Read token from stdin" )]
        public bool ReadToken { get; set; }

        protected Task<int> OnExecute( CommandLineApplication app )
        {
            if ( string.IsNullOrEmpty( Token ) && !ReadToken )
            {
                ConsoleEx.SetColor( ConsoleColor.Red );
                Console.WriteLine( "The --token or --token-stdin field is required." );
                ConsoleEx.ResetColor();

                return Task.FromResult( 1 );
            }

            if ( ReadToken )
            {
                var stdin = StdIn.ReadString();

                if ( !string.IsNullOrEmpty( Token ) )
                {
                    ConsoleEx.SetColor( ConsoleColor.DarkYellow );
                    Console.Write( "Warning: " );
                    ConsoleEx.ResetColor();
                    Console.WriteLine( "--token option replaced by stdin." );
                }

                Token = stdin.Trim();
            }

            if ( string.IsNullOrEmpty( Token ) )
            {
                ConsoleEx.SetColor( ConsoleColor.Red );
                Console.WriteLine( "The token must not be empty." );
                ConsoleEx.ResetColor();

                return Task.FromResult( 1 );
            }

            LocalConfiguration.Authorization.Write( $"Bearer {Token}" );

            Console.WriteLine( "Credentials stored." );

            return Task.FromResult( 0 );
        }
    }
}
