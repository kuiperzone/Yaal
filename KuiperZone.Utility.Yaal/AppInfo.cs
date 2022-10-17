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

                var info = FileVersionInfo.GetVersionInfo(ea.Location);
                ProductName = info.ProductName ?? AppName;
                Company = info.CompanyName ?? "";
                Copyright = info.LegalCopyright ?? "";
                FileName = info.FileName;
            }
        }
        catch
        {
        }

        AssemblyName ??= "";
        AppName ??= "";
        Version ??= new Version();
        ProductName ??= "";
        Company ??= "";
        Copyright ??= "";
        FileName ??= "";
    }

    /// <summary>
    /// Gets an hostname.
    /// </summary>
    public static string HostName { get; }

    /// <summary>
    /// Gets an application process-id.
    /// </summary>
    public static string Pid { get; }

    /// <summary>
    /// Gets the application name, being the last element in <see cref="AssemblyName"/>.
    /// </summary>
    public static string AppName { get; }

    /// <summary>
    /// Gets the entry assembly name.
    /// </summary>
    public static string AssemblyName { get; }

    /// <summary>
    /// Gets the entry assembly version.
    /// </summary>
    public static Version Version { get; }

    /// <summary>
    /// Gets the application ProductName, falling back to <see cref="AppName"/> if not defined.
    /// </summary>
    public static string ProductName { get; }

    /// <summary>
    /// Gets the CompanyName assigned to the entry assembly.
    /// </summary>
    public static string Company { get; }

    /// <summary>
    /// Gets the LegalCopyright string assigned to the entry assembly.
    /// </summary>
    public static string Copyright { get; }

    /// <summary>
    /// Gets entry assembly FileName.
    /// </summary>
    public static string FileName { get; }

    /// <summary>
    /// Gets whether the entry assembly has debug JIT tracking enabled.
    /// </summary>
    public static bool IsDebug { get; }

    /// <summary>
    /// Gets whether the entry assembly has JIT optimization enabled.
    /// </summary>
    public static bool IsOptimized { get; }

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
