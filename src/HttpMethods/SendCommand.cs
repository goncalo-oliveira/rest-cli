using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Faactory.RestClient;
using Faactory.RestClient.Json;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli.HttpMethods
{
    public abstract class SendCommand : HttpCommand
    {
        public SendCommand( IRestClientFactory clientFactory, HttpMethod httpMethod )
        : base( clientFactory, httpMethod )
        {}

        [Argument( 1, "data", Description = "The data to send in the request body" )]
        public string Data { get; set; }

        [Option( "--file|-f", Description = "The data file to send in the request body" )]
        public string DataFile { get; set; }

        protected override Task<int> ExecuteAsync( RestRequest request, CommandLineApplication app )
        {
            var stdin = StdIn.ReadString();

            if ( !string.IsNullOrEmpty( stdin ) )
            {
                if ( !string.IsNullOrEmpty( Data ) )
                {
                    ConsoleEx.SetColor( ConsoleColor.DarkYellow );
                    Console.Write( "Warning: " );
                    ConsoleEx.ResetColor();
                    Console.WriteLine( "data argument replaced by stdin." );
                }

                if ( !string.IsNullOrEmpty( DataFile ) )
                {
                    ConsoleEx.SetColor( ConsoleColor.DarkYellow );
                    Console.Write( "Warning: " );
                    ConsoleEx.ResetColor();
                    Console.WriteLine( "--file option replaced by stdin." );
                }

                Data = stdin;
            }
            else if ( !string.IsNullOrEmpty( DataFile ) )
            {
                string filepath = Path.GetFullPath( DataFile );
                if ( !File.Exists( filepath ) )
                {
                    ConsoleEx.SetColor( ConsoleColor.Red );
                    Console.Write( "Error: " );
                    ConsoleEx.ResetColor();
                    Console.WriteLine( $"Can't find '{filepath}'." );

                    return ( Task.FromResult( 1 ) );
                }

                if ( !string.IsNullOrEmpty( Data ) )
                {
                    ConsoleEx.SetColor( ConsoleColor.DarkYellow );
                    Console.Write( "Warning: " );
                    ConsoleEx.ResetColor();
                    Console.WriteLine( "data argument replaced by --file option." );
                }

                Data = File.ReadAllText( filepath );
            }

            if ( string.IsNullOrEmpty( Data ) )
            {
                ConsoleEx.SetColor( ConsoleColor.DarkYellow );
                Console.Write( "Warning: " );
                ConsoleEx.ResetColor();
                Console.WriteLine( "data argument is empty." );
            }

            var jsonContent = new JsonContent( Encoding.UTF8.GetBytes( Data ?? string.Empty ) );
            Task<RestResponse> send;

            switch ( Method )
            {
                case "POST":
                    send = request.PostAsync( Url, jsonContent );
                    break;

                case "PUT":
                    send = request.PutAsync( Url, jsonContent );
                    break;

                case "PATCH":
                    send = request.PatchAsync( Url, jsonContent );
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return ExecuteAsync( send );
        }
    }
}
