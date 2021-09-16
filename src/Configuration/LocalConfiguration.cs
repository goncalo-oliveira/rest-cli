using System;
using System.IO;
using System.Text;

namespace RestCli
{
    public static class LocalConfiguration
    {
        private static readonly UTF8Encoding encoding = new UTF8Encoding( false );

        static LocalConfiguration()
        {
            var path = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );

            LocalPath = Path.Combine( path, "rest-cli" );

            Authorization = new Configuration.ConfigurationItem( "authorization" );
        }

        public static string LocalPath { get; }

        public static void Clear( string name )
        {
            var filepath = Path.Combine( LocalPath, name );

            if ( File.Exists( filepath ) )
            {
                File.Delete( filepath );
            }
        }

        public static string Read( string name )
        {
            var filepath = Path.Combine( LocalPath, name );

            if ( !File.Exists( filepath ) )
            {
                return ( null );
            }

            var b64 = File.ReadAllText( filepath, encoding );
            var buffer = Convert.FromBase64String( b64 );

            return Encoding.UTF8.GetString( buffer );
        }

        public static void Write( string name, string value )
        {
            Directory.CreateDirectory( LocalPath );

            var filepath = Path.Combine( LocalPath, name );

            var buffer = Encoding.UTF8.GetBytes( value );
            var b64 = Convert.ToBase64String( buffer );

            File.WriteAllText( filepath, b64, encoding );
        }

        public static Configuration.ConfigurationItem Authorization { get; }
    }
}
