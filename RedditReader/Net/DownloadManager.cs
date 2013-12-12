using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace RedditReader.Net
{
    class DownloadManager
    {
        public struct Download
        {
            public Uri URL;
            public string Destination;
        }

        private ConcurrentQueue<Download> downloads = new ConcurrentQueue<Download>();
        private static int max_concurrent;
        private static int current = 0;
        private bool status = false;
        private System.Net.WebClient downloader; 
        private Timer timer;

        public DownloadManager(int concurrent = 2, double interval = 500)
        {
            downloader = new System.Net.WebClient();
            downloader.DownloadFileCompleted += downloader_DownloadFileCompleted;
            downloader.DownloadProgressChanged += downloader_DownloadProgressChanged;
            max_concurrent = concurrent;
            timer = new Timer(interval);
            timer.Elapsed += timer_Elapsed;
        }

        void downloader_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            
        }

        void downloader_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
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
                                downloader.DownloadFileAsync(download.URL, download.Destination);
                            }
                        }
                    };
            }
        }



        public void AddDownload(Download dl)
        {
            downloads.Enqueue(dl);
        }

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

        
    }
}
