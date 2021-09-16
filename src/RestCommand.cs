using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli
{
    [Command( Name = "rest" )]
    [Subcommand( 
        typeof( VersionCommand ),
        typeof( Auth.AuthCommand ),
        typeof( HttpMethods.GetCommand ),
        typeof( HttpMethods.PostCommand ),
        typeof( HttpMethods.PutCommand ),
        typeof( HttpMethods.PatchCommand ),
        typeof( HttpMethods.DeleteCommand )
    )]
    public class RestCommand
    {
        protected Task<int> OnExecute( CommandLineApplication app )
        {
            app.ShowHelp();

            return Task.FromResult( 0 );
        }
    }
}
