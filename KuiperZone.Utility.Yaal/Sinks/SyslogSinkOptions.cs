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

using System.Runtime.InteropServices;

namespace KuiperZone.Utility.Yaal.Sinks;

// Strip priority
// https://stackoverflow.com/questions/9209130/confused-with-syslog-message-format

/// <summary>
/// Construction options for the <see cref="SyslogSink"/> class. Implements
/// <see cref="SyslogSinkOptions"/> and provides setters.
/// </summary>
public sealed class SyslogSinkOptions : SinkOptions
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SyslogSinkOptions()
        : base(DefaultFormat())
    {
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public SyslogSinkOptions(LogFormat format, SeverityLevel threshold = SeverityLevel.Lowest)
        : base(format, threshold)
    {
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public SyslogSinkOptions(SyslogSinkOptions other)
        : base(other)
    {
        EventLogSource = other.EventLogSource;
    }

    /// <summary>
    /// Gets or sets the Windows EventLog.Source parameter (it is ignored on Linux).
    /// The default is ".NET Runtime" and allows for message to be written out of the box.
    /// In order to change this value, it is necessary to register the source.
    /// For information, see:
    /// https://www.jitbit.com/alexblog/266-writing-to-an-event-log-from-net-without-the-description-for-event-id-nonsense/
    /// </summary>
    public string EventLogSource = ".NET Runtime";

    private static LogFormat DefaultFormat()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return LogFormat.General;
        }

        return LogFormat.Rfc5424;
    }

}