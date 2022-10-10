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
    /// Gets the application name. It will be set on construction to a default value, but may be set explicitly.
    /// </summary>
    string AppName { get; }

    /// <summary>
    /// Gets the local host name. The value is populated by default, but may be set explicitly.
    /// </summary>
    string LocalHost { get; }

    /// <summary>
    /// Gets the output format. The default is <see cref="FormatKind.Text"/> on Windows, otherwise
    /// <see cref="FormatKind.Rfc5424"/>.
    /// </summary>
    FormatKind Format { get; }

    /// <summary>
    /// Gets the RFC 5424 syslog facility value. The default is <see cref="FacilityId.User"/> and
    /// should not normally be changed. It is ignored for other logging kinds.
    /// </summary>
	FacilityId Facility { get; }

    /// <summary>
    /// Gets the maximum message text length in chars, excluding structured and other data.
    /// If the text exceeds this, it will be truncated. A value of 0 or less implies no limit.
    /// </summary>
    int MaxTextLength { get; }

    /// <summary>
    /// Gets the RFC 5424 SD-ID to be used for debug <see cref="FormatKind.Rfc5424"/> structured data.
    /// The format is "name@enterprise_number", unless using an IANA ID. Example: "DGB@24601". The value
    /// must not equal that used in elements of messages themselves, otherwise it will be ignored. It will
    /// also be ignored if it is empty or the ID is invalid. The default is "DGB@00000000", but should
    /// typically be updated.
    /// </summary>
	string DebugSdId { get; }
}