using System.IO.Abstractions;

using AsyncAwaitBestPractices;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;

using CommunityToolkit.Mvvm.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using Modthara.App.Routing;
using Modthara.App.Services;
using Modthara.App.ViewModels;
using Modthara.App.Views;
using Modthara.Essentials.Packaging;

namespace Modthara.App;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // https://github.com/AvaloniaUI/Avalonia/discussions/10239
        BindingPlugins.DataValidators.RemoveAt(0);

        ServiceProvider services = ConfigureServices();
        Ioc.Default.ConfigureServices(services);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow { DataContext = services.GetRequiredService<MainViewModel>() };
        }

        services
            .GetRequiredService<PackagesViewModel>()
            .InitializeViewModel()
            .SafeFireAndForget();

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<Router<ViewModelBase>>(s =>
            new Router<ViewModelBase>(v => (ViewModelBase)s.GetRequiredService(v)));

        services.AddScoped<IFileSystem, FileSystem>();
        services.AddTransient<IModPackageManager, ModPackageManager>();
        services.AddSingleton<IModsService>(s =>
            new ModsService(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                @"\Larian Studios\Baldur's Gate 3\Mods",
                s.GetRequiredService<IModPackageManager>(),
                s.GetRequiredService<IFileSystem>()));

        services.AddSingleton<IModSettingsService>(s => new ModSettingsService(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
            @"\Larian Studios\Baldur's Gate 3\PlayerProfiles\Public\modsettings.lsx",
            s.GetRequiredService<IFileSystem>()));

        services.AddSingleton<IModGridService, ModGridService>();
        services.AddSingleton<IOrderGridService, OrderGridService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<PackagesViewModel>();

        return services.BuildServiceProvider();
    }
}
