using ImageService.Data;
using ImageService.Extensions;
using ImageService.Models;
using ImageService.Schemas;
using ImageService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = TokenAuthOption.Issuer,
        ValidateAudience = true,
        ValidAudience = TokenAuthOption.Audience,
        ValidateLifetime = true,
        IssuerSigningKey = TokenAuthOption.GetSymmetricSecurityKey(),
        ValidateIssuerSigningKey = true,
    };
});


builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IMinioClient, MinioClient>(s =>
{
    var accessKey = builder.Configuration.GetConnectionString("MINIO_ACCESS_KEY");
    var secretKey = builder.Configuration.GetConnectionString("MINIO_SECRET_KEY");
    var endpoint = builder.Configuration.GetConnectionString("MINIO_ENDPOINT");

    return new MinioClient()
        .WithCredentials(accessKey, secretKey)
        .WithEndpoint(endpoint, 9000)
        .Build();
});
builder.Services.AddScoped<IImageService, MinioImageService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    });

    options.OperationFilter<AuthorizeCheckOperationFilter>();
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .SetIsOriginAllowed(_ => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    db.Database.EnsureCreated();
    //db.Database.Migrate();
}

app.Run();