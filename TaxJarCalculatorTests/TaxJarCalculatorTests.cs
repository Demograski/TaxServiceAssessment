using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaxService.Models;

namespace TaxJarCalculatorTests
{
    [TestClass]
    public class TaxJarCalculatorTests
    {
        [TestMethod]
        public void GetTaxRate_TaxJar_WithProperRequestBody_ReturnsCombinedRateAsDouble()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("32821", "US", "Orlando", null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            double expectedCombinedRate = 0.065;

            var taxjarResponse = taxjarService.GetTaxRate(rateRequest);

            Assert.AreEqual(expectedCombinedRate, taxjarResponse, 0.001, "Tax Rate is different, not an error");

        }

        [TestMethod]
        public void GetTaxRate_TaxJar_CA_ProperBody_ReturnsCombinedRateAsDouble()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("V5K0A1", "CA", "Vancouver", null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            double expectedCombinedRate = 0.12;

            var taxjarResponse = taxjarService.GetTaxRate(rateRequest);

            Assert.AreEqual(expectedCombinedRate, taxjarResponse, 0.001, "Tax Rate is different, not an error");
        }

        [TestMethod]
        public void GetTaxRate_TaxJar_EU_ProperBody_ReturnsCombinedRateAsDouble()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("00150", "FI", "Helsinki", null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            double expectedCombinedRate = 0.24;
            var taxjarResponse = taxjarService.GetTaxRate(rateRequest);

            Assert.AreEqual(expectedCombinedRate, taxjarResponse, 0.001, "Tax Rate is different, not an error");
        }

        [TestMethod]
        public void GetTaxRate_TaxJar_AU_ProperBody_ReturnsCombinedRateAsDouble()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("2000", "AU", "Sydney", null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            double expectedCombinedRate = 0.1;

            var taxjarResponse = taxjarService.GetTaxRate(rateRequest);

            Assert.AreEqual(expectedCombinedRate, taxjarResponse, 0.001, "Tax Rate is different, not an error");
        }

        [TestMethod]
        public void GetTaxRate_TaxJar_MissingCountryCode_ReturnsArgumentException()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("32821", "", "Orlando", null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.GetTaxRate(rateRequest));
        }

        [TestMethod]
        public void GetTaxRate_TaxJar_ShortUSZipCode_ReturnsArgumentException()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("32", "US", "Orlando", null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.GetTaxRate(rateRequest));
        }

        [TestMethod]
        public void GetTaxRate_TaxJar_ShortCAZipCode_ReturnsArgumentException()
        {
            //Arrange
            var rateRequest = new GetTaxRateRequest("V5K", "CA", "Vancouver", null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.GetTaxRate(rateRequest));
        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ProperRequest_US_ReturnsAmountToCollectAsDouble()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "15.0", product_tax_code = "31000" });

            var taxRequest = new CalculateTaxRequest("US", null, "07001", "NJ", null, "US", null, "07446", "NJ", null, "16.50", "1.5", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            var expectedAmount = 1.09;

            var actualAmount = taxjarService.CalculateTaxForOrder(taxRequest);

            Assert.AreEqual(expectedAmount, actualAmount, 0.0);

        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ProperRequest_US_NYExemption_ReturnsAmountToCollectAsDouble()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99", product_tax_code = "20010" });
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "9.95", product_tax_code = "20010" });

            var taxRequest = new CalculateTaxRequest("US", "Delmar", "12054", "NY", null, "US", "Mahopac", "10541", "NY", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            var expectedAmount = 1.98;

            var actualAmount = taxjarService.CalculateTaxForOrder(taxRequest);

            Assert.AreEqual(expectedAmount, actualAmount);

        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ProperRequest_US_CAExemption_ReturnsAmountToCollectAsDouble()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { id = "3", quantity = "1", unit_price = "16.95", product_tax_code = "40030" });

            var taxRequest = new CalculateTaxRequest("US", "San Francisco", "94111", "CA", "600 Montgomery St", "US", "Campbell", "95008", "CA", "33 N. First Street", "16.95", "10", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            var expectedAmount = 0.0; //weirdly no extra taxes?

            var actualAmount = taxjarService.CalculateTaxForOrder(taxRequest);

            Assert.AreEqual(expectedAmount, actualAmount, 0.0);

        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ProperRequest_Canada_ReturnsAmountToCollectAsDouble()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "16.95" });

            var taxRequest = new CalculateTaxRequest("CA", null, "V6G 3E", "BC", null, "CA", null, "M5T 2T6", "ON", null, "16.95", "10", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            var expectedAmount = 3.5; //weirdly no extra taxes?

            var actualAmount = taxjarService.CalculateTaxForOrder(taxRequest);

            Assert.AreEqual(expectedAmount, actualAmount, 0.0);

        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ProperRequest_NoNexusAddresses_ReturnsAmountToCollectAsDouble()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { id = "3", quantity = "1", unit_price = "16.95", product_tax_code = "40030" });

            var taxRequest = new CalculateTaxRequest("US", "San Francisco", "94111", "CA", "600 Montgomery St", "US", "Orlando", "32801", "FL", "200 S. Orange Ave", "16.95", "10", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            var expectedAmount = 0.0; //for some reason, probably an error on their part. 

            var actualAmount = taxjarService.CalculateTaxForOrder(taxRequest);

            Assert.AreEqual(expectedAmount, actualAmount, 0.0);

        }

        //Weirdly, the API I'm running is returning a 2.9, but when I unit test it in my code, it returns a 1.98...
        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ProperRequest_WithNexusAddresses_ReturnsAmountToCollectAsDouble()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99" });
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "9.95" });

            var nexusItems = new List<CalculateTax_NexusAddress>();
            nexusItems.Add(new CalculateTax_NexusAddress { country = "US", state = "FL", zip = "32801" });
            nexusItems.Add(new CalculateTax_NexusAddress { country = "US", state = "MO", zip = "63101" });

            var taxRequest = new CalculateTaxRequest("US", "Orlando", "32801", "FL", null, "US", "Kansas City", "64155", "MO", null, "29.94", "7.99", lineItems, nexusItems);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");
            var expectedAmount = 2.9;

            var actualAmount = taxjarService.CalculateTaxForOrder(taxRequest);

            Assert.AreEqual(expectedAmount, actualAmount);

        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ShortCountryCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99" });

            var taxRequest = new CalculateTaxRequest("U", "Kansas City", "64115", "MO", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ShortStateCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99" });

            var taxRequest = new CalculateTaxRequest("US", "Kansas City", "64115", "M", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ShortUSZipCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "19.99" });

            var taxRequest = new CalculateTaxRequest("US", "Kansas City", "64", "MO", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_ShortCAZipCode_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();
            lineItems.Add(new CalculateTax_LineItem { quantity = "1", unit_price = "16.95" });

            var taxRequest = new CalculateTaxRequest("CA", null, "V6", "BC", null, "CA", null, "M5T 2T6", "ON", null, "16.95", "10", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

        [TestMethod]
        public void CalculateTaxForOrder_TaxJar_NoLineItems_ReturnsArgumentException()
        {
            //arrange
            var lineItems = new List<CalculateTax_LineItem>();

            var taxRequest = new CalculateTaxRequest("US", "Kansas City", "64", "MO", null, "US", "Orlando", "32801", "FL", null, "29.94", "7.99", lineItems, null);
            var taxjarService = new TaxJar_Calculator("https://api.taxjar.com/v2/");

            Assert.ThrowsException<System.ArgumentException>(() => taxjarService.CalculateTaxForOrder(taxRequest));
        }

    }
}
