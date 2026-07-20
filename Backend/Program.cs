using System.Text.Json.Serialization;
using Backend.Filters;
using Backend.Hubs;
using Backend.Mapper;
using Backend.Models;
using Backend.Services;
using Backend.StaticHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

var corsString = builder.Configuration["CorsSettings:AllowedOrigins"];
string[] allowedOrigins = [];

if (!string.IsNullOrWhiteSpace(corsString))
{
    allowedOrigins = corsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
}
else
{
    allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? [];
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<StimmtiDbContext>(options =>
    options.UseMySQL(connectionString!));

builder.Services.AddOpenApi();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.DocumentFilter<IncludeEnumDocumentFilter>();
});

builder.Services.AddSingleton<IApiMapper, ApiMapper>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<StimmtiDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddTransient<IAnswerService, AnswerService>();
builder.Services.AddTransient<IParticipantService, ParticipantService>();
builder.Services.AddTransient<ISessionService, SessionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseWebSockets();

app.UseCors();

var avatarsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads", "avatars");

if (!Directory.Exists(avatarsPath))
{
    Directory.CreateDirectory(avatarsPath);
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(avatarsPath),
    RequestPath = "/uploads/avatars"
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<DefaultHub>("/defaulthub");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StimmtiDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error migrating database");
    }
}

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<StimmtiDbContext>();
            DatabaseSeeder.SeedUsers(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error filling database with dummy data");
        }
    }
}

app.Run();
