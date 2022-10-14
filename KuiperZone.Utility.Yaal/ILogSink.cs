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
/// Interface for a loging sink. Implmentations should be thread instance safe and, once
/// constructed, have read-only properties. The interface inherits IDisposable so as to
/// facilitate file or IO closure where needed.
/// </summary>
public interface ILogSink : IDisposable
{
    /// <summary>
    /// Gets a <see cref="SeverityLevel"/> value. The value should be readonly and set on
    /// construction. Where the value is not null, only messages with a priority equal or
    /// higher will be written. Where the value is null, all messages are written. This allows
    /// messages to be filtered on a per sink basis. For example, a Console sink may only write
    /// message with <see cref="SeverityLevel.Informational"/> severity or higher, regardless
    /// of the severity threshold of its host logger. The initial default value should
    /// typically null.
    /// </summary>
    public SeverityLevel? Threshold { get; }

    /// <summary>
    /// Writes the message string.
    /// </summary>
    void WriteMessage(string message);
}