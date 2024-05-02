using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Monitoring.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Monitoring.Controllers
{
    public class DoctorsController : Controller
    {

        private readonly IFirebaseConfig _config;
        private readonly IFirebaseClient _client;

        public DoctorsController()
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
         
            FirebaseResponse response = _client.Get("Doctors");
            if (response.Body != "null") // Check if response contains data
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                var list = new List<Doctor>();
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Doctor>(((JProperty)item).Value.ToString()));
                }
                return View(list);
            }
            else
            {
                // Handle case where there is no data
                return View(new List<Doctor>());
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Doctor doctor)
        {
            try
            {
                AddDoctorToFirebase(doctor);
                ModelState.AddModelError(string.Empty, "Added successfully!");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

            }

            return RedirectToAction("Index");
        }
        private void AddDoctorToFirebase(Doctor doctor)
        {
          
            var data = doctor;
            PushResponse response = _client.Push("Doctors/", data);
            data.DocId = response.Result.name;
            SetResponse setResponse = _client.Set("Doctors/" + data.DocId, data);

        }
        [HttpGet]
        public ActionResult Details(string id)
        {
            FirebaseResponse response = _client.Get("Doctors/" + id);
            Doctor doctor= JsonConvert.DeserializeObject<Doctor>(response.Body);


            return View(doctor);
        }
    
        [HttpGet]
        public ActionResult Edit(string id)
        {
         
            FirebaseResponse response = _client.Get("Doctors/" + id);
            Doctor data = JsonConvert.DeserializeObject<Doctor>(response.Body);

            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(Doctor doctor)
        {
           
            SetResponse response = _client.Set("Doctors/" + doctor.DocId, doctor);
            return RedirectToAction("Index");
        }

        // GET: Child/Delete/5
        public IActionResult Delete(string id)
        {
            FirebaseResponse response = _client.Get("Doctors/" + id);
            Doctor doctor= JsonConvert.DeserializeObject<Doctor>(response.Body);
            return View(doctor);
        }

        // POST: Child/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            FirebaseResponse response = _client.Delete("Doctors/" + id);
            TempData["SuccessMessage"] = "Doctor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
