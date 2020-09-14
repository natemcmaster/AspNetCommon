// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// This file has been modified from the original form. See Notice.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary>
    /// Represents one or many positional command line arguments.
    /// Arguments are parsed in the order in which <see cref="CommandLineApplication.Arguments"/> lists them.
    /// </summary>
    /// <seealso cref="CommandOption"/>
    public class CommandArgument
    {
        private string? _defaultValue;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandArgument"/>.
        /// </summary>
        public CommandArgument()
        {
            Values = new List<string?>();
        }

        /// <summary>
        /// The name of the argument.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Determines if the argument appears in the generated help-text.
        /// </summary>
        public bool ShowInHelpText { get; set; } = true;

        /// <summary>
        /// A description of the argument.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// All values specified, if any.
        /// </summary>
        public List<string?> Values { get; private set; }

        /// <summary>
        /// Allow multiple values.
        /// </summary>
        public bool MultipleValues { get; set; }

        /// <summary>
        /// The first value from <see cref="Values"/>, if any.
        /// </summary>
        public string? Value => Values.FirstOrDefault();

        /// <summary>
        /// The default value of the argument.
        /// </summary>
        public string? DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                if (value != null)
                {
                    Values.Add(value);
                }
            }
        }

        /// <summary>
        /// A collection of validators that execute before invoking <see cref="CommandLineApplication.OnExecute(Func{int})"/>.
        /// When validation fails, <see cref="CommandLineApplication.ValidationErrorHandler"/> is invoked.
        /// </summary>
        public ICollection<IArgumentValidator> Validators { get; } = new List<IArgumentValidator>();

        /// <summary>
        /// Defines the underlying type of the argument for the help-text-generator
        /// </summary>
        internal Type UnderlyingType { get; set; }

        internal void Reset()
        {
            Values.Clear();
        }
    }
}
