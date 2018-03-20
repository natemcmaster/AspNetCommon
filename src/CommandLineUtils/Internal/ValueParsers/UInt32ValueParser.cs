﻿// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace McMaster.Extensions.CommandLineUtils.Abstractions
{
    using System;

    internal class UInt32ValueParser : IValueParser
    {
        private UInt32ValueParser()
        { }

        public static UInt32ValueParser Singleton { get; } = new UInt32ValueParser();

        public Type TargetType { get; } = typeof(uint);

        public object Parse(string argName, string value)
        {
            if (!uint.TryParse(value, out var result))
            {
                throw new FormatException($"Invalid value specified for {argName}. '{value}' is not a valid, non-negative number.");
            }
            return result;
        }
    }
}
