using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TaxService.Models;

namespace TaxService.Services
{
    /// <summary>
    /// Service class for an ITaxCalculator class object
    /// </summary>
    public class TaxService
    {

        public TaxService() { }

        ITaxCalculator calculator; 

        public TaxService(ITaxCalculator taxCalculator)
        {
            calculator = taxCalculator;
        }

        public double GetTaxRate(GetTaxRateRequest request)
        {
            return calculator.GetTaxRate(request);
        }

        public double CalculateTaxForOrder(CalculateTaxRequest order)
        {
            return calculator.CalculateTaxForOrder(order);
        }
       
    }
}