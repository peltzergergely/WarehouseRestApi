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
    [Route("api/orders/")]
    public class OrdersController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public OrdersController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("select * from Orders FOR JSON PATH", Response.Body, "[]");
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task Get(int id)
        {
            var cmd = new SqlCommand("select * from Orders where Id = @id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("id", id);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // GET: api/Orders/5
        [HttpGet("costumer/{id}")]
        public async Task GetByOwnerId(int id)
        {
            var cmd = new SqlCommand("select * from Orders where CostumerId = @id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("id", id);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST: api/Orders
        [HttpPost]
        public async Task Post()
        {
            string orders = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.InsertOrder")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("Orders", orders);
            await SqlCommand.Exec(cmd);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {
            string orders = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.UpdateOrderById")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("orders", orders);
            await SqlCommand.Exec(cmd);
        }

        // DELETE api/Orders/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var cmd = new SqlCommand(@"delete Orders where Id = @id");
            cmd.Parameters.AddWithValue("id", id);
            await SqlCommand.Exec(cmd);
        }
    }
}