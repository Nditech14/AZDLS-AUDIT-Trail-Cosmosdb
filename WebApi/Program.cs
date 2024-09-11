using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using WebApi.Extension;
using Cosmos.Application.Services;
using Cosmos.Infrastructure.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterApplicationServices(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddControllers();

builder.Services.AddAzureDataLakeServices(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "B2C|ADLS", Version = "v1" });

    // Add OAuth2 Authentication to Swagger
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{builder.Configuration["AzureAdB2C:Instance"]}{builder.Configuration["AzureAdB2C:Domain"]}/oauth2/v2.0/authorize?p={builder.Configuration["AzureAdB2C:SignUpSignInPolicyId"]}"),
                TokenUrl = new Uri($"{builder.Configuration["AzureAdB2C:Instance"]}{builder.Configuration["AzureAdB2C:Domain"]}/oauth2/v2.0/token?p={builder.Configuration["AzureAdB2C:SignUpSignInPolicyId"]}"),
                Scopes = new Dictionary<string, string>
                {
                    { builder.Configuration["AzureAdB2C:Scopes:FileRead"]!, "Read access to tasks" },
                    { builder.Configuration["AzureAdB2C:Scopes:FileWrite"]!, "Write access to tasks" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { builder.Configuration["AzureAdB2C:Scopes:FileRead"], builder.Configuration["AzureAdB2C:Scopes:FileWrite"] }
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "B2CTemplate/ADLS V1");
        c.OAuthClientId(builder.Configuration["AzureAdB2C:SwaggerClientId"]);
        //c.OAuthClientSecret(builder.Configuration["AzureAdB2C:SwaggerClientSecret"]);
        c.OAuthAppName("Swagger UI");
        c.OAuthUsePkce();
        c.OAuth2RedirectUrl(builder.Configuration["AzureAdB2C:RedirectUri"]);
    });
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
