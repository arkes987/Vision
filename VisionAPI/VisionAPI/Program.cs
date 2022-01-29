using VisionAPI.Hubs;
using VisionAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddTransient<ImageScrapperService>();
builder.Services.AddSingleton<ImageHub>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://192.168.5.115:9003", "http://77.87.73.205:8080")
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("ClientPermission");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

var scrapper = app.Services.GetRequiredService<ImageScrapperService>();
scrapper.Run();
//app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ImageHub>("/hubs/imagestream");
});

app.Run();