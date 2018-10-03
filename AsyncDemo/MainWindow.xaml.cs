using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        
        #region Executions

        private void executeSync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var results = Methods.RunSync();
            PrintResults(results, resultSync);

            resultSync.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";
        }

        private void executeParallelSync()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var results = Methods.RunParallelSync();
            PrintResults(results, resultParallelSync);

            resultSync.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";
        }

        private async Task executeAsync()
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += (sndr, args) => ReportProgress(sndr, args, pbParallelAsync, rParallelAsync);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var results = await Methods.RunAsync(progress, cts.Token);
            }
            catch (OperationCanceledException)
            {
                resultAsync.Text += $"Async operation cancelled. {Environment.NewLine}";
            }

            watch.Stop();

            resultAsync.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";
        }

        private async Task executeParallelAsync()
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += (sndr, args) => ReportProgress(sndr, args, pbParallelAsync, rParallelAsync);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = await Methods.RunParallelAsync();
            PrintResults(results, resultParallelAsync);

            watch.Stop();

            resultParallelAsync.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";
        }

        private async Task executeParallelSyncAsync()
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += (sndr, args) => ReportProgress(sndr, args, pbParallelAsync, rParallelAsync);

            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = await Methods.RunParallelAyncWithForeach(progress);
            PrintResults(results, resultParallelAsync);

            watch.Stop();

            resultParallelAsync.Text += $"Total Execution Time : {watch.ElapsedMilliseconds}";
        } 
        #endregion

        #region Detail Buttons

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            Methods.HideShowDetail(btnSync, spSync);
        }

        private void btnParallelSync_Click(object sender, RoutedEventArgs e)
        {
            Methods.HideShowDetail(btnParallelSync, spParallelSync);
        }

        private void btnAsync_Click(object sender, RoutedEventArgs e)
        {
            Methods.HideShowDetail(btnAsync, spAsync);
        }

        private void btnParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            Methods.HideShowDetail(btnParallelAsync, spParallelAsync);
        }

        private void btnParallelSyncAsync_Click(object sender, RoutedEventArgs e)
        {
            Methods.HideShowDetail(btnParallelSyncAsync, spParallelSyncAsync);
        }
        #endregion

        private void PrintResults(List<DataModel> results, TextBlock tb)
        {
            tb.Text = string.Empty;

            foreach (var item in results)
                tb.Text += $"{item.WebsiteUrl} downloaded: {item.WebsiteData.Length} characters long. {Environment.NewLine}";
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

        private void ReportProgress(object sender, ProgressReportModel e, ProgressBar pb, TextBlock tb)
        {
            pb.Value = e.PercentageComplete;
            PrintResults(e.SitesDownloaded, tb);
        }
    }
}
