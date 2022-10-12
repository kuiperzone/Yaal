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
using System.Runtime.InteropServices;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Interface for readonly logging options.
/// </summary>
public class LogOptions : IReadOnlyLogOptions
{
    private string _appName;
    private string _appPid;
    private string _localHost;
    private string _debugId = "DGB@00000000";

    public LogOptions()
    {
        _appName = ValueOrEmpty(Assembly.GetEntryAssembly()?.GetName().Name);
        _appPid = ValueOrEmpty(Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));
        _localHost = ValueOrEmpty(Dns.GetHostName());

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Format = FormatKind.Text;
        }
        else
        {
            Format = FormatKind.Rfc5424;
        }
    }

    public LogOptions(FormatKind format)
        : this()
    {
        Format = format;
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.AppName"/> and provides a setter.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public string AppName
    {
        get { return _appName; }
        set { _appName = AssertValue(value); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.AppPid"/> and provides a setter.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public string AppPid
    {
        get { return _appPid; }
        set { _appPid = AssertValue(value); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.LocalHost"/> and provides a setter.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public string LocalHost
    {
        get { return _localHost; }
        set { _localHost = AssertValue(value); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.Format"/> and provides a setter.
    /// </summary>
    public FormatKind Format { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.IsTimeUtc"/> and provides a setter.
    /// </summary>
    public bool IsTimeUtc { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.Facility"/> and provides a setter.
    /// </summary>
	public FacilityId Facility { get; set; } = FacilityId.User;

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.MaxTextLength"/> and provides a setter.
    /// </summary>
    public int MaxTextLength { get; set; } = 2048;

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.DebugId"/> and provides a setter.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
	public string DebugId
    {
        get { return _debugId; }
        set { _debugId = AssertValue(value); }
    }

    private static string ValueOrEmpty(string? s)
    {
        return StructuredData.IsValidKey(s) ? s : "";
    }

    private static string AssertValue(string s)
    {
        s = s.Trim();

        // Allow empty
        if (s.Length > 0)
        {
            StructuredData.AssertKey(s);
        }

        return s;
    }

}