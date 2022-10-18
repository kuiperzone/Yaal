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

using KuiperZone.Utility.Yaal.Sinks;

namespace KuiperZone.Utility.Yaal.Internal;

/// <summary>
/// Helps facilite alock-free design in Logger class. It holds everything that would otherwise
/// be volatile in one class which, therefore, can be accessed via one volatile reference.
/// </summary>
internal class LoggerHelper
{
    private readonly IReadOnlyCollection<string> _exclusions = Array.Empty<string>();
    private volatile Exception? v_error;

    public LoggerHelper(SeverityLevel threshold, IEnumerable<ILogSink>? sinks = null)
    {
        Options = new LoggerOptions();
        Threshold = threshold;

        var temp = new List<ILogSink>();

        if (sinks == null)
        {
            try
            {
                temp.Add(new SyslogLogSink());
            }
            catch (Exception e)
            {
                v_error = e;
            }
        }
        else
        {
            temp.AddRange(sinks);
        }

        Sinks = temp;
    }

    private LoggerHelper(IReadOnlyLoggerOptions opts, SeverityLevel severity,
        string excludes, IReadOnlyCollection<ILogSink> sinks, Exception? lastError)
    {
        Options = opts;
        Threshold = severity;
        Exclusions = excludes;
        Sinks = sinks;

        v_error = lastError;
        _exclusions = CreateIds(excludes);
    }

    public readonly IReadOnlyLoggerOptions Options;

    public readonly SeverityLevel Threshold;

    public readonly string Exclusions = "";

    public readonly IReadOnlyCollection<ILogSink> Sinks;

    public Exception? Error
    {
        get { return v_error; }
    }

    public bool Allow(LogMessage msg)
    {
        return Allow(msg.MsgId, msg.Severity);
    }

    public bool Allow(SeverityLevel severity)
    {
        return severity.IsHigherOrEqualPriority(Threshold);
    }

    public bool Allow(string? msgId, SeverityLevel severity)
    {
        if (severity.IsHigherOrEqualPriority(Threshold))
        {
            if (string.IsNullOrEmpty(msgId) || _exclusions.Count == 0)
            {
                return true;
            }

            return !_exclusions.Contains(msgId.ToLowerInvariant());
        }

        return false;
    }

    public void ClearError()
    {
        v_error = null;
    }

    public LoggerHelper NewOptions(IReadOnlyLoggerOptions opts)
    {
        return new LoggerHelper(opts, Threshold, Exclusions, Sinks, Error);
    }

    public LoggerHelper NewThreshold(SeverityLevel severity)
    {
        return new LoggerHelper(Options, severity, Exclusions, Sinks, Error);
    }

    public LoggerHelper NewExclusions(string excludes)
    {
        return new LoggerHelper(Options, Threshold, excludes, Sinks, Error);
    }

    public LoggerHelper NewSinks(IReadOnlyCollection<ILogSink> sinks)
    {
        return new LoggerHelper(Options, Threshold, Exclusions, sinks, Error);
    }

    public void Write(LogMessage msg)
    {
        foreach (var item in Sinks)
        {
                try
                {
                    item.Write(msg, Options);
                }
                catch (Exception e)
                {
                    v_error ??= e;
                }
        }
    }

    private static IReadOnlyCollection<string> CreateIds(string s)
    {
        s = s.ToLowerInvariant();
        var ids = s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (ids.Length < 10)
        {
            return ids;
        }

        var hash = new HashSet<string>(ids.Length);

        foreach (var item in ids)
        {
            hash.Add(item);
        }

        return hash;
    }

}