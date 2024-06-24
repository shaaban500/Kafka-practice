using Producer.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ProducerService and ILogger
builder.Services.AddSingleton<ProducerService>(); // Add as Singleton if it's appropriate for your use case
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    // Add other logging configurations as needed
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
