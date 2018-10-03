using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Media;

namespace AsyncDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = await Methods.RunParallelAyncWithForeach(progress);
            PrintResults(results);

            watch.Stop();

            result.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var results = await Methods.RunAsync(progress, cts.Token);
            }
            catch (OperationCanceledException)
            {
                result.Text += $"Async operation cancelled. {Environment.NewLine}";
            }

            watch.Stop();

            result.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";

        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            progressBar.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var results = Methods.RunParallelSync();
            PrintResults(results);

            result.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";
        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            result.Text = string.Empty;

            foreach (var item in results)
                result.Text += $"{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters long. {Environment.NewLine}";
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }
    }

    public class WebsiteDataModel
    {
        public string WebsiteUrl { get; set; } = string.Empty;
        public string WebsiteData { get; set; } = string.Empty;
    }
}
