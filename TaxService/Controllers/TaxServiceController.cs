using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.WebPages;
using TaxService.Models;
using TaxService.Services;

namespace TaxService.Controllers
{
    public class TaxServiceController : ApiController
    {
        /// <summary>
        /// Post a request to Get the tax rate from a TaxCalculator via the Tax Service
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public HttpResponseMessage GetTaxRate([FromBody]GetTaxRateRequest request)
        {
            //Prepare the Response
            var response = Request.CreateResponse();
            string result = "";

            Services.TaxService taxService;

            try
            {
                //Define the TaxService class; 
                //TODO: At some other time, we should define what consumer is calling this function to decide on
                //What to do with deciding on a Calculator to use
                taxService = new Services.TaxService(new TaxJar_Calculator());

                //Filter Inputs on the top; Assuming that any calculator would need this information. 
                if (request.ZipCode.IsEmpty()) throw new ArgumentException("Zip Code is Required", "ZipCode");
                if (request.Country.IsEmpty() || request.Country.Length < 2 ) throw new ArgumentException( "Country Code requires two characters", "Country");

                //Call TaxService to get the Calculator's TaxRate Function. 
                var objResult = taxService.GetTaxRate(request);
                //Serialize the result to be sent over HTTP and return a 200
                result = JsonConvert.SerializeObject(objResult);
                response.StatusCode = HttpStatusCode.OK;

            }
            catch (ArgumentException ex)
            {
                //Return 400 level errors here if any are caught
                if (ex.ParamName == "Country") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Country code requires two characters");
                if (ex.ParamName == "ZipCode") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Zip Code is Required");
                if (ex.ParamName == "US ZipCode") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "US Zip Code Must Be at least 3 characters long");
                if (ex.ParamName == "CA ZipCode") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "CA Zip Code Must Be 6 characters long");

            }

            //Make sure the response is formatted properly to be sent over HTTP correctly. 
            response.Content = new StringContent(result, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// Post a request to get the Amount of Tax to Collect on the specified Order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage CalculateTaxForOrder([FromBody]CalculateTaxRequest request)
        {
            var response = Request.CreateResponse();
            string result = "";

            //Define Tax Service Class
            Services.TaxService taxService;

            try
            {
                //Instantiate
                //TODO: Again, we need to think of a way to have either the service decide, or the calling application decide. 
                taxService = new Services.TaxService(new TaxJar_Calculator());

                //Call service's Calculate Tax function. 
                var objResult = taxService.CalculateTaxForOrder(request);
                //serialize for HTTP
                result = JsonConvert.SerializeObject(objResult);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (ArgumentException ex)
            {
                //Return 400 level errors here if any are caught
                if (ex.ParamName == "Country") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Country code requires two characters");
                if (ex.ParamName == "State") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "State code requires two characters");
                if (ex.ParamName == "ZipCode") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Zip Code is Required");
                if (ex.ParamName == "US ZipCode") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "US Zip Code Must Be at least 3 characters long");
                if (ex.ParamName == "CA ZipCode") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "CA Zip Code Must Be 6 characters long");
                if (ex.ParamName == "No Line Items") return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "An order is required to calculate the tax");

            }

            //Format for HTTP
            response.Content = new StringContent(result, Encoding.UTF8, "application/json");
            return response; 
        }
        

    }
}
