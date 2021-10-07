using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Faactory.RestClient;
using Faactory.RestClient.Json;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli.HttpMethods
{
    public abstract class HttpCommand
    {
        private readonly string[] jsonMediaTypes = new string[]
        {
            // standard json media type
            "application/json",

            // microsoft json media type for MVC errors
            "application/problem+json"
        };

        private readonly RestClient client;

        public HttpCommand( IRestClientFactory clientFactory, HttpMethod httpMethod )
        {
            client = clientFactory.CreateClient(); // TODO: use context
            Method = httpMethod.Method;

            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };

            PrettyJsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
        }

        [Required]
        [Argument( 0, "url", Description = "The url of the request" )]
        public string Url { get; set; }

        [Option( "--header|-h", Description = "Append an header to the request" )]
        public string[] Headers { get; set; }

        [Option( "--verbose|-v", Description = "Display request and response details" )]
        public bool Verbose { get; set; }

        //protected string StdIn { get; private set; }

        protected string Method { get; }

        protected JsonSerializerOptions JsonSerializerOptions { get; private set; }

        protected JsonSerializerOptions PrettyJsonSerializerOptions { get; private set; }

        protected Task<int> OnExecute( CommandLineApplication app )
        {
            //this.StdIn = RestCli.StdIn.ReadString();

            // normalize Url
            // TODO: consider contexts
            if ( Url.StartsWith( "localhost:" ) )
            {
                // use http:// scheme for localhost on custom ports only
                Url = "http://" + Url;
            }
            else if ( !Url.StartsWith( "http://" ) && !Url.StartsWith( "https://" ) )
            {
                // use https:// as default scheme
                Url = "https://" + Url;
            }

            // configure request
            var request = client.Configure( options =>
            {
                if ( Headers?.Any() == true )
                {
                    var headers = HeaderCollection.TryParse( Headers, out var errors );

                    if ( errors.Any() )
                    {
                        foreach ( var error in errors )
                        {
                            ConsoleEx.SetColor( ConsoleColor.DarkYellow );
                            Console.Write( "Warning: " );
                            ConsoleEx.ResetColor();
                            Console.WriteLine( error );
                        }
                    }

                    foreach ( var header in headers )
                    {
                        options.Headers.Add( header.Key, header.Value );
                    }
                }

                // infer authorization from local configuration, if it exists
                if ( options.Headers.Authorization == null )
                {
                    var auth = LocalConfiguration.Authorization.Read();

                    if ( !string.IsNullOrEmpty( auth ) )
                    {
                        options.Headers.Add( "Authorization", auth );
                    }
                }

                // use application/json by default if nothing else was specified
                if ( !( options.Headers.Accept?.Any() == true ) )
                {
                    options.Headers.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
                }

                // allow further customization
                Configure( options );
            } );

            if ( Verbose )
            {
                Console.WriteLine( $"{Method} {Url}" );
            }

            return ExecuteAsync( request, app );
        }

        protected virtual void Configure( RestRequestOptions requestOptions )
        {}

        protected virtual Task<int> ExecuteAsync( RestRequest request, CommandLineApplication app )
        {
            Task<RestResponse> send;

            switch ( Method )
            {
                case "GET":
                    send = request.GetAsync( Url );
                    break;

                case "DELETE":
                    send = request.DeleteAsync( Url );
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return ExecuteAsync( send );
        }

        protected async Task<int> ExecuteAsync( Task<RestResponse> send )
        {
            var response = await send;

            if ( Verbose )
            {
                ConsoleEx.WriteLine( new ColorExpression[]
                {
                    new ColorExpression
                    {
                        Color = ConsoleColor.Green,
                        Condition = x => (int)x < 300
                    },
                    new ColorExpression
                    {
                        Color = ConsoleColor.DarkYellow,
                        Condition = x => (int)x < 600 && (int)x >= 300
                    },
                    new ColorExpression
                    {
                        Color = ConsoleColor.Red,
                        Condition = x => (int)x >= 600
                    }
                }
                , (int)response.Duration.TotalMilliseconds
                , $"{response.Duration.TotalMilliseconds:f0}ms" );

                ConsoleEx.WriteLine( new ColorExpression[]
                {
                    new ColorExpression
                    {
                        Color = ConsoleColor.Green,
                        Condition = x => (int)x >= 200 && (int)x < 300
                    },
                    new ColorExpression
                    {
                        Color = ConsoleColor.DarkYellow,
                        Condition = x => (int)x >= 400 && (int)x < 500
                    },
                    new ColorExpression
                    {
                        Color = ConsoleColor.Red,
                        Condition = x => (int)x >= 500 && (int)x < 600
                    }
                }
                , response.StatusCode
                , $"{response.StatusCode}" );

                ConsoleEx.SetColor( ConsoleColor.DarkGray );
                Console.WriteLine( "---" );
                ConsoleEx.ResetColor();

                foreach ( var header in response.Headers )
                {
                    foreach ( var value in header.Value )
                    {
                        Console.WriteLine( $"{header.Key}: {value}" );
                    }
                }

                if ( !string.IsNullOrEmpty( response.ContentType ) )
                {
                    Console.WriteLine( $"Content-Type: {response.ContentType}" );
                }

                ConsoleEx.SetColor( ConsoleColor.DarkGray );
                Console.WriteLine( "---" );
                ConsoleEx.ResetColor();
            }

            if ( response.Content?.Any() == true )
            {
                var textContent = Encoding.UTF8.GetString( response.Content );

                // is it json? if so, attempt to serialize and deserialize
                if ( jsonMediaTypes.Contains( response.ContentType, StringComparer.OrdinalIgnoreCase ) )
                {
                    try
                    {
                        var tmpObj = JsonSerializer.Deserialize<Dictionary<string, object>>( textContent, JsonSerializerOptions );

                        textContent = JsonSerializer.Serialize( tmpObj, PrettyJsonSerializerOptions );
                    }
                    catch ( Exception )
                    {
                        // not json? maybe it's a collection
                        try
                        {
                            var tmpObj = JsonSerializer.Deserialize<IEnumerable<Dictionary<string, object>>>( textContent, JsonSerializerOptions );

                            textContent = JsonSerializer.Serialize( tmpObj, PrettyJsonSerializerOptions );
                        }
                        catch ( Exception )
                        { }
                    }
                }

                Console.WriteLine( textContent );
            }

            //Console.WriteLine();

            return ( response.IsServerError() ? 1 : 0 );
        }
    }
}
