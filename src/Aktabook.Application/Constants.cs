// Copyright (c) 2022 Omar Boukli-Hacene. All rights reserved.
// Distributed under an MIT-style license that can be
// found in the LICENSE file.

// SPDX-License-Identifier: MIT

namespace Aktabook.Application;

public static class Constants
{
    public const string AktabookEnvironmentVarName = "AKTABOOK_ENVIRONMENT";

#pragma warning disable S1075
    public const string AppLicenseUrl = "https://github.com/oboukli/aktabook/blob/main/LICENSE";
#pragma warning restore

    public static class Bus
    {
        public static class Configuration
        {
            public const string RequesterServiceBusSection = "RequesterServiceBus:RabbitMQConnectionOptions";
        }

        public static class EndpointName
        {
            public const string BookInfoRequestEndpoint = "BookInfoRequestEndpoint";

            public const string PublicRequesterEndpoint = "PublicRequesterEndpoint";
        }

        public static class QueueName
        {
            public const string AuditQueue = "AuditQueue";

            public const string ErrorQueue = "ErrorQueue";
        }
    }
}