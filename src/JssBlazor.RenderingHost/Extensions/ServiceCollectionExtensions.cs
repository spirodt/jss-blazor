using System;
using System.Net.Http;
using JssBlazor.Components.Models;
using JssBlazor.Components.Services;
using JssBlazor.Core.Models;
using JssBlazor.Core.Services;
using JssBlazor.RenderingHost.Models;
using JssBlazor.RenderingHost.Services;
using JssBlazor.Tracking;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JssBlazor.RenderingHost.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddJssBlazorRenderingHost(
            this IServiceCollection services,
            IConfiguration configuration,
            BlazorAppConfiguration blazorAppConfiguration)
        {
            services.AddBlazorServices();
            services.AddJssBlazorServices(configuration, blazorAppConfiguration);
        }

        private static void AddBlazorServices(this IServiceCollection services)
        {
            // Replace Blazor's out-of-the-box NavigationManager with one that correctly resolves URLs server side.
            services.AddScoped<NavigationManager, HardcodedRemoteNavigationManager>();

            // An HttpClient is registered out-of-the-box with Blazor WebAssembly.
            services.AddTransient(serviceProvider =>
            {
                var navigationManager = serviceProvider.GetRequiredService<NavigationManager>();
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(navigationManager.BaseUri)
                };
                return httpClient;
            });
        }

        private static void AddJssBlazorServices(
            this IServiceCollection services,
            IConfiguration configuration,
            BlazorAppConfiguration blazorAppConfiguration)
        {
            services.AddSingleton<ILayoutServiceResultProvider, StaticLayoutServiceResultProvider>();
            services.AddSingleton<ILayoutService, StaticLayoutService>();

            services.AddSingleton(blazorAppConfiguration);

            services.AddScoped<IPreRenderer, DefaultPreRenderer>();
            services.AddSingleton<IInitialStateLoader, ServerInitialStateLoader>();

            services.AddSingleton(_ => configuration.GetSection("ComponentFactory").Get<ComponentFactoryOptions>());
            services.AddSingleton<IComponentFactory, DefaultComponentFactory>();

            services.AddSingleton(_ => configuration.GetSection("SitecoreConfiguration").Get<SitecoreConfiguration>());
            services.AddJssBlazorTracking();
        }
    }
}
