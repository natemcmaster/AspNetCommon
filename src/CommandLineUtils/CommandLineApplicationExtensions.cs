// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// This file has been modified from the original form. See Notice.txt in the project root for more information.

using System;
using System.Reflection;

namespace McMaster.Extensions.CommandLineUtils
{
    /// <summary>
    /// Helper methods for <see cref="CommandLineApplication"/>.
    /// </summary>
    public static class CommandLineApplicationExtensions
    {
        /// <summary>
        /// Adds the help option with the template <c>-?|-h|--help</c>.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static CommandOption HelpOption(this CommandLineApplication app)
            => app.HelpOption("-?|-h|--help");

        /// <summary>
        /// Adds the verbose option with the template <c>-v|--verbose</c>.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static CommandOption VerboseOption(this CommandLineApplication app)
            => VerboseOption(app, "-v|--verbose");

        /// <summary>
        /// Adds the verbose option with the template <c>-v|--verbose</c>.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="template" />
        /// <returns></returns>
        public static CommandOption VerboseOption(this CommandLineApplication app, string template)
            => app.Option(template, "Show verbose output", CommandOptionType.NoValue, inherited: true);

        /// <summary>
        /// Sets <see cref="CommandLineApplication.Invoke"/> with a return code of <c>0</c>.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="action"></param>
        public static void OnExecute(this CommandLineApplication app, Action action)
            => app.OnExecute(() =>
                {
                    action();
                    return 0;
                });

        /// <summary>
        /// Finds <see cref="AssemblyInformationalVersionAttribute"/> on <paramref name="assembly"/> and uses that
        /// to set <see cref="CommandLineApplication.OptionVersion"/>.
        /// <para>
        /// Uses the Version that is part of the <see cref="AssemblyName"/> of the specified assembly
        /// if no <see cref="AssemblyInformationalVersionAttribute"/> is applied.
        /// </para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="assembly"></param>
        /// <exception cref="ArgumentNullException">Either <paramref name="app"/> or <paramref name="assembly"/> is <c>null</c>.</exception>
        public static CommandOption VersionOptionFromAssemblyAttributes(this CommandLineApplication app, Assembly assembly)
            => VersionOptionFromAssemblyAttributes(app, "--version", assembly);

        /// <summary>
        /// Finds <see cref="AssemblyInformationalVersionAttribute"/> on <paramref name="assembly"/> and uses that
        /// to set <see cref="CommandLineApplication.OptionVersion"/>.
        /// <para>
        /// Uses the Version that is part of the <see cref="AssemblyName"/> of the specified assembly
        /// if no <see cref="AssemblyInformationalVersionAttribute"/> is applied.
        /// </para>
        /// </summary>
        /// <param name="app"></param>
        /// <param name="template"></param>
        /// <param name="assembly"></param>
        /// <exception cref="ArgumentNullException">Either <paramref name="app"/> or <paramref name="assembly"/> is <c>null</c>.</exception>
        public static CommandOption VersionOptionFromAssemblyAttributes(CommandLineApplication app, string template, Assembly assembly)
            => app.VersionOption(template, GetInformationalVersion(assembly));

        private static string GetInformationalVersion(Assembly assembly)
        {
            string infoVersion = assembly?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;
            if (string.IsNullOrWhiteSpace(infoVersion))
                return assembly?.GetName().Version.ToString();
            return infoVersion;
        }
    }
}
