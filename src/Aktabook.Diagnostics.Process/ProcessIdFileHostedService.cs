// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Aktabook.Diagnostics.Process;

public sealed class ProcessIdFileHostedService : IHostedService, IDisposable
{
    private readonly string _filename;
    private FileStream? _pidFileStream;

    public ProcessIdFileHostedService(
        IOptions<ProcessIdFileHostedServiceOptions> options)
    {
        _filename = options.Value.FileName;

        if (string.IsNullOrEmpty(_filename))
        {
            throw new ArgumentException("Filename cannot be empty",
                nameof(options));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        CreatePidFile();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        ClosePidFile();

        return Task.CompletedTask;
    }

    private void CreatePidFile()
    {
        int pid = Environment.ProcessId;
        const FileOptions fileOptions = FileOptions.Asynchronous
            | FileOptions.WriteThrough | FileOptions.DeleteOnClose;

        byte[] bytesToWrite = Encoding.ASCII.GetBytes($"{pid}\n");
        FileStream pidFileStream =
            new(_filename, FileMode.Create, FileAccess.Write, FileShare.Read,
                16, fileOptions);

        pidFileStream.Write(bytesToWrite, 0, bytesToWrite.Length);

        pidFileStream.Flush();

        _pidFileStream = pidFileStream;
    }

    private void ClosePidFile()
    {
        _pidFileStream?.Close();
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pidFileStream?.Dispose();
        }
    }

    ~ProcessIdFileHostedService()
    {
        Dispose(false);
    }
}