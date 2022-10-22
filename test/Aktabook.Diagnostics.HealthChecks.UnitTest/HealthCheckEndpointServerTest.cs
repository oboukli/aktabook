// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Aktabook.Diagnostics.HealthChecks.UnitTest;

public class HealthCheckEndpointServerTest
{
    [Theory]
    [InlineData(HealthStatus.Healthy)]
    [InlineData(HealthStatus.Degraded)]
    public void GiveStartAsync_WhenHealthyEnough_ThenAClientCanConnect(
        HealthStatus healthStatus)
    {
        CancellationTokenSource cancellationTokenSource = new();
        Mock<HealthCheckService> healthCheckServiceMock =
            new(MockBehavior.Strict);

        healthCheckServiceMock.Setup(x =>
            x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(
            new HealthReport(
                new Dictionary<string, HealthReportEntry>
                {
                    {
                        "dummy", new HealthReportEntry(healthStatus,
                            null, TimeSpan.Zero, null, null)
                    }
                }, TimeSpan.Zero));

        NullLogger<HealthCheckEndpointServer> logger =
            NullLogger<HealthCheckEndpointServer>.Instance;
        IOptions<HealthCheckTcpServiceOptions> options = Options.Create(
            new HealthCheckTcpServiceOptions
            {
                Port = 15001,
                IpAddress = IPAddress.Loopback,
                Interval = TimeSpan.FromMilliseconds(50.0)
            });

        HealthCheckEndpointServer healthCheckEndpointServer = new(
            healthCheckServiceMock.Object, logger, options);

        healthCheckEndpointServer.SetStoppingToken(
            cancellationTokenSource.Token);

        Thread t = new(() =>
        {
            try
            {
                healthCheckEndpointServer.StartAsync().GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                healthCheckEndpointServer.Stop();
            }
        })
        { IsBackground = true };
        t.Start();

        Thread.Sleep(TimeSpan.FromMilliseconds(75.0));

        TcpClient tcpClient = new();

        tcpClient
            .Invoking(x =>
                x.Connect(options.Value.IpAddress, options.Value.Port))
            .Should().NotThrow();

        cancellationTokenSource.Cancel();
        t.Join();
    }

    [Fact]
    public void GiveStartAsync_WhenUnhealthy_ThenAClientCannotConnect()
    {
        CancellationTokenSource cancellationTokenSource = new();
        Mock<HealthCheckService> healthCheckServiceMock =
            new(MockBehavior.Strict);

        healthCheckServiceMock.Setup(x =>
            x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(
            new HealthReport(
                new Dictionary<string, HealthReportEntry>
                {
                    {
                        "dummy", new HealthReportEntry(HealthStatus.Unhealthy,
                            null, TimeSpan.Zero, null, null)
                    }
                }, TimeSpan.Zero));

        NullLogger<HealthCheckEndpointServer> logger =
            NullLogger<HealthCheckEndpointServer>.Instance;
        IOptions<HealthCheckTcpServiceOptions> options = Options.Create(
            new HealthCheckTcpServiceOptions
            {
                Port = 15010,
                IpAddress = IPAddress.Loopback,
                Interval = TimeSpan.FromMilliseconds(50.0)
            });

        HealthCheckEndpointServer healthCheckEndpointServer = new(
            healthCheckServiceMock.Object, logger, options);

        healthCheckEndpointServer.SetStoppingToken(
            cancellationTokenSource.Token);

        Thread t = new(() =>
        {
            try
            {
                healthCheckEndpointServer.StartAsync().GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                healthCheckEndpointServer.Stop();
            }
        })
        { IsBackground = true };
        t.Start();

        Thread.Sleep(TimeSpan.FromMilliseconds(75.0));

        TcpClient tcpClient = new();

        tcpClient
            .Invoking(x =>
                x.Connect(options.Value.IpAddress, options.Value.Port))
            .Should().Throw<SocketException>();

        cancellationTokenSource.Cancel();
        t.Join();
    }

    [Fact]
    public void GivenStop_WhenInvokedAfterStart_ThenClientCannotConnect()
    {
        CancellationTokenSource cancellationTokenSource = new();
        Mock<HealthCheckService> healthCheckServiceMock =
            new(MockBehavior.Strict);
        healthCheckServiceMock.Setup(x =>
            x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(
            new HealthReport(
                new Dictionary<string, HealthReportEntry>
                {
                    {
                        "dummy", new HealthReportEntry(HealthStatus.Healthy,
                            null, TimeSpan.Zero, null, null)
                    }
                }, TimeSpan.Zero));
        NullLogger<HealthCheckEndpointServer> logger =
            NullLogger<HealthCheckEndpointServer>.Instance;
        IOptions<HealthCheckTcpServiceOptions> options = Options.Create(
            new HealthCheckTcpServiceOptions
            {
                Port = 15020,
                IpAddress = IPAddress.Loopback,
                Interval = TimeSpan.FromMilliseconds(50.0)
            });

        HealthCheckEndpointServer healthCheckEndpointServer = new(
            healthCheckServiceMock.Object, logger, options);

        healthCheckEndpointServer.SetStoppingToken(
            cancellationTokenSource.Token);

        Thread t = new(() =>
        {
            try
            {
                healthCheckEndpointServer.StartAsync().GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                healthCheckEndpointServer.Stop();
            }
        })
        { IsBackground = true };
        t.Start();

        Thread.Sleep(TimeSpan.FromMilliseconds(75.0));

        TcpClient tcpClient = new();

        tcpClient
            .Invoking(x =>
                x.Connect(options.Value.IpAddress, options.Value.Port))
            .Should().NotThrow();

        Thread.Sleep(TimeSpan.FromMilliseconds(75.0));

        healthCheckEndpointServer.Stop();

        tcpClient
            .Invoking(x =>
                x.Connect(options.Value.IpAddress, options.Value.Port))
            .Should().Throw<SocketException>();

        cancellationTokenSource.Cancel();
        t.Join();
    }

    [Fact]
    public void
        GivenSetStoppingToken_WhenIsRunning_ThenInvalidOperationException()
    {
        CancellationTokenSource cancellationTokenSource = new();
        Mock<HealthCheckService> healthCheckServiceMock =
            new(MockBehavior.Strict);
        healthCheckServiceMock.Setup(x =>
            x.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>?>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(
            new HealthReport(
                new Dictionary<string, HealthReportEntry>
                {
                    {
                        "dummy", new HealthReportEntry(HealthStatus.Healthy,
                            null,
                            TimeSpan.Zero, null, null)
                    }
                }, TimeSpan.Zero));

        NullLogger<HealthCheckEndpointServer> logger =
            NullLogger<HealthCheckEndpointServer>.Instance;
        IOptions<HealthCheckTcpServiceOptions> options = Options.Create(
            new HealthCheckTcpServiceOptions
            {
                Port = 15030,
                IpAddress = IPAddress.Loopback,
                Interval = TimeSpan.FromMilliseconds(50.0)
            });

        HealthCheckEndpointServer healthCheckEndpointServer = new(
            healthCheckServiceMock.Object, logger, options);

        healthCheckEndpointServer.SetStoppingToken(
            cancellationTokenSource.Token);

        Thread t = new(() =>
        {
            try
            {
                healthCheckEndpointServer.StartAsync().GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                healthCheckEndpointServer.Stop();
            }
        })
        { IsBackground = true };
        t.Start();
        cancellationTokenSource.Cancel();
        t.Join();

        healthCheckEndpointServer
            .Invoking(x => x.SetStoppingToken(CancellationToken.None)).Should()
            .ThrowExactly<InvalidOperationException>();

        cancellationTokenSource.Cancel();
    }

    [Fact]
    public void GivenSetStoppingToken_WhenIsNotRunning_ThenNoException()
    {
        Mock<HealthCheckService> healthCheckServiceMock =
            new(MockBehavior.Strict);
        NullLogger<HealthCheckEndpointServer> logger =
            NullLogger<HealthCheckEndpointServer>.Instance;
        IOptions<HealthCheckTcpServiceOptions> options = Options.Create(
            new HealthCheckTcpServiceOptions
            {
                Port = 15040,
                IpAddress = IPAddress.Loopback,
                Interval = TimeSpan.FromMilliseconds(50.0)
            });

        HealthCheckEndpointServer healthCheckEndpointServer = new(
            healthCheckServiceMock.Object, logger, options);

        healthCheckEndpointServer
            .Invoking(x => x.SetStoppingToken(CancellationToken.None)).Should()
            .NotThrow();
    }
}
