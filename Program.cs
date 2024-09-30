using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApiRequestLogs.DbContexts;
using WebApiRequestLogs.Filters;
using WebApiRequestLogs.Middlewares;
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
builder.Services.AddOptions<RequestResponseLoggerOption>().
    Bind(builder.Configuration.GetSection("RequestResponseLogger")).ValidateDataAnnotations();
/*IOC*/
builder.Services.AddSingleton<IRequestResponseLogger, RequestResponseLogger>();
builder.Services.AddScoped<IRequestResponseLogModelCreator, RequestResponseLogModelCreator>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new RequestResponseLoggerActionFilter());
    options.Filters.Add(new RequestResponseLoggerErrorFilter());
});
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

/*error manage*/
app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
    var response = new { details = "An error occurred" };
    await context.Response.WriteAsJsonAsync(response);
}));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
