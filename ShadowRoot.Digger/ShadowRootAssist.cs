﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Reflection;

namespace ShadowRoot.Digger
{
    public static class ShadowRootAssist
    {
        /// <summary>
        /// Returns shadow root element for provided selector.
        /// Throws - 'WebDriverException' incase any shadow root element is not found.
        /// </summary>
        /// <param name="webDriver">Selenium webdriver instance.</param>
        /// <param name="shadowRootSelector">Shadow root element selectors (probably jQuery or CssSelectors).</param>
        /// <param name="timeInSeconds">Wait time in seconds. Default - '20 seconds'.</param>
        /// <param name="pollingIntervalInMilliseconds">Polling interval time in milliseconds. Default - '2000 milliseconds'.</param>
        /// <returns>Shadow root web element.</returns>
        public static IWebElement GetShadowRootElement(this IWebDriver webDriver, string shadowRootSelector, int timeInSeconds = 20, int pollingIntervalInMilliseconds = 2000)
        {
            var shadowRootQuerySelector = "return document.querySelector('{0}').shadowRoot";
            var GlobalDriverImplicitWait = webDriver.Manage().Timeouts().ImplicitWait.Ticks;
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds);
            var shadowRootElement = string.Format(shadowRootQuerySelector, shadowRootSelector);
            try
            {
                var webDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeInSeconds))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds)
                };
                webDriverWait.Until(item => (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(shadowRootElement) != null);
            }
            catch (WebDriverException)
            {
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
                throw new WebDriverException(string.Format("{0}: Shadow root element for selector '{1}' Not Found.", MethodBase.GetCurrentMethod().Name, shadowRootSelector));
            }
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
            var requiredShadowRoot = (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(shadowRootElement);
            return requiredShadowRoot;
        }

        /// <summary>
        /// Returns nested shadow root element from DOM hierarchy of shadow root elements with selectors separated by '>'.
        /// Throws - 'WebDriverException' incase any shadow root element is not found in the nested hierarchy.
        /// </summary>
        /// <param name="webDriver">Selenium webdriver instance.</param>
        /// <param name="shadowRootSelectors">List of shadow root element selectors (probably jQuery or CssSelectors) separated by '>'.</param>
        /// <param name="timeInSeconds">Wait time in seconds. Default - '20 seconds'.</param>
        /// <param name="pollingIntervalInMilliseconds">Polling interval time in milliseconds. Default - '2000 milliseconds'.</param>
        /// <returns>Nested shadow root web element.</returns>
        public static IWebElement GetNestedShadowRootElement(this IWebDriver webDriver, string shadowRootSelectors, int timeInSeconds = 20, int pollingIntervalInMilliseconds = 2000)
        {
            var listShadowRootSelectors = shadowRootSelectors.Split('>')
                .Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p));
            var shadowRootQuerySelector = ".querySelector('{0}').shadowRoot";
            var shadowRootQueryString = "";
            var shadowRootElement = "";
            foreach (var shadowRoot in listShadowRootSelectors)
            {
                var documentReturn = "return document{0};";
                var tempQueryString = string.Format(shadowRootQuerySelector, shadowRoot);
                shadowRootQueryString += tempQueryString;
                shadowRootElement = string.Format(documentReturn, shadowRootQueryString);
                var GlobalDriverImplicitWait = webDriver.Manage().Timeouts().ImplicitWait.Ticks;
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds);
                try
                {
                    var webDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeInSeconds))
                    {
                        PollingInterval = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds)
                    };
                    webDriverWait.Until(item => (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(shadowRootElement) != null);
                }
                catch (WebDriverException)
                {
                    webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
                    throw new WebDriverException(string.Format("{0}: Nested shadow root element for selector '{1}' in DOM hierarchy '{2}' Not Found.", MethodBase.GetCurrentMethod().Name, shadowRoot, shadowRootSelectors));
                }
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
            }
            var requiredShadowRoot = (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(shadowRootElement);
            return requiredShadowRoot;
        }

        /// <summary>
        /// Checks if the shadow root element exists or not.
        /// Throws - 'WebDriverException' incase any shadow root element is not found when <param name="throwError"> is set to 'true'.
        /// </summary>
        /// <param name="webDriver">Selenium webdriver instance.</param>
        /// <param name="shadowRootSelector">Shadow root element selectors (probably jQuery or CssSelectors).</param>
        /// <param name="throwError">Boolean value to throw error if nested shadow root element hierarchy does not exists. Default - 'false'.</param>
        /// <param name="timeInSeconds">Wait time in seconds. Default - '20 seconds'.</param>
        /// <param name="pollingIntervalInMilliseconds">Polling interval time in milliseconds. Default - '2000 milliseconds'.</param>
        /// <returns>Boolean value shadow root element exists or not.</returns>
        public static bool IsShadowRootElementPresent(this IWebDriver webDriver, string shadowRootSelector, bool throwError = false, int timeInSeconds = 20, int pollingIntervalInMilliseconds = 2000)
        {
            var isPresent = false;
            var shadowRootQuerySelector = "return document.querySelector('{0}').shadowRoot";
            var shadowRootElement = string.Format(shadowRootQuerySelector, shadowRootSelector);
            var GlobalDriverImplicitWait = webDriver.Manage().Timeouts().ImplicitWait.Ticks;
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds);
            try
            {
                var webDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeInSeconds))
                {
                    PollingInterval = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds)
                };
                webDriverWait.Until(item => (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(shadowRootElement) != null);
                isPresent = true;
            }
            catch (WebDriverException) 
            {
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
                if (throwError)
                    throw new WebDriverException(string.Format("{0}: Shadow root element for selector '{1}' Not Found.", MethodBase.GetCurrentMethod().Name, shadowRootSelector));
                else
                    isPresent = false;
            }
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
            return isPresent;
        }

        /// <summary>
        /// Checks if the nested shadow root element hierarchy exists or not.
        /// Throws - 'WebDriverException' incase any shadow root element is not found in the nested hierarchy when <param name="throwError"> is set to 'true'.
        /// </summary>
        /// <param name="webDriver">Selenium webdriver instance.</param>
        /// <param name="shadowRootSelectors">List of shadow root element selectors (probably jQuery or CssSelectors) separated by '>'.</param>
        /// <param name="throwError">Boolean value to throw error if nested shadow root element hierarchy does not exists. Default - 'false'.</param>
        /// <param name="timeInSeconds">Wait time in seconds. Default - '20 seconds'.</param>
        /// <param name="pollingIntervalInMilliseconds">Polling interval time in milliseconds. Default - '2000 milliseconds'.</param>
        /// <returns>Boolean value if nested shadow root element hierarchy exists or not.</returns>
        public static bool IsNestedShadowRootElementPresent(this IWebDriver webDriver, string shadowRootSelectors, bool throwError = false, int timeInSeconds = 20, int pollingIntervalInMilliseconds = 2000)
        {
            var isPresent = false;
            var listShadowRootSelectors = shadowRootSelectors.Split('>')
                .Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p));
            var shadowRootQuerySelector = ".querySelector('{0}').shadowRoot";
            var shadowRootQueryString = "";
            var shadowRootElement = "";
            foreach (var shadowRoot in listShadowRootSelectors)
            {
                var documentReturn = "return document{0};";
                var tempQueryString = string.Format(shadowRootQuerySelector, shadowRoot);
                shadowRootQueryString += tempQueryString;
                shadowRootElement = string.Format(documentReturn, shadowRootQueryString);
                var GlobalDriverImplicitWait = webDriver.Manage().Timeouts().ImplicitWait.Ticks;
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds);
                try
                {
                    var webDriverWait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeInSeconds))
                    {
                        PollingInterval = TimeSpan.FromMilliseconds(pollingIntervalInMilliseconds)
                    };
                    webDriverWait.Until(item => (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript(shadowRootElement) != null);
                    isPresent = true;
                }
                catch (WebDriverException) 
                {
                    webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
                    if (throwError)
                        throw new WebDriverException(string.Format("{0}: Nested shadow root element for selector '{1}' in DOM hierarchy '{2}' Not Found.", MethodBase.GetCurrentMethod().Name, shadowRoot, shadowRootSelectors));
                    else
                        isPresent = false;
                }
                webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromTicks(GlobalDriverImplicitWait);
            }
            return isPresent;
        }
    }
}