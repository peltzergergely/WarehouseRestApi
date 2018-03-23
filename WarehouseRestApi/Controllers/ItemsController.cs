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
    [Route("api/Items/")]
    public class ItemsController : Controller
    {
        private readonly IQueryPipe SqlPipe;
        private readonly ICommand SqlCommand;

        public ItemsController(ICommand sqlCommand, IQueryPipe sqlPipe)
        {
            this.SqlCommand = sqlCommand;
            this.SqlPipe = sqlPipe;
        }

        // GET: api/Items
        [HttpGet]
        public async Task Get()
        {
            await SqlPipe.Stream("dbo.SelectAllItems", Response.Body, "[]");
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task GetById(int id)
        {
            var cmd = new SqlCommand("dbo.GetItemById")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST: api/Items
        [HttpPost]
        public async Task Post()
        {
            string items = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.InsertItem")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@items", items);
            await SqlCommand.Exec(cmd);
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {
            string items = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.PutItemById")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("items", items);
            await SqlCommand.Exec(cmd);
        }

        // PATCH api/Items
        [HttpPatch]
        public async Task Patch(int id)
        {
            string items = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand("dbo.PatchItemById")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("items", items);
            await SqlCommand.Exec(cmd);
        }

        // DELETE api/Todo/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var cmd = new SqlCommand("dbo.DeleteItemById")
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("id", id);
            await SqlCommand.Exec(cmd);
        }
    }
}
