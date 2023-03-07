// Copyright (c) Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using System;
using FluentAssertions;
using Xunit;

namespace Aktabook.Bus.Common.UnitTest;

public class AmqpUriBuilderUnitTest
{
    [Fact]
    public void GivenDefaultCtor_WhenConstructed_ThenDefault()
    {
        AmqpUriBuilder amqpUriBuilder = new();

        amqpUriBuilder.Should().BeEquivalentTo(new
        {
            HostName = string.Empty,
            PortNumber = 0,
            VirtualHost = string.Empty,
            UserName = string.Empty,
            Password = string.Empty
        }, options =>
            options.RespectingRuntimeTypes());
    }

    [Fact]
    public void GivenConnectionUri_WhenDefaultCtor_ThenDefaultUri()
    {
        AmqpUriBuilder amqpUriBuilder = new();

        amqpUriBuilder.ConnectionUri.Should().Be(new Uri("amqp:/"));
    }

    [Fact]
    public void GivenConnectionUriToString_WhenDefaultCtor_ThenDefaultUriString()
    {
        AmqpUriBuilder amqpUriBuilder = new();

        amqpUriBuilder.ConnectionUri.ToString().Should().Be("amqp:/");
    }

    [Theory]
    [InlineData("", 0, "", "", "", "amqp:/")]
    [InlineData("localhost", 5672, "dummy_vhost", "dummy_user", "dummy_pw",
        "amqp://dummy_user:dummy_pw@localhost:5672/dummy_vhost")]
    [InlineData("127.0.0.1", 1012, "\ud83d\ude42",
        "dummy_\ud83d\ude42_username", "dummy_\ud83d\ude42_password",
        "amqp://dummy_\ud83d\ude42_username:dummy_\ud83d\ude42_password@127.0.0.1:1012/\ud83d\ude42")]
    [InlineData("www.example.com", 1013, @"{+\@#$!""", @"{+\@#$!""",
        @"{+\@#$!""",
        @"amqp://%7B%2B%5C%40%23%24%21%22:%7B%2B%5C%40%23%24%21%22@www.example.com:1013/{%2B%5C%40%23%24%21""")]
    public void GivenConnectionUriToString_WhenInitialized_ThenUriString(
        string hostName, int portNumber, string virtualHost, string userName, string password,
        string expected)
    {
        AmqpUriBuilder amqpUriBuilder = new()
        {
            HostName = hostName,
            PortNumber = portNumber,
            VirtualHost = virtualHost,
            UserName = userName,
            Password = password
        };

        amqpUriBuilder.ConnectionUri.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("", 0, "", "", "")]
    [InlineData("localhost", 1000, "dummy_vhost", "dummy_user", "dummy_pw")]
    [InlineData("127.0.0.1", 1001, "\ud83d\ude42",
        "dummy_\ud83d\ude42_username", "dummy_\ud83d\ude42_password")]
    [InlineData("www.example.com", 1002, @"{+\@#$!""", @"{+\@#$!""",
        @"{+\@#$!""")]
    public void GivenConnectionUri_WhenInitialized_ThenUri(
        string hostName, int portNumber, string virtualHost, string userName, string password)
    {
        AmqpUriBuilder amqpUriBuilder = new()
        {
            HostName = hostName,
            PortNumber = portNumber,
            VirtualHost = virtualHost,
            UserName = userName,
            Password = password
        };

        amqpUriBuilder.ConnectionUri.Should().BeOfType<Uri>();
    }

    [Theory]
    [InlineData(@"\u1F642", 5672, "", @"{+\@#$!", @"{+\@#$!")]
    public void GivenConnectionUri_WhenInitialized_ThenException(
        string hostName, int portNumber, string virtualHost, string userName, string password)
    {
        AmqpUriBuilder amqpUriBuilder = new()
        {
            HostName = hostName,
            PortNumber = portNumber,
            VirtualHost = virtualHost,
            UserName = userName,
            Password = password
        };

        new Func<Uri>(() => amqpUriBuilder.ConnectionUri).Should().ThrowExactly<UriFormatException>();
    }
}
