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
/// Identifies the logging sink kind.
/// </summary>
public enum SinkKind
{
	/// <summary>
    /// None.
    /// </summary>
	None = 0,

	/// <summary>
    /// Writes to console.
    /// </summary>
	Console,

	/// <summary>
    /// Writes to local syslog on LINUX, and Event Log on Windows.
    /// </summary>
	Syslog,

	/// <summary>
    /// Writes to file.
    /// </summary>
	File,
};
