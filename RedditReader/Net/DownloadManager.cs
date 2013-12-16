using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RedditReader.Net
{
    class DownloadManager : IDisposable
    {
        public struct Download
        {
            public Download(Uri url, string dest)
            {
                this.URL = url;
                this.Destination = dest;
            }

            /// <summary>
            /// The URL to download
            /// </summary>
            public Uri URL;

            // The destination to place the file
            public string Destination;
        }

        private ConcurrentQueue<Download> downloads = new ConcurrentQueue<Download>();
        private static int max_concurrent;
        private static int current = 0;
        private bool status = false;
        private List<System.Net.WebClient> downloader; 
        private Timer timer;

        public DownloadManager(int concurrent = 2, double interval = 100)
        {
            max_concurrent = concurrent;
            this.ConstructDownloaders();
            timer = new Timer(interval);
            timer.Elapsed += DispatchDownload;
        }

        /// <summary>
        /// Builds a list of webclients to use for downloading
        /// </summary>
        private void ConstructDownloaders()
        {
            downloader = new List<System.Net.WebClient>(max_concurrent);

            for (int i = 0; i < max_concurrent; i++) {
                downloader.Add(new System.Net.WebClient());
                downloader[i].DownloadFileCompleted += DownloadFileCompleted;
                downloader[i].DownloadProgressChanged += DownloadProgressChanged;
            }
        }

        /// <summary>
        /// Dispatches the download the first available downloader
        /// </summary>
        /// <param name="?"></param>
        private void DispatchToFirstAvailable(Download download)
        {
            foreach (var dl in downloader) {
                if (!dl.IsBusy) {
                    dl.DownloadFileAsync(download.URL, download.Destination);
                    break;
                }
            }
        }

        void DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            
        }

        void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Threading.Interlocked.Add(ref current, -1);
        }

        void DispatchDownload(object sender, ElapsedEventArgs e)
        {
            if (IsRunning())
            {
                Action action = () =>
                    {
                        if (current < max_concurrent)
                        {
                            System.Threading.Interlocked.Add(ref current, 1);
                            Download download;
                            if (downloads.TryDequeue(out download))
                            {
                                // Try to download the file
                                DispatchToFirstAvailable(download);
                            }
                        }
                    };

                action.Invoke();
            }
        }


        public void AddDownload(Uri url, String destination)
        {
            downloads.Enqueue(new Download(url, destination));

        }
        
        //
        public bool IsRunning()
        {
            return status;
        }
        public void Start() {
            status = true;
            timer.Start();
        }
        public void Stop() {
            status = false;
            timer.Stop();
        }

        #region Download Complete
        public class DownloadCompleteArgs : EventArgs
        {
            public string Id;
            public DownloadCompleteArgs(string Id)
            {
                this.Id = Id;
            }
        }
        public EventHandler<DownloadCompleteArgs> DownloadComplete;
        public virtual void OnDownloadComplete(DownloadCompleteArgs e)
        {
            if (DownloadComplete != null)
                DownloadComplete(this, e);
        }
        #endregion

        /// <summary>
        /// Waits for all open connections to close and then disposes this gracefully! 
        /// </summary>
        public void Dispose()
        {
            this.Stop();
            bool isBusy = false;
            do {
                this.Stop();
                isBusy = false;

                downloader.ForEach(dl =>
                {
                    if (dl.IsBusy)
                        isBusy = true;
                    else
                        dl.Dispose();
                });
            }  while (isBusy);
        }
    }
}
