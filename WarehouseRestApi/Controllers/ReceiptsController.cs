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
    [Route("api/Receipts")]
    public class ReceiptsController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public ReceiptsController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET: api/Receipts
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select * from Receipts FOR JSON PATH", Response.Body, "[]");
        }

        // POST: api/Receipts
        [HttpPost]
        public async Task Post()
        {
            string receipts = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.InsertReceipt")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("Receipts", receipts);
            await SqlCommand.Exec(cmd);
        }
    }
}