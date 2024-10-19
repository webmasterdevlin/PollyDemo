using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using PollyDemo.FacadeApi.Policies;
using PollyDemo.FacadeApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// builder.Services.AddYourClientPolicy();

builder.Services.AddHttpClient("YourClient").AddStandardResilienceHandler();

builder.Services.AddResiliencePipeline("default", x =>
{
    x.AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            Delay = TimeSpan.FromSeconds(5),
            MaxRetryAttempts = 5,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true
        })
        .AddTimeout(TimeSpan.FromSeconds(30));
    x.AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        BreakDuration = TimeSpan.FromSeconds(30),
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* CORS Policy */
app.UseCors(b =>
{
    b.AllowAnyOrigin();
    b.AllowAnyHeader();
    b.AllowAnyMethod();
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();