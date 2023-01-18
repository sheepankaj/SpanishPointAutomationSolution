using SpanishPointAutomationSolution.Framework.Common;
using SpanishPointAutomationSolution.Framework.POM;
using Xunit;

namespace SpanishPointAutomationSolution;

[Collection("Solutions And Services Tab- Modern Work Tests")]
public class SolutionsAndServicesTests : TestBase
{
    /// <summary>
    /// Navigate to Modern Work and validate texts in the Employee Experience tab
    /// </summary>
    [Fact]
    public void ValidateEmployeeExperienceTabTexts()
    {
        new SolutionsAndServicesTabPage(this, true)
            .SelectModernWorkOptionFromTheDropDownListOfSolutionAndServicesTab()
            .ClickEmployeeExperienceTabAndValidateText();
    }
}
