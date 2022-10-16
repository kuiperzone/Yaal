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

using System.Diagnostics;

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Implements <see cref="ILogSink"/> for Windows EventLog.
/// </summary>
public sealed class EventLogSink : ILogSink
{
    private readonly EventLog _event;

    /// <summary>
    /// Constructor with option values. Serves as default constructor.
    /// </summary>
    public EventLogSink(FormatKind format = FormatKind.Text, SeverityLevel threshold = SeverityLevel.Lowest)
        : this(new SinkOptions(format, threshold))
    {
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public EventLogSink(IReadOnlySinkOptions options)
    {
        // Take a copy
        Options = new SinkOptions(options);

        _event = new("Application");
        _event.Source = "Application";
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Options"/>.
    /// </summary>
    public IReadOnlySinkOptions Options { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(LogMessage message, IReadOnlyLogOptions options)
    {
        if (_event.MachineName.Length < 2 && !string.IsNullOrEmpty(options.HostName))
        {
            _event.MachineName = options.HostName;
        }

        var text = message.ToString(Options.Format, options);
        _event.WriteEntry(text, EventLogEntryType.Information);
    }

    private static EventLogEntryType ToEntryType(SeverityLevel severity)
    {
        switch (severity)
        {
            case SeverityLevel.Emergency:
            case SeverityLevel.Alert:
            case SeverityLevel.Critical:
            case SeverityLevel.Error:
                return EventLogEntryType.Error;

            case SeverityLevel.Warning:
                return EventLogEntryType.Warning;

            default:
                return EventLogEntryType.Information;
        }
    }

}