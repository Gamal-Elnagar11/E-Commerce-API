
using E_Commerce_API.Controllers;
using E_Commerce_API.Data;
using E_Commerce_API.Mapping;
using E_Commerce_API.Models;
using E_Commerce_API.OpenAPI;
using E_Commerce_API.Reposatory.Implementation;
using E_Commerce_API.Reposatory.Interface;
using E_Commerce_API.Service.Implementation;
using E_Commerce_API.Service.Interface;
 using E_Commerce_API.Static;
using E_Commerce_API.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;


//"conn": "Server=db44691.public.databaseasp.net; Database=db44691; User Id=db44691; Password=A+h2q7@E6!Cb; Encrypt=False; MultipleActiveResultSets=True;"
//        // "conn": "Server=db44540.public.databaseasp.net; Database=db44540; User Id=db44540; Password=8d@L-Ex7Zm5?; Encrypt=False; MultipleActiveResultSets=True;"
//        // "conn": "server=.; database = E-Commerce; integrated security = true; trustservercertificate=true"
//    },

namespace E_Commerce_API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<Application>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

            // تسجيل Redis Distributed Cache
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "Ecommerce_";  
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            }); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {


                opt.OperationFilter<OpenApiSpecification>();
                opt.CustomOperationIds(apiDesc =>
                { 
                    var name = apiDesc.ActionDescriptor.AttributeRouteInfo?.Name;
                    if (!string.IsNullOrEmpty(name)) return name;
                    return apiDesc.RelativePath?.Split('/').Last();
                });
                //opt.CustomOperationIds(apiDesc =>
                //{
                //    var controllerName = apiDesc.ActionDescriptor.RouteValues["controller"];
                //    var actionName = apiDesc.ActionDescriptor.RouteValues["action"];
                //    return $"{controllerName}_{actionName}";
                //});
                //opt.CustomOperationIds(options =>
                //    {
                //        return options.RelativePath?.Split('/').Last();
                //    });


                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                // here create jwt in header for evvery endpoint
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            opt.SwaggerDoc("v1", new OpenApiInfo()   // here V1 -> version 1 to api after audated on this you can set V2
            {
                Version = "v1",
                Title = "E-Commerce API",
                Description = "api to manage project e-commerce",
                TermsOfService = new Uri("http://tempuri.gamal"),    // here linke to page has info to api
                Contact = new OpenApiContact                         // here info to owner
                {
                    Name = "Gamal saeed",
                    Email = "gamalelnagar@gmail.com"
                }

            });
        });


            var jwtSettings = builder.Configuration.GetSection("JWT");
           
            // إضافة Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,                     // تحقق من الـ Issuer
                    ValidateAudience = true,                   // تحقق من الـ Audience
                    ValidateLifetime = true,                    // تحقق من انتهاء الصلاحية
                    ValidateIssuerSigningKey = true,           // تحقق من التوقيع
                    ValidIssuer = jwtSettings["Issuer"],       // من appsettings.json
                    ValidAudience = jwtSettings["Audience"],   // من appsettings.json
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
                };
            });

            builder.Services.AddAuthorization();
 
            builder.Services.AddHttpContextAccessor();


            builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());
             builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<Application>()
                .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // لو حاول يروح Login redirect خلي الاستجابة 401
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };

                // لو حاول يروح AccessDenied redirect خلي الاستجابة 403
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });
            builder.Services.AddScoped<JWTService>();
             builder.Services.AddScoped<IUnitOfWork, UnitOfWork_Implement>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepo, OrderRepo>();
            builder.Services.AddScoped<IFAQService,FAQService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IdentityUsers>();
            builder.Services.AddHttpClient();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UserOrAdmin", policy =>
                    policy.RequireRole("User", "Admin"));
            });
           
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }
                app.UseSwagger();
                app.UseSwaggerUI();
            

            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    await SeedData.CreateRoles(services);
            //    await SeedData.CreateAdmin(services);
            //}

            app.UseStaticFiles();
           // app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAll");

            app.MapControllers();

            app.Run();
        }
    }
}
