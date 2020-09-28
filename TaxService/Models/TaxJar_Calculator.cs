using System;
using System.Text;
using System.IO;
using System.Configuration;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TaxService.Models
{
    /// <summary>
    /// TaxJarCalculator: Will call Taxjar's Api to make these two calls
    /// GetTaxRate: returns the Combined Tax Rate, or the Standard Rate if it's not present
    /// CalculateTaxForOrder: returns the Amount to Collect from the given order
    /// </summary>
    public class TaxJar_Calculator : ITaxCalculator
    {
        //set the URl as a string builder to save on memory usage
        internal StringBuilder TaxJarUrl;
        //Storing API token here as a private value; should proabbly find a better way to obsfucate this information. 
        private string apiToken = "5da2f821eee4035db4771edab942a4cc";
        //Serializer settings object, this is used to tell the JSON Serializer what to do with null or default values when it serializes an object.
        internal JsonSerializerSettings settings = new JsonSerializerSettings();

        //Constructor will take a URL or will use the internal one; 
        //Given url is only for the purpose of Unit Testing. 
        public TaxJar_Calculator(string Url = null)
        {
            if(Url == null) TaxJarUrl = new StringBuilder(ConfigurationManager.ConnectionStrings["TaxJarApi"].ConnectionString);
            else TaxJarUrl = new StringBuilder(Url);

            //set null and default values to ignore, so they arent present in the request when serialized
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
        }

        /// <summary>
        /// GETs the tax rate from TaxJar, given a ZipCode and a Country code
        /// All other parameters are not required, but will give more information if given
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public double GetTaxRate(GetTaxRateRequest request)
        {
            //FIlter inputs at the top to ensure a properly generated request body
            string errorText = CheckForValidInput(request, null); 
            string responseText;
            double combinedRate = 0.0;

            if (errorText == "OK")
            {
                try
                {
                    //make call to TaxJar API...
                    TaxJarUrl.Append($"rates/{request.ZipCode}?country={request.Country}&city={request.City}");
                    //Append initial values; if street isnt null, append that after this. 
                    if (request.Street != null && request.Street != "") TaxJarUrl.Append($"&street={request.Street}");

                    //Create a Web Request; 
                    var httpWebRequest = WebRequest.CreateHttp(TaxJarUrl.ToString());
                    //GET command
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Timeout = 300000;
                    //Set a Custom Authorization token; The API requires something that isn't standard. 
                    httpWebRequest.Headers["Authorization"] = $"Token token=\"{apiToken}\"";

                    //Call the API and get a response object
                    var response = (HttpWebResponse)httpWebRequest.GetResponse();

                    //Stream it to Read it.
                    var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            responseText = streamReader.ReadToEnd();
                        }

                        //Parse the stream into a JObject so we can get the info wenwant from a specific node. 
                        var contentObject = JObject.Parse(responseText);

                        //Return the Combined Rate member value, or the Standard Rate, if combined rate isn't present. 
                        if (contentObject["rate"]["combined_rate"] == null)
                            combinedRate = Convert.ToDouble(contentObject["rate"]["standard_rate"]);
                        else combinedRate = Convert.ToDouble(contentObject["rate"]["combined_rate"]);
                    }
                }
                catch (WebException wex)
                {
                    //Toss the web exception up to the Controller to handle it. 
                    throw wex;
                }
            }
            else
            {
                switch (errorText)
                {
                    case "US ZipCode Length":
                        throw new ArgumentException("US ZipCode Requires at least 3 characters", "US ZipCode");
                    case "CA ZipCode Length":
                        throw new ArgumentException("CA Zipcode must be at least 5 characters", "CA ZipCode");
                    case "No Country Code":
                        throw new ArgumentException("No Country Code Present", "Country");
                }
            }
            

            //Assuming Combined_Rate is Requested, return that, or, again, the standard rate
            return combinedRate;
        }

        /// <summary>
        /// POSTS a request to the TaxJar API to return all applicable taxable information on a given order. 
        /// This will return a double value with the total amount of tax to collect
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public double CalculateTaxForOrder(CalculateTaxRequest request)
        {
            //Filter Inputs
            string errorText = CheckForValidInput(null, request);
            string responseText;
            double amountToCollect = 0.0;

            if (errorText == "OK")
            {
                try
                {
                    //Finish Url
                    TaxJarUrl.Append("taxes");

                    //Create HTTP Web Request
                    var httpWebRequest = WebRequest.CreateHttp(TaxJarUrl.ToString());
                    httpWebRequest.Method = "POST";
                    httpWebRequest.Timeout = 300000;

                    //Build Custom Auth header
                    httpWebRequest.Headers["Authorization"] = $"Token token=\"{apiToken}\"";

                    httpWebRequest.ContentType = "application/json";

                    //pass JSON body for POST
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        //Serialize the request with the Serializer settings that Ignore null / default values
                        streamWriter.Write(JsonConvert.SerializeObject(request, settings));
                    }
                    //Get Response from POST
                    var response = (HttpWebResponse)httpWebRequest.GetResponse();
                    var stream = response.GetResponseStream();
                    if (stream != null)
                    {
                        using (var streamReader = new StreamReader(stream))
                        {
                            responseText = streamReader.ReadToEnd();
                        }
                        //Parse object for easy access
                        var contentObject = JObject.Parse(responseText);

                        //Grab Amount to collect value from parsed object
                        amountToCollect = Convert.ToDouble(contentObject["tax"]["amount_to_collect"]);
                    }
                }
                catch (WebException wex)
                {
                    //Throw web exceptions to the Controller
                    throw wex;
                }
            }
            else
            {
                switch (errorText)
                {
                    case "Short Country Code":
                        throw new ArgumentException("Country Code must be two characters", "Country");
                    case "Short State Code":
                        throw new ArgumentException("State Code must be two characters", "State");
                    case "US ZipCode Length":
                        throw new ArgumentException("US Zipcodes must be at least 3 characters in length", "US ZipCode");
                    case "CA ZipCode Length":
                        throw new ArgumentException("CA Zipcodes must be at least 5 character in length", "CA ZipCode");
                    case "No Line Items":
                        throw new ArgumentException("Order Requests must have at least one line item", "Line_Items");
                }
            }

            //return Amount_to_Collect
            return amountToCollect;
        }

        /// <summary>
        /// This function is used purely to check the inputs for each given function, to ensure that the request bodies are built properly. 
        /// </summary>
        /// <param name="rateRequest"></param>
        /// <param name="calculateRequest"></param>
        /// <returns></returns>
        private string CheckForValidInput(GetTaxRateRequest rateRequest = null, CalculateTaxRequest calculateRequest = null)
        {
            if (rateRequest != null)
            {
                switch (rateRequest.Country)
                {
                    case "US":
                        if (rateRequest.ZipCode.Length < 3) return "US ZipCode Length";
                        break;
                    case "CA":
                        if (rateRequest.ZipCode.Length < 5) return "CA ZipCode Length";
                        break;
                    case "":
                    case null:
                        return "No Country Code";
                }
            }

            if(calculateRequest != null)
            {
                //If Country is not correct, throw an error; no need to check the switch for that
                if (calculateRequest.from_country.Length < 2 || calculateRequest.to_country.Length < 2)
                    return "Short Country Code";
                if (calculateRequest.from_state.Length < 2 || calculateRequest.to_state.Length < 2)
                    return "Short State Code";
                if (calculateRequest.from_country == "US" && calculateRequest.from_zip.Length < 3 || calculateRequest.to_zip.Length < 3)
                    return "US ZipCode Length";
                if (calculateRequest.from_country == "CA" && calculateRequest.from_zip.Length < 5 || calculateRequest.from_zip.Length < 5)
                    return "CA ZipCode Length";
                if (calculateRequest.line_items.Count < 1)
                    return "No Line Items";
            }

            return "OK";
        }


        //TODO: Build overloads to return full object instead of just the value possibly
    }
}