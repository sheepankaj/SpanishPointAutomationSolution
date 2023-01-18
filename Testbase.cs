using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SpanishPointAutomationSolution.Framework.Common
{
    public abstract class TestBase : IDisposable
    {
        #region Driver Setup
        public static IWebDriver driver;

        /// <summary>
        /// Gets the Driver from CreateBrowserDriver method
        /// </summary>

        protected TestBase()
        {
            driver = CreateBrowserDriver("chrome");
        }
        #endregion Driver Setup

        #region Dispose test execution after test done
        /// <summary>
        /// Dispose the test after we are done
        /// </summary>
        public virtual void Dispose()
        {
            driver.Quit();
        }
        #endregion


        #region Browser SetUp
        /// <summary>
        /// Generate a browser driver for test
        /// </summary>
        /// <param name="BrowserDriver"></param>
        /// <returns></returns>
        private static IWebDriver CreateBrowserDriver(string browserDriver)
        {

            if (browserDriver == null) return new ChromeDriver();

            switch (browserDriver)
            {
                case "chrome":
                    ChromeOptions chrome = new ChromeOptions();
                    chrome.AddArgument("--start-maximized");

                    return new ChromeDriver(chrome);

                default:
                    throw new Exception("Cannot find browser driver with the name: " + browserDriver);

            }

        }

        #endregion

        #region Go to URl and open it in the browser
        /// <summary>
        /// Open an browser in the browser
        /// </summary>
        /// <param name="url"></param>
        public void VisitUrl(string url)
        {
            try
            {
                driver.Navigate().GoToUrl(url);
                Wait().Until(ExpectedConditions.UrlMatches(url));
            }
            catch (Exception)
            {
                string actualURL = driver.Url.ToString();

                throw new Exception("This url was opened ----- " + actualURL + " ----- But was expecting this URL ----- " + url);
            }
        }

        #endregion


        #region Useful Methods to do task in the browser like click, move, etc
        /// <summary>
        /// Used to Wait for element to be clickable then move to the element
        /// </summary>
        /// <param name="selector"></param>
        private void WaitForElementTobeClickableAndMoveToElement(By selector)
        {
            IWebElement element = Wait().Until(ExpectedConditions.ElementToBeClickable(selector));
            MoveToElement(selector);

            element.Click();

        }

        /// <summary>
        /// This moves to the specific element
        /// </summary>
        /// <param name="selector"></param>
        public void MoveToElement(By selector)
        {
            try
            {
                IWebElement onlyElement = driver.FindElement(selector);
                Action().MoveToElement(onlyElement).Perform();
            }
            catch (Exception)
            {
                throw new Exception("Could not find element to move to it - " + selector);
            }
        }

        /// <summary>
        /// Scroll to the element value in the page
        /// </summary>
        /// <param name="selector"></param>
        /// <exception cref="Exception"></exception>
        public void ScrollDownToElement(By selector)
        {
            try
            {
                IWebElement elementTextValue = driver.FindElement(selector);
                Action().MoveToElement(elementTextValue).Perform();
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);" + "window.scrollBy(0,100);", elementTextValue);
            }
            catch (Exception)
            {
                throw new Exception("Could not find element to move to it - " + selector);
            }

        }

        /// <summary>
        /// Proper use of this is as per below
        /// Example: TryTo.Action().SendKeys(Keys.Arrowdown)
        /// </summary>
        /// <returns></returns>
        public Actions Action()
        {
            return new Actions(driver);
        }

        /// <summary>
        /// This is a polling method to wait for an Expected Condition and periodically check it and fail if time runs out
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public WebDriverWait Wait(int seconds = 15)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds))
            {
                PollingInterval = TimeSpan.FromMilliseconds(200)
            };
            return wait;
        }

        /// <summary>
        /// Press the button with the given selector.
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TestBase Click(By selector)
        {
            try
            {
                RetryTimer(() =>
                {
                    try
                    {
                        WaitForElementTobeClickableAndMoveToElement(selector);
                    }
                    catch (ElementNotVisibleException)
                    {
                        WaitForElementTobeClickableAndMoveToElement(selector);
                    }
                    catch (StaleElementReferenceException)
                    {
                        WaitForElementTobeClickableAndMoveToElement(selector);
                    }
                });
            }

            catch (Exception)
            {
                throw new Exception("Unable to click element - " + selector);
            }

            return this;
        }

        /// <summary>
        /// This is a wrapper retry function ..it will retry a function for the set amount of seconds then fail
        /// </summary>
        /// <param name="function"></param>
        public static void RetryTimer(Action function)
        {
            int seconds = 10;
            bool elementFound;
            Stopwatch stopWatch = new Stopwatch();

            stopWatch.Start();

            while (stopWatch.Elapsed < TimeSpan.FromSeconds(seconds))
            {
                try
                {
                    function();
                    elementFound = true;
                }
                catch (Exception)
                {
                    if (stopWatch.Elapsed <= TimeSpan.FromSeconds(seconds))
                    {
                        elementFound = false;
                    }
                    else
                    {
                        throw;
                    }
                }

                if (elementFound)
                    break;
            }
            stopWatch.Reset();
        }

        /// <summary>
        /// Waits for element to be visible
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public IWebElement WaitForElementToBeVisible(By selector)
        {
            try
            {
                Wait().Until(ExpectedConditions.ElementIsVisible(selector));

                return driver.FindElement(selector);
            }
            catch (Exception)
            {
                throw new Exception("This element is not visible when it should be - " + selector);
            }
        }

        /// <summary>
        /// This takes the text of an element and compares it against text given to verify if its correct
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="elementText"></param>
        public void VerifyElementTextIsCorrect(By selector, string elementText)
        {
            string actualElementText = "";
            string noWhiteSpaceExpectedElementText = "";
            string noWhiteSpaceActualElementText = "";
            int breakWhileLoopCounter = 100;

            try
            {
                RetryTimer(() =>
                {
                    while (true)
                    {
                        actualElementText = driver.FindElement(selector).Text;

                        noWhiteSpaceExpectedElementText = Regex.Replace(elementText, " ", "");
                        noWhiteSpaceActualElementText = Regex.Replace(actualElementText, " ", "");


                        if (noWhiteSpaceActualElementText == noWhiteSpaceExpectedElementText)
                            break;

                        breakWhileLoopCounter--;

                        if (breakWhileLoopCounter == 0)
                            break;
                    }
                });

                noWhiteSpaceActualElementText.Should().Contain(noWhiteSpaceExpectedElementText);

            }
            catch (Exception)
            {
                throw new Exception("Unable to find this text { " + elementText + " } In this text  - " + actualElementText);
            }
        }

        #endregion
    }
}

