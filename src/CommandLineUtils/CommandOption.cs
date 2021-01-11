// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// This file has been modified from the original form. See Notice.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary>
    /// Represents one or many command line option that is identified by flag proceeded by '-' or '--'.
    /// Options are not positional. Compare to <see cref="CommandArgument"/>.
    /// </summary>
    public class CommandOption
    {
        private protected readonly List<string?> _values = new();

        /// <summary>
        /// Initializes a new <see cref="CommandOption"/>.
        /// </summary>
        /// <param name="template">The template string. This is parsed into <see cref="ShortName"/> and <see cref="LongName"/>.</param>
        /// <param name="optionType">The option type.</param>
        public CommandOption(string template, CommandOptionType optionType)
        {
            OptionType = optionType;

            var separators = (optionType == CommandOptionType.SingleOrNoValue)
                ? new[] { ' ', '|', ':', '=', '[', ']' }
                : new[] { ' ', '|', ':', '=' };
            foreach (var part in template.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.StartsWith("--"))
                {
                    LongName = part.Substring(2);
                }
                else if (part.StartsWith("-"))
                {
                    var optName = part.Substring(1);

                    // If there is only one char and it is not an English letter, it is a symbol option (e.g. "-?")
                    if (optName.Length == 1 && !IsEnglishLetter(optName[0]))
                    {
                        SymbolName = optName;
                    }
                    else
                    {
                        ShortName = optName;
                    }
                }
                else if (part.StartsWith("<") && part.EndsWith(">"))
                {
                    ValueName = part.Substring(1, part.Length - 2);
                }
                else
                {
                    throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
                }
            }

            if (string.IsNullOrEmpty(LongName) && string.IsNullOrEmpty(ShortName) && string.IsNullOrEmpty(SymbolName))
            {
                throw new ArgumentException($"Invalid template pattern '{template}'", nameof(template));
            }
        }

        internal CommandOption(CommandOptionType type)
        {
            OptionType = type;
        }

        /// <summary>
        /// The short command line flag used to identify this option. On command line, this is preceded by a single '-{ShortName}'.
        /// </summary>
        public string? ShortName { get; set; }

        /// <summary>
        /// The long command line flag used to identify this option. On command line, this is preceded by a double dash: '--{LongName}'.
        /// </summary>
        public string? LongName { get; set; }

        /// <summary>
        /// Can be used in addition to <see cref="ShortName"/> to add a single, non-English character.
        /// Example "-?".
        /// </summary>
        public string? SymbolName { get; set; }

        /// <summary>
        /// The name of value(s) shown in help text when <see cref="OptionType"/> is not <see cref="CommandOptionType.NoValue"/>.
        /// </summary>
        public string? ValueName { get; set; }

        /// <summary>
        /// A description of this option to show in generated help text.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Any values found during parsing, if any.
        /// </summary>
        public IReadOnlyList<string?> Values
        {
            get
            {
                if (_values.Count == 0 && DefaultValue != null)
                {
                    return new List<string?> { DefaultValue };
                }
                return _values;
            }
        }

        /// <summary>
        /// The default value of the option.
        /// </summary>
        public string? DefaultValue { get; set; }

        /// <summary>
        /// Defines the type of the option.
        /// </summary>
        public CommandOptionType OptionType { get; private set; }

        /// <summary>
        /// Determines if this option should be shown in generated help text.
        /// </summary>
        public bool ShowInHelpText { get; set; } = true;

        /// <summary>
        /// Determines if subcommands added to <see cref="CommandLineApplication.Commands"/>
        /// should also have access to this option.
        /// </summary>
        public bool Inherited { get; set; }

        /// <summary>
        /// Defines the underlying type of the option for the help-text-generator
        /// </summary>
        internal Type UnderlyingType { get; set; }

        /// <summary>
        /// A collection of validators that execute before invoking <see cref="CommandLineApplication.OnExecute(Func{int})"/>.
        /// When validation fails, <see cref="CommandLineApplication.ValidationErrorHandler"/> is invoked.
        /// </summary>
        public ICollection<IOptionValidator> Validators { get; } = new List<IOptionValidator>();

        /// <summary>
        /// Attempt to parse the value that follows after the flag.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryParse(string? value)
        {
            switch (OptionType)
            {
                case CommandOptionType.MultipleValue:
                    _values.Add(value);
                    break;
                case CommandOptionType.SingleOrNoValue:
                case CommandOptionType.SingleValue:
                    if (_values.Any())
                    {
                        return false;
                    }
                    _values.Add(value);
                    break;
                case CommandOptionType.NoValue:
                    if (value != null)
                    {
                        return false;
                    }

                    // Add a value so .HasValue() == true
                    // Also, the count can be used to determine how many times a flag was specified
                    _values.Add(null);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        /// <summary>
        /// True when <see cref="Values"/> is not empty or when a <see cref="DefaultValue" /> is given.
        /// </summary>
        /// <returns></returns>
        public bool HasValue()
        {
            return Values.Any();
        }

        /// <summary>
        /// Returns the first element of <see cref="Values"/>, if any, or <see cref="DefaultValue" />.
        /// </summary>
        /// <returns></returns>
        public string? Value() => Values.FirstOrDefault();

        /// <summary>
        /// Generates the template string in the format "-{Symbol}|-{Short}|--{Long} &lt;{Value}&gt;" for display in help text.
        /// </summary>
        /// <returns>The template string</returns>
        internal string ToTemplateString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(SymbolName))
            {
                sb.Append('-').Append(SymbolName);
            }

            if (!string.IsNullOrEmpty(ShortName))
            {
                if (sb.Length > 0)
                {
                    sb.Append('|');
                }

                sb.Append('-').Append(ShortName);
            }

            if (!string.IsNullOrEmpty(LongName))
            {
                if (sb.Length > 0)
                {
                    sb.Append('|');
                }

                sb.Append("--").Append(LongName);
            }

            if (!string.IsNullOrEmpty(ValueName) && OptionType != CommandOptionType.NoValue)
            {
                if (OptionType == CommandOptionType.SingleOrNoValue)
                {
                    sb.Append("[:<").Append(ValueName).Append(">]");
                }
                else
                {
                    sb.Append(" <").Append(ValueName).Append('>');
                }
            }

            return sb.ToString();
        }


        private bool IsEnglishLetter(char c)
        {
            return c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');
        }

        /// <summary>
        /// Clear any parsed values from this argument.
        /// </summary>
        public virtual void Reset()
        {
            _values.Clear();
        }
    }
}
