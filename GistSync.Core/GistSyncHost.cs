using System;
using System.Threading;
using System.Threading.Tasks;
using GistSync.Core.Extensions;
using GistSync.Core.Factories;
using GistSync.Core.Factories.Contracts;
using GistSync.Core.Services;
using GistSync.Core.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GistSync.Core;

public class GistSyncHost
{
    private IHost? _host;

    public async Task Start(string[] args = null!, Action<WebApplicationBuilder>? options = null, CancellationToken ct = default)
    {
        _host = CreateAppHost(args, options);

        using (_host)
            await _host.RunAsync(token: ct);
    }

    public async Task Stop()
    {
        if (_host != null)
            await _host.StopAsync();
    }

    private WebApplication CreateAppHost(string[] args, Action<WebApplicationBuilder>? options = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
        builder.Services.AddSingleton<IFileChecksumService, Md5FileChecksumService>();
        builder.Services.AddSingleton<IGitHubApiService, GitHubApiService>();
        builder.Services.AddSingleton<IAppDataService, DefaultAppDataService>();
        builder.Services.AddSingleton<ISyncTaskDataService, SyncTaskDataService>();
        builder.Services.AddSingleton<IGistWatchFactory, GistWatchFactory>();
        builder.Services.AddSingleton<IGistWatcherService, GistWatcherService>();
        builder.Services.AddSingleton<IFileWatchFactory, FileWatchFactory>();
        builder.Services.AddSingleton<IFileWatcherService, FileWatcherService>();
        builder.Services.AddSingleton<ISynchronizedFileAccessService, SynchronizedFileAccessService>();
        builder.Services.AddSingleton<INotificationService, DefaultNotificationService>();
        builder.Services.RegisterSyncStrategyProvider();

        builder.Services.AddDbContext<GistSyncDbContext>();

        builder.Services.AddControllers();
        builder.Services.AddRazorPages();

        // Background Services
        builder.Services.AddHostedService<GistSyncBackgroundService>();

        // Allow options add in
        options?.Invoke(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.MapRazorPages();
        app.MapControllers();
        app.UseBlazorFrameworkFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapFallbackToFile("index.html");
        });

        // Database migration
        using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var db = serviceScope.ServiceProvider.GetService<GistSyncDbContext>();
            if (db.Database.CanConnect())
            {
                db.Database.Migrate();
            }

            return app;
        }

        return app;
    }
}