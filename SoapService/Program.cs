using CoreWCF;
using CoreWCF.Configuration;
using SoapService.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServiceModelServices();
builder.Services.AddSingleton<ILongJobService, LongJobService>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<LongJobService>();
    serviceBuilder.AddServiceEndpoint<LongJobService, ILongJobService>(
    new BasicHttpBinding(),
        "/LongJobService"
    );
});

app.Run();