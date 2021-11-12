﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ShadowRoot.Digger;
using System.Linq;

namespace ShadowRootDigger.NET.Tests
{
    [TestClass]
    public class ShadowRootDiggerTests : TestBase
    {
        private readonly string _tabRootElement = "settings-ui > settings-main#main > settings-basic-page.cr-centered-card-container > settings-privacy-page > settings-clear-browsing-data-dialog > cr-tabs[role=\"tablist\"]";
        private readonly string _clearBrowsingDataDialogRootElement = "settings-ui > settings-main#main > settings-basic-page.cr-centered-card-container > settings-privacy-page > settings-clear-browsing-data-dialog";
        private readonly string _settingsDropdownMenuRootElement = "settings-ui > settings-main#main > settings-basic-page.cr-centered-card-container > settings-privacy-page > settings-clear-browsing-data-dialog > settings-dropdown-menu#clearFromBasic";
        private readonly string _divTabIdentifier = "div.tab";
        private readonly string _selectTimeRangeIdentifier = "select#dropdownMenu";
        private readonly string _basicTabCheckboxesIdentifier = "settings-checkbox[id*=\"CheckboxBasic\"]";
        private readonly string _buttonClearDataIdentifier = "#clearBrowsingDataConfirm";
        private readonly string _notexistsRootElement = "settings-ui > settings-main.main";

        [TestMethod]
        [TestCategory("TESTS-DOTNETFRAMEWORK")]
        public void Test_GetNestedShadowRootElement_ClearChromeData()
        {
            WebDriver.Navigate().GoToUrl("https://www.google.com");
            WebDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            var clearBrowsingTab = ShadowRootHelper.GetNestedShadowRootElement(WebDriver, _tabRootElement);
            clearBrowsingTab.FindElements(By.CssSelector(_divTabIdentifier))
                .FirstOrDefault(item => item.Text == "Basic").Click();
            var settingsDropdownMenu = ShadowRootHelper.GetNestedShadowRootElement(WebDriver, _settingsDropdownMenuRootElement);
            var timeRangeSelect = settingsDropdownMenu.FindElement(By.CssSelector(_selectTimeRangeIdentifier));
            new SelectElement(timeRangeSelect).SelectByText("Last hour");
            var clearBrowsingDataDialog = ShadowRootHelper.GetNestedShadowRootElement(WebDriver, _clearBrowsingDataDialogRootElement);
            var basicCheckboxes = clearBrowsingDataDialog.FindElements(By.CssSelector(_basicTabCheckboxesIdentifier));
            foreach (var checkbox in basicCheckboxes)
            {
                var isCheckboxChecked = checkbox.GetAttribute("checked");
                if (isCheckboxChecked == null)
                    checkbox.Click();
            }
            clearBrowsingDataDialog.FindElement(By.CssSelector(_buttonClearDataIdentifier)).Click();
        }

        [TestMethod]
        [TestCategory("TESTS-DOTNETFRAMEWORK")]
        public void Test_GetNestedShadowRootElement_RootElementDoesNotExists()
        {
            var expectedErrorMessage = "GetNestedShadowRootElement: Nested shadow root element for selector 'settings-main.main' in DOM hierarchy 'return document.querySelector('settings-ui').shadowRoot.querySelector('settings-main.main').shadowRoot;' Not Found.";
            WebDriver.Navigate().GoToUrl("https://www.google.com");
            WebDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            try
            {
                ShadowRootHelper.GetNestedShadowRootElement(WebDriver, _notexistsRootElement);
                Assert.Fail("No Exception Thrown.");
            }
            catch (AssertFailedException ex) { throw ex; }
            catch (WebDriverException ex) { Assert.AreEqual(expectedErrorMessage, ex.Message); }
        }

        [TestMethod]
        [TestCategory("TESTS-DOTNETFRAMEWORK")]
        public void Test_IsNestedShadowRootElementPresent_ShodowRootExists()
        {
            WebDriver.Navigate().GoToUrl("https://www.google.com");
            WebDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            var exists = ShadowRootHelper.IsNestedShadowRootElementPresent(WebDriver, _tabRootElement);
            Assert.AreEqual(true, exists);
            exists = ShadowRootHelper.IsNestedShadowRootElementPresent(WebDriver, _clearBrowsingDataDialogRootElement);
            Assert.AreEqual(true, exists);
            exists = ShadowRootHelper.IsNestedShadowRootElementPresent(WebDriver, _tabRootElement);
            Assert.AreEqual(true, exists);
        }

        [TestMethod]
        [TestCategory("TESTS-DOTNETFRAMEWORK")]
        public void Test_IsNestedShadowRootElementPresent_ShodowRootNotExists()
        {
            var expectedErrorMessage = "IsNestedShadowRootElementPresent: Nested shadow root element for selector 'settings-main.main' in DOM hierarchy 'return document.querySelector('settings-ui').shadowRoot.querySelector('settings-main.main').shadowRoot;' Not Found.";
            WebDriver.Navigate().GoToUrl("https://www.google.com");
            WebDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            var exists = ShadowRootHelper.IsNestedShadowRootElementPresent(WebDriver, _notexistsRootElement);
            Assert.AreEqual(false, exists);
            try
            {
                ShadowRootHelper.IsNestedShadowRootElementPresent(WebDriver, _notexistsRootElement, throwError: true);
                Assert.Fail("No Exception Thrown.");
            }
            catch (AssertFailedException ex) { throw ex; }
            catch (WebDriverException ex) { Assert.AreEqual(expectedErrorMessage, ex.Message); }
        }

        [TestMethod]
        [TestCategory("TESTS-DOTNETFRAMEWORK")]
        public void Test_GetShadowRootElement()
        {
            WebDriver.Navigate().GoToUrl("https://www.google.com");
            WebDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            var clearBrowsingTab = ShadowRootHelper.GetShadowRootElement(WebDriver, "settings-ui");
            Assert.IsNotNull(clearBrowsingTab);
        }

        [TestMethod]
        [TestCategory("TESTS-DOTNETFRAMEWORK")]
        public void Test_GetShadowRootElement_RootElementDoesNotExists()
        {
            var expectedErrorMessage = "GetShadowRootElement: Shadow root element for selector 'not-exists' Not Found.";
            WebDriver.Navigate().GoToUrl("https://www.google.com");
            WebDriver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
            try
            {
                ShadowRootHelper.GetShadowRootElement(WebDriver, "not-exists");
                Assert.Fail("No Exception Thrown.");
            }
            catch (AssertFailedException ex) { throw ex; }
            catch (WebDriverException ex) { Assert.AreEqual(expectedErrorMessage, ex.Message); }
        }
    }
}
