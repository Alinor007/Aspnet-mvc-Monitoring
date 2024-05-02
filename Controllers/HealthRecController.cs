using System;
using System.Collections.Generic;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Monitoring.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Monitoring.Controllers
{
    public class HealthRecController : Controller
    {
        private readonly IFirebaseConfig _config;
        private readonly IFirebaseClient _client;

        public HealthRecController()
        {
            _config = new FirebaseConfig
            {
                AuthSecret = "xd3KJAJ273yOWuvxQVMNDToMysjVG3RecrtxVLvT",
                BasePath = "https://health-monitoring-5c665-default-rtdb.asia-southeast1.firebasedatabase.app/",
            };
            _client = new FireSharp.FirebaseClient(_config);
        }

        // Action to list all health records
        public IActionResult Index()
        {
            var healthRecList = new List<HealthRec>();

            FirebaseResponse response = _client.Get("HealthRecs");
            if (response.Body != "null")
            {
                dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                foreach (var item in data)
                {
                    var healthRec = JsonConvert.DeserializeObject<HealthRec>(((JProperty)item).Value.ToString());

                    FirebaseResponse ChildResponse = _client.Get($"Children/{healthRec.ChildId}");
                    if (ChildResponse.Body != "null")
                    {
                        var ChildData = JsonConvert.DeserializeObject<Child>(ChildResponse.Body);
                        healthRec.ChildName = ChildData.Name;

                    }
                    FirebaseResponse DocResponse = _client.Get($"Doctors/{healthRec.DocId}");
                    if (DocResponse.Body != "null")
                    {
                        var DocData = JsonConvert.DeserializeObject<Doctor>(DocResponse.Body);
                        healthRec.DocName = DocData.Name;

                    }


                    healthRecList.Add(healthRec);

                }
            }

            return View(healthRecList);
        }

        // Action to display the form for creating a new health record
        [HttpGet]
        public ActionResult Create()
        {
            SetDoctorOptionsInViewBag();
            SetChildOptionsInViewBag();
            return View();
        }

        // Action to handle the submission of the form for creating a new health record
        [HttpPost]
        public ActionResult Create(HealthRec healthRec)
        {
            try
            {
                CalculateBMI(healthRec);
                AddHealthRecToFirebase(healthRec);
                TempData["SuccessMessage"] = "Health record added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            SetDoctorOptionsInViewBag();
            SetChildOptionsInViewBag();
            return RedirectToAction("Index");
        }

        // Action to display the details of a specific health record
        public ActionResult Details(string id)
        {
            FirebaseResponse response = _client.Get("HealthRecs/" + id);
            HealthRec healthRec = JsonConvert.DeserializeObject<HealthRec>(response.Body);

            return View(healthRec);
        }

        // Action to display the form for editing a health record
        [HttpGet]
        public ActionResult Edit(string id)
        {
            FirebaseResponse response = _client.Get("HealthRecs/" + id);
            HealthRec healthRec = JsonConvert.DeserializeObject<HealthRec>(response.Body);
            SetChildOptionsInViewBag();
            SetDoctorOptionsInViewBag();
            return View(healthRec);
        }

        // Action to handle the submission of the form for editing a health record
        [HttpPost]
        public ActionResult Edit(HealthRec healthRec)
        {
            try
            {
                CalculateBMI(healthRec); // Calculate BMI before updating in Firebase
                UpdateHealthRecInFirebase(healthRec);
                TempData["SuccessMessage"] = "Health record updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private void UpdateHealthRecInFirebase(HealthRec healthRec)
        {
            var data = healthRec;
            SetResponse response = _client.Set("HealthRecs/" + data.RecordId, data);
        }


        // GET: HealthRec/Delete/5
        public IActionResult Delete(string id)
        {
            FirebaseResponse response = _client.Get("HealthRecs/" + id);
            HealthRec healthRec = JsonConvert.DeserializeObject<HealthRec>(response.Body);
            return View(healthRec);
        }

        // POST: HealthRec/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            FirebaseResponse response = _client.Delete("HealthRecs/" + id);
            TempData["SuccessMessage"] = "Health record deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Method to add a health record to Firebase
        private void AddHealthRecToFirebase(HealthRec healthRec)
        {
            var data = healthRec;
            PushResponse response = _client.Push("HealthRecs/", data);
            data.RecordId = response.Result.name;
            SetResponse setResponse = _client.Set("HealthRecs/" + data.RecordId, data);
        }

        // Method to set doctor options in ViewBag
        private void SetDoctorOptionsInViewBag()
        {
            FirebaseResponse Docresponse = _client.Get("Doctors");
            if (Docresponse.Body != "null")
            {
                dynamic Docdata = JsonConvert.DeserializeObject<dynamic>(Docresponse.Body);
                List<SelectListItem> doctorsOption = new List<SelectListItem>();
                foreach (var item in Docdata)
                {
                    var doctor = JsonConvert.DeserializeObject<Doctor>(((JProperty)item).Value.ToString());
                    doctorsOption.Add(new SelectListItem { Value = doctor.DocId, Text = doctor.Name });
                }
                ViewBag.DocId = doctorsOption;
            }
        }

        // Method to set child options in ViewBag
        private void SetChildOptionsInViewBag()
        {
            FirebaseResponse Childresponse = _client.Get("Children");
            if (Childresponse.Body != "null")
            {
                dynamic ChildData = JsonConvert.DeserializeObject<dynamic>(Childresponse.Body);
                List<SelectListItem> ChildOption = new List<SelectListItem>();
                foreach (var item in ChildData)
                {
                    var chid = JsonConvert.DeserializeObject<Child>(((JProperty)item).Value.ToString());
                    ChildOption.Add(new SelectListItem { Value = chid.ChildId, Text = chid.Name });
                }
                ViewBag.ChildId = ChildOption;
            }
        }
        private void CalculateBMI(HealthRec healthRec)
        {
            // Calculate BMI using the formula: BMI = weight(kg) / height(m)^2
            // Convert height from centimeters to meters
            double heightInMeters = healthRec.Height / 100.0;
            // Calculate BMI
            double bmi = healthRec.Weight / (heightInMeters * heightInMeters);
            // Set calculated BMI in the HealthRec object
            bmi = Math.Round(bmi, 2);
            healthRec.BMI = bmi;
        }
    }
}
