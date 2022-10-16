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
/// Implements <see cref="ILogSink"/> for syslog on Linux. The class calls the "logger"
/// command line utility to log a message. It is not supported on Windows.
/// </summary>
public sealed class SyslogSink : ILogSink
{
    private static readonly bool _isSupported = GetSupported();
    private volatile bool v_isFailed;

    /// <summary>
    /// Constructor with option values. Serves as default constructor.
    /// Throws if "logger" not supported on this platform.
    /// </summary>
    /// <exception cref="PlatformNotSupportedException">Not supported on this platform</exception>
    public SyslogSink(FormatKind format = FormatKind.Rfc5424, SeverityLevel threshold = SeverityLevel.Lowest)
        : this(new SinkOptions(format, threshold))
    {
    }

    /// <summary>
    /// Constructor with options instance.
    /// Throws if "logger" not supported on the system.
    /// </summary>
    /// <exception cref="InvalidOperationException">Not supported on this system</exception>
    public SyslogSink(IReadOnlySinkOptions options)
    {
        if (!IsSupported)
        {
            throw new PlatformNotSupportedException($"{nameof(SyslogSink)} not supported on this system");
        }

        // Take a copy
        Options = new SinkOptions(options);
    }

    /// <summary>
    /// Gets whether syslog is supported on the platform.
    /// </summary>
    public static bool IsSupported
    {
        get { return _isSupported; }
    }

    /// <summary>
    /// Gets whether the instance has failed.
    /// </summary>
    public bool IsFailed
    {
        get { return v_isFailed; }
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
        if (!v_isFailed)
        {
            // It seems that we need to provide priority as an option
            var buffer = new StringBuilder("-p ", 1024);
            buffer.Append(message.Severity.ToPriorityPair(options.Facility));
            buffer.Append(' ');

            buffer.Append('"');
            var text = message.ToString(Options.Format, options, false);
            buffer.Append(LogUtil.Escape(text, "\\\""));
            buffer.Append('"');

            Console.WriteLine(buffer.ToString());

            try
            {
                if (!ExecLog(buffer.ToString()))
                // if (!ExecLog("\"" + LogUtil.Escape(message, "\\\"") + "\""))
                {
                    v_isFailed = true;
                }
            }
            catch
            {
                v_isFailed = true;
                throw;
            }
        }
    }

    private static bool GetSupported()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        try
        {
            return ExecLog("--version", true);
        }
        catch
        {
            return false;
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
}