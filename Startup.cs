namespace FirstAPI;
using FirstAPI.Services;
using FirstAPI.Services.Interfaces;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public class Startup {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration) {
        Configuration = configuration;

    }

    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],    
                    ValidAudience = Configuration["Jwt:Issuer"],    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))    
                };
            });
        services.AddMvc();
        services.AddCors();
        services.AddSingleton<IUserAccountManager, UserAccountManager>();
        services.AddSingleton<IBankAccountManager, BankAccountManager>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
        app.UseHttpsRedirection();
        app.UseCors(builder => builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod());
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}