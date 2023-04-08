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
    public void CleanUp_MapsProperlyCasing(string input, string expectedOutput)
    {
        var subject = new ActivationRequest
        {
            FirstName = input
        };

        subject.CleanUp();

        subject.FirstName.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData("  Anna  ", "Anna")]
    [InlineData("\t\t  \n\rbożena\t\t\n\r  ", "Bożena")]
    [InlineData("  von    \t\n\r  vordon  ", "Von Vordon")]
    public void CleanUp_TrimsWhitechars(string input, string expectedOutput)
    {
        var subject = new ActivationRequest
        {
            FirstName = input
        };

        subject.CleanUp();

        subject.FirstName.Should().Be(expectedOutput);
    }

    [Fact]
    public void CleanUp_CleansBothFirstAndLastName()
    {
        var subject = new ActivationRequest
        {
            FirstName = "joanna",
            LastName = "kowalska"
        };

        subject.CleanUp();

        subject.FirstName.Should().Be("Joanna");
        subject.LastName.Should().Be("Kowalska");
    }
}
