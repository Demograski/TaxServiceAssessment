using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxService.Models
{
    /// <summary>
    /// Interface for enforcing the behaviour of any Tax Calculation Classes
    /// </summary>
    public interface ITaxCalculator
    {
        double GetTaxRate(GetTaxRateRequest request);
        double CalculateTaxForOrder(CalculateTaxRequest request);
    }
}
