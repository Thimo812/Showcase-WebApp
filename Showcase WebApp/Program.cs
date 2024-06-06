using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Showcase_WebApp.data;
using Showcase_WebApp.data.DataAccessObjects;
using Showcase_WebApp.hubs;
using Showcase_WebApp.Managers;
using Showcase_WebApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://127.0.0.1:5501", "http://localhost:5501")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddSignalR()
    .AddHubOptions<GameHub>(o => o.MaximumParallelInvocationsPerClient = 5);

builder.Services.AddSingleton(typeof(GameManager));
builder.Services.AddSingleton(typeof(GameDAO));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");

app.MapGroup("/api/account").MapIdentityApi<IdentityUser>();

app.MapHub<GameHub>("game-hub");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
