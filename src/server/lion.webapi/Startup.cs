using LottoLion.BaseLib.Models.Entity;
using LottoLion.BaseLib.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OdinSdk.BaseLib.Configuration;
using OdinSdk.BaseLib.Cryption;
using System;
using System.Text;

namespace LottoLion.WebApi
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            var _builder = new ConfigurationBuilder()
                                .SetBasePath(env.ContentRootPath)
                                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                                .AddEnvironmentVariables()
                                .AddEncryptedProvider();

            Configuration = _builder.Build();
        }

        public IConfiguration Configuration
        {
            get;
        }

        private SymmetricSecurityKey SigningKey
        {
            get
            {
                return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Tokens:Key"]));
            }
        }

        private static string __connection_string = null;

        private string ConnectionString
        {
            get
            {
                if (__connection_string == null)
                {
                    var _key = Configuration["aes_key"];
                    var _iv = Configuration["aes_iv"];

                    var _cryptor = new CCryption(_key, _iv);
                    __connection_string = _cryptor.ChiperToPlain(Configuration.GetConnectionString("DefaultConnection"));
                }

                return __connection_string;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            services.AddSingleton(Configuration);

            services.AddDbContext<LottoLionContext>(builder =>
            {
                builder.UseNpgsql(ConnectionString);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                                                //.AllowAnyOrigin()
                                                .AllowAnyMethod()
                                                .AllowAnyHeader()
                                                .AllowCredentials());
            });

            // Add framework services.
            services.AddOptions();

            // Make authentication compulsory across the board (i.e. shutdown EVERYTHING unless explicitly opened up).
            {
                // Get options from app settings
                var _jwt_appsettings = Configuration.GetSection(nameof(JwtIssuerOptions));

                var _jwt_useroptions = new JwtIssuerOptions
                {
                    Issuer = _jwt_appsettings[nameof(JwtIssuerOptions.Issuer)],
                    Audience = _jwt_appsettings[nameof(JwtIssuerOptions.Audience)],
                    ValidFor = TimeSpan.FromMinutes(Convert.ToInt32(_jwt_appsettings[nameof(JwtIssuerOptions.ValidFor)])),
                    SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256)
                };

                services
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
                        option.JsonSerializerOptions.IgnoreNullValues = true;
                    });

                // For Authentication
                services
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            // global policy - assign here or on each controller
            app.UseCors("CorsPolicy");

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}