﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using NServiceBus;
using NServiceBus.Logging;

public class Runner : IWantToRunWhenBusStartsAndStops
{
    ILog Log = LogManager.GetLogger(typeof(Runner));
    public static CountdownEvent X;
    public IBus Bus { get; set; }
    private Task loopTask;
    private bool shutdown;
    private System.Threading.CancellationTokenSource stopLoop;

    // Message per second
    const string TimeUnit = "s";
    private readonly long TicksPerTimeUnit = Stopwatch.Frequency;

    // Message per hour
    //const string TimeUnit = "h";
    //private readonly long TicksPerTimeUnit = Stopwatch.Frequency * 3600;

    public void Start()
    {
        Log.Info("Starting...");
        stopLoop = new CancellationTokenSource();
        loopTask = Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
        Log.Info("Started");
    }

    void Loop(object o)
    {
        X = new CountdownEvent(256);

        long orderId = 0;

        Log.InfoFormat("IsServerGC:{0} ({1})", System.Runtime.GCSettings.IsServerGC, System.Runtime.GCSettings.LatencyMode);
        Log.InfoFormat("ProcessorCount: {0}", Environment.ProcessorCount);
        Log.InfoFormat("64bit: {0}", Environment.Is64BitProcess);

        Console.WriteLine("Press CTRL+C key to exit");
        var start = Stopwatch.StartNew();
        var interval = Stopwatch.StartNew();

        Log.Warn("Sleeping for the bus to purge the queue. Loop requires the queue to be empty.");
        Thread.Sleep(5000);

        while (!shutdown)
        {
            X.Reset();

            Console.Write("*");
            Parallel.For(0, X.InitialCount, new ParallelOptions{MaxDegreeOfParallelism = 5}, i =>
            {
                var id = Interlocked.Increment(ref orderId);
                Bus.SendLocal(new SubmitOrder
                {
                    OrderId = id.ToString(CultureInfo.InvariantCulture)
                });
            });

            try
            {
                X.Wait(stopLoop.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            var elapsed = interval.ElapsedTicks;
            interval.Restart();

            var currentPerMessageTicks = elapsed / X.InitialCount;
            var currentThroughput = TicksPerTimeUnit / currentPerMessageTicks;
            var averagePerMessageTicks = start.ElapsedTicks / orderId;
            var averageThroughput = TicksPerTimeUnit / averagePerMessageTicks;

            Console.Title = string.Format("{0:N0}/{5} ~{1:N0}/{5}  +{2:N0} @{3} p{4:N0}",
                currentThroughput,
                averageThroughput,
                orderId,
                start.Elapsed.Humanize(2),
                processedCount,
                TimeUnit
                );

            //Thread.Sleep(15000); // Should result in downscale of polling sql server threads
        }
    }

    public void Stop()
    {
        Log.Info("Stopping...");
        shutdown = true;
        using (stopLoop)
        {
            stopLoop.Cancel();
            using (loopTask)
            {
                loopTask.Wait();
            }
        }

        Log.Info("Stopped");
    }


    private static long processedCount;

    internal static void Signal()
    {
        Interlocked.Increment(ref processedCount);
        X.Signal();
    }
}
