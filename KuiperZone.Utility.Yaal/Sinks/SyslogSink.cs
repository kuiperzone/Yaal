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
    /// Constructor. Throws if "logger" not supported on the system.
    /// </summary>
    /// <exception cref="InvalidOperationException">Not supported on this system</exception>
    public SyslogSink(SeverityLevel? threshold = null)
    {
        if (!IsSupported)
        {
            throw new InvalidOperationException($"{nameof(SyslogSink)} not supported on this system");
        }

        Threshold = threshold;
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Threshold"/>.
    /// </summary>
    public SeverityLevel? Threshold { get; }

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
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(SeverityLevel severity, string message)
    {
        if (message.Length != 0 && !v_isFailed)
        {
            try
            {
                if (!ExecLog("\"" + LogUtil.Escape(message, "\\\"") + "\""))
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