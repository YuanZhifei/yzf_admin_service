using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YZF.Common;
using YZFAdmin.Service;

namespace YZFAdmin
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

            #region 方法1
            services.AddDapper("YZFConnection", m =>
            {
                m.ConnectionString = Configuration.GetConnectionString("YZFConnection");
                m.DbType = DbStoreType.SqlServer;
            });
            //连接Oracle
            services.AddDapper("OracleConnection", m =>
            {
                m.ConnectionString = Configuration.GetConnectionString("OracleConnectionString");
                m.DbType = DbStoreType.Oracle;
            });
            #endregion

            services.AddCors(options =>
                options.AddPolicy("any",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        //                        .WithOrigins("") // 允许指定域
                        .AllowAnyOrigin() // 允许所有域
                                          //                        .AllowCredentials()
                        .WithExposedHeaders(new[] { "x-token-need-refresh", "x-token-need-relogin" })
                        
                        )
            );
            services.RegisterAllService(this.GetType());
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("any");
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            var conString = Configuration.GetConnectionString("YZFConnection");
            app.UserDapper(conString);
        }
    }
}
