using Avalonia;
using System;
using Velopack;

namespace VelopackTest.Desktop;

class Program
{
    public static FileLogger? Log { get; private set; }
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            // Logging is essential for debugging! Ideally you should write it to a file.
            //Log = new MemoryLogger();
            Log = new FileLogger("Log.txt");

            // It's important to Run() the VelopackApp as early as possible in app startup.
            VelopackApp.Build()
                .WithFirstRun((v) =>
                {
                    //MessageBox.Show("Thanks for installing my application!");
                })
                .Run(Log);

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            string message = "Unhandled exception: " + ex.ToString();
            Console.WriteLine(message);
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

}
