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
/// Logging output format.
/// </summary>
public enum LogFormat
{
    /// <summary>
    /// A format for general human readability. The output leads with the message text
    /// or debug stack trace information.
    /// </summary>
    General = 0,

	/// <summary>
    /// RFC 5424 syslog format.
    /// </summary>
	Rfc5424,

	/// <summary>
    /// Legacy BSD syslog format (RFC 3164). Any debug stack trace information
    /// is appended after the message.
    /// </summary>
	Bsd,

	/// <summary>
    /// Only the message text is written. All other information,
    /// including date/time, stack trace etc. is discarded.
    /// </summary>
	TextOnly,
};
