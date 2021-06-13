﻿// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace McMaster.Extensions.CommandLineUtils
{
    internal class CommandLineValidationContextFactory
    {
        private readonly CommandLineApplication _app;

        public CommandLineValidationContextFactory(CommandLineApplication app)
        {
            _app = app ?? throw new ArgumentNullException(nameof(app));
        }

        public ValidationContext Create(CommandLineApplication app) => new(app, _app, null);

        public ValidationContext Create(CommandArgument argument) => new(argument, _app, null);

        public ValidationContext Create(IOption option) => new(option, _app, null);
    }
}
