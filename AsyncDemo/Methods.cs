using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    public static class Methods
    {
        public static List<WebsiteDataModel> RunSync()
        {
            List<string> webSites = PrepData();
            List<WebsiteDataModel> list = new List<WebsiteDataModel>();
            foreach (var site in webSites)
            {
                list.Add(DownloadWebsite(site));
            }

            return list;
        }

        public static async Task<List<WebsiteDataModel>> RunAsync(IProgress<ProgressReportModel> progress, CancellationToken cancellationToken)
        {
            List<string> webSites = PrepData();
            List<WebsiteDataModel> list = new List<WebsiteDataModel>();
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

        public static async Task<List<WebsiteDataModel>> RunParalelAsync()
        {
            List<string> webSites = PrepData();

            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (var site in webSites)
            {
                tasks.Add(DownloadWebsiteAsync(site)); // is the same with tasks.Add(Task.Run(() => DownloadWebsite(site)));
            }

            var response = await Task.WhenAll(tasks); // Call all task as parallel

            return new List<WebsiteDataModel>(response);
        }
        public static List<WebsiteDataModel> RunParallelSync()
        {
            List<string> webSites = PrepData();
            List<WebsiteDataModel> list = new List<WebsiteDataModel>();

            Parallel.ForEach<string>(webSites, (site) =>
            {
                list.Add(DownloadWebsite(site));
            });

            return list;
        }

        public static async Task<List<WebsiteDataModel>> RunParallelAyncWithForeach(IProgress<ProgressReportModel> progress)
        {
            List<string> webSites = PrepData();
            List<WebsiteDataModel> list = new List<WebsiteDataModel>();
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

        private static WebsiteDataModel DownloadWebsite(string site)
        {
            WebsiteDataModel output = new WebsiteDataModel()
            {
                WebsiteUrl = site
            };

            WebClient client = new WebClient();
            output.WebsiteData = client.DownloadString(site);

            return output;
        }

        private static async Task<WebsiteDataModel> DownloadWebsiteAsync(string site)
        {
            WebsiteDataModel output = new WebsiteDataModel()
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


    }
}
