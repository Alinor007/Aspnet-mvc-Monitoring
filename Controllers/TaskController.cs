//using Microsoft.AspNetCore.Mvc;
//using Monitoring.Models;

//namespace Monitoring.Controllers
//{
//    public class TaskController : Controller
//    {
//        private readonly TaskDBContext _dbContext;

//        public TaskController(TaskDBContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public IActionResult Index()
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            ViewData["userId"] = userId;

//            var tasks = _dbContext.Task.Where(t => t.UserID == userId && !t.IsCompleted).ToList();

//            return View(tasks);
//        }


//        [HttpGet]
//        public IActionResult Create()
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            ViewData["userId"] = userId;

//            return View();
//        }

//        [HttpPost]
//        public IActionResult Create(TaskModel task)
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            if (userId == null)
//            {
//                return RedirectToAction("Create");
//            }

//            task.UserID = userId.Value;
//            task.IsCompleted = false;

//            _dbContext.Task.Add(task);
//            _dbContext.SaveChanges();

//            return RedirectToAction("Index", new { userId = task.UserID });
//        }



//        [HttpGet]
//        public IActionResult Edit(int taskId)
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            var task = _dbContext.Task.Find(taskId);
//            if (task == null)
//            {
//                return NotFound();
//            }
//            return View(task);
//        }

//        [HttpPost]
//        public IActionResult Edit(TaskModel task)
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            var existingTask = _dbContext.Task.FirstOrDefault(t => t.TaskID == task.TaskID && t.UserID == userId);

//            if (existingTask == null)
//            {
//                return RedirectToAction("Error");
//            }

//            existingTask.TaskName = task.TaskName;
//            existingTask.TaskDescription = task.TaskDescription;
//            existingTask.IsCompleted = task.IsCompleted;

//            _dbContext.SaveChanges();

//            return RedirectToAction("Index", new { userId = userId });
//        }


//        [HttpGet]
//        public IActionResult Delete(int? taskId)
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            var task = _dbContext.Task.FirstOrDefault(t => t.TaskID == taskId && t.UserID == userId);

//            if (task == null)
//            {
//                return RedirectToAction("Error");
//            }

//            ViewData["userId"] = userId;
//            return View(task);
//        }

//        [HttpPost, ActionName("Delete")]
//        public IActionResult DeleteConfirmed(int taskId)
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            var task = _dbContext.Task.FirstOrDefault(t => t.TaskID == taskId && t.UserID == userId);

//            if (task == null)
//            {
//                return RedirectToAction("Error");
//            }

//            _dbContext.Task.Remove(task);
//            _dbContext.SaveChanges();

//            return RedirectToAction("Index", new { userId = userId });
//        }

//        public IActionResult Completed()
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            ViewData["userId"] = userId;


//            var completedTasks = _dbContext.Task.Where(t => t.UserID == userId && t.IsCompleted).ToList();

//            return View(completedTasks);
//        }

//        [HttpPost]
//        public IActionResult SetComplete(int taskId)
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            var task = _dbContext.Task.FirstOrDefault(t => t.TaskID == taskId && t.UserID == userId);

//            if (task == null)
//            {
//                return RedirectToAction("Error");
//            }

//            task.IsCompleted = true;

//            _dbContext.SaveChanges();

//            return RedirectToAction("Index", new { userId = userId });
//        }


//        [HttpPost]
//        public IActionResult SetInComplete(int taskId)
//        {
//            int? userId = HttpContext.Session.GetInt32("UserID");

//            if (!userId.HasValue)
//            {
//                return RedirectToAction("Login", "Login");
//            }

//            var task = _dbContext.Task.FirstOrDefault(t => t.TaskID == taskId && t.UserID == userId);

//            if (task == null)
//            {
//                return RedirectToAction("Error");
//            }

//            task.IsCompleted = false;

//            _dbContext.SaveChanges();

//            return RedirectToAction("Index", new { userId = userId });
//        }

//    }
//}
