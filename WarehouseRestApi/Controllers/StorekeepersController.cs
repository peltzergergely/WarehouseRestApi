using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Belgrade.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseRestApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Storekeepers")]
    public class StorekeepersController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public StorekeepersController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET: api/storekeepers
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select * from Storekeepers FOR JSON PATH", Response.Body, "[]");
        }

        // GET: api/dispatchers/name/password
        [HttpGet("{name}/{pw}")]
        public async Task Get(string name, string pw)
        {
            var cmd = new SqlCommand("dbo.StorekeeperLogin")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("pw", pw);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }
    }
}