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

using System.Diagnostics;
using System.Runtime.InteropServices;
using Xunit;
using Xunit.Abstractions;

namespace KuiperZone.Utility.Yaal.Sinks.Test;

public class LocalSyslogSinkTest
{
    private readonly ITestOutputHelper Helper;

    public LocalSyslogSinkTest(ITestOutputHelper helper)
    {
        Helper = helper;
    }

    [Fact]
    public void IsSupported_ResultAccordingToPlatform()
    {
        Assert.Equal(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), !SyslogSink.IsSupported);
    }

    [Fact]
    public void WriteMessage_WritesOK()
    {
        Debug.WriteLine("Hello xyz");
        Trace.WriteLine("Hello xyz2", "category");

        var ev = new EventLogSink();
        var m = new LogMessage("\"Hello\", he said");
        ev.Write(m, new LoggerOptions());

        if (SyslogSink.IsSupported)
        {
            var sink = new SyslogSink();

            m.Debug = new();
            sink.Write(m, new LoggerOptions());

//            var pid = Process.GetCurrentProcess().Id.ToString();
  //          sink.Write(SeverityLevel.Informational, $"--rfc5424 --id={pid} \"Hello World\"");
            Assert.False(sink.IsFailed);
        }
    }

}