using System;
using System.IO;
using System.Text;
using System.Threading;

namespace RestCli
{
    public static class StdIn
    {
        public static string ReadString()
        {
            if ( !Console.IsInputRedirected )
            {
                return ( null );
            }

            string stdin = null;

            using ( Stream stream = Console.OpenStandardInput() )
            {
                using ( var reader = new StreamReader( stream, Console.InputEncoding ) )
                {
                    stdin = reader.ReadToEnd();
                }
            }

            return ( stdin );
        }
    }
}
