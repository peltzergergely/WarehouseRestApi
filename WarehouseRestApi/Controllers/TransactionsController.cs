using System;
using System.Collections.Generic;
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
            var cmd = new SqlCommand(
                                    @"insert into Transactions
                                        select *
                                        from OPENJSON(@Transactions)
                                      WITH(
                                           OrderId int,
                                           Gate int,
                                           Time nvarchar(50),
                                           Location int,
                                           Direction nvarchar(20),
                                           TimeStamp datetime,
                                           DispatcherId int
                                        )");
            cmd.Parameters.AddWithValue("Transactions", transaction);
            await SqlCommand.Exec(cmd);
        }

        //// PUT: api/Orders/5
        //[HttpPut("{id}")]
        //public async Task Put(int id)
        //{
        //    string transaction = new StreamReader(Request.Body).ReadToEnd();
        //    var cmd = new SqlCommand(
        //                            @"update Transactions
        //                                set Name = json.Name,
        //                                OwnerId = json.OwnerId,
        //                                Location = json.Location,
        //                                Status = json.Status
        //                            from OPENJSON( @transaction )
        //                                WITH(Name nvarchar(200), OwnerId int, 
        //                                Location int, Status(nvarchar20)) AS json
        //                                where Id = @id");
        //    cmd.Parameters.AddWithValue("id", id);
        //    cmd.Parameters.AddWithValue("Transactions", transaction);
        //    await SqlCommand.Exec(cmd);
        //}

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