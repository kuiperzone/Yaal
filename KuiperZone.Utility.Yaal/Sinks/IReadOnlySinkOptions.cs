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

namespace KuiperZone.Utility.Yaal.Sinks;

/// <summary>
/// Interface for readonly logging options.
/// </summary>
public interface IReadOnlySinkOptions
{
    /// <summary>
    /// Gets the output format. The default is to depend on the sink kind.
    /// </summary>
    FormatKind Format { get; }

    /// <summary>
    /// Gets the threshold severity for the sink. Setting this value will prevent the sink form
    /// logging any message with a lower priority, irrespective of the threshold value of the
    /// host logger. Although this allows control on a per sink basis, this threshold cannot be
    /// changed in-flight. Typically, therefore, the default should be <see cref="SeverityLevel.Lowest"/>.
    /// </summary>
    SeverityLevel Threshold { get; }
}