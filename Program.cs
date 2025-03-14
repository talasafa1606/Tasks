using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Task1Bank.Consumers;
using Task1Bank.Data;
using Task1Bank.Services;
using Task1Bank.UOF;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddOData(options => options.Select().Filter().OrderBy().Count().SetMaxTop(100));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BankDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DbContext, BankDBContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBankingService, BankingService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddMassTransit(x =>
{
    var config = builder.Configuration.GetSection("RabbitMQ");
    string? host = config["HostName"];
    string? username = config["UserName"] ?? "guest";
    string? password = config["Password"] ?? "guest";

    if (string.IsNullOrEmpty(host))
    {
        throw new ArgumentNullException(nameof(host), "RabbitMQ HostName is missing from configuration.");
    }

    x.AddConsumer<LogConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ReceiveEndpoint("logging-service", e =>
        {
            e.ConfigureConsumer<LogConsumer>(context);
        });
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();