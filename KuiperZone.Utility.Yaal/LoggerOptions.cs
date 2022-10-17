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
/// Implementation of <see cref="IReadOnlyLoggerOptions"/> with setters.
/// </summary>
public class LoggerOptions : IReadOnlyLoggerOptions
{
    private string _appName = "";
    private string _procId = "";
    private string _hostName = "";
    private string _debugId = "DGB@00000000";

    /// <summary>
    /// Gets the <see cref="LoggerOptions.AppName"/> max length in characters.
    /// </summary>
    public const int AppNameMaxLength = 48;

    /// <summary>
    /// Gets the <see cref="LoggerOptions.AppPid"/> max length in characters.
    /// </summary>
    public const int PidMaxLength = 128;

    /// <summary>
    /// Gets the <see cref="LoggerOptions.HostName"/> max length in characters.
    /// </summary>
    public const int HostNameMaxLength = 255;

    /// <summary>
    /// Gets the <see cref="LoggerOptions.DebugId"/> max length in characters.
    /// </summary>
    public const int DebugMaxLength = 32;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public LoggerOptions()
    {
        HostName = AppInfo.HostName;
        AppPid = AppInfo.Pid;

        // Truncate at last "." if too long
        AppName = EnsureId(AppInfo.AppName, AppNameMaxLength, true);
    }

    /// <summary>
    /// Copy constructor.
    /// </summary>
    public LoggerOptions(LoggerOptions other)
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
    /// Implements <see cref="IReadOnlyLoggerOptions.HostName"/> and provides a setter.
    /// </summary>
    public string HostName
    {
        get { return _hostName; }
        set { _hostName = EnsureId(value, HostNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerOptions.AppName"/> and provides a setter.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 name value</exception>
    public string AppName
    {
        get { return _appName; }
        set { _appName = EnsureId(value, AppNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerOptions.AppPid"/> and provides a setter.
    /// </summary>
    public string AppPid
    {
        get { return _procId; }
        set { _procId = EnsureId(value, PidMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerOptions.IsTimeUtc"/> and provides a setter.
    /// </summary>
    public bool IsTimeUtc { get; set; }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerOptions.Facility"/> and provides a setter.
    /// </summary>
	public FacilityId Facility { get; set; } = FacilityId.User;

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerOptions.MaxTextLength"/> and provides a setter.
    /// </summary>
    public int MaxTextLength { get; set; } = 2048;

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerOptions.DebugId"/> and provides a setter.
    /// </summary>
	public string DebugId
    {
        get { return _debugId; }
        set { _debugId = EnsureId(value, HostNameMaxLength); }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyLoggerOptions.Clone"/>.
    /// </summary>
    public LoggerOptions Clone()
    {
        return new LoggerOptions(this);
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