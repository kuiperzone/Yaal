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

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// Logging facility code which specifying what type of program is logging the message. It used for RFC 5424 only.
/// </summary>
public enum FacilityId
{
    /// <summary>
    /// Miscellaneous user process. This is default for applications.
    /// </summary>
    User = 1 << 3,

    /// <summary>
    /// Mail system.
    /// </summary>
    Mail = 2 << 3,

    /// <summary>
    /// System daemons.
    /// </summary>
    Daemon = 3 << 3,

    /// <summary>
    /// Security/authorization messages.
    /// </summary>
    Auth = 4 << 3,

    /// <summary>
    /// Messages generated internally by syslogd.
    /// </summary>
    Syslog = 5 << 3,

	/// <summary>
    /// Line printer subsystem.
    /// </summary>
    Lpr = 6 << 3,

    /// <summary>
    /// Network news subsystem.
    /// </summary>
    News = 7 << 3,

	/// <summary>
    /// UUCP subsystem.
    /// </summary>
    Uucp = 8 << 3,

	/// <summary>
    /// Clock daemon.
    /// </summary>
    Cron = 9 << 3,

	/// <summary>
    /// Security/authorization messages private.
    /// </summary>
    AuthPriv = 10 << 3,

	/// <summary>
    /// Ftp daemon.
    /// </summary>
    Ftp = 11 << 3,

	/// <summary>
    /// Ntp subsystem.
    /// </summary>
    Ntp = 12 << 3,

	/// <summary>
    /// Log audit.
    /// </summary>
    LogAudit = 13 << 3,

	/// <summary>
    /// Log alert
    /// </summary>
    LogAlert = 14 << 3,

	/// <summary>
    /// Clock daemon
    /// </summary>
    Clock = 15 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local0 = 16 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local1 = 17 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local2 = 18 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local3 = 19 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local4 = 20 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local5 = 21 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local6 = 22 << 3,

	/// <summary>
    /// Reserved for local use.
    /// </summary>
    Local7 = 23 << 3,
};
