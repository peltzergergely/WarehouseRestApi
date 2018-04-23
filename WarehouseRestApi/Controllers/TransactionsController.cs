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
    [Route("api/Transactions")]
    public class TransactionsController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public TransactionsController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select * from Transactions FOR JSON PATH", Response.Body, "[]");
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task Get(int id)
        {
            var cmd = new SqlCommand("select * from Transactions where Id = @id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("id", id);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST: api/Transactions
        [HttpPost]
        public async Task Post()
        {
            string transaction = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.InsertTransaction")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("transaction", transaction);
            await SqlCommand.Exec(cmd);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {
            string transaction = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.UpdateTransactionById")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("transaction", transaction);
            await SqlCommand.Exec(cmd);
        }

        // DELETE api/Transactions/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var cmd = new SqlCommand(@"delete Transactions where Id = @id");
            cmd.Parameters.AddWithValue("id", id);
            await SqlCommand.Exec(cmd);
        }
    }
}