using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxService.Models;

namespace TaxServiceTests
{
    [TestClass]
    public class TaxServiceTest
    {
        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_GetTaxRate_ReturnsDouble()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("32821", "US", "Orlando", null);

            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            double expectedCombinedRate = 0.065;

            var taxjarResponse = taxjarService.GetTaxRate(rateRequest);

            Assert.AreEqual(expectedCombinedRate, taxjarResponse, 0.0);
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_CalculateTaxForOrder_ReturnsTotalTaxAsDouble()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "15.0", product_tax_code = "31000" });

            var taxRequest = new CalculateTaxRequest("US", null, "07001", "NJ", null, "US", null, "07446", "NJ", null, "16.50", "1.5", lineItems, null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));
            var expectedAmount = 1.09;

            var actualAmount = taxjarService.CalculateTaxForOrder(taxRequest);

            Assert.AreEqual(expectedAmount, actualAmount, 0.0);

        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_GetTaxRate_MissingCountryCode_ReturnsArgumentException()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("32821", "", "Orlando", null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.GetTaxRate(rateRequest));
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_GetTaxRate_ShortUSZipCode_ReturnsArgumentException()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("32", "US", "Orlando", null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.GetTaxRate(rateRequest));
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_GetTaxRate_ShortCAZipCode_ReturnsArgumentException()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("V5K", "CA", "Vancouver", null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.GetTaxRate(rateRequest));
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_CalculateTaxForOrder_ShortCountryCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99" });

            var taxRequest = new CalculateTaxRequest("U", "Kansas City", "64115", "MO", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_CalculateTaxForOrder_ShortStateCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99" });

            var taxRequest = new CalculateTaxRequest("US", "Kansas City", "64115", "M", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_CalculateTaxForOrder_ShortUSZipCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99" });

            var taxRequest = new CalculateTaxRequest("US", "Kansas City", "64", "MO", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_CalculateTaxForOrder_ShortCAZipCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "16.95" });

            var taxRequest = new CalculateTaxRequest("CA", null, "V6", "BC", null, "CA", null, "M5T 2T6", "ON", null, "16.95", "10", lineItems, null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CallTaxService_GenerateTaxJarCalculator_CalculateTaxForOrder_NoLineItems_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();

            var taxRequest = new CalculateTaxRequest("US", "Kansas City", "64", "MO", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxService.Services.TaxService(new TaxJar_Calculator("https://api.taxjar.com/v2/"));

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }


    }
}
