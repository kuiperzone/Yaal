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
/// Interface for a loging sink. Implementations should be thread instance safe. Note. Implementations
/// are not expected to implement <see cref="IDisposable"/>, as creation is to be infrequement and
/// instances long-lived. We leave it to finalizers to clean up, if ever necessary.
/// </summary>
public interface ILogSink
{
    /// <summary>
    /// Writes the message. The method should do nothing unless the <see cref="LogMessage.Severity"/>
    /// is equal or higher in priority than <see cref="SinkOptions.Threshold"/>.
    /// </summary>
    void Write(LogMessage msg, IReadOnlyLogOptions opts);
}