// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aktabook.Diagnostics.HealthChecks;

public class HealthCheckEndpointServer : IHealthCheckEndpointServer
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthCheckEndpointServer> _logger;
    private readonly HealthCheckTcpServiceOptions _options;
    private readonly TcpListener _tcpListener;
    private bool _isStarted;

    private CancellationToken _stoppingToken;

    public HealthCheckEndpointServer(
        HealthCheckService healthCheckService,
        ILogger<HealthCheckEndpointServer> logger,
        IOptions<HealthCheckTcpServiceOptions> options)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
        _options = options.Value;
        _stoppingToken = CancellationToken.None;
        _tcpListener = new TcpListener(_options.IpAddress, _options.Port);
    }

    public async Task StartAsync()
    {
        _logger.LogDebug(
            "Health check endpoint {Name} started and configured to listen at {IP:l}:{Port}",
            _options.Name, _options.IpAddress, _options.Port);

        _isStarted = true;

        while (!_stoppingToken.IsCancellationRequested)
        {
            await UpdateHeartbeatAsync(_stoppingToken).ConfigureAwait(false);

            await Task.Delay(_options.Interval, _stoppingToken).ConfigureAwait(false);
        }

        _tcpListener.Stop();

        _logger.LogInformation(
            @"Health check TCP endpoint listener {Name} stopped listening to requests", _options.Name);
    }

    public void Stop()
    {
        _tcpListener.Stop();

        _logger.LogInformation(
            "Health check endpoint {Name} shut down", _options.Name);
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

    private async Task UpdateHeartbeatAsync(CancellationToken cancellationToken)
    {
        HealthReport healthReport = await _healthCheckService.CheckHealthAsync(x =>
            x.Tags.Overlaps(_options.Tags), cancellationToken).ConfigureAwait(false);

        switch (healthReport.Status)
        {
            case HealthStatus.Unhealthy:
                _tcpListener.Stop();

                _logger.LogError(
                    "Service health status is: {HealthStatus}. Listener stopped", healthReport.Status);

                return;

            case HealthStatus.Degraded:
                _logger.LogWarning("Service health status is: {HealthStatus}", healthReport.Status);
                break;
        }

        _tcpListener.Start();
        while (_tcpListener.Server.IsBound && _tcpListener.Pending())
        {
            TcpClient client =
                await _tcpListener.AcceptTcpClientAsync(cancellationToken).ConfigureAwait(false);
            client.Close();

            _logger.LogDebug(
                "Successfully processed {Name} health check request", _options.Name);
        }

        _logger.LogDebug("Heartbeat {Name} check executed", _options.Name);
    }
}
