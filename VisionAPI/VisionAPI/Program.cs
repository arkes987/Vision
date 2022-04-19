using Quartz;
using VisionAPI.Hubs;
using VisionAPI.Jobs;
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
            .WithOrigins("http://192.168.5.115", "http://77.87.73.205:8080")
            .AllowCredentials();
    });
});

builder.Services.Configure<QuartzOptions>(options =>
{
    options.Scheduling.IgnoreDuplicates = true; // default: false
    options.Scheduling.OverWriteExistingData = true; // default: true
});

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    // base quartz scheduler, job and trigger configuration

    var deleteOldVideosJobSchedule = builder.Configuration["Cron:DeleteOldVideosCron"];

    if (!string.IsNullOrEmpty(deleteOldVideosJobSchedule))
    {
        q.ScheduleJob<DeleteOldVideosJob>(trigger => trigger
            .WithIdentity("DeleteOldVideosCron", "CronJobs")
            .WithCronSchedule(deleteOldVideosJobSchedule)
        );
    }
});

// ASP.NET Core hosting
builder.Services.AddQuartzServer(options =>
{
    // when shutting down we want jobs to complete gracefully
    options.WaitForJobsToComplete = true;
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