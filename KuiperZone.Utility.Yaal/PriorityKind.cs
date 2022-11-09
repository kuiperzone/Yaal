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
/// Defines a priority field inclusion behavior for <see cref="LogFormat.Rfc5424"/> and <see cref="LogFormat.Bsd"/>
/// (RFC 3164) formats. Both these RFCs specify a leading numeric priority code (a combination of severity and
/// facility) at the start of messages. However, it is comman practice to either omit this code or provide a
/// <see cref="SeverityLevel"/> keyword instead.
/// </summary>
public enum PriorityKind
{
    /// <summary>
    /// According RFC. I.e, "&lt;165&gt;...".
    /// </summary>
    Default = 0,

    /// <summary>
    /// Severity keyword instead of numeric code. I.e.  "&lt;notice&gt;...".
    /// </summary>
    Keyword,

    /// <summary>
    /// Omits the priority leader.
    /// </summary>
    Omit,

}