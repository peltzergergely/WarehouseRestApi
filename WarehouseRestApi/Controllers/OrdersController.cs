﻿using System;
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
    [Route("api/Orders")]
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

        // POST: api/Orders
        [HttpPost]
        public async Task Post()
        {
            string orders = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                    @"insert into Orders
                                        select *
                                        from OPENJSON(@orders)
                                      WITH(CostumerId int,
                                           ItemName nvarchar(50),
                                           Quantity int,
                                           Status nvarchar(20),
                                           Direction nvarchar(20),
                                           TimeStamp datetime
                                        )");
            cmd.Parameters.AddWithValue("Orders", orders);
            await SqlCommand.Exec(cmd);
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task Put(int id)
        {
            string orders = new StreamReader(Request.Body).ReadToEnd();
            var cmd = new SqlCommand(
                                    @"update orders
                                        set CostumerId = json.CostumerId,
                                        ItemName = json.ItemName,
                                        Quantity = json.Quantity,
                                        Status = json.Status,
                                        Direction = json.Direction,
                                        TimeStamp = json.TimeStamp
                                    from OPENJSON( @orders )
                                        WITH(CostumerId int,
                                           ItemName nvarchar(50),
                                           Quantity int,
                                           Status nvarchar(20),
                                           Direction nvarchar(20),
                                           TimeStamp datetime) AS json
                                        where Id = @id");
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