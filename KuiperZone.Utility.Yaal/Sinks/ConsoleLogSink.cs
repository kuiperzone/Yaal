// -----------------------------------------------------------------------------
// PROJECT   : Yaal
// COPYRIGHT : Andy Thomas (C) 2022
// LICENSE   : GPL-3.0-or-later
// HOMEPAGE  : https://github.com/kuiperzone/Yaal
//
// This file is part of Yaal.
//
// Yaal is free software: you can redistribute it and/or modify it under the terms of the GNU General
// Public License as published by the Free Software Foundation, either version 3 of the License, or at your option
// any later version.
//
// Yaal is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with Yaal.
// If not, see <https://www.gnu.org/licenses/>.
// -----------------------------------------------------------------------------

using KuiperZone.Utility.Yaal.Internal;

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Implements <see cref="ILogSink"/>. Messages are written directly to <see cref="Console"/>.
/// </summary>
public sealed class ConsoleLogSink : ILogSink
{
    private readonly ConsoleSinkOptions _options;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ConsoleLogSink()
    {
        _options = new ConsoleSinkOptions();
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public ConsoleLogSink(ConsoleSinkOptions opts)
    {
        // Take a copy
        _options = new ConsoleSinkOptions(opts);
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(LogMessage msg, IReadOnlyLoggerOptions opts)
    {
        if (msg.Severity.IsHigherOrEqualPriority(_options.Threshold))
        {
            Console.WriteLine(msg.ToString(new MessageStringOptions(_options, opts)));
        }
    }

}