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

using KuiperZone.Utility.Yaal.Internal;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Implementation of <see cref="IReadOnlyLogOptions"/> with setters.
/// </summary>
public class LogOptions : IReadOnlyLogOptions
{
    private string _appName = "";
    private string _procId = "";
    private string _hostName = "";
    private string _debugId = "DGB@00000000";

    /// <summary>
    /// Gets the <see cref="LogOptions.AppName"/> max length in characters.
    /// </summary>
    public const int AppNameMaxLength = 48;

    /// <summary>
    /// Gets the <see cref="LogOptions.AppPid"/> max length in characters.
    /// </summary>
    public const int PidMaxLength = 128;

    /// <summary>
    /// Gets the <see cref="LogOptions.HostName"/> max length in characters.
    /// </summary>
    public const int HostNameMaxLength = 255;

    /// <summary>
    /// Gets the <see cref="LogOptions.DebugId"/> max length in characters.
    /// </summary>
    public const int DebugMaxLength = 32;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public LogOptions()
    {
        HostName = AppInfo.HostName;
        AppPid = AppInfo.Pid;

        // Truncate at last "." if too long
        AppName = LogUtil.EnsureId(AppInfo.AppName, AppNameMaxLength);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public LogOptions(LogOptions other)
    {
        HostName = other.HostName;
        AppName = other.AppName;
        AppPid = other.AppPid;
        IsTimeUtc = other.IsTimeUtc;
        Facility = other.Facility;
        DebugId = other.DebugId;
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.HostName"/> and provides a setter.
    /// </summary>
    public string HostName
    {
        get { return _hostName; }
        set { _hostName = LogUtil.EnsureId(value, HostNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.AppName"/> and provides a setter.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public string AppName
    {
        get { return _appName; }
        set { _appName = LogUtil.EnsureId(value, AppNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.AppPid"/> and provides a setter.
    /// </summary>
    public string AppPid
    {
        get { return _procId; }
        set { _procId = LogUtil.EnsureId(value, PidMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.IsTimeUtc"/> and provides a setter.
    /// </summary>
    public bool IsTimeUtc { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.Facility"/> and provides a setter.
    /// </summary>
	public FacilityId Facility { get; set; } = FacilityId.User;

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.DebugId"/> and provides a setter.
    /// </summary>
	public string DebugId
    {
        get { return _debugId; }
        set { _debugId = LogUtil.EnsureId(value, HostNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLogOptions.Clone"/>.
    /// </summary>
    public LogOptions Clone()
    {
        return new LogOptions(this);
    }

}