// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aktabook.Diagnostics.HealthChecks;

public class HealthCheckEndpointServer : IHealthCheckEndpointServer, IDisposable
{
    private static readonly Action<ILogger, HealthStatus, Exception?> HealthStatusLoggerMessage =
        LoggerMessage.Define<HealthStatus>(
            LogLevel.Error,
            new EventId(-1, nameof(HealthCheckEndpointServer)),
            "Service health status is: {HealthStatus}. Listener stopped");

    private static readonly Action<ILogger, HealthStatus, Exception?> HealthStatusWarningLoggerMessage =
        LoggerMessage.Define<HealthStatus>(
            LogLevel.Warning,
            new EventId(-1, nameof(HealthCheckEndpointServer)),
            "Service health status is: {HealthStatus}");

    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthCheckEndpointServer> _logger;
    private readonly HealthCheckTcpServiceOptions _options;
    private readonly TcpListener _tcpListener;

    private bool _disposed;
    private bool _isStarted;
    private CancellationToken _stoppingToken;

    public HealthCheckEndpointServer(
        HealthCheckService healthCheckService,
        ILogger<HealthCheckEndpointServer> logger,
        IOptions<HealthCheckTcpServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        _healthCheckService = healthCheckService;
        _logger = logger;
        _options = options.Value;
        _stoppingToken = CancellationToken.None;
        _tcpListener = new TcpListener(_options.IpAddress, _options.Port);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task StartServerAsync()
    {
#if DEBUG
#pragma warning disable CA1848
        _logger.LogDebug(
            "Health check endpoint {Name} started and configured to listen at {IP:l}:{Port}",
            _options.Name, _options.IpAddress, _options.Port);
#pragma warning restore CA1848
#endif

        _isStarted = true;

        while (!_stoppingToken.IsCancellationRequested)
        {
            await UpdateHeartbeatAsync(_stoppingToken).ConfigureAwait(false);

            await Task.Delay(_options.Interval, _stoppingToken).ConfigureAwait(false);
        }

        _tcpListener.Stop();

#pragma warning disable CA1848
        _logger.LogInformation("Health check TCP endpoint listener {Name} stopped listening to requests",
            _options.Name);
#pragma warning restore CA1848
    }

    public void StopServer()
    {
        _tcpListener.Stop();

#pragma warning disable CA1848
        _logger.LogInformation("Health check endpoint {Name} shut down", _options.Name);
#pragma warning restore CA1848
    }

    public void SetStoppingToken(CancellationToken stoppingToken)
    {
        if (_isStarted)
        {
            throw new InvalidOperationException(
                "Cannot set stopping token because listener has already started.");
        }

        _stoppingToken = stoppingToken;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _tcpListener.Dispose();
        }

        _disposed = true;
    }

    ~HealthCheckEndpointServer()
    {
        Dispose(false);
    }

    private async Task UpdateHeartbeatAsync(CancellationToken cancellationToken)
    {
        HealthReport healthReport = await _healthCheckService.CheckHealthAsync(x =>
            x.Tags.Overlaps(_options.Tags), cancellationToken).ConfigureAwait(false);

        switch (healthReport.Status)
        {
            case HealthStatus.Unhealthy:
                _tcpListener.Stop();

                HealthStatusLoggerMessage(_logger, healthReport.Status, null);
                return;

            case HealthStatus.Degraded:
                HealthStatusWarningLoggerMessage(_logger, healthReport.Status, null);
                break;
        }

        _tcpListener.Start();
        while (_tcpListener.Server.IsBound && _tcpListener.Pending())
        {
            TcpClient client =
                await _tcpListener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
            client.Close();

#if DEBUG
#pragma warning disable CA1848
            _logger.LogDebug("Successfully processed {Name} health check request", _options.Name);
#pragma warning restore CA1848
#endif
        }
#if DEBUG
#pragma warning disable CA1848
        _logger.LogDebug("Heartbeat {Name} check executed", _options.Name);
#pragma warning restore CA1848
#endif
    }
}
