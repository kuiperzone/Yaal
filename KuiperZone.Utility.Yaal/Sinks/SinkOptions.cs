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

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// A base class for sink options.
/// </summary>
public class SinkOptions
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SinkOptions()
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public SinkOptions(LogFormat format, SeverityLevel threshold = SeverityLevel.Lowest)
    {
        Format = format;
        Threshold = threshold;
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public SinkOptions(SinkOptions other)
    {
        Format = other.Format;
        Threshold = other.Threshold;
        MaxTextLength = other.MaxTextLength;
    }

    /// <summary>
    /// Gets or sets the output format. The default value shall depend on the sink kind. For example,
    /// the default for <see cref="SyslogLogSink"/> shall be <see cref="LogFormat.Rfc5424"/>.
    /// For others, it may typically be <see cref="LogFormat.General"/>.
    /// </summary>
    public LogFormat Format { get; set; } = LogFormat.General;

    /// <summary>
    /// Gets or sets the threshold severity for the sink. Setting this value will prevent the sink from
    /// logging any message with a lower priority, irrespective of the threshold value of the
    /// host logger. For example, it may desirable to set it to <see cref="SeverityLevel.Error"/>
    /// for the <see cref="SyslogLogSink"/> so that only errors and higher priority messages are
    /// logged to this specific sink. Moreover, althougth this setting allows control on a per
    /// sink basis, this threshold cannot be changed in-flight. Typically, therefore, the default
    /// value is to be <see cref="SeverityLevel.Lowest"/>.
    /// </summary>
    public SeverityLevel Threshold { get; set; } = SeverityLevel.Lowest;

    /// <summary>
    /// Gets or sets the maximum message text length in chars, excluding structured and other data.
    /// If the text exceeds this, it will be truncated. A value of 0 or less implies no limit.
    /// The default value is 2048, although sinks are free to set their own.
    /// </summary>
    public int MaxTextLength { get; set; } = 2048;

}