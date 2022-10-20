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

using KuiperZone.Utility.Yaal.Internal;
using KuiperZone.Utility.Yaal.Sinks;
using Xunit;

namespace KuiperZone.Utility.Yaal.Test;

public class LoggerHelperTest
{
    [Fact]
    public void NewThreshold_SetsAndPreservesOthers()
    {
        var sinks = new ILogSink[] { new BufferLogSink(), new ConsoleLogSink() };
        var help = new LoggerHelper(SeverityLevel.Critical, sinks );
        var opts = help.Options;
        Assert.Equal(SeverityLevel.Critical, help.Threshold);

        help = help.NewThreshold(SeverityLevel.Emergency);
        Assert.Equal(SeverityLevel.Emergency, help.Threshold);

        // Unchanged
        Assert.Equal(sinks, help.Sinks);
        Assert.Equal(opts, help.Options);
        Assert.True(help.Allow("Kdjuf", SeverityLevel.Emergency));
    }

    [Fact]
    public void NewOptions_SetsAndPreservesOthers()
    {
        var sinks = new ILogSink[] { new BufferLogSink(), new ConsoleLogSink() };
        var help = new LoggerHelper(SeverityLevel.Critical, sinks );
        var opts = help.Options;

        opts = new LogOptions();
        help = help.NewOptions(opts);

        Assert.Equal(opts, help.Options);
        Assert.Equal(SeverityLevel.Critical, help.Threshold);
        Assert.Equal(sinks, help.Sinks);
        Assert.True(help.Allow("Kdjuf", SeverityLevel.Emergency));
    }

    [Fact]
    public void NewSinks_SetsAndPreservesOthers()
    {
        var sinks = new ILogSink[] { new BufferLogSink(), new ConsoleLogSink() };
        var help = new LoggerHelper(SeverityLevel.Critical, sinks );
        var opts = help.Options;

        sinks = new ILogSink[] { new ConsoleLogSink(), new BufferLogSink() };
        help = help.NewSinks(sinks);

        Assert.Equal(sinks, help.Sinks);
        Assert.Equal(SeverityLevel.Critical, help.Threshold);
        Assert.Equal(opts, help.Options);
        Assert.True(help.Allow("Kdjuf", SeverityLevel.Emergency));
    }

    [Fact]
    public void NewExclusions_SetsAndPreservesOthers()
    {
        var sinks = new ILogSink[] { new BufferLogSink(), new ConsoleLogSink() };
        var help = new LoggerHelper(SeverityLevel.Critical, sinks );
        var opts = help.Options;

        string excl = "msgid1, msgid2,msgid3";
        help = help.NewExclusions(excl);

        Assert.Equal(excl, help.Exclusions);
        Assert.False(help.Allow("msgid1", SeverityLevel.Emergency));
        Assert.False(help.Allow("msgid1", SeverityLevel.DebugL3));

        Assert.False(help.Allow("msgid2", SeverityLevel.Emergency));
        Assert.False(help.Allow("msgid3", SeverityLevel.Emergency));

        Assert.True(help.Allow("Kdjuf", SeverityLevel.Emergency));
        Assert.True(help.Allow("", SeverityLevel.Emergency));
        Assert.False(help.Allow("Kdjuf", SeverityLevel.DebugL3));

        Assert.Equal(sinks, help.Sinks);
        Assert.Equal(SeverityLevel.Critical, help.Threshold);
        Assert.Equal(opts, help.Options);
    }

}