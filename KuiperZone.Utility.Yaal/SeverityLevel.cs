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
/// Also note that higher numerical value have lower priority.
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
	/// Additional debug level of lower priority than <see cref="Debug"/>. For RFC 5424 and BSD formats, it is
    /// equivalent to <see cref="Debug"/> but provides for granularity in filtering out low level debug statements.
    /// </summary>
	DebugL1,

	/// <summary>
	/// Additional debug level of lower priority than <see cref="DebugL1"/>. For RFC 5424 and BSD formats, it is
    /// equivalent to <see cref="Debug"/> but provides for granularity in filtering out low level debug statements.
    /// </summary>
	DebugL2,

	/// <summary>
	/// Additional debug level of lower priority than <see cref="DebugL2"/>. For RFC 5424 and BSD formats, it is
    /// equivalent to <see cref="Debug"/> but provides for granularity in filtering out low level debug statements.
    /// </summary>
	DebugL3,

	/// <summary>
    /// A special value used to disable logging.
    /// </summary>
	Disabled, // Must be last
};

/// <summary>
/// Extension methods.
/// </summary>
public static class SeverityLevelExtension
{
	/// <summary>
    /// Returns true if a has a higher or equal priority than b.
    /// </summary>
    public static bool IsGreaterOrEqualPriorityThan(this SeverityLevel a, SeverityLevel b)
    {
        return b != SeverityLevel.Disabled && b >= a;
    }

	/// <summary>
    /// Returns an RFC 5424 (and BSD) priority value given a <see cref="FacilityId"/> value.
    /// The result is an integer for the severity clamped within the legal RFC 5424 range and
    /// OR-ed with facility.
    /// </summary>
    public static int ToPriority(this SeverityLevel severity, FacilityId facility)
    {
        const int Min = (int)SeverityLevel.Emergency;
		const int Max = (int)SeverityLevel.Debug;
        return Math.Clamp((int)severity, Min, Max) | (int)facility;
    }
}

