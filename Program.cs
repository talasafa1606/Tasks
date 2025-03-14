using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Task1Bank.Consumers;
using Task1Bank.Data;
using Task1Bank.Middlewares;
using Task1Bank.Services;
using Task1Bank.UOF;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddOData(options => options.Select().Filter().OrderBy().Count().SetMaxTop(100));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();

builder.Services.AddDbContext<BankDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<DbContext, BankDBContext>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBankingService, BankingService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<LanguageMiddleware>();
app.MapControllers();
app.Run();
