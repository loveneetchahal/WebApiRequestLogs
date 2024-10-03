using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApiRequestLogs.DbContexts; 
using WebApiRequestLogs.Middlewares;
using WebApiRequestLogs.Models;
using WebApiRequestLogs.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. 
builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }));
/*Options*/
//builder.Services.AddOptions<RequestResponseLoggerOption>().
//    Bind(builder.Configuration.GetSection("RequestResponseLogger")).ValidateDataAnnotations();
/*IOC*/
builder.Services.AddTransient<RequestLogs>();
builder.Services.AddScoped<IRequestResponseLogger, RequestResponseLogger>(); 

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*Middleware*/
app.UseMiddleware<RequestResponseLoggerMiddleware>(); 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
