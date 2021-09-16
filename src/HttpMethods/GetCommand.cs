using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Faactory.RestClient;
using McMaster.Extensions.CommandLineUtils;

namespace RestCli.HttpMethods
{
    [Command( Name = "get", Description = "Executes a GET operation" )]
    public class GetCommand : HttpCommand
    {
        public GetCommand( IRestClientFactory clientFactory )
        : base( clientFactory, HttpMethod.Get )
        {}
    }
}
