using OpenQA.Selenium;
using SpanishPointAutomationSolution.Framework.Common;

namespace SpanishPointAutomationSolution.Framework.POM
{
    public class SolutionsAndServicesTabPage
    {
        #region Page Driver setup
        /// <summary>
        /// Giving the driver to the class
        /// </summary>
        private readonly TestBase TryTo;

        /// <summary>
        /// Locators values
        /// </summary>
        private readonly static By RejectCookiesButton = By.Id("wt-cli-reject-btn");
        private readonly static By SolutionsAndServicesTab = By.Id("menu-item-6191");
        private readonly static By ModernWorkOption = By.Id("menu-item-23119");
        private readonly static By ModernWorkTitleText = By.XPath("//h2[@class='vc_custom_heading']");
        private readonly static By ModernWorkplaceSolutionsText = By.XPath("//section/section/div[3]//h2");
        private readonly static By EmployeeExperienceTab = By.XPath("//body/div[2]//ul/li[2]/a/span");
        private readonly static By EngagingMobileIntranetAndDigitlWorkspaceCollabSolutionText = By.XPath("//*[@id='1612869843368-c8e2f605-d38c']//strong");

        /// <summary>
        /// Open the spanishpoint.ie website
        /// </summary>  
        /// <param name="test"></param>
        /// <param name="startingPage"></param>
        public SolutionsAndServicesTabPage(TestBase test, bool startingPage = false)
        {
            TryTo = test;
            if (startingPage)
            {
                test.VisitUrl("https://www.spanishpoint.ie/");
            }

        }
        #endregion

        /// <summary>
        /// Click the Soultions & Services Tab, then select the 'Modern Work' option from the list and validate the text too
        /// </summary>
        /// <returns></returns>
        public SolutionsAndServicesTabPage SelectModernWorkOptionFromTheDropDownListOfSolutionAndServicesTab()
        {
            TryTo.Click(RejectCookiesButton);
            TryTo.MoveToElement(SolutionsAndServicesTab);
            TryTo.WaitForElementToBeVisible(ModernWorkOption);
            TryTo.Click(ModernWorkOption);
            TryTo.VerifyElementTextIsCorrect(ModernWorkTitleText, "Modern Work");
            TryTo.ScrollDownToElement(EmployeeExperienceTab);
            TryTo.VerifyElementTextIsCorrect(ModernWorkplaceSolutionsText, "Modern Workplace Solutions");

            return new SolutionsAndServicesTabPage(TryTo);
        }

        /// <summary>
        /// Click the Employee Experience tab and validate the text-Engaging, Mobile Intranet and Digital Workspace collaboration solution.
        /// </summary>
        /// <returns></returns>
        public SolutionsAndServicesTabPage ClickEmployeeExperienceTabAndValidateText()
        {
            TryTo.Click(EmployeeExperienceTab);
            TryTo.VerifyElementTextIsCorrect(EngagingMobileIntranetAndDigitlWorkspaceCollabSolutionText, "Engaging, Mobile Intranet and Digital Workspace collaboration solution.");

            return new SolutionsAndServicesTabPage(TryTo);
        }

    }
}

