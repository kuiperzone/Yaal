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
public enum FormatKind
{
	/// <summary>
    /// RFC 5424 syslog format.
    /// </summary>
	Rfc5424 = 0,

	/// <summary>
    /// Legacy BSD syslog format (RFC 3164).
    /// </summary>
	Bsd,

	/// <summary>
    /// Only the message text is written.
    /// </summary>
	Text,
};
