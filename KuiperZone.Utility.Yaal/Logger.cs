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
using KuiperZone.Utility.Yaal.Internal;
using KuiperZone.Utility.Yaal.Sinks;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// </summary>
public sealed class Logger
{
    private readonly object _syncObj = new();
    public volatile IReadOnlyLogOptions v_options = new LogOptions();
    private volatile IReadOnlyCollection<ILogSink> v_sinks = CreateSinks();
    private volatile SeverityLevel v_threshold = SeverityLevel.Informational;

    public Logger()
    {

    }

    /// <summary>
    /// Gets a global singleton.
    /// </summary>
    public static readonly Logger Global = new();

    public IReadOnlyCollection<ILogSink> Sinks
    {
        get { return v_sinks; }
        set { v_sinks = new List<ILogSink>(value); }
    }

    public IReadOnlyLogOptions Options
    {
        get { return v_options; }
        set { v_options = value; }
    }

    public SeverityLevel Threshold
    {
        get { return v_threshold; }
        set { v_threshold = value; }
    }

    public void AddSink(params ILogSink[] sinks)
    {
        var temp = new List<ILogSink>(v_sinks);
        temp.AddRange(sinks);
        v_sinks = temp;
    }

    public void Write(LogMessage message)
    {
        if (message.Severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(message);
        }
    }

    public void Write(string? text)
    {
        if (SeverityLevel.Informational.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(SeverityLevel.Informational, text));
        }
    }

    public void Write(SeverityLevel severity, string? text)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(severity, text));
        }
    }

    public void Write(string? msgId, SeverityLevel severity, string? text)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(msgId, severity, text));
        }
    }

    public void Write(Exception e)
    {
        if (SeverityLevel.Error.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(SeverityLevel.Error, e.Message));
        }
    }

    public void Write(SeverityLevel severity, Exception e)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(severity, e.Message));
        }
    }

    public void Write(string? msgId, SeverityLevel severity, Exception e)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(msgId, severity, e.Message));
        }
    }

    public void WriteIf(bool condition, LogMessage message)
    {
        if (condition && message.Severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(message);
        }
    }

    public void WriteIf(bool condition, string? text)
    {
        if (condition && SeverityLevel.Informational.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(SeverityLevel.Informational, text));
        }
    }

    public void WriteIf(bool condition, SeverityLevel severity, string? text)
    {
        if (condition && severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(severity, text));
        }
    }

    public void WriteIf(bool condition, string? msgId, SeverityLevel severity, string? text)
    {
        if (condition && severity.IsHigherOrEqualThan(v_threshold))
        {
            WriteInternal(new LogMessage(msgId, severity, text));
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(LogMessage message)
    {
        if (message.Severity.IsHigherOrEqualThan(v_threshold))
        {
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(string? text)
    {
        if (SeverityLevel.Debug.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(SeverityLevel.Debug, text);
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(SeverityLevel severity, string? text)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(severity, text);
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(string? msgId, SeverityLevel severity, string? text)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(msgId, severity, text);
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(Exception e)
    {
        if (SeverityLevel.Error.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(SeverityLevel.Error, e.ToString());
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(SeverityLevel severity, Exception e)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(severity, e.ToString());
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void Debug(string? msgId, SeverityLevel severity, Exception e)
    {
        if (severity.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(msgId, severity, e.ToString());
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, LogMessage message)
    {
        if (condition && message.Severity.IsHigherOrEqualThan(v_threshold))
        {
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, string? text)
    {
        if (condition && SeverityLevel.Debug.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(SeverityLevel.Debug, text);
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, SeverityLevel severity, string? text)
    {
        if (condition && severity.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(severity, text);
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    [System.Diagnostics.Conditional("DEBUG")]
    public void DebugIf(bool condition, string? msgId, SeverityLevel severity, string? text)
    {
        if (condition && severity.IsHigherOrEqualThan(v_threshold))
        {
            var message = new LogMessage(msgId, severity, text);
            message.Debug ??= new DebugInfo(nameof(Debug));
            WriteInternal(message);
        }
    }

    private static ILogSink[] CreateSinks()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var opts = new FileSinkOptions();
            return new ILogSink[] { new FileSink(opts) };
        }

        return new ILogSink[] { new SyslogSink() };
    }

    private void WriteInternal(LogMessage message)
    {
    }
}