using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.VisualBasic;
using System.Data.Common;
using System.Text;
using System;
using Velopack;
using Microsoft.Extensions.Logging;
using Velopack.Sources;

namespace VelopackTest.Views;

public partial class MainWindow : Window
{
    private UpdateManager? _updateManager;
    private UpdateInfo? _update;

    private const string UPDATEPATH = @"https://mysite/DownloadFiles/Desktop";

    private readonly FileLogger Log;


    public MainWindow()
    {
        InitializeComponent();

        Log = new FileLogger("Log.txt");

        _updateManager = new UpdateManager(UPDATEPATH, logger: Log);

        //_updateManager = new UpdateManager(new GitHubSource("https://github.com/NorMoll/VelopackTest");

        TextLog.Text = Log.ToString();
        //Log.LogUpdated += LogUpdated;
        UpdateStatus();
    }

    private async void BtnCheckUpdateClick(object sender, RoutedEventArgs e)
    {
        Working();
        try
        {
            // ConfigureAwait(true) so that UpdateStatus() is called on the UI thread
            _update = await _updateManager.CheckForUpdatesAsync().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Log.LogError($"Error Checking for Updates({ex.Message})...");

        }
        UpdateStatus();
    }

    private async void BtnDownloadUpdateClick(object sender, RoutedEventArgs e)
    {
        Working();
        try
        {
            // ConfigureAwait(true) so that UpdateStatus() is called on the UI thread
            await _updateManager.DownloadUpdatesAsync(_update, Progress).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            Log.LogError($"Error Downloading Updates({ex.Message})...");
        }
        UpdateStatus();
    }

    private void BtnRestartApplyClick(object sender, RoutedEventArgs e)
    {
        _updateManager.ApplyUpdatesAndRestart(_update);
    }

    private void Progress(int percent)
    {
        // progress can be sent from other threads
        Dispatcher.UIThread.InvokeAsync(() => {
            TextStatus.Text = $"Downloading ({percent}%)...";
        });
    }

    private void Working()
    {
        Log.LogInformation("");
        BtnCheckUpdate.IsEnabled = false;
        BtnDownloadUpdate.IsEnabled = false;
        BtnRestartApply.IsEnabled = false;
        TextStatus.Text = "Working...";
    }

    private void UpdateStatus()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Velopack version: {VelopackRuntimeInfo.VelopackNugetVersion}");
        sb.AppendLine($"This app version: {(_updateManager.IsInstalled ? _updateManager.CurrentVersion : "(n/a - not installed)")}");

        if (_update != null)
        {
            sb.AppendLine($"Update available: {_update.TargetFullRelease.Version}");
            BtnDownloadUpdate.IsEnabled = true;
        }
        else
        {
            BtnDownloadUpdate.IsEnabled = false;
        }

        if (_updateManager.IsUpdatePendingRestart)
        {
            sb.AppendLine("Update ready, pending restart to install");
            BtnRestartApply.IsEnabled = true;
        }
        else
        {
            BtnRestartApply.IsEnabled = false;
        }

        TextStatus.Text = sb.ToString();
        BtnCheckUpdate.IsEnabled = true;
    }
}
