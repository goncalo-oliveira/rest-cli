using System;

namespace RestCli.Configuration
{
    public class ConfigurationItem
    {
        public ConfigurationItem( string name )
        {
            Name = name;
        }

        protected string Name { get; }

        public void Clear() => LocalConfiguration.Clear( Name );
        
        public string Read() => LocalConfiguration.Read( Name );

        public void Write( string value ) => LocalConfiguration.Write( Name, value );
    }
}
