using Cosmos.Application.Entities;
using Cosmos.Application.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
namespace WebApi.Extension
{
    public static class ExtensionService
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CosmosClient>(provider =>
            {
                var cosmosDbConfig = configuration.GetSection("CosmosDb");
                var account = cosmosDbConfig["Account"];
                var key = cosmosDbConfig["Key"];
                return new CosmosClient(account, key);
            });


            services.AddSingleton<ICosmosDbService<Product>>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new CosmosDbService<Product>(cosmosClient, configuration);
            });


            services.AddSingleton<ICosmosDbService<Category>>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new CosmosDbService<Category>(cosmosClient, configuration);
            });


            services.AddSingleton<ICosmosDbService<SubCategory>>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new CosmosDbService<SubCategory>(cosmosClient, configuration);
            });


            services.AddSingleton<IAuditRepository>(provider =>
            {
                var cosmosClient = provider.GetRequiredService<CosmosClient>();
                return new AuditRepository(cosmosClient, configuration);
            });
            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name", // Map to the claim you added
                    RoleClaimType = "roles"
                };
            });

            services.AddHttpContextAccessor();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubCategoryService, SubCategoryService>();
            services.AddScoped<IAuditService, AuditService>();

            return services;
        }
    }
}
