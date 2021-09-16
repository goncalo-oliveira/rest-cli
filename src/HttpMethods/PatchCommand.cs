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
    [Command( Name = "patch", Description = "Executes a PATCH operation" )]
    public class PatchCommand : SendCommand
    {
        public PatchCommand( IRestClientFactory clientFactory )
        : base( clientFactory, HttpMethod.Patch )
        { }
    }
}
