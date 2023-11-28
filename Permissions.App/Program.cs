using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Nest;
using Permissions.Domain.Entities;
using Permissions.Domain.Services;
using Permissions.Infrastructure.SQLServer;
using Permissions.Infrastructure.SQLServer.Repositories;
using Permissions.Infrastructure.SQLServer.UnitOfWork;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DataBase Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"))
);

// ElasticSearch
var elasticsearchSettings = builder.Configuration.GetSection("Elasticsearch");
var elasticsearchUri = new Uri(elasticsearchSettings["Uri"]);
var defaultIndex = elasticsearchSettings["DefaultIndex"];

var settings = new ConnectionSettings(elasticsearchUri)
     .DefaultIndex(defaultIndex);

var elasticClient = new ElasticClient(settings);
builder.Services.AddSingleton<IElasticClient>(elasticClient);

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Serilog
var logger = new LoggerConfiguration()
 .ReadFrom.Configuration(builder.Configuration)
 .Enrich.FromLogContext()
 .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// Repositories
builder.Services.AddScoped<Permissions.Infrastructure.SQLServer.Repositories.IRepository<Permission>, PermissionRepository>();
builder.Services.AddScoped<Permissions.Infrastructure.SQLServer.Repositories.IRepository<PermissionType>, PermissionTypeRepository>();

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IElasticSearchService, ElasticSearchService>();

// Kafka producer
var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"];
var producerConfig = new ProducerConfig { BootstrapServers = kafkaBootstrapServers };

builder.Services.AddSingleton(producerConfig);
builder.Services.AddScoped<IKafkaProducerService, KafkaProducerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// DBInitializer
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

// GlobalHandlerException
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
