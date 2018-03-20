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
    [Route("api/Items")]
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
            await SqlPipe.Stream("select * from Items FOR JSON PATH", Response.Body, "[]");
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task Get(int id)
        {
            var cmd = new SqlCommand("select * from Items where Id = @id FOR JSON PATH, WITHOUT_ARRAY_WRAPPER");
            cmd.Parameters.AddWithValue("id", id);
            await SqlPipe.Stream(cmd, Response.Body, "{}");
        }

        // POST: api/Items
        [HttpPost]
        public async Task Post()
        {
            string items = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                    @"insert into Items
                                        select *
                                        from OPENJSON(@items)
                                        WITH(Name nvarchar(200), Owner nvarchar(100), Pos int)");
            cmd.Parameters.AddWithValue("Items", items);
            await SqlCommand.Exec(cmd);
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {
            string items = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                    @"update Items
                                        set Name = json.Name,
                                        Owner = json.Owner,
                                        Pos = json.Pos
                                    from OPENJSON( @items )
                                         WITH(Name nvarchar(200), Owner nvarchar(100),
                                            Pos int) AS json
                                        where Id = @id");
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("items", items);
            await SqlCommand.Exec(cmd);
        }

        // PATCH api/Todo
        [HttpPatch]
        public async Task Patch(int id)
        {
            string items = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                @"update Items
                                     set Name = ISNULL(json.Name, Name),
                                     Owner = ISNULL(json.Owner, Owner),
                                     Pos = ISNULL(json.Pos, Pos)
                                from OPENJSON(@items)
                                WITH(Name nvarchar(200), 
                                     Owner nvarchar(100),
                                     Pos int) AS json
                                where Id = @id
                                ");
            cmd.Parameters.AddWithValue("id", id);
            cmd.Parameters.AddWithValue("items", items);
            await SqlCommand.Exec(cmd);
        }

        // DELETE api/Todo/5
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            var cmd = new SqlCommand(@"delete Items where Id = @id");
            cmd.Parameters.AddWithValue("id", id);
            await SqlCommand.Exec(cmd);
        }
    }
}
