using FluentAssertions;
using Xunit;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Tests.Domain.Model;

public class ActivationRequestTests
{
    [Theory]
    [InlineData("Jan", "Jan")]
    [InlineData("ANNA", "Anna")]
    [InlineData("renée", "Renée")]
    [InlineData("ToMaSz", "Tomasz")]
    [InlineData("bARTŁOMIEJ", "Bartłomiej")]
    [InlineData("JAN KANTY", "Jan Kanty")]
    [InlineData("øyvind", "Øyvind")]
    [InlineData("JEAN-BAPTISTE", "Jean-Baptiste")]
    [InlineData("von vordon", "Von Vordon")] // acceptable, but wrong
    public void MapsProperlyCasing(string input, string expectedOutput)
    {
        var subject = new ActivationRequest
        {
            FirstName = input
        };

        subject.FirstName.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData("  Anna  ", "Anna")]
    [InlineData("\t\t  \n\rbożena\t\t\n\r  ", "Bożena")]
    [InlineData("  von    \t\n\r  vordon  ", "Von Vordon")]
    public void TrimsWhitechars(string input, string expectedOutput)
    {
        var subject = new ActivationRequest
        {
            FirstName = input
        };

        subject.FirstName.Should().Be(expectedOutput);
    }

    [Fact]
    public void CleansBothFirstAndLastName()
    {
        var subject = new ActivationRequest
        {
            FirstName = "joanna",
            LastName = "kowalska"
        };

        subject.FirstName.Should().Be("Joanna");
        subject.LastName.Should().Be("Kowalska");
    }

    [Theory]
    [InlineData("XY12344", "XY12344")]
    [InlineData("XY-12344", "XY12344")]
    [InlineData("xy123xy44", "XY123XY44")]
    [InlineData("  xy 12344 ", "XY12344")]
    public void CleansMembershipNumber(string input, string expectedOutput)
    {
        var subject = new ActivationRequest
        {
            MembershipNumber = input
        };

        subject.MembershipNumber.Should().Be(expectedOutput);
    }
}
