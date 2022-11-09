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

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Implements <see cref="ILogSink"/> for syslog (logger) on Linux, and EventLog (Application) on Windows.
/// </summary>
public sealed class SyslogSink : ILogSink
{
    // EVENTLOG REF:
    // https://www.jitbit.com/alexblog/266-writing-to-an-event-log-from-net-without-the-description-for-event-id-nonsense/
    private static readonly object s_syncObj = new();
    private static IntPtr s_identPtr = IntPtr.Zero;
    private static bool s_isSysOpen;

    private readonly object _syncObj = new();
    private readonly SyslogSinkOptions _options;

    private EventLog? _winLog;
    private volatile bool v_isFailed;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SyslogSink()
    {
        _options = new SyslogSinkOptions();
    }

    /// <summary>
    /// Constructor with options instance.
    /// </summary>
    public SyslogSink(SyslogSinkOptions opts)
    {
        // Take a copy
        _options = new SyslogSinkOptions(opts);
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Not supported on this platform</exception>
    public void Write(LogMessage msg, IReadOnlyLogOptions opts)
    {
        if (msg.Severity.IsHigherOrEqualPriority(_options.Threshold) && !v_isFailed)
        {
            try
            {
                if (OpenSyslog(opts))
                {
                    WriteSyslog(msg, opts);
                }
                else
                {
                    WriteEventLog(msg, opts);
                }
            }
            catch
            {
                // This only prevents repeated calls if failing
                // It is not of critical importance, neither is any race.
                v_isFailed = true;
                throw;
            }
        }
    }

    [DllImport("libc")]
    private static extern void openlog(IntPtr ident, int option, int facility);

    [DllImport("libc")]
    private static extern void syslog(int priority, string message);

    [DllImport("libc")]
    private static extern void closelog();

    private static bool OpenSyslog(IReadOnlyLogOptions opts)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        // See if we detect non-locked value
        if (s_isSysOpen)
        {
            return true;
        }

        lock (s_syncObj)
        {
            if (!s_isSysOpen)
            {
                s_isSysOpen = true;

                if (!string.IsNullOrEmpty(opts.AppName))
                {
                    // Open with AppName in options of first call.
                    s_identPtr = Marshal.StringToHGlobalAnsi(opts.AppName);
                }

                // We keep open for lifetime of application.
                openlog(s_identPtr, 0x01, (int)opts.Facility);
            }

            return true;
        }
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

    private void WriteSyslog(LogMessage msg, IReadOnlyLogOptions opts)
    {
        // It seems that we need to provide priority as an option for syslog
        int priority = msg.Severity.ToPriorityCode(opts.Facility);
        var text = msg.ToString(_options.Format, opts, _options.MaxTextLength);
        syslog(priority, text);
    }

    private void WriteEventLog(LogMessage msg, IReadOnlyLogOptions opts)
    {
        var text = msg.ToString(_options.Format, opts, _options.MaxTextLength);

        lock (_syncObj)
        {
            if (_winLog == null)
            {
                _winLog = new("Application");
                _winLog.Source = _options.EventLogSource;
            }

            // Leave at local machine
            // _event.MachineName = opts.HostName;
            _winLog.WriteEntry(text, ToEntryType(msg.Severity));
        }
    }

    /*
    // Leave this for reference for time being. It is an implementation
    // for syslog which calls the logger command utility. It is some 10 times
    // slower than DllImport, but otherwise works well. If there's a portability
    // issue with lib import, we could use this.

    private void WriteLinux(LogMessage msg, IReadOnlyLogOptions opts)
    {
        // It seems that we need to provide priority as an option for syslog
        var text = msg.ToString(_options.Format, opts, _options.MaxTextLength);

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
    */

}