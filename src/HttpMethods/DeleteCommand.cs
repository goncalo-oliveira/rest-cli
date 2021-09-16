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
    [Command( Name = "delete", Description = "Executes a DELETE operation" )]
    public class DeleteCommand : HttpCommand
    {
        public DeleteCommand( IRestClientFactory clientFactory )
        : base( clientFactory, HttpMethod.Delete )
        {}
    }
}
