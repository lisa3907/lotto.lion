using Lion.Share.Controllers;
using Lion.Share.Data;
using Lion.Share.Data.DTO;
using Lion.Share.Options;
using Lion.Share.Pipe;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OdinSdk.BaseLib.Cryption;
using System.Text;
using System.Text.Json.Serialization;

namespace Lion.WebApi
{
    public class Program
    {
        private static SymmetricSecurityKey SigningKey
        {
            get; set;
        }

        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureLogging((hostContext, logging) =>
            {
                logging.ClearProviders()
                        .AddConsole()
                        .AddFile(
                            outputTemplate: "[{Timestamp:yyyy/MM/dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
#if DEBUG
                            minimumLevel: LogLevel.Debug,
#else
                            minimumLevel: LogLevel.Information,
#endif
                            pathFormat: "D:/project-data/LottoLION/Logger/WebAPI/log-{Date}.txt"
                        );
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddSingleton<WinnerReader>();
            builder.Services.AddSingleton<PrizeReader>();
            builder.Services.AddSingleton<PipeClient>(); 
            builder.Services.AddSingleton<CCryption>();
            builder.Services.AddSingleton<dForcast>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                                                //.AllowAnyOrigin()
                                                .AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .AllowCredentials());
            });


            // Make authentication compulsory across the board (i.e. shutdown EVERYTHING unless explicitly opened up).
            {
                SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Tokens:Key"]));

                // Get options from app settings
                var _jwt_appsettings = builder.Configuration.GetSection(nameof(JwtIssuerOptions));

                var _jwt_useroptions = new JwtIssuerOptions
                {
                    Issuer = _jwt_appsettings[nameof(JwtIssuerOptions.Issuer)],
                    Audience = _jwt_appsettings[nameof(JwtIssuerOptions.Audience)],
                    ValidFor = TimeSpan.FromMinutes(Convert.ToInt32(_jwt_appsettings[nameof(JwtIssuerOptions.ValidFor)])),
                    SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
                };

                builder.Services
                    .Configure<JwtIssuerOptions>(options =>
                    {
                        // For Injection => Configure JwtIssuerOptions
                        options.Issuer = _jwt_useroptions.Issuer;
                        options.Audience = _jwt_useroptions.Audience;
                        options.ValidFor = _jwt_useroptions.ValidFor;
                        options.SigningCredentials = _jwt_useroptions.SigningCredentials;
                    })
                    .AddMvc(config =>
                    {
                        var _policy = new AuthorizationPolicyBuilder()
                                        .RequireAuthenticatedUser()
                                        .Build();

                        config.Filters.Add(new AuthorizeFilter(_policy));
                    })
                    .AddJsonOptions(option =>
                    {
                        option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    });

                // For Authentication
                builder.Services
                    .AddAuthorization(options =>
                    {
                        // Use policy auth.
                        options.AddPolicy("LottoLionGuest",
                                policy =>
                                {
                                    policy.RequireClaim("UserType", "Guest");
                                });

                        options.AddPolicy("LottoLionMember",
                                policy =>
                                {
                                    policy.RequireClaim("UserType", "Member");
                                });

                        options.AddPolicy("LottoLionUsers",
                                policy =>
                                {
                                    policy.RequireRole("ValidUsers");
                                });
                    })
                    .AddAuthentication(sharedOptions =>
                    {
                        sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.SaveToken = true;

                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidIssuer = _jwt_useroptions.Issuer,

                            ValidateAudience = true,
                            ValidAudience = _jwt_useroptions.Audience,

                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = SigningKey,

                            RequireExpirationTime = true,
                            ValidateLifetime = true,

                            ClockSkew = TimeSpan.Zero
                        };
                    });
            }


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();

            // global policy - assign here or on each controller
            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}