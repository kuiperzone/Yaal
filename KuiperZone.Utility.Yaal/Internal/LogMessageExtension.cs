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

namespace KuiperZone.Utility.Yaal.Internal;

/// <summary>
/// Extends <see cref="LogMessage"/>. Internal but public for unit testing.
/// </summary>
public static class MessageExtension
{
    private const string Time5424 = "yyyy-MM-ddTHH:mm:ss.ffffffK";
    private const string TimeBsd = "MMM dd HH:mm:ss";
    private const string TimeText = "MMM dd HH:mm:ss.ffffff";

    /// <summary>
    /// Returns a string output according to options.
    /// </summary>
    public static string ToString(this LogMessage msg, MessageStringOptions opts)
    {
        var buffer = new StringBuilder(1024);

        switch (opts.Format)
        {
            case LogFormat.Rfc5424:
                AppendRfc5424(buffer, msg, opts);
                break;
            case LogFormat.Bsd:
                AppendBsd(buffer, msg, opts);
                break;
            case LogFormat.Clean:
                AppendClean(buffer, msg, opts);
                break;
            default:
                buffer.Append(msg.Text);
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

    private static void AppendRfc5424(StringBuilder buffer, LogMessage msg, MessageStringOptions opts)
    {
        var lcfg = opts.LoggerConfig;

        if (opts.IncludePriority)
        {
            // Will clamp with legal severity range
            buffer.Append('<');
            buffer.Append(msg.Severity.ToPriorityCode(lcfg.Facility));
            buffer.Append('>');
        }

        buffer.Append("1 ");
        buffer.Append(ToTimestamp(msg, opts));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(lcfg.HostName));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(lcfg.AppName));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(lcfg.AppPid));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(msg.MsgId));

        buffer.Append(' ');
        bool hasData = false;

        if (!msg.Data.IsEmpty)
        {
            hasData = true;
            msg.Data.AppendTo(buffer, opts.LoggerConfig);
        }

        string? dbgId = opts.LoggerConfig.DebugId;

        if (msg.Debug?.Function != null &&
            !string.IsNullOrEmpty(dbgId) &&
            msg.Data?.ContainsKey(dbgId) != true)
        {
            hasData = true;

            var e = new SdElement(dbgId);
            e.Add("SEVERITY", msg.Severity.ToString().ToUpperInvariant());
            e.Add("FUNCTION", msg.Debug.Function);

            if (msg.Debug.LineNumber > 0)
            {
                e.Add("LINE", msg.Debug.LineNumber.ToString(CultureInfo.InvariantCulture));
            }

            e.Add("THREAD", AppInfo.Pid + "-" + LogUtil.ThreadName);
            e.AppendTo(buffer, lcfg);
        }

        if (!hasData)
        {
            // NIL SD
            buffer.Append('-');
        }

        buffer.Append(' ');
        AppendMessage(buffer, msg, opts);
    }

    private static void AppendBsd(StringBuilder buffer, LogMessage msg, MessageStringOptions opts)
    {
        var lcfg = opts.LoggerConfig;

        if (opts.IncludePriority)
        {
            // Will clamp with legal severity range
            buffer.Append('<');
            buffer.Append(msg.Severity.ToPriorityCode(lcfg.Facility));
            buffer.Append('>');
        }

        buffer.Append(ToTimestamp(msg, opts));

        buffer.Append(' ');
        buffer.Append(ValueOrNil(lcfg.HostName));

        if (!string.IsNullOrEmpty(lcfg.AppName))
        {
            buffer.Append(' ');
            buffer.Append(ValueOrNil(lcfg.AppName));

            if (!string.IsNullOrEmpty(lcfg.AppPid))
            {
                buffer.Append('[');
                buffer.Append(lcfg.AppPid);
                buffer.Append(']');
            }

            buffer.Append(':');
        }

        buffer.Append(' ');
        AppendMessage(buffer, msg, opts);

        if (msg.Debug?.Function != null)
        {
            // No standard for BSD debug data, so just
            // tag on SD output after message text.
            buffer.Append(' ');
            buffer.Append(msg.Debug.ToString());
        }
    }

    private static void AppendClean(StringBuilder buffer, LogMessage msg, MessageStringOptions opts)
    {
        if (msg.Debug?.Function != null)
        {
            // Debug leads with: ConsoleApp1.Program.Main(String[] args) #47
            buffer.Append(msg.Debug.ToString());
            buffer.Append(" : ");
        }

        int pad = Math.Clamp(opts.IndentClean - buffer.Length, 0, 1000);
        buffer.Append(' ', pad);

        if (!string.IsNullOrEmpty(msg.MsgId))
        {
            buffer.Append('[');
            buffer.Append(msg.MsgId);
            buffer.Append("] ");
        }

        AppendMessage(buffer, msg, opts);

        buffer.Append(" @");
        buffer.Append(ToTimestamp(msg, opts));
    }

    private static string ToTimestamp(LogMessage msg, MessageStringOptions opts)
    {
        var t = opts.LoggerConfig.IsTimeUtc ? msg.Time.ToUniversalTime() : msg.Time.ToLocalTime();

        switch (opts.Format)
        {
            case LogFormat.Rfc5424:
                return t.ToString(Time5424, CultureInfo.InvariantCulture);
            case LogFormat.Bsd:
                return t.ToString(TimeBsd, CultureInfo.InvariantCulture);
            default:
                return t.ToString(TimeText, CultureInfo.InvariantCulture);
        }
    }

    private static void AppendMessage(StringBuilder buffer, LogMessage msg, MessageStringOptions opts)
    {
        if (!string.IsNullOrEmpty(msg.Text))
        {
            int max = opts.MaxTextLength;

            if (max > 0 && msg.Text.Length > max)
            {
                buffer.Append(LogUtil.Escape(msg.Text.Substring(0, max)));
            }
            else
            {
                buffer.Append(LogUtil.Escape(msg.Text));
            }
        }
    }

}