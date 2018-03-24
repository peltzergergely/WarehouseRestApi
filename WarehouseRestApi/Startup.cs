using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Belgrade.SqlClient;
using Belgrade.SqlClient.SqlDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// made with the help of the following tutorial: https://www.codeproject.com/Articles/1106622/Building-REST-services-with-ASP-NET-Core-Web-API-a
/// to create an executable run from console: dotnet publish -c Release -r win10-x64
/// also add <RuntimeIdentifiers>win10-x64</RuntimeIdentifiers> to the .csproj file
/// for the database use the following in T-SQL (obviously change the connstring):
/// CREATE TABLE [dbo].[Items] (
///[Id] INT IDENTITY(1, 1) NOT NULL,
///[Name]  NVARCHAR(200) NOT NULL,
///[Owner] NVARCHAR(100) NOT NULL,
///[Pos]   INT NOT NULL,
///    PRIMARY KEY CLUSTERED([Id] ASC)
///);
///
/// </summary>

namespace WarehouseRestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            const string ConnString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=WarehouseDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            services.AddTransient<IQueryPipe>(_ => new QueryPipe(new SqlConnection(ConnString)));
            services.AddTransient<ICommand>(_ => new Command(new SqlConnection(ConnString)));

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{category}/{id}",
            //    defaults: new { category = "all", id = RouteParameter.Optional }
            //    );
            //{controller}/{action}/{id}

            //routes.MapHttpRoute("DefaultApiWithId", "Api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            //routes.MapHttpRoute("DefaultApiWithAction", "Api/{controller}/{action}");
            //routes.MapHttpRoute("DefaultApiGet", "Api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //routes.MapHttpRoute("DefaultApiPost", "Api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

//            public class TestController : ApiController
//        {
//            public string Get()
//            {
//                return string.Empty;
//            }

//            public string Get(int id)
//            {
//                return string.Empty;
//            }

//            public string GetAll()
//            {
//                return string.Empty;
//            }

//            public void Post([FromBody]string value)
//            {
//            }

//            public void Put(int id, [FromBody]string value)
//            {
//            }

//            public void Delete(int id)
//            {
//            }
//        }
//        I verified that it supports the following requests:

//          GET /Test
//          GET /Test/1
//          GET /Test/GetAll
//          POST /Test
//          PUT /Test/1
//          DELETE /Test/1
        }
    }
}
