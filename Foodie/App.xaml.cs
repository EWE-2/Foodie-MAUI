using System.Diagnostics;

namespace Foodie;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
#if ANDROID
        MainPage = new AppShellMobile();
#elif  WINDOWS
        MainPage = new AppShell();
#endif  
        // Handle unhandled exceptions
        AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
    }
    private void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        Debug.WriteLine($"********** UNHANDLED EXCEPTION! Details: {e.Exception} | {e.Exception.InnerException?.Message} | {e.Exception.Source} " +
            $"| {e.Exception.StackTrace} | {e.Exception.TargetSite} || {e.Exception.Message} || {e.Exception.Data.Values} {e.Exception.HelpLink}");

        //var home = IPlatformApplication.Current!.Services.GetService<HomePageVM>();
        //await home.ExitingApp();
        LogException(e.Exception);
    }
    private static readonly object _logLock = new object();
    private void LogException(Exception ex)
    {

        try
        {
            // Define the directory path
            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DimmerCrashLogs");

            // Ensure the directory exists; if not, create it
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            string filePath = Path.Combine(directoryPath, "crashlog.txt");
            string logContent = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\nMsg:{ex.Message}\nStackTrace:{ex.StackTrace}\n\n";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            // Retry mechanism for file writing
            bool success = false;
            int retries = 3;
            int delay = 500; // Delay between retries in milliseconds

            lock (_logLock)
            {
                while (retries-- > 0 && !success)
                {
                    try
                    {
#if RELEASE
                        File.AppendAllText(filePath, logContent);
                        success = true; // Write successful
#endif
                    }
                    catch (IOException ioEx) when (retries > 0)
                    {
                        Debug.WriteLine($"Failed to log, retrying... ({ioEx.Message})");
                        Thread.Sleep(delay); // Wait and retry
                    }
                }

                if (!success)
                {
                    Debug.WriteLine("Failed to log exception after multiple attempts.");
                }
            }
        }
        catch (Exception loggingEx)
        {
            Debug.WriteLine($"Failed to log exception: {loggingEx}");
        }
    }

}
