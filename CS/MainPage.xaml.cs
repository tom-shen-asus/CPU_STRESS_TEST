// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using System.Diagnostics;
using DeviceInfo.Utils;
using Windows.System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CPU
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private static bool isBurnCPU = false;
        private CancellationTokenSource ts;
        private AutoResetEvent awaitReplyOnRequestEvent = new AutoResetEvent(false);
        private Thread runningTaskThread;

        public MainPage()
        {
            InitializeComponent();
            routineWork();
        }

        private void ClickStart_Click(object sender, RoutedEventArgs e)
        {
            if (isBurnCPU) return;
            isBurnCPU = true;
            ts = new CancellationTokenSource();
            BurnCPU();
            progress.IsActive = true;
        }

        private void ClickStop_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
            isBurnCPU = false;
            CPU_Burn.Text = "press start to burning CPU";
            ts.Cancel();
            progress.IsActive = false;
        }

        private void ClickShutdown_Click(object sender, RoutedEventArgs e)
        {
            Shutdown(false);
        }

        private void BurnCPU()
        {
            CPU_Burn.Text = "Burning CPU";
            int cpuUsage = 97;
            int duration = 60 * 1000; //time in milliseconds
            int targetThreadCount = Environment.ProcessorCount;
            List<Thread> threads = new List<Thread>();
            //Ensure the current process takes presendence thus (hopefully) holidng the utilisation steady
            Process Proc = Process.GetCurrentProcess();
            Proc.PriorityClass = ProcessPriorityClass.RealTime;
            long AffinityMask = (long)Proc.ProcessorAffinity;

            Task task = Task.Factory.StartNew(async () =>
            {
                runningTaskThread = Thread.CurrentThread;
                for (int i = 0; i < targetThreadCount; i++)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(CPUKill));
                    t.Start(cpuUsage);
                    threads.Add(t);
                }
                while (isBurnCPU)
                {
                    Thread.Sleep(duration);
                }
                foreach (var t in threads)
                {
                    t.Abort();
                }
            }, ts.Token);      
        }

        void StopTask()
        {
            // Attempt to cancel the task politely
            if (ts != null)
            {
                if (ts.IsCancellationRequested)
                    return;
                else
                    ts.Cancel();
            }

            // Notify a waiting thread that an event has occurred
            if (awaitReplyOnRequestEvent != null)
                awaitReplyOnRequestEvent.Set();

            // If 1 sec later the task is still running, kill it cruelly
            if (runningTaskThread != null)
            {
                try
                {
                    runningTaskThread.Join(TimeSpan.FromSeconds(1));
                }
                catch (Exception ex)
                {
                    runningTaskThread.Abort();
                }
            }
        }

        public static void CPUKill(object cpuUsage)
        {
            Parallel.For(0, 1, new Action<int>((int i) =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                while (true)
                {
                    if (watch.ElapsedMilliseconds > (int)cpuUsage)
                    {
                        Thread.Sleep(100 - (int)cpuUsage);
                        watch.Reset();
                        watch.Start();
                    }
                }
            }));

        }

        private void routineWork()
        {
            var uiContext = TaskScheduler.FromCurrentSynchronizationContext();
            Task task = Task.Factory.StartNew(async () =>
            {
            int i = 0;
            int h = 0;
            int m = 0;
            int s = 0;
            while (true)
            {
                h = (i / 3600);
                m = (i - (3600 * h)) / 60;
                s = (i - (3600 * h) - (m * 60));
                Task.Factory.StartNew(async () =>
                {
                    IP_Address.Text = "IP: " + getIpV4Address();
                        CPU_Burn.Text = "Burning CPU: " + h +" h "+ m + " m " + s + " s ";
                        CPU_Usage.Text = await DeviceUtil.getCPULoad();
                        i = i + 1;
                    }, CancellationToken.None, TaskCreationOptions.None, uiContext);
                    Thread.Sleep(1000);
                }
            });
        }

        private string getIpV4Address()
        {
            string strHostName = "";
            //把HostName換成網址可查該網址對應的IP位址
            string name = Dns.GetHostName();
            IPAddress[] ip = Dns.GetHostEntry(name).AddressList;
            for (int i = 0; i < ip.Length; i++)
            {
                //System.Net.Sockets.AddressFamily.InterNetwork為IPv4位址
                //System.Net.Sockets.AddressFamily.InterNetworkV6為IPv6位址
                if (ip[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    strHostName += " " + ip[i].ToString();
                }
            }
            return strHostName;
        }

        private void Shutdown(bool restart = false)
        {
            ShutdownManager.BeginShutdown(restart ? ShutdownKind.Restart : ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
        }

    }

}
