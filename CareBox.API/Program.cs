
using CareBox.BLL.Repositories;
using CareBox.BLL.Repositories.Interfaces;
using CareBox.BLL.Services.AuthServices;
using CareBox.BLL.Services.AuthServices.Interfaces;
using CareBox.BLL.Services.AuthServices.Settings;
using CareBox.BLL.Services.ClientServices;
using CareBox.BLL.Services.ClientServices.Interfaces;
using CareBox.BLL.Services.EmailServices;
using CareBox.BLL.Services.EmailServices.Interfaces;
using CareBox.BLL.Services.EmailServices.Settings;
using CareBox.BLL.Services.FileServices;
using CareBox.BLL.Services.FileServices.Interfaces;
using CareBox.DAL.Contexts;
using CareBox.DAL.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CareBox.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region DbContext
            builder.Services.AddDbContext<CareBoxContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("CS"),
                    x => x.UseNetTopologySuite()
                ));
            #endregion

            #region Identity
            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<CareBoxContext>()
            .AddDefaultTokenProviders();
            #endregion


            #region AddScopeds

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IFileService, FileService>();
            builder.Services.AddScoped<IClientService, ClientService>();


            #endregion

            #region EmailServies

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


            builder.Services.AddTransient<IEmailService, EmailService>();
            #endregion

            #region JWT

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));

            #region JWT_AddAuthentication

            builder.Services.AddAuthentication(options =>
            {
                // تعريف الـ Default Scheme
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                // استدعاء الإعدادات للتأكد من إننا شغالين HTTPS (في الإنتاج خليه true)
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;

                // تعريف معايير التحقق من التوكين
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    // التحقق من مفتاح التشفير (أهم حاجة)
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,

                    // قراءة القيم من الـ Configuration
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),

                    // عشان يخلي وقت انتهاء التوكين دقيق جداً (بدون فترة سماح 5 دقايق)
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion

            #region Swagger_JWT

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyProject API", Version = "v1" });

                // تعريف الـ Bearer Auth
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                        }
                    });
            });

            #endregion

            #endregion





            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // هذا السطر سيخفي أي قيمة null من الرد
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            }); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();//<----- Add this line to enable authentication middleware  
            app.UseAuthorization();


            #region For Add Role
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

                    string[] roles = { "CLIENT", "SERVICEPROVIDER", "ADMIN" };

                    foreach (var role in roles)
                    {
                        try
                        {
                            if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                            {
                                var result = roleManager.CreateAsync(new IdentityRole<int> { Name = role }).GetAwaiter().GetResult();
                                if (!result.Succeeded)
                                {
                                    Console.WriteLine($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                                }
                                else
                                {
                                    Console.WriteLine($"Role '{role}' created successfully.");
                                }
                            }
                            else
                            {
                                Console.WriteLine($"Role '{role}' already exists.");
                            }
                        }
                        catch (Exception innerEx)
                        {
                            Console.WriteLine($"Error creating role '{role}': {innerEx.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("============= SEEDING ERROR =============");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("=========================================");
                }
            }

            #endregion

            app.UseStaticFiles(); // <-- دي بتخلي فولدر wwwroot متشاف
            app.MapControllers();

            app.Run();




        
        }
    }
}
