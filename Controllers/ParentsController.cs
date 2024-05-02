using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Monitoring.Controllers
{
    public class ParentsController : Controller
    {
        private readonly IFirebaseConfig _config;
        private readonly IFirebaseClient _client;

        public ParentsController()
        {
            _config = new FirebaseConfig
            {
                AuthSecret = "xd3KJAJ273yOWuvxQVMNDToMysjVG3RecrtxVLvT",
                BasePath = "https://health-monitoring-5c665-default-rtdb.asia-southeast1.firebasedatabase.app/",
            };
            _client = new FireSharp.FirebaseClient(_config);
        }

        public IActionResult Index()
        {
           
            FirebaseResponse response = _client.Get("Parents");
            if (response.Body != "null") // Check if response contains data
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Parent>();
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Parent>(((JProperty)item).Value.ToString()));
                }
                return View(list);
            }
            else
            {
                // Handle case where there is no data
                return View(new List<Parent>());
            }
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Parent parent)
        {
            try
            {
                AddParentToFirebase(parent);
                ModelState.AddModelError(string.Empty, "Added successfully!");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Details(string id)
        {
          
            FirebaseResponse response = _client.Get("Parents/" + id);
            Parent data = JsonConvert.DeserializeObject<Parent>(response.Body);

            return View(data);
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            
            FirebaseResponse response = _client.Get("Parents/" + id);
            Parent data = JsonConvert.DeserializeObject<Parent>(response.Body);

            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Parent parent)
        {
            
            SetResponse response = _client.Set("Parents/" + parent.ParentId, parent);
            return RedirectToAction("Index");
        }

        private void AddParentToFirebase(Parent parent)
        {
           
            var data = parent;
            PushResponse response = _client.Push("Parents/", data);
            data.ParentId = response.Result.name;
            SetResponse setResponse = _client.Set("Parents/" + data.ParentId, data);

        }
        // GET: Child/Delete/5
        public IActionResult Delete(string id)
        {
            FirebaseResponse response = _client.Get("Parents/" + id);
            Parent parent= JsonConvert.DeserializeObject<Parent>(response.Body);
            return View(parent);
        }

        // POST: Child/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            FirebaseResponse response = _client.Delete("Parents/" + id);
            TempData["SuccessMessage"] = "Parent deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
