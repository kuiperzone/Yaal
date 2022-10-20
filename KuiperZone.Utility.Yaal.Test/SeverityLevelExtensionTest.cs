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
using Xunit;

namespace KuiperZone.Utility.Yaal.Test;

public class SeverityLevelExtensionTest
{
    [Fact]
    public void ToColor_CorrectMappings()
    {
        Assert.Equal(ConsoleColor.DarkRed, SeverityLevel.Emergency.ToColor());
        Assert.Equal(ConsoleColor.DarkRed, SeverityLevel.Alert.ToColor());
        Assert.Equal(ConsoleColor.DarkRed, SeverityLevel.Critical.ToColor());
        Assert.Equal(ConsoleColor.Red, SeverityLevel.Error.ToColor());
        Assert.Equal(ConsoleColor.DarkGreen, SeverityLevel.Notice.ToColor());
        Assert.Equal(ConsoleColor.DarkCyan, SeverityLevel.Info.ToColor());
        Assert.Equal(ConsoleColor.DarkGray, SeverityLevel.Debug.ToColor());

        Assert.Equal(ConsoleColor.DarkGray, SeverityLevel.DebugL1.ToColor());
        Assert.Equal(ConsoleColor.DarkGray, SeverityLevel.DebugL2.ToColor());
        Assert.Equal(ConsoleColor.DarkGray, SeverityLevel.DebugL3.ToColor());
        Assert.Equal(ConsoleColor.DarkGray, SeverityLevel.Lowest.ToColor());
        Assert.Equal(ConsoleColor.DarkGray, SeverityLevel.Disabled.ToColor());
    }

    [Fact]
    public void ToKeyword_CorrectMappings()
    {
        Assert.Equal("emerg", SeverityLevel.Emergency.ToKeyword());
        Assert.Equal("alert", SeverityLevel.Alert.ToKeyword());
        Assert.Equal("crit", SeverityLevel.Critical.ToKeyword());
        Assert.Equal("err", SeverityLevel.Error.ToKeyword());
        Assert.Equal("notice", SeverityLevel.Notice.ToKeyword());
        Assert.Equal("info", SeverityLevel.Info.ToKeyword());
        Assert.Equal("debug", SeverityLevel.Debug.ToKeyword());

        Assert.Equal("debug", SeverityLevel.DebugL1.ToKeyword());
        Assert.Equal("debug", SeverityLevel.DebugL2.ToKeyword());
        Assert.Equal("debug", SeverityLevel.DebugL3.ToKeyword());
        Assert.Equal("debug", SeverityLevel.Lowest.ToKeyword());
        Assert.Equal("debug", SeverityLevel.Disabled.ToKeyword());
    }

    [Fact]
    public void ToPriorityCode_CorrectMappings()
    {
        // RFC: For example, a kernel message (Facility=0) with a Severity of Emergency
        // (Severity=0) would have a Priority value of 0.  Also, a "local use 4"
        // message (Facility=20) with a Severity of Notice (Severity=5) would
        // have a Priority value of 165.
        Assert.Equal("0", SeverityLevel.Emergency.ToPriorityCode((FacilityId)0));
        Assert.Equal("165", SeverityLevel.Notice.ToPriorityCode(FacilityId.Local4));
    }
}