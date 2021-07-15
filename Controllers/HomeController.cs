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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using System.Dynamic;
using Newtonsoft.Json;


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
        public IActionResult Enforcement()
        {
            // APIHandler webHandler = new APIHandler();
            //Enforcements enforcement = webHandler.GetEnforcements();


            List<Enforcement> enforcement = dbContext.Enforcements.ToList();

            var results = enforcement.Take(10);

            //var webClient = new WebClient();
            //var json = webClient.DownloadString(@"C:\Users\saahi\source\repos\FoodRecallEnforcements\wwwroot\lib\FoodJson");
            //var meta = JsonConvert.DeserializeObject<Enforcement>(json);

            return View(results);
        }

        public IActionResult DbConnect()
        {
            IList < Recall_Item> enforcement = dbContext.Recall_Items_data.ToList();
            


            for (var i = 0; i < enforcement.Count; i++)
            {
                Recall re = new Recall();
                Location loc = new Location();
                Classification classification = new Classification();
                State state = new State();
                Firm firm = new Firm();

                //setting Classifications table from DB
                classification.center_classification_date = enforcement[i].center_classification_date;
                classification.classification = enforcement[i].classification;


                //setting Recalls table from DB
                re.reason_for_recall = enforcement[i].reason_for_recall;
                re.code_info = enforcement[i].code_info;
                re.product_quantity = enforcement[i].product_quantity;
                re.distribution_pattern = enforcement[i].distribution_pattern;
                re.product_description = enforcement[i].product_description;
                re.report_date = enforcement[i].report_date;
                re.recall_number = enforcement[i].recall_number;
                re.recalling_firm = enforcement[i].recalling_firm;
                re.initial_firm_notification = enforcement[i].initial_firm_notification;
                re.event_id = enforcement[i].event_id;
                re.product_type = enforcement[i].product_type;
                re.termination_date = enforcement[i].termination_date;
                re.recall_initiation_date = enforcement[i].recall_initiation_date;
                re.voluntary_mandated = enforcement[i].voluntary_mandated;


                //Setting Firms table from DB
                firm.recalling_firm = enforcement[i].recalling_firm;

                //setting Location table from DB

                loc.postal_code = enforcement[i].postal_code;
                loc.country = enforcement[i].country;
                loc.city = enforcement[i].city;
                loc.address_1 = enforcement[i].address_1;
                loc.address_2 = enforcement[i].address_2;
                //loc.state = enforcement[i].state;

                //setting State table from DB
                state.State_Code = enforcement[i].state;


                /*re.classification = classification;
                re.location = loc;*/
                re.classification = classification;
                re.location = loc;
                re.State = state;
                re.Firm = firm;

               

            }

            dbContext.SaveChanges();
            return View();
        }





        //SEARCH BY FIRM
        public IActionResult SearchFirm(string searching)
        {
            return View(dbContext.Recall_Items_data.Where(x => x.recalling_firm.Contains(searching) || searching == null).ToList().Take(10));
        }

        //Search By Voluntary or Mandated
        public ActionResult SearchBy(string searchBy, string search)
        {
            if (searchBy == "Voluntary or Mandated")
                return View(dbContext.Recall_Items_data.Where(x => x.voluntary_mandated.Contains(search) || search == null).ToList().Take(10));
            else
                return View(dbContext.Recall_Items_data.Where(x => x.recalling_firm.Contains(search) || search == null).ToList().Take(10));
        }

        public IActionResult AddOrEdit(int id = 0)
        {
            ViewData["ClassificationId"] = new SelectList(dbContext.Recall_Items_data, "classificationId", "classificationId");
            ViewData["LocationId"] = new SelectList(dbContext.Recall_Items_data, "AddressId", "AddressId");

            if (id == 0)
                return View(new Recall());
            else
                return View(dbContext.Recall_Items_data.Find(id));
        }

        // POST: Recall/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("RecallId,reason_for_recall,code_info,product_quantity,distribution_pattern,product_description,report_date,recall_number,recalling_firm,initial_firm_notification,event_id,product_type,termination_date,recall_initiation_date,voluntary_mandated,LocationId,ClassificationId")] Recall_Item recall)
           
           
        {
            if (ModelState.IsValid)
            {
                if (recall.RecallId == 0)
                    dbContext.Add(recall);
                else
                    dbContext.Update(recall);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
             return View(recall);
        }


        // GET: Recall/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var recall = await dbContext.Recall_Items_data.FindAsync(id);
            dbContext.Recall_Items_data.Remove(recall);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Visualize()
        {

            var states = dbContext.Recall_Items_data.Select(m => m.state).Distinct();
            //int[] datacount = new int[states.Count()];
            //int c = 0;
            //List<DataPoint> dataPoints = new List<DataPoint>();
            List<string> State_Code = new List<string>();
            List<int> points = new List<int>();
            foreach (string state in states)
            {


                int count = dbContext.Recall_Items_data.Where(t => t.state == state).Count();
                State_Code.Add(state);
                points.Add(count);

            }
            dynamic model = new ExpandoObject();
            model.State_Code = JsonConvert.SerializeObject(State_Code);

            model.points = JsonConvert.SerializeObject(points);
            // ViewBag.DataPoints = JsonConvert.SerializeObject(dataPoints);

            return View(model);
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
