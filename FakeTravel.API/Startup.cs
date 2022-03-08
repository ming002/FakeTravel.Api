using FakeTravel.API.Database;
using FakeTravel.API.Models;
using FakeTravel.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakeTravel.API
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
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(option => { 
                    var secretByte= Encoding.UTF8.GetBytes(Configuration["Authentication:SecreKey"]);
                    option.TokenValidationParameters = new TokenValidationParameters()
                    {
                        //��֤������
                        ValidateIssuer=true,
                        ValidIssuer= Configuration["Authentication:Issuer"],
                        //��֤������
                        ValidateAudience=true,
                        ValidAudience = Configuration["Authentication:Audience"],
                        //��֤�Ƿ����
                        ValidateLifetime=true,
                        //˽Կ���м���
                        IssuerSigningKey=new SymmetricSecurityKey(secretByte),
                    };
                });
            services.AddControllers(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;

            }).AddNewtonsoftJson(setupAction => {
                setupAction.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            })
              .AddXmlDataContractSerializerFormatters()
              .ConfigureApiBehaviorOptions(setupAction =>
              {
                  setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetail = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = "����ν",
                            Title = "������֤ʧ��",
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = "�뿴��ϸ˵��",
                            Instance = context.HttpContext.Request.Path
                        };
                        problemDetail.Extensions.Add("tranceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetail)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
              });//���xml��ʽ

            services.AddTransient<ITouristRouteReposity, TouristRouteReposity>();
            services.AddDbContext<AppDbContext>(option =>
            {
                //option.UseSqlServer("server=localhost;Database=FakeTravelDb;User Id=sa;Password=PassWord22!");
                option.UseSqlServer(Configuration["DbContext:ConnectionString"]);
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddRazorPages();
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
                app.UseExceptionHandler("/Error");
            }
            //ʹ��·��
            app.UseRouting();
            //�����֤
            app.UseAuthentication();
            //Ȩ����֤
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
