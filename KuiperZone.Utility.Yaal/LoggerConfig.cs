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
/// Implementation of <see cref="IReadOnlyLoggerConfig"/> with setters.
/// </summary>
public class LoggerConfig : IReadOnlyLoggerConfig
{
    private string _appName = "";
    private string _procId = "";
    private string _hostName = "";
    private string _debugId = "DGB@00000000";

    /// <summary>
    /// Gets the <see cref="LoggerConfig.AppName"/> max length in characters.
    /// </summary>
    public const int AppNameMaxLength = 48;

    /// <summary>
    /// Gets the <see cref="LoggerConfig.AppPid"/> max length in characters.
    /// </summary>
    public const int PidMaxLength = 128;

    /// <summary>
    /// Gets the <see cref="LoggerConfig.HostName"/> max length in characters.
    /// </summary>
    public const int HostNameMaxLength = 255;

    /// <summary>
    /// Gets the <see cref="LoggerConfig.DebugId"/> max length in characters.
    /// </summary>
    public const int DebugMaxLength = 32;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public LoggerConfig()
    {
        HostName = AppInfo.HostName;
        AppPid = AppInfo.Pid;

        // Truncate at last "." if too long
        AppName = EnsureId(AppInfo.AppName, AppNameMaxLength, true);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public LoggerConfig(LoggerConfig other)
    {
        HostName = other.HostName;
        AppName = other.AppName;
        AppPid = other.AppPid;
        IsTimeUtc = other.IsTimeUtc;
        Facility = other.Facility;
        MaxTextLength = other.MaxTextLength;
        DebugId = other.DebugId;
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.HostName"/> and provides a setter.
    /// </summary>
    public string HostName
    {
        get { return _hostName; }
        set { _hostName = EnsureId(value, HostNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.AppName"/> and provides a setter.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public string AppName
    {
        get { return _appName; }
        set { _appName = EnsureId(value, AppNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.AppPid"/> and provides a setter.
    /// </summary>
    public string AppPid
    {
        get { return _procId; }
        set { _procId = EnsureId(value, PidMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.IsTimeUtc"/> and provides a setter.
    /// </summary>
    public bool IsTimeUtc { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.Facility"/> and provides a setter.
    /// </summary>
	public FacilityId Facility { get; set; } = FacilityId.User;

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.MaxTextLength"/> and provides a setter.
    /// </summary>
    public int MaxTextLength { get; set; } = 2048;

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.DebugId"/> and provides a setter.
    /// </summary>
	public string DebugId
    {
        get { return _debugId; }
        set { _debugId = EnsureId(value, HostNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerConfig.Clone"/>.
    /// </summary>
    public LoggerConfig Clone()
    {
        return new LoggerConfig(this);
    }

    private static string EnsureId(string id, int maxLength, bool assembly = false)
    {
        id = id.Trim();

        if (string.IsNullOrEmpty(id))
        {
            return "";
        }

        if (id.Length > maxLength)
        {
            if (assembly)
            {
                int pos = id.LastIndexOf('.');

                if (pos > 0)
                {
                    id = id.Substring(pos + 1);
                }
            }

            if (id.Length > maxLength)
            {
                id = id.Substring(AppNameMaxLength);
            }
        }

        if (LogUtil.IsValidId(id, maxLength))
        {
            return id;
        }

        return "";
   }
}