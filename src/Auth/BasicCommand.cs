using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli.Auth
{
    [Command( Name = "basic", Description = "Stores Basic Authentication credentials" )]
    public class BasicCommand
    {
        [Required]
        [Option( "--username|-u", Description = "The username" )]
        public string Username { get; set; }

        [Option( "--password|-p", Description = "The password" )]
        public string Password { get; set; }

        [Option( "--password-stdin|-ps", Description = "Read password from stdin" )]
        public bool ReadPassword { get; set; }

        protected Task<int> OnExecute( CommandLineApplication app )
        {
            if ( string.IsNullOrEmpty( Password ) && !ReadPassword )
            {
                ConsoleEx.SetColor( ConsoleColor.Red );
                Console.WriteLine( "The --password or --password-stdin field is required." );
                ConsoleEx.ResetColor();

                return Task.FromResult( 1 );
            }

            if ( ReadPassword )
            {
                var stdin = StdIn.ReadString();

                if ( !string.IsNullOrEmpty( Password ) )
                {
                    ConsoleEx.SetColor( ConsoleColor.DarkYellow );
                    Console.Write( "Warning: " );
                    ConsoleEx.ResetColor();
                    Console.WriteLine( "--password option replaced by stdin." );
                }

                Password = stdin.Trim();
            }

            if ( string.IsNullOrEmpty( Password ) )
            {
                ConsoleEx.SetColor( ConsoleColor.Red );
                Console.WriteLine( "The password must not be empty." );
                ConsoleEx.ResetColor();

                return Task.FromResult( 1 );
            }

            var token = string.Concat( Username, ":", Password );

            token = Convert.ToBase64String( Encoding.ASCII.GetBytes( token ) );

            LocalConfiguration.Authorization.Write( $"Basic {token}" );

            Console.WriteLine( "Credentials stored." );

            return Task.FromResult( 0 );
        }
    }
}
