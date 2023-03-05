// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aktabook.Diagnostics.HealthChecks;

public sealed class HealthCheckBackgroundService : BackgroundService
{
    private readonly CancellationTokenSource _listenerStoppingTokenSource;
    private readonly Thread _livenessListenerThread;
    private readonly ILogger<HealthCheckBackgroundService> _logger;
    private readonly Thread _readinessListenerThread;

    public HealthCheckBackgroundService(
        IReadinessListener readinessListener,
        ILivenessListener livenessListener,
        ILogger<HealthCheckBackgroundService> logger)
    {
        ArgumentNullException.ThrowIfNull(readinessListener);
        ArgumentNullException.ThrowIfNull(livenessListener);

        _listenerStoppingTokenSource = new CancellationTokenSource();
        readinessListener.SetStoppingToken(_listenerStoppingTokenSource.Token);
        livenessListener.SetStoppingToken(_listenerStoppingTokenSource.Token);

        _logger = logger;

        _readinessListenerThread =
            new Thread(new ThreadStart(MakeStartDelegate(readinessListener))) { IsBackground = true };

        _livenessListenerThread =
            new Thread(new ThreadStart(MakeStartDelegate(livenessListener))) { IsBackground = true };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        _readinessListenerThread.Start();
        _livenessListenerThread.Start();

#pragma warning disable CA1848
        _logger.LogInformation("Health check background service started");
#pragma warning restore CA1848
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _listenerStoppingTokenSource.Cancel();

        base.StopAsync(cancellationToken);

        _livenessListenerThread.Join();
        _readinessListenerThread.Join();

#pragma warning disable CA1848
        _logger.LogInformation("Health check background service stopped");
#pragma warning restore CA1848

        return Task.CompletedTask;
    }

    private static Action MakeStartDelegate(IHealthCheckEndpointServer healthCheckEndpointServer)
    {
        return () =>
        {
            try
            {
                healthCheckEndpointServer.StartServerAsync().GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                healthCheckEndpointServer.StopServer();
            }
        };
    }
}
