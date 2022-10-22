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

using Xunit;

namespace KuiperZone.Utility.Yaal.Test;

public class StructuredDataTest
{
    [Fact]
    public void StructuredData_ToString()
    {
        var sd = new StructuredData();
        sd.Add("exampleSDID@32473", new SdElement());
        sd["exampleSDID@32473"].Add("iut", "9");
        sd["exampleSDID@32473"].Add("eventSource", "rawr");
        sd["exampleSDID@32473"].Add("eventID", "123");

        Assert.Equal("[exampleSDID@32473 eventID=\"123\" eventSource=\"rawr\" iut=\"9\"]", sd.ToString());
    }

}