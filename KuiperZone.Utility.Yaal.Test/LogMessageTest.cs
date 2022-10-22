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

using System;
using KuiperZone.Utility.Yaal.Internal;
using Xunit;

namespace KuiperZone.Utility.Yaal.Test;

public class LogMessageTest
{
    private static readonly DateTime TestTime = new(1988, 12, 5, 12, 54, 23);

    [Fact]
    public void ToString5424_GivesExpected()
    {
        var msg = CreateMessage();
        var opts = new LogOptions();

        var msgStr = msg.ToString(LogFormat.Rfc5424, opts);

        string Time = "1988-12-05T12:54:23.000000+00:00";
        string Thread = $"{opts.AppPid}-{LogUtil.ThreadName}";
        string Expect = $"<10>1 {Time} {opts.HostName} {opts.AppName} {opts.AppPid} MsgId7653 [e1 key1=\"value\\n1\"]" +
            $"[e2 key2=\"value\\n2\"][DGB@00000000 FUNC=\"Function\" LINE=\"668\" SEVERITY=\"crit\" THREAD=\"{Thread}\"] Text84763";

        Assert.Equal(Expect, msgStr);
    }

    [Fact]
    public void ToStringBsd_GivesExpected()
    {
        var msg = CreateMessage();
        var opts = new LogOptions();
        opts.Priority = PriorityKind.Keyword;

        var msgStr = msg.ToString(LogFormat.Bsd, opts);

        string time = "Dec 05 12:54:23";
        string expect = $"<crit>{time} {opts.HostName} {opts.AppName}[{opts.AppPid}]: Text84763 @ Function #668";
        Assert.Equal(expect, msgStr);
    }

    [Fact]
    public void ToStringGeneralNoPad_GivesExpected()
    {
        var msg = CreateMessage();
        var opts = new LogOptions();

        var msgStr = msg.ToString(LogFormat.General, opts);

        string time = "Dec 05 12:54:23.000000";
        string expect = $"Function #668 : [MsgId7653] Text84763 @ {time}";
        Assert.Equal(expect, msgStr);
    }

    [Fact]
    public void ToStringGeneralPad80_GivesExpected()
    {
        const int Pad = 80;
        var msg = CreateMessage();
        var opts = new LogOptions();

        var msgStr = msg.ToString(LogFormat.General, opts, 2000, Pad);

        string time = "Dec 05 12:54:23.000000";
        string expect = $"Function #668 :";
        string pad = new string(' ', Pad - expect.Length);
        expect += $"{pad}[MsgId7653] Text84763 @ {time}";
        Assert.Equal(expect, msgStr);
    }

    [Fact]
    public void ToStringTextOnly_GivesExpected()
    {
        var msg = CreateMessage();

        var msgStr = msg.ToString(LogFormat.TextOnly);
        Assert.Equal("Text84763", msgStr);
    }

    private static LogMessage CreateMessage()
    {
        var msg = new LogMessage();
        Assert.Equal(SeverityLevel.Info, msg.Severity);

        msg.Severity = SeverityLevel.Critical;
        msg.MsgId = "MsgId7653";
        msg.Text = "Text84763";
        msg.Data.Add("e1", new SdElement("key1", "value\n1"));
        msg.Data.Add("e2", new SdElement("key2", "value\n2"));

        msg.Time = TestTime;
        msg.Debug = new DebugInfo("Function", 668);
        return msg;
    }

}