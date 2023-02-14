using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using ApexRestaurant.Mvc.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
public class CustomerController : Controller
{
    private string? baseUri;
    public CustomerController(IConfiguration iConfig)
    {
        // Get baseUri of Web API from appsettings.json > ApiBaseUrl
        baseUri = iConfig.GetValue<string>("ApiBaseUrl");
    }

    // GET: Customer
    public ActionResult Index()
    {
        IEnumerable<CustomerViewModel>? customers = null;
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUri ?? throw new InvalidOperationException("baseUri is null"));
            client.DefaultRequestHeaders.Add("accept", "application/json");
            var responseTask = client.GetAsync("api/customer");
            responseTask.Wait();

            var result = responseTask.Result;
            if (result.IsSuccessStatusCode)
            {
                var apiResponse = result.Content.ReadAsStringAsync();
                apiResponse.Wait();
                customers = JsonConvert.DeserializeObject<IList<CustomerViewModel>>(apiResponse.Result);
            }
            else //web api sent error response 
            {
                customers = Enumerable.Empty<CustomerViewModel>();
                ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            }
        }
        return View(customers);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Create(CustomerViewModel customer)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUri ?? throw new InvalidOperationException("baseUri is null"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            //HTTP POST
            customer.EnrollDate = DateTime.Now;
            customer.CreatedBy = "admin";
            customer.CreatedOn = DateTime.Now;
            customer.UpdatedBy = "admin";
            customer.UpdatedOn = DateTime.Now;

            var customerJson = JsonConvert.SerializeObject(customer);
            var postTask = client.PostAsync("api/customer", new StringContent(customerJson, Encoding.UTF8, "application/json"));
            postTask.Wait();

            var result = postTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
        }

        ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");
        return View(customer);
    }

    public async Task<ActionResult> Edit(int id)
    {
        CustomerViewModel? customer = null;
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUri ?? throw new InvalidOperationException("baseUri is null"));
            //HTTP GET
            var result = await client.GetAsync("api/customer/" + id.ToString());
            if (result.IsSuccessStatusCode)
            {
                var readTask = await result.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<CustomerViewModel>(readTask);
            }
        }
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(CustomerViewModel customer)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUri ?? throw new InvalidOperationException("baseUri is null"));

            //HTTP PUT
            var customerJson = JsonConvert.SerializeObject(customer);
            var content = new StringContent(customerJson, Encoding.UTF8, "application/json");
            var putTask = client.PutAsync("api/customer", content);
            putTask.Wait();
            var result = putTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
        }
        return View(customer);
    }

    public ActionResult Delete(int id)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUri ?? throw new InvalidOperationException("baseUri is null"));
            //HTTP DELETE
            var deleteTask = client.DeleteAsync("api/customer/" + id.ToString());
            deleteTask.Wait();
            var result = deleteTask.Result;
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
        }
        return RedirectToAction("Index");
    }

}
