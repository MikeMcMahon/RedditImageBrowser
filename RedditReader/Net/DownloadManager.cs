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
        private ConcurrentQueue<Download> downloads = new ConcurrentQueue<Download>();
        private static int max_concurrent;
        private static int current = 0;
        private bool running = false;
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
            }
        }

        void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            System.Threading.Interlocked.Add(ref current, -1);
        }

        void DispatchDownload(object sender, ElapsedEventArgs e)
        {
            if (IsRunning()) {
                Action action = () =>
                    {
                        if (current < max_concurrent) {
                            System.Threading.Interlocked.Add(ref current, 1);
                            Download download;
                            if (downloads.TryDequeue(out download)) {
                                // Try to download the file
                                DispatchToFirstAvailable(download);
                            }
                        }
                    };

                action.Invoke();
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
                    // Create an assign a handler for relaying the dl is complete that will uregister the handler once it is complete
                    System.ComponentModel.AsyncCompletedEventHandler handler = null;
                    handler = delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                    {
                        this.DownloadComplete(this, new DownloadCompleteArgs(download.Id));
                        (sender as System.Net.WebClient).DownloadFileCompleted -= handler;
                    };

                    dl.DownloadFileCompleted += handler;
                    dl.DownloadFileAsync(download.URL, download.Destination);
                    break;
                }
            }
        }

        public void AddDownload(String id, Uri url, String destination)
        {
            downloads.Enqueue(new Download(url, destination, id));

        }

        /// <summary>
        /// Checks to determine if the system has been started or is running
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            if (running)
                return true;
            if (DownloaderBusy())
                return true;

            return false;
        }

        /// <summary>
        /// Checks allocated download slots to determine if the downloader is in a busy or active state
        /// </summary>
        /// <returns></returns>
        public bool DownloaderBusy()
        {
            foreach (var dl in downloader) {
                if (dl.IsBusy)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Starts the downloader and begins downloading files
        /// </summary>
        public void Start()
        {
            running = true;
            timer.Start();
        }

        /// <summary>
        /// Stops the downloader (NOT A PAUSE)
        /// </summary>
        public void Stop()
        {
            running = false;
            timer.Stop();
        }

        #region DownloadEvents
        public class DownloadCompleteArgs : EventArgs
        {
            public string Id;
            public DownloadCompleteArgs(string Id)
            {
                this.Id = Id;
            }
        }

        public class DownloadProgressChangedArgs : EventArgs
        {
            public string Id;
            public int TotalBytesReceived;
            public int TotalBytesExpected;

            public DownloadProgressChangedArgs(string id, int totalreceived, int totalexpected)
            {
                Id = id;
                TotalBytesReceived = totalreceived;
                TotalBytesExpected = totalexpected;
            }
        }

        /// <summary>
        /// Fires when the progress for a download changes
        /// </summary>
        public event EventHandler<DownloadProgressChangedArgs> DownloadProgressChanged;
        public virtual void OnDownloadProgressChanged(DownloadProgressChangedArgs e)
        {
            if (DownloadProgressChanged != null)
                DownloadProgressChanged(this, e);
        }

        /// <summary>
        /// Fires when a download is complete 
        /// </summary>
        public event EventHandler<DownloadCompleteArgs> DownloadComplete;
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
                isBusy = false;

                downloader.ForEach(dl =>
                {
                    if (dl.IsBusy)
                        isBusy = true;
                    else
                        dl.Dispose();
                });
            } while (isBusy);
        }
        
        public struct Download
        {
            public Download(Uri url, string dest, string id)
            {
                this.URL = url;
                this.Destination = dest;
                this.Id = id;
            }

            /// <summary>
            /// The URL to download
            /// </summary>
            public Uri URL;

            /// <summary>
            /// The destination to place the file
            /// </summary>
            public string Destination;

            /// <summary>
            /// The id to track this download
            /// </summary>
            public string Id;
        }
    }
}
