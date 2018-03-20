﻿// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace McMaster.Extensions.CommandLineUtils.Abstractions
{
    using System;

    internal class ByteValueParser : IValueParser
    {
        private ByteValueParser()
        { }

        public static ByteValueParser Singleton { get; } = new ByteValueParser();

        public Type TargetType { get; } = typeof(byte);

        public object Parse(string argName, string value)
        {
            if (!byte.TryParse(value, out var result))
            {
                throw new FormatException($"Invalid value specified for {argName}. '{value}' is not a valid number.");
            }
            return result;
        }
    }
}
