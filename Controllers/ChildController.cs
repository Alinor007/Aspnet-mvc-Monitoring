using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Monitoring.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Monitoring.Controllers
{
    public class ChildController : Controller
    {

        private readonly IFirebaseConfig _config;
        private readonly IFirebaseClient _client;

        public ChildController()
        {
            _config = new FirebaseConfig
            {
                AuthSecret = "xd3KJAJ273yOWuvxQVMNDToMysjVG3RecrtxVLvT",
                BasePath = "https://health-monitoring-5c665-default-rtdb.asia-southeast1.firebasedatabase.app/",
            };
            _client = new FireSharp.FirebaseClient(_config);
        }

        // Index action to list all children
        public IActionResult Index()
        {
            var childList = new List<Child>();


            FirebaseResponse childResponse = _client.Get("Children");
            if (childResponse.Body != "null")
            {
                dynamic childData = JsonConvert.DeserializeObject<dynamic>(childResponse.Body);
                foreach (var item in childData)
                {
                    var child = JsonConvert.DeserializeObject<Child>(((JProperty)item).Value.ToString());

                    // Fetch parent information for each child
                    FirebaseResponse parentResponse = _client.Get($"Parents/{child.ParentId}");
                    if (parentResponse.Body != "null")
                    {
                        var parentData = JsonConvert.DeserializeObject<Parent>(parentResponse.Body);
                        child.ParentName = parentData.Name;

                    }

                    childList.Add(child);

                }
            }

            return View(childList);
        }

        // Action to display the form for creating a new child
        [HttpGet]
        public ActionResult Create()
        {
            SetParentOptionsInViewBag();
            return View();
        }

        // Action to handle the submission of the form for creating a new child
        [HttpPost]
        public ActionResult Create(Child child)
        {
            try
            {
                AddChildToFirebase(child);
                TempData["SuccessMessage"] = "Child added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            SetParentOptionsInViewBag();
            return RedirectToAction("Index");
        }

        // Action to display the details of a specific child
        public ActionResult Detail(string id)
        {
            FirebaseResponse response = _client.Get("Children/" + id);
            Child child = JsonConvert.DeserializeObject<Child>(response.Body);

            return View(child);
        }

        // Action to display the form for editing a child
        [HttpGet]
        public ActionResult Edit(string id)
        {
            FirebaseResponse response = _client.Get("Children/" + id);
            Child child = JsonConvert.DeserializeObject<Child>(response.Body);
            SetParentOptionsInViewBag();
            return View(child);
        }

        // Action to handle the submission of the form for editing a child
        [HttpPost]
        public ActionResult Edit(Child child)
        {
            SetResponse response = _client.Set("Children/" + child.ChildId, child);
            SetParentOptionsInViewBag();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
          
           
            FirebaseResponse response = _client.Get("Children/"+id);
            Child child = JsonConvert.DeserializeObject<Child>(response.Body);


            return View(child);
        }



        // GET: Child/Delete/5
        public IActionResult Delete(string id)
        {
            FirebaseResponse response = _client.Get("Children/" + id);
            Child child = JsonConvert.DeserializeObject<Child>(response.Body);
            return View(child);
        }

        // POST: Child/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            FirebaseResponse response = _client.Delete("Children/" + id);
            TempData["SuccessMessage"] = "Child deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        // Method to add a child to Firebase
        private void AddChildToFirebase(Child child)
        {
            var data = child;
            PushResponse response = _client.Push("Children/", data);
            data.ChildId = response.Result.name;
            SetResponse setResponse = _client.Set("Children/" + data.ChildId, data);
        }
        private void SetParentOptionsInViewBag()
        {
            FirebaseResponse parentResponse = _client.Get("Parents");
            if (parentResponse.Body != "null")
            {
                dynamic parentData = JsonConvert.DeserializeObject<dynamic>(parentResponse.Body);
                List<SelectListItem> parentOptions = new List<SelectListItem>();

                foreach (var item in parentData)
                {
                    var parent = JsonConvert.DeserializeObject<Parent>(((JProperty)item).Value.ToString());
                    parentOptions.Add(new SelectListItem { Value = parent.ParentId, Text = parent.Name });
                }

                ViewBag.ParentId = parentOptions;
            }
        }
    }
}
