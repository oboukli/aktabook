// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

using Aktabook.Application.Messages.Commands;
using Aktabook.Application.Messages.Events;
using FluentAssertions;
using Xunit;

namespace Aktabook.Bus.Common.UnitTest;

public class BusMessageConventionUnitTest
{
    [Fact]
    public void GivenIsCommandType_WhenCommandType_ThenTrue()
    {
        BusMessageConvention busMessageConvention = new();

        busMessageConvention.IsCommandType(typeof(PlaceBookInfoRequest))
            .Should().BeTrue();
    }

    [Fact]
    public void GivenIsCommandType_WhenObject_ThenFalse()
    {
        BusMessageConvention busMessageConvention = new();

        busMessageConvention.IsCommandType(typeof(object)).Should().BeFalse();
    }

    [Fact]
    public void GivenIsEventType_WhenEventType_ThenTrue()
    {
        BusMessageConvention busMessageConvention = new();

        busMessageConvention.IsEventType(typeof(BookInfoRequestProcessed))
            .Should().BeTrue();
    }

    [Fact]
    public void GivenIsEventType_WhenObject_ThenFalse()
    {
        BusMessageConvention busMessageConvention = new();

        busMessageConvention.IsEventType(typeof(object)).Should().BeFalse();
    }

    [Fact]
    public void GivenIsMessageType_WhenMessageType_ThenTrue()
    {
        BusMessageConvention busMessageConvention = new();

        busMessageConvention.IsMessageType(typeof(PlaceBookInfoRequest))
            .Should().BeTrue();
    }

    [Fact]
    public void GivenIsMessageType_WhenObject_ThenFalse()
    {
        BusMessageConvention busMessageConvention = new();

        busMessageConvention.IsMessageType(typeof(object)).Should().BeFalse();
    }

    [Fact]
    public void GivenName_WhenGet_ThenNotEmpty()
    {
        BusMessageConvention busMessageConvention = new();

        busMessageConvention.Name.Should().NotBeEmpty();
    }
}