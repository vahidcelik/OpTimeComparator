using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AsyncDemo
{
    public static class Methods
    {
        public static List<DataModel> RunSync()
        {
            List<string> webSites = PrepData();
            List<DataModel> list = new List<DataModel>();
            foreach (var site in webSites)
            {
                list.Add(DownloadWebsite(site));
            }

            return list;
        }

        public static async Task<List<DataModel>> RunAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            List<string> webSites = PrepData();
            List<DataModel> list = new List<DataModel>();
            ProgressReportModel report = new ProgressReportModel();

            foreach (var site in webSites)
            {
                list.Add(await DownloadWebsiteAsync(site)); //Task.Run(() => DownloadWebsite(site));

                cancellationToken.ThrowIfCancellationRequested();

                report.SitesDownloaded = list;
                report.PercentageComplete = (list.Count * 100 / webSites.Count);
                progress.Report(report);
            }

            return list;
        }

        public static async Task<List<DataModel>> RunParallelAsync()
        {
            List<string> webSites = PrepData();

            List<Task<DataModel>> tasks = new List<Task<DataModel>>();

            foreach (var site in webSites)
            {
                tasks.Add(DownloadWebsiteAsync(site)); // is the same with tasks.Add(Task.Run(() => DownloadWebsite(site)));
            }

            var response = await Task.WhenAll(tasks); // Call all task as parallel

            return new List<DataModel>(response);
        }
        public static List<DataModel> RunParallelSync()
        {
            List<string> webSites = PrepData();
            List<DataModel> list = new List<DataModel>();

            Parallel.ForEach<string>(webSites, (site) =>
            {
                list.Add(DownloadWebsite(site));
            });

            return list;
        }

        public static async Task<List<DataModel>> RunParallelAyncWithForeach(IProgress<ProgressReportModel> progress)
        {
            List<string> webSites = PrepData();
            List<DataModel> list = new List<DataModel>();
            ProgressReportModel report = new ProgressReportModel();

            await Task.Run(() =>
            {
                Parallel.ForEach<string>(webSites, (site) =>
              {
                  list.Add(DownloadWebsite(site));
                  report.SitesDownloaded = list;
                  report.PercentageComplete = (list.Count * 100 / webSites.Count);
                  progress.Report(report);
              });
            });

            return list;
        }

        private static DataModel DownloadWebsite(string site)
        {
            DataModel output = new DataModel()
            {
                WebsiteUrl = site
            };

            WebClient client = new WebClient();
            output.WebsiteData = client.DownloadString(site);

            return output;
        }

        private static async Task<DataModel> DownloadWebsiteAsync(string site)
        {
            DataModel output = new DataModel()
            {
                WebsiteUrl = site
            };

            WebClient client = new WebClient();
            output.WebsiteData = await client.DownloadStringTaskAsync(site);

            return output;
        }

        private static List<string> PrepData()
        {
            List<string> output = new List<string>();

            output.Add("https://www.yahoo.com");
            //output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.cnn.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");

            return output;
        }

        public static void HideShowDetail(Button b, StackPanel sp)
        {
            if (sp.Visibility == Visibility.Collapsed)
            {
                sp.Visibility = Visibility.Visible;
                b.Content = "Hide Detail";
            }
            else
            {
                sp.Visibility = Visibility.Collapsed;
                b.Content = "More Detail";
            }
        }
    }
}
