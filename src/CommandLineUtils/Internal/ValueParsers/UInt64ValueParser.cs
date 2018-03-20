﻿// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace McMaster.Extensions.CommandLineUtils.Abstractions
{
    using System;

    internal class UInt64ValueParser : IValueParser
    {
        private UInt64ValueParser()
        { }

        public static UInt64ValueParser Singleton { get; } = new UInt64ValueParser();

        public Type TargetType { get; } = typeof(ulong);

        public object Parse(string argName, string value)
        {
            if (!ulong.TryParse(value, out var result))
            {
                throw new FormatException($"Invalid value specified for {argName}. '{value}' is not a valid, non-negative number.");
            }
            return result;
        }
    }
}
