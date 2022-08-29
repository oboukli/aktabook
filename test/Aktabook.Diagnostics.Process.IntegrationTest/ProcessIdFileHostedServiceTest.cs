// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Text;
using Microsoft.Extensions.Options;

namespace Aktabook.Diagnostics.Process.IntegrationTest;

public class ProcessIdFileHostedServiceTest
{
    [Fact]
    public void GivenCtor_WhenFileNameIsEmpty_ThenArgumentException()
    {
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions());

        Action act = () =>
        {
            ProcessIdFileHostedService _ = new(options);
        };

        act.Invoking(x => x()).Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void GivenCtor_WhenInvoked_ThenDoNotCreatePidFile()
    {
        const string fileName =
            "GivenCtor_WhenInvoked_ThenDoNotCreatePidFile.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        File.Exists(fileName).Should().BeFalse();

        ProcessIdFileHostedService sut = new(options);

        File.Exists(fileName).Should().BeFalse();
        sut.Should().BeOfType<ProcessIdFileHostedService>();
    }

    [Fact]
    public void GivenStartAsync_WhenRunning_ThenCreatePidFile()
    {
        const string fileName =
            "GivenStartAsync_WhenRunning_ThenCreatePidFile.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        ProcessIdFileHostedService sut = new(options);

        File.Exists(fileName).Should().BeFalse();

        sut.StartAsync(CancellationToken.None);

        File.Exists(fileName).Should().BeTrue();

        File.Delete(fileName);
        File.Exists(fileName).Should().BeFalse();

        sut.Should().BeOfType<ProcessIdFileHostedService>();
    }

    [Fact]
    public void GivenStartAsync_WhenFileCreated_ThenFileContentIsProcessId()
    {
        const string fileName =
            "GivenStartAsync_WhenFileCreated_ThenFileContentIsProcessId.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        ProcessIdFileHostedService sut = new(options);

        File.Exists(fileName).Should().BeFalse();

        sut.StartAsync(CancellationToken.None);

        File.Exists(fileName).Should().BeTrue();

        new FileInfo(fileName).Length.Should().BeLessOrEqualTo(16);

        File.ReadAllBytes(fileName).Should()
            .BeEquivalentTo(
                Encoding.ASCII.GetBytes($"{Environment.ProcessId}\n"));

        File.Delete(fileName);
        File.Exists(fileName).Should().BeFalse();

        sut.Should().BeOfType<ProcessIdFileHostedService>();
    }

    [Fact]
    public void GivenStopAsync_WhenStopped_ThenDeletePidFile()
    {
        const string fileName =
            "GivenStopAsync_WhenStopped_ThenDeletePidFile.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        File.Exists(fileName).Should().BeFalse();

        ProcessIdFileHostedService sut = new(options);

        sut.StartAsync(CancellationToken.None);

        File.Exists(fileName).Should().BeTrue();

        sut.StopAsync(CancellationToken.None);

        File.Exists(fileName).Should().BeFalse();

        sut.Should().BeOfType<ProcessIdFileHostedService>();
    }

    [Fact]
    public void GivenStopAsync_WhenNotStarted_ThenDoNotCreatePidFile()
    {
        const string fileName =
            "GivenStopAsync_WhenNotStarted_ThenDoNotCreatePidFile.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        File.Exists(fileName).Should().BeFalse();

        ProcessIdFileHostedService sut = new(options);

        sut.StopAsync(CancellationToken.None);

        File.Exists(fileName).Should().BeFalse();

        sut.Should().BeOfType<ProcessIdFileHostedService>();
    }

    [Fact]
    public void GivenStopAsync_WhenExternallyDeleted_ThenNoError()
    {
        const string fileName =
            "GivenStopAsync_WhenExternallyDeleted_ThenNoError.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        File.Exists(fileName).Should().BeFalse();

        ProcessIdFileHostedService sut = new(options);

        sut.StartAsync(CancellationToken.None);

        File.Exists(fileName).Should().BeTrue();
        File.Delete(fileName);
        File.Exists(fileName).Should().BeFalse();

        sut.StopAsync(CancellationToken.None);

        File.Exists(fileName).Should().BeFalse();

        sut.Should().BeOfType<ProcessIdFileHostedService>();
    }

    [Fact]
    public void GivenStartAsync_WhenDisposed_ThenDeletePidFile()
    {
        const string fileName =
            "GivenStartAsync_WhenDisposed_ThenDeletePidFile.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        File.Exists(fileName).Should().BeFalse();

        using (ProcessIdFileHostedService sut = new(options))
        {
            sut.StartAsync(CancellationToken.None);

            File.Exists(fileName).Should().BeTrue();
        }

        File.Exists(fileName).Should().BeFalse();
    }

    [Fact]
    public void
        GivenGarbageCollectorRun_WhenPidServiceStarted_ThenDeletePidFile()
    {
        const string fileName =
            "GivenGarbageCollectorRun_WhenPidServiceStarted_ThenDeletePidFile.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        File.Exists(fileName).Should().BeFalse();

        void Act()
        {
            ProcessIdFileHostedService sut = new(options);

            sut.StartAsync(CancellationToken.None);
        }

        Act();

        File.Exists(fileName).Should().BeTrue();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        File.Exists(fileName).Should().BeFalse();
    }

    [Fact]
    public void
        GivenGarbageCollectorRun_WhenPidServiceNotStarted_ThenNoError()
    {
        const string fileName =
            "GivenGarbageCollectorRun_WhenPidServiceNotStarted_ThenNoError.pid";
        IOptions<ProcessIdFileHostedServiceOptions> options = Options.Create(
            new ProcessIdFileHostedServiceOptions { FileName = fileName });

        File.Exists(fileName).Should().BeFalse();

        void Act()
        {
            ProcessIdFileHostedService _ = new(options);
        }

        Act();

        File.Exists(fileName).Should().BeFalse();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        File.Exists(fileName).Should().BeFalse();
    }
}