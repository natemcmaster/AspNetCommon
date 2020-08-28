// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using McMaster.Extensions.CommandLineUtils.Abstractions;

namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary>
    /// Represents one or many command line option that is identified by flag proceeded by '-' or '--'.
    /// Options are not positional. Compare to <see cref="CommandArgument{T}"/>. The raw value must be
    /// parsable into type <typeparamref name="T" />
    /// </summary>
    /// <typeparam name="T">The type of the option value(s)</typeparam>
    public class CommandOption<T> : CommandOption, IInternalCommandParamOfT
    {
        private readonly List<T> _parsedValues = new List<T>();
        private readonly IValueParser<T> _valueParser;
        private T _defaultValue;

        /// <summary>
        /// Intializes a new instance of <see cref="CommandOption{T}" />
        /// </summary>
        /// <param name="valueParser">The parser use to convert values into type of T.</param>
        /// <param name="template">The option tempalte.</param>
        /// <param name="optionType">The optiont type</param>
        public CommandOption(IValueParser<T> valueParser, string template, CommandOptionType optionType)
            : base(template, optionType)
        {
            _valueParser = valueParser ?? throw new ArgumentNullException(nameof(valueParser));
            UnderlyingType = typeof(T);
            base.DefaultValue = default(T)?.ToString();
        }

        /// <summary>
        /// The parsed value.
        /// </summary>
        public T ParsedValue => _parsedValues.FirstOrDefault();

        /// <summary>
        /// All parsed values;
        /// </summary>
        public IReadOnlyList<T> ParsedValues => _parsedValues;

        /// <summary>
        /// The default value of the option
        /// </summary>

        public new T DefaultValue
        {
            get => _defaultValue;
            set
            {
                _defaultValue = value;
                base.DefaultValue = value?.ToString();
            }
        }

        void IInternalCommandParamOfT.Parse(CultureInfo culture)
        {
            _parsedValues.Clear();
            for (int i = 0; i < Values.Count; i++)
            {
                _parsedValues.Add(_valueParser.Parse(LongName ?? ShortName ?? SymbolName, Values[i], culture));
            }
        }
    }
}
