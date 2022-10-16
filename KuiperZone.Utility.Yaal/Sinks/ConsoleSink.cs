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
/// Implements <see cref="ILogSink"/>. Messages are written directly to <see cref="Console"/>.
/// </summary>
public sealed class ConsoleSink : ILogSink
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public ConsoleSink(SeverityLevel threshold = SeverityLevel.DebugL3)
    {
        Threshold = threshold;
    }

    /// <summary>
    /// Gets the <see cref="SeverityLevel"/> value supplied on construction, providing
    /// additional filter on this sink type. For example, it may be desirable to have
    /// this sink write messages only with <see cref="SeverityLevel.Informational"/>
    /// severity or higher, regardless of the severity threshold of the host logger.
    /// </summary>
    public SeverityLevel Threshold { get; }

    /// <summary>
    /// Implements <see cref="ILogSink.Write"/>.
    /// </summary>
    public void Write(SeverityLevel severity, string message)
    {
        if (severity.IsHigherOrEqualThan(Threshold))
        {
            Console.WriteLine(message);
        }
    }

}