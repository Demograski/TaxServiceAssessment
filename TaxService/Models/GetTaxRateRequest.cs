using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaxService.Models
{
    /// <summary>
    /// Request Object built to Parse the Rate Requests being sent in to the API
    /// </summary>
    public class GetTaxRateRequest
    {
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public GetTaxRateRequest(string zipCode, string country, string city, string street = null)
        {
            ZipCode = zipCode;
            Country = country;
            City = city;
            Street = street;
        }
    }
}