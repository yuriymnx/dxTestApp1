using System.Globalization;
using System.Windows;

namespace dxTestApp1;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        CultureInfo culture = CultureInfo.CreateSpecificCulture("ru-RU");

        // The following line provides localization for the application's user interface. 
        Thread.CurrentThread.CurrentUICulture = culture;

        // The following line provides localization for data formats. 
        Thread.CurrentThread.CurrentCulture = culture;

        // Set this culture as the default culture for all threads in this application. 
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        base.OnStartup(e);
    }
}