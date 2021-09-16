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
    [Command( Name = "put", Description = "Executes a PUT operation" )]
    public class PutCommand : SendCommand
    {
        public PutCommand( IRestClientFactory clientFactory )
        : base( clientFactory, HttpMethod.Put )
        { }
    }
}
