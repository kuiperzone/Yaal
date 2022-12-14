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
/// Interface for readonly logging options.
/// </summary>
public interface IReadOnlyLogOptions
{
    /// <summary>
    /// Gets the local host name. It is set on construction to a default value. If not empty,
    /// all characters are to be within the printable ASCII range, and must not contain:
    /// '=', SP, ']', '"'. Assigning an invalid string will be same as assigning an empty string.
    /// The value may also be truncated if too long.
    /// </summary>
    string HostName { get; }

    /// <summary>
    /// Gets the application name. It is set on construction to a default value.
    /// If not empty, all characters are to be within the printable ASCII range, and must not
    /// contain: '=', SP, ']', '"'. The value may be truncated if too long.
    /// </summary>
    string AppName { get; }

    /// <summary>
    /// Gets the application process ID. For syslog, this need only be a string unique to the
    /// application execution instance. It is set on construction to a default value.
    /// If not empty, all characters are to be within the printable ASCII range, and must not
    /// contain: '=', SP, ']', '"'. The value may be truncated if too long.
    /// </summary>
    string AppPid { get; }

    /// <summary>
    /// Gets whether times are written as UTC. Default is false (local).
    /// </summary>
    bool IsTimeUtc { get; }

    /// <summary>
    /// Gets the RFC 5424 syslog facility value. The default is <see cref="FacilityId.User"/> and
    /// invariably should not be modified. It is ignored for other logging formats.
    /// </summary>
	FacilityId Facility { get; }

    /// <summary>
    /// Gets whether the priority inclusion behavior for <see cref="LogFormat.Rfc5424"/> and
    /// <see cref="LogFormat.Bsd"/> formats.
    /// </summary>
    PriorityKind Priority { get; }

    /// <summary>
    /// Gets the RFC 5424 SD-ID to be used for debug related <see cref="LogFormat.Rfc5424"/> structured data.
    /// The format is "name@enterprise_number", unless using an IANA ID. Example: "DGB@24601". The value
    /// must not equal any that may be used in other structure elements otherwise it will be ignored.
    /// It will also be ignored if it is empty or the ID is invalid. The default is "DGB@00000000", but
    /// should typically be updated. If not empty, all characters are to be within the printable ASCII
    /// range, and must not contain: '=', SP, ']', '"'.
    /// </summary>
	string DebugId { get; }

    /// <summary>
    /// Clones the instance returning a mutable instance.
    /// </summary>
    LogOptions Clone();
}