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
using KuiperZone.Utility.Yaal.Internal;

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
    public LogMessage(string? msgId, SeverityLevel severity, string? text)
    {
        MsgId = msgId;
        Severity = severity;
        Text = text;
    }

    /// <summary>
    /// Gets or sets the time. The initial value is always set from the system clock.
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the MSG-ID.
    /// </summary>
    public string? MsgId { get; set; }

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    public SeverityLevel Severity { get; set; } = SeverityLevel.Informational;

    /// <summary>
    /// Gets or sets the message text.
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Gets or sets attached debug information.
    /// This is for internal use only.
    /// </summary>
    public DebugInfo? Debug { get; set; }

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
    /// Overrides and equivalent to: ToString(<see cref="FormatKind.Text"/>).
    /// </summary>
    public override string ToString()
    {
        return ToString(FormatKind.Text);
    }

    /// <summary>
    /// Returns a string output according to format and options. The "includePriority" applies
    /// to RFC 5424 and BSD and, if false, the leading priority code is omitted.
    /// </summary>
    public string ToString(FormatKind format, IReadOnlyLogOptions? options = null, bool includePriority = true)
    {
        options ??= new LogOptions();
        var buffer = new StringBuilder(1024);

        switch (format)
        {
            case FormatKind.Rfc5424:
                AppendRfc5424(buffer, options, includePriority);
                break;
            case FormatKind.Bsd:
                AppendBsd(buffer, options, includePriority);
                break;
            default:
                AppendText(buffer, options);
                break;
        }

        return buffer.ToString();
    }

    private static int Clamp(SeverityLevel value)
    {
        if (value >= SeverityLevel.Debug)
        {
            return (int)SeverityLevel.Debug;
        }

        return (int)(value > SeverityLevel.Emergency ? value : SeverityLevel.Emergency);
    }

    private static string ValueOrNil(string? value)
    {
        return string.IsNullOrEmpty(value) ? "-" : value;
    }

    private void AppendRfc5424(StringBuilder buffer, IReadOnlyLogOptions options, bool includePriority)
    {
        if (includePriority)
        {
            // Will clamp with legal severity range
            buffer.Append('<');
            buffer.Append(Severity.ToPriorityCode(options.Facility));
            buffer.Append('>');
        }

        buffer.Append("1 ");
        buffer.Append(ToTimestamp(FormatKind.Rfc5424, options));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.HostName));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.AppName));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.AppPid));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(MsgId));

        buffer.Append(' ');
        bool hasSd = false;

        if (_data?.IsEmpty == false)
        {
            hasSd = true;
            _data.AppendTo(buffer, options);
        }

        if (Debug?.Function != null &&
            !string.IsNullOrEmpty(options.DebugId) &&
            _data?.ContainsKey(options.DebugId) != true)
        {
            hasSd = true;

            var e = new SdElement(options.DebugId);
            e.Add("SEVERITY", Severity.ToString().ToUpperInvariant());
            e.Add("FUNCTION", Debug.Function);

            if (Debug.LineNumber > 0)
            {
                e.Add("LINE", Debug.LineNumber.ToString(CultureInfo.InvariantCulture));
            }

            e.Add("THREAD", AppInfo.Pid + "-" + LogUtil.ThreadName);
            e.AppendTo(buffer, options);
        }

        if (!hasSd)
        {
            // NIL SD
            buffer.Append('-');
        }

        AppendMessage(buffer, options);
    }

    private void AppendBsd(StringBuilder buffer, IReadOnlyLogOptions options, bool includePriority)
    {
        if (includePriority)
        {
            // Will clamp with legal severity range
            buffer.Append('<');
            buffer.Append(Severity.ToPriorityCode(options.Facility));
            buffer.Append('>');
        }

        buffer.Append(ToTimestamp(FormatKind.Bsd, options));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(options.HostName));

        if (!string.IsNullOrEmpty(options.AppName))
        {
            buffer.Append(' ');
            buffer.Append(ValueOrNil(options.AppName));

            if (!string.IsNullOrEmpty(options.AppPid))
            {
                buffer.Append('[');
                buffer.Append(options.AppPid);
                buffer.Append(']');
            }

            buffer.Append(':');
        }

        AppendMessage(buffer, options);

        if (Debug?.Function != null)
        {
            // No standard for BSD debug data, so just
            // tag on SD output after message text.
            buffer.Append(' ');
            buffer.Append(Debug.ToString());
        }
    }

    private void AppendText(StringBuilder buffer, IReadOnlyLogOptions options)
    {
        if (Debug?.Function != null)
        {
            // Debug leads with: ConsoleApp1.Program.Main(String[] args) #47 : MESSAGE
            buffer.Append(Debug.ToString());
            buffer.Append(" :");
        }

        AppendMessage(buffer, options);
    }

    private string ToTimestamp(FormatKind format, IReadOnlyLogOptions options)
    {
        var t = options.IsTimeUtc ? Time.ToUniversalTime() : Time.ToLocalTime();

        switch (format)
        {
            case FormatKind.Rfc5424: return t.ToString(TimeFormat5424, CultureInfo.InvariantCulture);
            case FormatKind.Bsd: return t.ToString(TimeFormatBsd, CultureInfo.InvariantCulture);
            default: return t.ToString(TimeFormatText, CultureInfo.InvariantCulture);
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
                buffer.Append(LogUtil.Escape(Text.Substring(0, max)));
            }
            else
            {
                buffer.Append(LogUtil.Escape(Text));
            }
        }
    }

}