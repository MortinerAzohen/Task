using IntegrationService.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Enable controllers (Web API)
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});

// HttpClients for REST + SOAP
builder.Services.AddHttpClient("RestService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5190/"); // REST Service URL
});

builder.Services.AddHttpClient("SoapService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5238/"); // SOAP Service URL
    client.DefaultRequestHeaders.Add("Accept", "text/xml");
});

builder.Services.AddScoped<SoapHelper>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var client = httpClientFactory.CreateClient("SoapService");
    return new SoapHelper(client);
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
