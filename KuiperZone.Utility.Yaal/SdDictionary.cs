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

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using KuiperZone.Utility.Yaal.Internal;

namespace KuiperZone.Utility.Yaal;

/// <summary>
/// A dictionary implementation where the key is a string, but the value is generic. This serves as base class
/// for structure data. Keys must meet RFC 5424 SD-NAME requirements, i.e. all characters are to be within the
/// printable ASCII range, and must not contain: '=', SP, ']', '"'. Keys are ordinal sorted and case sensitive.
/// </summary>
public class SdDictionary<T> : IDictionary<string, T>,
    IReadOnlyDictionary<string, T>,
    ICollection<KeyValuePair<string, T>>,
    IEnumerable<KeyValuePair<string, T>>,
    IEnumerable
{
    private readonly SortedDictionary<string, T> _dictionary = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the maximum allowed length of SD-NAME.
    /// </summary>
    public const int NameMaxLength = 32;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public SdDictionary()
	{
    }

    /// <summary>
    /// Constructor with initial key=value.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 ID</exception>
    public SdDictionary(string key, T value)
	{
        Add(key, value);
    }

    /// <summary>
    /// Implements <see cref="IDictionary{K,V}"/> indexer.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 ID</exception>
    public T this[string key]
    {
        get { return _dictionary[key]; }

        set
        {
            LogUtil.AssertId(key, NameMaxLength);
            _dictionary[key] = value;
        }
    }

    /// <summary>
    /// Implements <see cref="ICollection{T}.Count"/>.
    /// </summary>
    public int Count
    {
        get { return _dictionary.Count; }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyDictionary{K,V}.Keys"/>.
    /// </summary>
    public IEnumerable<string> Keys
    {
        get { return _dictionary.Keys; }
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyDictionary{K,V}.Values"/>.
    /// </summary>
    public IEnumerable<T> Values
    {
        get { return _dictionary.Values; }
    }

    /// <summary>
    /// Implements <see cref="IDictionary{K,V}.Keys"/>.
    /// </summary>
    ICollection<string> IDictionary<string, T>.Keys
    {
        get { return _dictionary.Keys; }
    }

    /// <summary>
    /// Implements <see cref="IDictionary{K,V}.Values"/>.
    /// </summary>
    ICollection<T> IDictionary<string, T>.Values
    {
        get { return _dictionary.Values; }
    }

    /// <summary>
    /// Implements <see cref="ICollection{T}.IsReadOnly"/>.
    /// </summary>
    public bool IsReadOnly { get; } = false;

    /// <summary>
    /// Gets whether empty.
    /// </summary>
    public bool IsEmpty
    {
        get { return _dictionary.Count == 0; }
    }

    /// <summary>
    /// Implements <see cref="IDictionary{K,V}.Add"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 ID</exception>
    public void Add(string key, T value)
    {
        LogUtil.AssertId(key, NameMaxLength);
        _dictionary.Add(key, value);
    }

    /// <summary>
    /// Implements <see cref="ICollection{T}.Add(T)"/>.
    /// </summary>
    /// <exception cref="ArgumentException">Invalid RFC 5424 ID</exception>
    public void Add(KeyValuePair<string, T> item)
    {
        LogUtil.AssertId(item.Key, NameMaxLength);
        ((IDictionary<string, T>)_dictionary).Add(item);
    }

    /// <summary>
    /// Implements <see cref="ICollection{T}.Clear"/>.
    /// </summary>
    public void Clear()
    {
        _dictionary.Clear();
    }

    /// <summary>
    /// Implements <see cref="ICollection{T}.Contains(T)"/>.
    /// </summary>
    public bool Contains(KeyValuePair<string, T> item)
    {
        return ((IDictionary<string, T>)_dictionary).Contains(item);
    }

    /// <summary>
    /// Implements <see cref="IDictionary{K,V}.ContainsKey(K)"/>.
    /// </summary>
    public bool ContainsKey(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// Implements <see cref="ICollection{T}.CopyTo(T[], int)"/>.
    /// </summary>
    public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
    {
        ((IDictionary<string, T>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Implements <see cref="IDictionary{K,V}.Remove(K)"/>.
    /// </summary>
    public bool Remove(string key)
    {
        return _dictionary.Remove(key);
    }

    /// <summary>
    /// Implements <see cref="ICollection{T}.Remove(T)"/>.
    /// </summary>
    public bool Remove(KeyValuePair<string, T> item)
    {
        return ((IDictionary<string, T>)_dictionary).Remove(item);
    }

    /// <summary>
    /// Implements <see cref="IDictionary{K,V}.TryGetValue(K, out V)"/>.
    /// </summary>
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <summary>
    /// Implements <see cref="IEnumerable{T}.GetEnumerator"/>.
    /// </summary>
    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    /// <summary>
    /// Implements <see cref="IEnumerable.GetEnumerator"/>.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    /// <summary>
    /// Efficiently appends RFC 5424 formatted data to the StringBuilder.
    /// The resulting string data may be escaped according RFC 5424.
    /// </summary>
    public virtual void AppendTo(StringBuilder buffer, IReadOnlyLoggerOptions options)
    {
        foreach (var item in _dictionary)
        {
            if (buffer.Length != 0)
            {
                buffer.Append(' ');
            }

            buffer.Append(item.Key);
            buffer.Append('=');
            buffer.Append('"');
            buffer.Append(LogUtil.Escape(item.Value?.ToString(), "\\]\""));
            buffer.Append('"');
        }
    }

    /// <summary>
    /// Overrides. Calls <see cref="AppendTo"/>.
    /// </summary>
    public override string ToString()
    {
        var buffer = new StringBuilder(1024);
        AppendTo(buffer, new LoggerOptions());
        return buffer.ToString();
    }
}

