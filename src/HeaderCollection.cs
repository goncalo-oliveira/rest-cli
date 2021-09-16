using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestCli
{
    public class HeaderCollection : Dictionary<string, string>
    {
        private const string headerRegex = @"([\w-]+): (.*)";
        private const string variableRegex = @"([\w-]+)=(.*)";

        public HeaderCollection()
        : base( StringComparer.OrdinalIgnoreCase )
        {}

        public static HeaderCollection Parse( IEnumerable<string> values )
        {
            var headers = TryParse( values, out var errors );

            if ( errors.Any() )
            {
                throw new FormatException( string.Join( "\n", errors ) );
            }

            return ( headers );
        }

        public static HeaderCollection TryParse( IEnumerable<string> values, out IEnumerable<string> errors )
        {
            var headers = new HeaderCollection();
            var failed = new List<string>();

            foreach ( var value in values )
            {
                // parse with format: 'name: value'
                if ( headers.Parse( value, headerRegex ) )
                {
                    continue;
                }

                // parse with format: 'name=value'
                if ( headers.Parse( value, variableRegex ) )
                {
                    continue;
                }

                failed.Add( $"Invalid header: '{value}'" );
            }

            errors = failed.Any() ? failed.ToArray() : Enumerable.Empty<string>();

            return ( headers );
        }
    }

    internal static class HeaderCollectionExtensions
    {
        public static bool Parse( this HeaderCollection source, string headerValue, string pattern )
        {
            var match = Regex.Match( headerValue, pattern );

            if ( match.Success )
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;

                source.Add( key, value );

                return ( true );
            }

            return ( false );
        }
    }
}
