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
/// Log message severity codes. Note that numeric values align with RFC 5424 sys-log.
/// </summary>
public enum SeverityLevel
{
	/// <summary>
    /// Emergency: system is unusable. Highest severity.
    /// </summary>
	Emergency = 0,

	/// <summary>
    /// Alert: action must be taken immediately.
    /// </summary>
	Alert = 1,

	/// <summary>
    /// Critical: critical conditions.
    /// </summary>
	Critical = 2,

	/// <summary>
    /// Error: error conditions.
    /// </summary>
	Error = 3,

	/// <summary>
    /// Warning: warning conditions.
    /// </summary>
	Warning = 4,

	/// <summary>
    /// Notice: normal but significant condition.
    /// </summary>
	Notice = 5,

	/// <summary>
    /// Informational: informational messages.
    /// </summary>
	Informational = 6,

	/// <summary>
    /// Debug: debug-level messages. Lowest severity.
    /// </summary>
	Debug = 7,

	/// <summary>
	/// Additional debug level of lower priority than DEBUG. For RFC 5424 and BSD formats, it is
	/// equivalent to DEBUG but provides for granularity in filtering out low level debug statements.
    /// </summary>
	DebugL1,

	/// <summary>
	/// Additional debug level of lower priority than DebugL1. For RFC 5424 and BSD formats, it is
	/// equivalent to DEBUG but provides for granularity in filtering out low level debug statements.
    /// </summary>
	DebugL2,

	/// <summary>
	/// Additional debug level of lower priority than DebugL2. For RFC 5424 and BSD formats, it is
	/// equivalent to DEBUG but provides for granularity in filtering out low level debug statements.
    /// </summary>
	DebugL3,

	/// <summary>
    /// A special value used to disable logging.
    /// </summary>
	Disabled, // Must be last
};

