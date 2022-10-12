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

using System.Globalization;
using System.Text;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// </summary>
public class LogMessage
{
    private const string TimeFormat5424 = "yyyy-MM-ddTHH:mm:ss.ffffffK";
    private const string TimeFormatBsd = "MMM dd HH:mm:ss";
    private const string TimeFormatText = TimeFormat5424;

    private StructuredData? _data;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public LogMessage()
    {
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public LogMessage(string? text)
    {
        Text = text;
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public LogMessage(SeverityLevel severity, string? text)
    {
        Severity = severity;
        Text = text;
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public LogMessage(string? msgId, string? text)
    {
        MsgId = msgId;
        Text = text;
    }

    /// <summary>
    /// Constructor variant.
    /// </summary>
    public LogMessage(SeverityLevel severity, string? msgId, string? text)
    {
        Severity = severity;
        MsgId = msgId;
        Text = text;
    }

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    public SeverityLevel Severity { get; set; } = SeverityLevel.Informational;

    /// <summary>
    /// Gets or sets the time. The initial value is always set from the system clock.
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the MSG-ID.
    /// </summary>
    public string? MsgId { get; set; }

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public StructuredData Data
    {
        get
        {
            _data ??= new();
            return _data;
        }
    }

    /// <summary>
    /// Overrides and equivalent to: ToString(<see cref="FormatKind.Rfc5424"/>).
    /// </summary>
    public override string ToString()
    {
        return ToString(new LogOptions());
    }

    /// <summary>
    /// Returns a formatted string according to kind.
    /// </summary>
    public string ToString(FormatKind format)
    {
        return ToString(new LogOptions(format));
    }

    /// <summary>
    ///
    /// </summary>
    public string ToString(IReadOnlyLogOptions options, SdDebug? debug = null)
    {
        var buffer = new StringBuilder(1024);

        switch (options.Format)
        {
            case FormatKind.Rfc5424:
                AppendRfc5424(buffer, options, debug);
                break;
            case FormatKind.Bsd:
                AppendBsd(buffer, options, debug);
                break;
            default:
                AppendText(buffer, options, debug);
                break;
        }

        return buffer.ToString();
    }

    private static string ValueOrNil(string? value)
    {
	    if (!StructuredData.IsValidKey(value))
	    {
            return "-";

	    }

        return value;
    }

    private static int Clamp(SeverityLevel value)
    {
        if (value >= SeverityLevel.Debug)
        {
            return (int)SeverityLevel.Debug;
        }

        return (int)(value > SeverityLevel.Emergency ? value : SeverityLevel.Emergency);
    }

    private static string GetAppThread(string pid)
    {
        var buffer = new StringBuilder(pid);
        var tname = Thread.CurrentThread.Name;

        if (string.IsNullOrEmpty(tname))
        {
            tname = Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
        }

        if (pid.Length != 0)
        {
            buffer.Append('-');
        }

        return buffer.ToString();
    }

    private void AppendRfc5424(StringBuilder buffer, IReadOnlyLogOptions options, SdDebug? debug)
    {
        // Clamp within valid range (see DEBUG_L1 etc.)
        buffer.Append('<');
        buffer.Append((Clamp(Severity) | (int)options.Facility).ToString(CultureInfo.InvariantCulture));
        buffer.Append('>');

        buffer.Append("1 ");
        buffer.Append(ToTimestamp(options));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.LocalHost));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.AppName));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.AppPid));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(MsgId));

        if (_data?.IsEmpty == false)
        {
            buffer.Append(' ');
            _data.AppendTo(buffer);
        }

        if (debug != null && _data?.ContainsKey(debug.Id) != true)
        {
            // Space if null or empty is true
            if (_data?.IsEmpty != false)
            {
                buffer.Append(' ');
            }

            ExtendDebug(options, debug);
            debug.AppendTo(buffer);
        }

        AppendMessage(buffer, options);
    }

    private void AppendBsd(StringBuilder buffer, IReadOnlyLogOptions options, SdDebug? debug)
    {
        // Clamp within valid range (see DEBUG_L1 etc.)
        buffer.Append('<');
        buffer.Append((Clamp(Severity) | (int)options.Facility).ToString(CultureInfo.InvariantCulture));
        buffer.Append('>');

        buffer.Append(ToTimestamp(options));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.LocalHost));

        AppendMessage(buffer, options);
        AppendSimpleDebug(buffer, options, debug);
    }

    private void AppendText(StringBuilder buffer, IReadOnlyLogOptions options, SdDebug? debug)
    {
        AppendSimpleDebug(buffer, options, debug);

        buffer.Append(" :");
        AppendMessage(buffer, options);
    }

    private string ToTimestamp(IReadOnlyLogOptions options)
    {
        var t = options.IsTimeUtc ? Time.ToUniversalTime() : Time.ToLocalTime();

        switch (options.Format)
        {
            case FormatKind.Rfc5424: return t.ToString(TimeFormat5424, CultureInfo.InvariantCulture);
            case FormatKind.Bsd: return t.ToString(TimeFormatBsd, CultureInfo.InvariantCulture);
            default: return t.ToString(TimeFormatText, CultureInfo.InvariantCulture);
        }
    }

    private void ExtendDebug(IReadOnlyLogOptions options, SdDebug? debug)
    {
        if (debug != null)
        {
            debug.Add("SEVERITY", Severity.ToString().ToUpperInvariant());
            debug.Add("THREAD", GetAppThread(options.AppPid));
        }
    }

    private void AppendSimpleDebug(StringBuilder buffer, IReadOnlyLogOptions options, SdDebug? debug)
    {
        if (debug != null)
        {
            if (buffer.Length != 0)
            {
                buffer.Append(' ');
            }

            buffer.Append('[');
            buffer.Append(Severity.ToString().ToUpperInvariant());
            buffer.Append(", ");
            buffer.Append(GetAppThread(options.AppPid));

            if (debug.Function != null)
            {
                buffer.Append(", ");
                buffer.Append(debug.Function);

                if (debug.LineNumber != null)
                {
                    buffer.Append(", #");
                    buffer.Append(debug.LineNumber);
                }
            }

            buffer.Append(']');
        }
    }

    private void AppendMessage(StringBuilder buffer, IReadOnlyLogOptions options)
    {
        if (!string.IsNullOrEmpty(Text))
        {
            int max = options.MaxTextLength;

            if (buffer.Length != 0)
            {
                buffer.Append(' ');
            }

            if (max > 0 && Text.Length > max)
            {
                buffer.Append(StructuredData.Escape(Text.Substring(0, max), false));
            }
            else
            {
                buffer.Append(StructuredData.Escape(Text, false));
            }
        }
    }

}