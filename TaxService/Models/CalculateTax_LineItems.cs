using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TaxService.Models
{
    public class CalculateTax_LineItem
    {
        [DefaultValue("")]
        public string id { get; set; }

        public string quantity { get; set; }
        public string unit_price { get; set; }

        [DefaultValue("")]
        public string product_tax_code { get; set; }
    }
}