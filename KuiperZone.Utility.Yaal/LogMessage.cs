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
    /// <summary>
    /// Maximum <see cref="MsgId"/> length.
    /// </summary>
    public const int MaxMsgIdLength = 32;

    private const string Time5424 = "yyyy-MM-ddTHH:mm:ss.ffffffK";
    private const string TimeBsd = "MMM dd HH:mm:ss";
    private const string TimeGeneral = "MMM dd HH:mm:ss.ffffff";

    private string? _msgId;

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
    /// Gets or sets the MSG-ID. The value is limited to ASCII and 32 characters.
    /// The value is not included in the BSD format.
    /// </summary>
    public string? MsgId
    {
        get { return _msgId; }
        set { _msgId = LogUtil.EnsureId(value, MaxMsgIdLength); }
    }

    /// <summary>
    /// Gets or sets the severity level.
    /// </summary>
    public SeverityLevel Severity { get; set; } = SeverityLevel.Info;

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
    public StructuredData Data { get; } = new();

    /// <summary>
    /// Overrides and equivalent to: ToString(<see cref="LogFormat.General"/>).
    /// </summary>
    public override string ToString()
    {
        return ToString(LogFormat.General);
    }

    /// <summary>
    /// Returns a string output according to format.
    /// </summary>
    public string ToString(LogFormat format, PriorityKind priority = PriorityKind.Default)
    {
        var opts = new LogOptions();
        opts.Priority = priority;
        return ToString(format, opts, 2048);
    }

    /// <summary>
    /// Returns a string output according to the options and maximum text length.
    /// </summary>
    public string ToString(LogFormat format, IReadOnlyLogOptions opts,
        int maxLength = 1024, int debugPad = 0)
    {
        var buffer = new StringBuilder(1024);

        switch (format)
        {
            case LogFormat.Rfc5424:
                AppendRfc5424(buffer, opts, maxLength);
                break;
            case LogFormat.Bsd:
                AppendBsd(buffer, opts, maxLength);
                break;
            case LogFormat.General:
                AppendGeneral(buffer, opts, maxLength, debugPad);
                break;
            default:
                AppendText(buffer, opts, maxLength);
                break;
        }

        return buffer.ToString();
    }

    private static void AppendSpace(StringBuilder buffer)
    {
        if (buffer.Length != 0)
        {
            buffer.Append(' ');
        }
    }

    private static void AppendValueOrNil(StringBuilder buffer, string? value)
    {
        buffer.Append(string.IsNullOrEmpty(value) ? "-" : value);
    }


    private void AppendRfc5424(StringBuilder buffer, IReadOnlyLogOptions opts, int maxLength)
    {
        buffer.Append(Severity.ToPriority(opts.Priority, opts.Facility));
        buffer.Append("1 ");
        AppendTime(buffer, LogFormat.Rfc5424, opts);

        buffer.Append(' ');
        AppendValueOrNil(buffer, opts.HostName);

        buffer.Append(' ');
        AppendValueOrNil(buffer, opts.AppName);

        buffer.Append(' ');
        AppendValueOrNil(buffer, opts.AppPid);

        buffer.Append(' ');
        AppendValueOrNil(buffer, LogUtil.EnsureId(MsgId, MaxMsgIdLength));

        buffer.Append(' ');
        bool hasData = false;

        if (!Data.IsEmpty)
        {
            hasData = true;
            Data.AppendTo(buffer);
        }

        if (Debug?.Function != null && !string.IsNullOrEmpty(opts.DebugId) && Data?.ContainsKey(opts.DebugId) != true)
        {
            hasData = true;

            var e = new SdElement();
            e.Add("SEVERITY", Severity.ToKeyword());
            e.Add("FUNC", Debug.Function);

            if (Debug.LineNumber > 0)
            {
                e.Add("LINE", Debug.LineNumber.ToString(CultureInfo.InvariantCulture));
            }

            e.Add("THREAD", AppInfo.Pid + "-" + LogUtil.ThreadName);
            e.AppendTo(buffer, opts.DebugId);
        }

        if (!hasData)
        {
            // NIL SD
            buffer.Append('-');
        }

        AppendText(buffer, opts, maxLength);
    }

    private void AppendBsd(StringBuilder buffer, IReadOnlyLogOptions opts, int maxLength)
    {
        buffer.Append(Severity.ToPriority(opts.Priority, opts.Facility));
        AppendTime(buffer, LogFormat.Bsd, opts);

        buffer.Append(' ');
        AppendValueOrNil(buffer, opts.HostName);

        if (!string.IsNullOrEmpty(opts.AppName))
        {
            buffer.Append(' ');
            AppendValueOrNil(buffer, opts.AppName);

            if (!string.IsNullOrEmpty(opts.AppPid))
            {
                buffer.Append('[');
                buffer.Append(opts.AppPid);
                buffer.Append(']');
            }

            buffer.Append(':');
        }

        AppendText(buffer, opts, maxLength);

        if (Debug?.Function != null)
        {
            // No standard for BSD debug data, so just
            // tag on after message text.
            buffer.Append(" @ ");
            buffer.Append(Debug.ToString());
        }
    }

    private void AppendGeneral(StringBuilder buffer, IReadOnlyLogOptions opts, int maxLength, int debugPad)
    {
        if (Debug?.Function != null)
        {
            // Debug leads with: ConsoleApp1.Program.Main(String[] args) #47
            buffer.Append(Debug.ToString());
            buffer.Append(" :");

            int pad = Math.Clamp(debugPad - buffer.Length - 1, 0, 1000);
            buffer.Append(' ', pad);
        }

        var msgId = LogUtil.EnsureId(MsgId, MaxMsgIdLength);

        if (!string.IsNullOrEmpty(msgId))
        {
            AppendSpace(buffer);
            buffer.Append('[');
            buffer.Append(msgId);
            buffer.Append(']');
        }

        AppendText(buffer, opts, maxLength);

        if (Debug?.Function != null)
        {
            // Only if debug
            AppendSpace(buffer);
            buffer.Append("@ ");
            AppendTime(buffer, LogFormat.General, opts);
        }
    }

    private void AppendText(StringBuilder buffer, IReadOnlyLogOptions opts, int maxLength)
    {
        if (!string.IsNullOrEmpty(Text))
        {
            // Separator
            AppendSpace(buffer);

            if (maxLength > 0 && Text.Length > maxLength)
            {
                buffer.Append(LogUtil.Escape(Text.Substring(0, maxLength)));
            }
            else
            {
                buffer.Append(LogUtil.Escape(Text));
            }
        }
    }

    private void AppendTime(StringBuilder buffer, LogFormat format, IReadOnlyLogOptions opts)
    {
        var t = opts.IsTimeUtc ? Time.ToUniversalTime() : Time.ToLocalTime();

        switch (format)
        {
            case LogFormat.Rfc5424:
                buffer.Append(t.ToString(Time5424, CultureInfo.InvariantCulture));
                break;
            case LogFormat.Bsd:
                buffer.Append(t.ToString(TimeBsd, CultureInfo.InvariantCulture));
                break;
            default:
                buffer.Append(t.ToString(TimeGeneral, CultureInfo.InvariantCulture));
                break;
        }
    }
}