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
        ProcId = GetProcId();

        try
        {
            var ea = Assembly.GetEntryAssembly();

            if (ea != null)
            {
                var eaName = ea.GetName();
                AssemblyName = eaName.Name ?? "";
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
                ProductName = info.ProductName ?? AssemblyName;
                Company = info.CompanyName ?? "";
                Copyright = info.LegalCopyright ?? "";
                FileName = info.FileName;
            }
        }
        catch
        {
        }

        AssemblyName ??= "";
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
    public static string ProcId { get; }

    /// <summary>
    /// Gets the entry assembly name.
    /// </summary>
    public static string AssemblyName { get; }

    /// <summary>
    /// Gets the entry assembly version.
    /// </summary>
    public static Version Version { get; }

    /// <summary>
    /// Gets the application ProductName, falling back to the entry assembly name if it is not
    /// defined.
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

    /// <summary>
    /// Gets an ID number for the calling thread.
    /// </summary>
    public static string ThreadId
    {
        get { return Thread.CurrentThread.ManagedThreadId.ToString(); }
    }

    /// <summary>
    /// Get a name for the calling thread. If it has no name, one is formed from the thread-id.
    /// </summary>
    public static string ThreadName
    {
        get
        {
            const int MaxThreadLength = 60;

            var name = Thread.CurrentThread.Name;

            if (string.IsNullOrEmpty(name))
            {
                // Use integer ID instead
                name = "Thread" + ThreadId;
            }
            else
            if (name.Length > MaxThreadLength)
            {
                // Not too long
                name = name.Substring(0, MaxThreadLength).Trim();
            }

            return name;
        }
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
