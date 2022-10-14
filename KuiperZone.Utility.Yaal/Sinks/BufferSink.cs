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
/// Implements <see cref="ILogSink"/> for a memory string buffer. Message written to
/// it will be held in an array. It is useful only in unit testing, where the caller
/// holds on to a reference to sink instance and can query whether certain code paths
/// have correctly executed based on their logging output. An instance is thread-safe.
/// </summary>
public sealed class BufferSink : ILogSink, IDisposable
{
    private readonly object _syncObj = new();
    private readonly List<string> _history = new();

    /// <summary>
    /// Constructor with optional <see cref="Capacity"/>.
    /// The <see cref="Threshold"/> value will be null (ignored).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">capacity</exception>
    public BufferSink(int capacity = 100)
        : this(SeverityLevel.DebugL3)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        Capacity = capacity;
    }

    /// <summary>
    ///Constructor with <see cref="Threshold"/> value and optional <see cref="Capacity"/>.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">capacity</exception>
    public BufferSink(SeverityLevel threshold, int capacity = 100)
    {
        Threshold = threshold;

        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        Capacity = capacity;
    }

    /// <summary>
    /// Implements <see cref="ILogSink.Threshold"/>.
    /// </summary>
    public SeverityLevel? Threshold { get; }

    /// <summary>
    /// Gets the capacity. When this limit is reached, older messages are lost.
    /// The default is 100.
    /// </summary>
    public int Capacity { get; }

    /// <summary>
    /// Gets up to count recent log messages. The result is a new instance on each call.
    /// </summary>
    public string[] GetRecent(int count = int.MaxValue)
    {
        lock (_syncObj)
        {
            count = Math.Clamp(count, 0, _history.Count);

            if (count > 0)
            {
                int from = _history.Count - count;
                var array = new string[count];

                _history.CopyTo(from, array, 0, count);
                return array;
            }

            return Array.Empty<string>();
        }

    }

    /// <summary>
    /// Returns true if the string is contained with recent log messages.
    /// </summary>
    public bool Contains(string? s, StringComparison comp = StringComparison.Ordinal)
    {
        if (!string.IsNullOrEmpty(s))
        {
            foreach (var item in GetRecent())
            {
                if (item.Contains(s, comp))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Implements <see cref="ILogSink.WriteMessage(string)"/>.
    /// </summary>
    public void WriteMessage(string message)
    {
        lock(_syncObj)
        {
            if (_history.Count == Capacity)
            {
                _history.RemoveAt(0);
            }

            _history.Add(message);
        }
    }

    /// <summary>
    /// Implements dispose. Does nothing.
    /// </summary>
    public void Dispose()
    {
    }

}