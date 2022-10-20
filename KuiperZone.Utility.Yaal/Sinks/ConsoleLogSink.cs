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
    private readonly object _syncObj = new();
    private readonly ConsoleSinkOptions _options;
    private readonly ConsoleColor _initColor = ConsoleColor.Gray;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ConsoleLogSink()
    {
        _options = new ConsoleSinkOptions();
        _initColor = GetForegroundColor();
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public ConsoleLogSink(ConsoleSinkOptions opts)
    {
        // Take a copy
        _options = new ConsoleSinkOptions(opts);
        _initColor = GetForegroundColor();
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(LogMessage msg, IReadOnlyLogOptions opts)
    {
        if (msg.Severity.IsHigherOrEqualPriority(_options.Threshold))
        {
            if (_options.UseColor)
            {
                // Must lock, other race on color setting
                lock (_syncObj)
                {
                    try
                    {
                        SetForegroundColor(msg.Severity.ToColor());
                        Console.WriteLine(msg.ToString(new MessageParams(_options, opts)));
                    }
                    finally
                    {
                        SetForegroundColor(_initColor);
                    }
                }
            }
            else
            {
                Console.WriteLine(msg.ToString(new MessageParams(_options, opts)));
            }
        }
    }

    private ConsoleColor GetForegroundColor()
    {
        try
        {
            return Console.ForegroundColor;
        }
        catch
        {
            // Unset
            _options.UseColor = false;
            return ConsoleColor.Gray;;
        }
    }

    private void SetForegroundColor(ConsoleColor color)
    {
        try
        {
            Console.ForegroundColor = color;
        }
        catch
        {
            // Don't do it again
            _options.UseColor = false;
        }
    }

}