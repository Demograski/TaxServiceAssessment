using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TaxService.Models
{
    /// <summary>
    /// Request Object Manually generated from the POST examples given for TaxJar
    /// Everything set to public for testability;
    /// </summary>
    public class CalculateTaxRequest
    {
        public string from_country { get; set; }

        [DefaultValue("")]
        public string from_city { get; set; }

        public string from_zip { get; set; }

        [DefaultValue("")]
        public string from_state { get; set; }

        [DefaultValue("")]
        public string from_street { get; set; }

        public string to_country { get; set; }

        [DefaultValue("")]
        public string to_city { get; set; }

        public string to_zip { get; set; }

        [DefaultValue("")]
        public string to_state { get; set; }

        [DefaultValue("")]
        public string to_street { get; set; }

        public string amount { get; set; }
        public string shipping { get; set; }

        public List<CalculateTax_LineItem> line_items { get; set; }

        [DefaultValue("")]
        public List<CalculateTax_NexusAddress> nexus_addresses { get; set; }

        public CalculateTaxRequest(string From_Country, string From_City, string From_Zip, string From_State, string From_Street, 
                                   string To_Country, string To_City, string To_Zip, string To_State, string To_Steet, 
                                   string Amount, string Shipping,
                                   List<CalculateTax_LineItem> Line_Items, List<CalculateTax_NexusAddress> Nexus_Addresses)
        {
            from_country = From_Country;
            from_city = From_City;
            from_zip = From_Zip;
            from_state = From_State;
            from_street = From_Street;
            to_country = To_Country;
            to_city = To_City;
            to_zip = To_Zip;
            to_state = To_State;
            to_street = To_Steet;
            amount = Amount;
            shipping = Shipping;

            line_items = Line_Items;
            nexus_addresses = Nexus_Addresses;

        }
    }
}