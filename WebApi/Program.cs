using System.Text.Json.Serialization;
using Infrastructure.Interfaces;
using Infrastructure.Persistence.DataContext;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi.Middlewares;

Log.Logger = new LoggerConfiguration().WriteTo.Console().WriteTo.File("logs/log.txt").CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IAnalyticService, AnalyticService>();
builder.Services.AddScoped<IAnalyticService2, AnalyticService2>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionMiddleware1>();
app.UseMiddleware<ExceptionMiddleware2>();
app.MapControllers();
app.Run();