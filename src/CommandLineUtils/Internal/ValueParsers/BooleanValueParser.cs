﻿// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace McMaster.Extensions.CommandLineUtils.Abstractions
{
    using System;

    internal class BooleanValueParser : IValueParser
    {
        private BooleanValueParser()
        { }

        public static BooleanValueParser Singleton { get; } = new BooleanValueParser();

        public Type TargetType { get; } = typeof(bool);

        public object Parse(string argName, string value)
        {
            if (!bool.TryParse(value, out var result))
            {
                throw new FormatException($"Invalid value specified for {argName}. Cannot convert '{value}' to a boolean.");
            }
            return result;
        }
    }
}
