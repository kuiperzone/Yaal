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
using System.Runtime.InteropServices;
using System.Text;
using KuiperZone.Utility.Yaal.Internal;

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Implements <see cref="ILogSink"/> for syslog (logger) on Linux, and EventLog on Windows.
/// </summary>
public sealed class SyslogLogSink : ILogSink
{
    private static readonly object _syncObj = new();
    private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private readonly SyslogSinkOptions _options;
    private EventLog? _winLog;
    private volatile bool v_isFailed;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SyslogLogSink()
    {
        _options = new SyslogSinkOptions();
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public SyslogLogSink(SyslogSinkOptions opts)
    {
        // Take a copy
        _options = new SyslogSinkOptions(opts);
    }

    /// <summary>
    /// Gets whether the instance has failed.
    /// </summary>
    public bool IsFailed
    {
        get { return v_isFailed; }
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>. It may throw on Linux where the
    /// "logger" shell command is not available.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Not supported on this platform</exception>
    public void Write(LogMessage msg, IReadOnlyLoggerOptions opts)
    {
        if (msg.Severity.IsHigherOrEqualPriority(_options.Threshold) && !v_isFailed)
        {
            try
            {
                if (_isWindows)
                {
                    WriteWindows(msg, opts);
                }
                else
                {
                    WriteLinux(msg, opts);
                }
            }
            catch
            {
                v_isFailed = true;

                if (!_isWindows && !ExecLog("--version", true))
                {
                    throw new PlatformNotSupportedException("Linux logger not available");
                }

                throw;
            }
        }
    }

    /// <summary>
    /// Implements.
    /// </summary>
    public void Dispose()
    {
        try
        {
            _winLog?.Dispose();
        }
        catch
        {
        }
    }

    private static bool ExecLog(string args, bool wait = false)
    {
        var info = new ProcessStartInfo
        {
            FileName = "logger",
            Arguments = args,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using var proc = Process.Start(info);

        if (proc != null)
        {
            if (!wait)
            {
                return true;
            }

            if (proc.WaitForExit(1000))
            {
                return proc.ExitCode == 0;
            }
        }

        return false;
    }

    private static EventLogEntryType ToEntryType(SeverityLevel severity)
    {
        #pragma warning disable CA1416

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

    public void WriteLinux(LogMessage msg, IReadOnlyLoggerOptions opts)
    {
        // It seems that we need to provide priority as an option for syslog
        var mo = new MessageParams(_options, opts);
        mo.IncludePriority = false;

        var text = msg.ToString(mo);

        // It seems that we need to provide priority as an option
        var buffer = new StringBuilder("-p ", 1024);
        buffer.Append(msg.Severity.ToPriorityPair(opts.Facility));
        buffer.Append(' ');

        buffer.Append('"');
        buffer.Append(LogUtil.Escape(text, "\\\""));
        buffer.Append('"');

        if (!ExecLog(buffer.ToString()))
        {
            throw new InvalidOperationException("Syslog 'logger' command failed: " + buffer.ToString());
        }
    }

    public void WriteWindows(LogMessage msg, IReadOnlyLoggerOptions opts)
    {
        var mo = new MessageParams(_options, opts);
        mo.IncludePriority = true;

        var text = msg.ToString(mo);

        lock (_syncObj)
        {
            if (_winLog == null)
            {
                _winLog = new("Application");
                _winLog.Source = "Application";
            }

            // _event.MachineName = config.HostName;
            _winLog.WriteEntry(text, ToEntryType(msg.Severity));
        }
    }

}