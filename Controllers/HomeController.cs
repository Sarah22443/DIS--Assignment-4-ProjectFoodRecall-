using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectFoodRecall.Data_Access;
using ProjectFoodRecall.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectFoodRecall.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext dbContext;
        static string BASE_URL = "https://api.fda.gov/food/enforcement.json?";
        static string API_KEY = "CABrC4KLzBtLHUZZY1atwU5eNdyb3AplHf3YE5Sn"; //Add your API key here inside ""
        private readonly ILogger<HomeController> _logger;
       
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            dbContext = context;

        }

        public IActionResult ApiCall(HttpClient httpClient)
        {
            string FOOD_RECALL_API_PATH = BASE_URL + "api_key=" + API_KEY + "&search=report_date:[20040101+TO+20200410]&limit=100";
            string enforcementData = "";
            Recall_Items enforcements=new Recall_Items();
            //public ApplicationDbContext _context;

            try
            {
                HttpResponseMessage response = httpClient.GetAsync(FOOD_RECALL_API_PATH).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    enforcementData = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }

                if (!enforcementData.Equals(""))
                {
                    //need to remove the meta object from the string
                    dynamic jsonData = JsonConvert.DeserializeObject(enforcementData);
                    //to get only results object
                    var resultsData = jsonData["results"];
                    //convert json to string
                    string jsonString = JsonConvert.SerializeObject(resultsData);
                    //json[1];
                    // JsonConvert is part of the NewtonSoft.Json Nuget package
                    // binds with the model
                    enforcements.results = JsonConvert.DeserializeObject<List<Recall_Item>>(jsonString);
                    //add data to database tables
                    dbContext.Recall_Items.Add(enforcements);
                    //save changes to the database
                    dbContext.SaveChanges();
                }
               
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            return View(dbContext.Recall_Items_data.ToList());

        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Food()
        {
            return View();
        }
        public IActionResult Fda()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
