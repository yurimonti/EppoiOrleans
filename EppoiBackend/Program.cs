using Abstractions;
using EppoiBackend.Dtos;
using EppoiBackend.Services;
using StackExchange.Redis;

Console.WriteLine("Website: Waiting");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddSingleton<IStateToDtoConverter<IItineraryGrain,ItineraryStateDto>, ItineraryStateToDtoConverter>();
builder.Services.AddSingleton<IStateToDtoConverter<IPoiGrain,PoiStateDto>, PoiStateDtoConverter>();
builder.Services.AddSingleton<IPoiService,PoiService>();
builder.Services.AddSingleton<IItineraryService,ItineraryService>();

builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering();
    siloBuilder.UseDashboard();
    siloBuilder.AddRedisGrainStorageAsDefault(options =>
    {
        options.ConfigurationOptions = new ConfigurationOptions();
        options.ConfigurationOptions.EndPoints.Add("localhost", 6379);
    });
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
