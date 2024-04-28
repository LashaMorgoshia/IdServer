using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Configure Identity Server 4
builder.Services.AddIdentityServer()
    .AddInMemoryClients(new List<Client>
        {
            new Client
            {
                ClientId = "your_client_id",
                ClientSecrets = { new Secret("your_client_secret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ClientCredentials, // Use Resource Owner Password (ROPC) flow for testing
                AllowedScopes = { "api1" } // Define the scopes your client can access
            }
        })
    .AddInMemoryIdentityResources(new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
    })
    .AddInMemoryApiResources(new List<ApiResource>
    {
        new ApiResource("api1", "My API"),
    })
    .AddInMemoryApiScopes(new List<ApiScope>
    {
        new ApiScope("api1", "My API"),
    })
    .AddTestUsers(new List<TestUser>
    {
        new TestUser
        {
            SubjectId = "1",
            Username = "testuser",
            Password = "password",
        }
    })
    .AddDeveloperSigningCredential();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:7264"; // Identity Server 4 Authority URL
        options.RequireHttpsMetadata = false;
        options.Audience = "api1";
    });

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1"));
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
