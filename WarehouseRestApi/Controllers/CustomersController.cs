using System;
using System.Collections.Generic;
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
    [Route("api/Customers/")]
    public class CustomersController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public CustomersController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET: api/costumers
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select * from Customers FOR JSON PATH", Response.Body, "[]");
        }

        // GET: api/costumers/name/password
        [HttpGet("{name}/{pw}")]
        public async Task Get(string name, string pw)
        {
            var cmd = new SqlCommand("dbo.CustomerLogin")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("name", name);
            cmd.Parameters.AddWithValue("pw", pw);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // PUT: api/costumers/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {
            string customer = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.PutCustomersById")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("customer", customer);
            await SqlCommand.Exec(cmd);
        }
    }
}