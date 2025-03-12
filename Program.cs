using MassTransit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Task1Bank.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddOData(options => options.Select().Filter().OrderBy().Count().SetMaxTop(100));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BankDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var config = builder.Configuration.GetSection("RabbitMQ");
        string? host = config["HostName"];
        string? username = config["UserName"];
        string? password = config["Password"];

        if (string.IsNullOrEmpty(host))
        {
            throw new ArgumentNullException(nameof(host), "RabbitMQ HostName is missing from configuration.");
        }

        cfg.Host(host, "/", h =>
        {
            h.Username(username ?? "guest");
            h.Password(password ?? "guest");
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