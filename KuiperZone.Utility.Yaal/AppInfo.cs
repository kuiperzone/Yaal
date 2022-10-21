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
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Provides application (entry assembly) information.
/// </summary>
public static class AppInfo
{
    /// <summary>
    /// Static constructor.
    /// </summary>
    static AppInfo()
    {
        HostName = GetHostName();
        Pid = GetProcId();

        try
        {
            var ea = Assembly.GetEntryAssembly();

            if (ea != null)
            {
                var eaName = ea.GetName();
                AssemblyName = eaName.Name ?? "";

                int pos = AssemblyName.LastIndexOf('.');
                AppName = pos > 0 ? AssemblyName.Substring(pos + 1) : AssemblyName;

                Version = eaName.Version ?? new Version();

                foreach (var attrib in ea.GetCustomAttributes(false))
                {
                    if (attrib.GetType() == typeof(DebuggableAttribute))
                    {
                        IsDebug = ((DebuggableAttribute)attrib).IsJITTrackingEnabled;
                        IsOptimized = !((DebuggableAttribute)attrib).IsJITOptimizerDisabled;
                        break;
                    }
                }
            }
        }
        catch
        {
        }

        AssemblyName ??= "";
        AppName ??= "";
        Version ??= new Version();
    }

    /// <summary>
    /// Gets an hostname.
    /// </summary>
    public static string HostName { get; }

    /// <summary>
    /// Gets the application name. This is to be the last element in <see cref="AssemblyName"/>.
    /// </summary>
    public static string AppName { get; }

    /// <summary>
    /// Gets the entry assembly version.
    /// </summary>
    public static Version Version { get; }

    /// <summary>
    /// Gets the entry assembly name.
    /// </summary>
    public static string AssemblyName { get; }

    /// <summary>
    /// Gets an application process-id.
    /// </summary>
    public static string Pid { get; }

    /// <summary>
    /// Gets whether the entry assembly has debug JIT tracking enabled.
    /// </summary>
    public static bool IsDebug { get; }

    /// <summary>
    /// Gets whether the entry assembly has JIT optimization enabled.
    /// </summary>
    public static bool IsOptimized { get; }

    public static string ToString(bool verbose)
    {
        var buffer = new StringBuilder(512);

        if (verbose)
        {
            buffer.Append(nameof(HostName));
            buffer.Append('=');
            buffer.Append(HostName);
            buffer.Append(", ");
        }

        buffer.Append(nameof(AppName));
        buffer.Append('=');
        buffer.Append(AppName);
        buffer.Append(", ");

        buffer.Append(nameof(Version));
        buffer.Append('=');
        buffer.Append(Version);
        buffer.Append(", ");

        buffer.Append(nameof(AssemblyName));
        buffer.Append('=');
        buffer.Append(AssemblyName);
        buffer.Append(", ");

        buffer.Append(nameof(Pid));
        buffer.Append('=');
        buffer.Append(Pid);
        buffer.Append(", ");

        if (verbose)
        {
            buffer.Append(nameof(IsDebug));
            buffer.Append('=');
            buffer.Append(IsDebug);
            buffer.Append(", ");

            buffer.Append(nameof(IsOptimized));
            buffer.Append('=');
            buffer.Append(IsOptimized);
            buffer.Append(", ");
        }

        return buffer.ToString();
    }

    private static string GetProcId()
    {
        try
        {
            return Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);
        }
        catch
        {
            return "";
        }
    }

    private static string GetHostName()
    {
        try
        {
            return Dns.GetHostName();
        }
        catch
        {
            return "";
        }
    }
}
