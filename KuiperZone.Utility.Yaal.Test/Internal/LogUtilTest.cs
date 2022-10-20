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
using Xunit;

namespace KuiperZone.Utility.Yaal.Test;

public class LogUtilTest
{
    [Fact]
    public void Escape_EscapesMixedString()
    {
        const string TestString = " Hello world \" test \\ ] Test\nTest \0 \x05 Test ∞ Test";
        const string ExpectFalse = " Hello world \\\" test \\\\ \\] Test\\nTest \\0 \\x05 Test ∞ Test";
        const string ExpectTrue = " Hello world \\\" test \\\\ \\] Test\\nTest \\0 \\x05 Test \\u221E Test";

        var s = LogUtil.Escape(TestString, false, "\\]\"");
        Assert.Equal(ExpectFalse, s);

        s = LogUtil.Escape(TestString, true, "\\]\"");
        Assert.Equal(ExpectTrue, s);
    }

    [Fact]
    public void IsValidId_FalseIfInvalid()
    {
        Assert.False(LogUtil.IsValidId(null));
        Assert.False(LogUtil.IsValidId(""));
        Assert.False(LogUtil.IsValidId("Hello", 3));
        Assert.False(LogUtil.IsValidId("Hello World"));
        Assert.False(LogUtil.IsValidId("Hello=World"));
        Assert.False(LogUtil.IsValidId("Hello]World"));
        Assert.False(LogUtil.IsValidId("Hello\"World"));
        Assert.False(LogUtil.IsValidId("Hello\nWorld"));
        Assert.False(LogUtil.IsValidId("Hello∞World"));

        Assert.False(LogUtil.IsValidId("Hello", 3));

        Assert.True(LogUtil.IsValidId("HelloWorld"));
    }
}