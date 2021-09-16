using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli
{
    [Command( Name = "version", Description = "Displays the version information" )]
    public class VersionCommand
    {
        protected Task<int> OnExecute( CommandLineApplication app )
        {
            Console.WriteLine( "rest " + GetType().Assembly.GetVersion() );
            Console.WriteLine( "Copyright (C) 2021 Goncalo Oliveira" );
            Console.WriteLine();
            Console.WriteLine( "https://github.com/goncalo-oliveira/rest-cli/" );
            Console.WriteLine();

            return Task.FromResult( 0 );
        }
    }
}
