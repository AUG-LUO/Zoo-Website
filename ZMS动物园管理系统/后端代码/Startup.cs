/*-----------------------------------------
 * ���ߣ�����
 * ����޸ģ�2021.7.16
 * ���ܣ����ó��򣬰������ݿ������ַ�����swagger�����token��Ȩ������api·��
 ----------------------------------------*/
using BackEnd.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace BackEnd
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

            services.AddControllers();

            services.AddHttpContextAccessor();

            services.AddAuthorization(config =>
            {
                config.AddPolicy("EmployeePolicy", o =>
                {
                    o.RequireRole("Employee", "Admin").Build();
                });

                config.AddPolicy("AdminPolicy", o =>
                 {
                     o.RequireRole("Admin").Build();
                 });

                config.AddPolicy("TouristPolicy", o =>
                {
                    o.RequireRole("Tourist").Build();
                });
            });

            services.AddAuthentication(config =>
            {
                //��֤��������ΪJwt
                config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;  //����token
                config.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://47.118.78.110:80/",
                    ValidateAudience = true,
                    ValidAudience = "api",
                    ValidateIssuerSigningKey = true, //��֤ǩ������Կ
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is a test token secret key")), //ǩ������Կ
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
            });
            //������token����ע��Ϊ����
            services.AddSingleton<IJwtAuthenticationHandler>(new JwtAuthenticationHandler("this is a test token secret key"));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BackEnd", Version = "v1" });
            });

            services.AddDbContext<ModelContext>(options => options.UseOracle(Configuration.GetConnectionString("BackEndContext")));

            services.AddCors(options => options.AddPolicy("cors", p =>
            p.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("cors");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BackEnd v1"));
        }
    }
}

//Scaffold-DbContext "USER ID=SYSTEM;password=SJKyszh2021;DATA SOURCE=47.118.78.110:1521/orcl;PERSIST SECURITY INFO=True" Oracle.EntityFrameworkCore -OutputDir "E:\���ݿ�\����Ŀ\��ҵ4\��ʽ\BackEnd\Model"