using HirePoint.Data;
using HirePoint.Helpers;
using HirePoint.Models;
using HirePoint.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HirePoint.Controllers
{
    public class JobseekerController : Controller
    {
        private readonly HirePointDbContext db;

        public JobseekerController(HirePointDbContext context)
        {
            db = context;
        }


        // =========================================================
        // GET USER ID
        // =========================================================

        private int GetUserID()
        {
            return SessionHelper.GetUserID(
                HttpContext.Session) ?? 0;
        }


        // =========================================================
        // CHECK JOBSEEKER
        // =========================================================

        private bool IsJobseeker()
        {
            int userID = GetUserID();

            string? userType =
                SessionHelper.GetUserType(
                    HttpContext.Session);

            return userID > 0 &&
                   !string.IsNullOrWhiteSpace(userType) &&
                   userType.Equals(
                       "Jobseeker",
                       StringComparison.OrdinalIgnoreCase);
        }


        // =========================================================
        // GET JOBSEEKER QUALIFICATION
        // =========================================================

        private string? GetJobseekerQualification()
        {
            int userID = GetUserID();

            var profile =
                db.JobseekerProfiles
                .AsNoTracking()
                .FirstOrDefault(x =>
                    x.UserID == userID);

            return profile?.Qualification?.Trim();
        }


        // =========================================================
        // GET RECOMMENDED JOBS
        // =========================================================

        private List<Job> GetRecommendedJobs(int userID)
        {
            var interestIds =
                db.JobseekerInterests
                .AsNoTracking()
                .Where(x =>
                    x.JobseekerId == userID)
                .Select(x =>
                    x.JobInterestId)
                .ToList();

            if (!interestIds.Any())
            {
                return new List<Job>();
            }


            // -----------------------------------------------------
            // GET INTEREST NAMES
            // -----------------------------------------------------

            var interestNames =
                db.JobInterests
                .AsNoTracking()
                .Where(x =>
                    interestIds.Contains(x.Id))
                .Select(x =>
                    x.Name)
                .ToList();

            if (!interestNames.Any())
            {
                return new List<Job>();
            }


            // -----------------------------------------------------
            // GET ACTIVE JOBS
            // -----------------------------------------------------

            var jobs =
                db.Jobs
                .AsNoTracking()
                .Include(x => x.Recruiter)
                    .ThenInclude(x =>
                        x.RecruiterProfile)
                .Include(x => x.Qualification)
                .Where(x =>
                    x.JobStatus == "Active" &&
                    x.ClosingDate.Date >= DateTime.Today)
                .ToList();


            // -----------------------------------------------------
            // SCORE JOBS
            // -----------------------------------------------------

            var recommendedJobs =
                jobs
                .Select(job =>
                {
                    int score = 0;

                    foreach (var interest in interestNames)
                    {
                        if (string.IsNullOrWhiteSpace(interest))
                        {
                            continue;
                        }


                        // JOB TITLE MATCH

                        if (!string.IsNullOrWhiteSpace(
                            job.JobTitle) &&
                            job.JobTitle.Contains(
                                interest,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            score += 50;
                        }


                        // JOB DESCRIPTION MATCH

                        if (!string.IsNullOrWhiteSpace(
                            job.JobDescription) &&
                            job.JobDescription.Contains(
                                interest,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            score += 20;
                        }


                        // REQUIREMENTS MATCH

                        if (!string.IsNullOrWhiteSpace(
                            job.Requirements) &&
                            job.Requirements.Contains(
                                interest,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            score += 15;
                        }


                        // SKILLS MATCH

                        if (!string.IsNullOrWhiteSpace(
                            job.Skills) &&
                            job.Skills.Contains(
                                interest,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            score += 15;
                        }
                    }

                    return new
                    {
                        Job = job,
                        Score = score
                    };
                })
                .Where(x =>
                    x.Score > 0)
                .OrderByDescending(x =>
                    x.Score)
                .ThenByDescending(x =>
                    x.Job.DatePosted)
                .Take(5)
                .Select(x =>
                    x.Job)
                .ToList();

            return recommendedJobs;
        }


        // =========================================================
        // DASHBOARD
        // =========================================================

        [HttpGet]
        public IActionResult Dashboard()
        {
            // -----------------------------------------------------
            // CHECK LOGIN
            // -----------------------------------------------------

            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();


            // -----------------------------------------------------
            // GET USER
            // -----------------------------------------------------

            var user =
                db.Users
                .FirstOrDefault(x =>
                    x.UserID == userID);

            if (user == null)
            {
                SessionHelper.ClearSession(
                    HttpContext.Session);

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            // =====================================================
            // CHECK INTERESTS FIRST
            // =====================================================

            bool hasInterests =
                db.JobseekerInterests
                .Any(x =>
                    x.JobseekerId == userID);

            if (!hasInterests)
            {
                return RedirectToAction(
                    "Interests",
                    "Jobseeker");
            }


            // =====================================================
            // DASHBOARD USER NAME
            // =====================================================

            ViewBag.UserName =
                user.FullName;


            // =====================================================
            // APPLICATIONS
            // =====================================================

            ViewBag.Applications =
                db.Applications.Count(x =>
                    x.JobseekerID == userID);


            // =====================================================
            // GET JOBSEEKER QUALIFICATION
            // =====================================================

            string? qualification =
                GetJobseekerQualification();


            // =====================================================
            // AVAILABLE JOBS
            // =====================================================

            if (!string.IsNullOrWhiteSpace(
                qualification))
            {
                string qualificationSearch =
                    qualification
                        .Trim()
                        .ToLower();

                ViewBag.AvailableJobs =
                    db.Jobs
                    .Include(x => x.Qualification)
                    .Count(x =>
                        x.JobStatus == "Active" &&
                        x.ClosingDate.Date >= DateTime.Today &&
                        x.Qualification != null &&
                        x.Qualification.Name != null &&
                        (
                            x.Qualification.Name
                                .ToLower()
                                .Contains(
                                    qualificationSearch)

                            ||

                            qualificationSearch
                                .Contains(
                                    x.Qualification.Name
                                        .ToLower())
                        ));
            }
            else
            {
                ViewBag.AvailableJobs = 0;
            }


            // =====================================================
            // UNREAD NOTIFICATIONS
            // =====================================================

            ViewBag.UnreadNotifications =
                db.Notifications.Count(x =>
                    x.UserID == userID &&
                    !x.IsRead);


            // =====================================================
            // RECENT JOBS
            // =====================================================

            if (!string.IsNullOrWhiteSpace(
                qualification))
            {
                string qualificationSearch =
                    qualification
                        .Trim()
                        .ToLower();

                ViewBag.RecentJobs =
                    db.Jobs
                    .Include(x => x.Recruiter)
                        .ThenInclude(x =>
                            x.RecruiterProfile)
                    .Include(x => x.Qualification)
                    .Where(x =>
                        x.JobStatus == "Active" &&
                        x.ClosingDate.Date >= DateTime.Today &&
                        x.Qualification != null &&
                        x.Qualification.Name != null &&
                        (
                            x.Qualification.Name
                                .ToLower()
                                .Contains(
                                    qualificationSearch)

                            ||

                            qualificationSearch
                                .Contains(
                                    x.Qualification.Name
                                        .ToLower())
                        ))
                    .OrderByDescending(x =>
                        x.DatePosted)
                    .Take(5)
                    .ToList();
            }
            else
            {
                ViewBag.RecentJobs =
                    new List<Job>();
            }


            // =====================================================
            // RECOMMENDED JOBS
            // =====================================================

            ViewBag.RecommendedJobs =
                GetRecommendedJobs(userID);


            // =====================================================
            // START FULL TOUR AFTER SUCCESSFUL INTEREST SAVE
            // =====================================================
            //
            // This flag is created ONLY after Interests POST
            // successfully saves to the database.
            //
            // It is consumed once here and immediately removed.
            //

            bool startFullTour =
                HttpContext.Session.GetString(
                    "HirePointStartFullTour") == "true";

            if (startFullTour)
            {
                HttpContext.Session.Remove(
                    "HirePointStartFullTour");

                ViewBag.StartFullTour = true;
            }
            else
            {
                ViewBag.StartFullTour = false;
            }


            // =====================================================
            // INTERESTS EXIST
            // =====================================================

            ViewBag.HasInterests = true;


            return View();
        }


        // =========================================================
        // SELECT INTERESTS - GET
        // =========================================================

        [HttpGet]
        public IActionResult Interests()
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();


            // =====================================================
            // GET CURRENTLY SELECTED INTERESTS
            // =====================================================

            var selectedIds =
                db.JobseekerInterests
                .AsNoTracking()
                .Where(x =>
                    x.JobseekerId == userID)
                .Select(x =>
                    x.JobInterestId)
                .ToList();


            // =====================================================
            // GET ALL INTERESTS
            // =====================================================

            var interests =
                db.JobInterests
                .AsNoTracking()
                .OrderBy(x =>
                    x.Name)
                .Select(x =>
                    new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value =
                            x.Id.ToString(),

                        Text =
                            x.Name,

                        Selected =
                            selectedIds.Contains(
                                x.Id)
                    })
                .ToList();


            var model =
                new JobInterestViewModel
                {
                    SelectedInterestIds =
                        selectedIds,

                    Interests =
                        interests
                };


            return View(model);
        }


        // =========================================================
        // SELECT INTERESTS - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Interests(
            JobInterestViewModel model)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();


            // =====================================================
            // MAKE SURE LIST IS NOT NULL
            // =====================================================

            if (model.SelectedInterestIds == null)
            {
                model.SelectedInterestIds =
                    new List<int>();
            }


            // =====================================================
            // REMOVE OLD INTERESTS
            // =====================================================

            var existingInterests =
                db.JobseekerInterests
                .Where(x =>
                    x.JobseekerId == userID)
                .ToList();

            db.JobseekerInterests
                .RemoveRange(
                    existingInterests);


            // =====================================================
            // REMOVE DUPLICATES
            // =====================================================

            var selectedIds =
                model.SelectedInterestIds
                .Distinct()
                .ToList();


            // =====================================================
            // ADD NEW INTERESTS
            // =====================================================

            foreach (int interestId in selectedIds)
            {
                bool interestExists =
                    db.JobInterests.Any(x =>
                        x.Id == interestId);

                if (!interestExists)
                {
                    continue;
                }

                db.JobseekerInterests.Add(
                    new JobseekerInterest
                    {
                        JobseekerId =
                            userID,

                        JobInterestId =
                            interestId
                    });
            }


            // =====================================================
            // SAVE
            // =====================================================

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "Your job interests have been saved successfully.";


                // =================================================
                // ACTIVATE TOUR ONLY AFTER SUCCESSFUL SAVE
                // =================================================
                //
                // Do NOT put this before SaveChanges().
                //
                // The Dashboard will consume this flag and
                // immediately remove it.
                //

                HttpContext.Session.SetString(
                    "HirePointStartFullTour",
                    "true");
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message
                    ?? ex.Message;

                return RedirectToAction(
                    "Interests",
                    "Jobseeker");
            }


            // =====================================================
            // GO TO DASHBOARD
            // =====================================================

            return RedirectToAction(
                "Dashboard",
                "Jobseeker");
        }


        // =========================================================
        // SEARCH JOB TITLE AUTOCOMPLETE
        // =========================================================

        [HttpGet]
        public IActionResult SearchJobs(
            string? term)
        {
            if (!IsJobseeker())
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(
                    new List<object>());
            }

            string searchTerm =
                term.Trim();

            if (searchTerm.Length < 2)
            {
                return Json(
                    new List<object>());
            }


            // GET QUALIFICATION

            string? qualification =
                GetJobseekerQualification();

            if (string.IsNullOrWhiteSpace(
                qualification))
            {
                return Json(
                    new List<object>());
            }

            string qualificationSearch =
                qualification
                    .Trim()
                    .ToLower();


            // SEARCH JOB TITLES

            var results =
                db.Jobs
                .AsNoTracking()
                .Include(x => x.Qualification)
                .Where(x =>
                    x.JobStatus == "Active" &&
                    x.ClosingDate.Date >= DateTime.Today &&
                    x.Qualification != null &&
                    x.Qualification.Name != null &&
                    (
                        x.Qualification.Name
                            .ToLower()
                            .Contains(
                                qualificationSearch)

                        ||

                        qualificationSearch
                            .Contains(
                                x.Qualification.Name
                                    .ToLower())
                    ) &&
                    x.JobTitle.Contains(
                        searchTerm))
                .OrderBy(x =>
                    x.JobTitle)
                .ThenByDescending(x =>
                    x.DatePosted)
                .Take(10)
                .Select(x => new
                {
                    id = x.JobID,

                    title = x.JobTitle,

                    qualification =
                        x.Qualification != null
                            ? x.Qualification.Name
                            : string.Empty,

                    location =
                        x.City + ", " + x.Province
                })
                .ToList();

            return Json(results);
        }


        // =========================================================
        // SEARCH PROVINCE AUTOCOMPLETE
        // =========================================================

        [HttpGet]
        public IActionResult SearchJobProvinces(
            string? term)
        {
            if (!IsJobseeker())
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(
                    new List<object>());
            }

            string searchTerm =
                term.Trim();

            if (searchTerm.Length < 2)
            {
                return Json(
                    new List<object>());
            }


            string? qualification =
                GetJobseekerQualification();

            if (string.IsNullOrWhiteSpace(
                qualification))
            {
                return Json(
                    new List<object>());
            }

            string qualificationSearch =
                qualification
                    .Trim()
                    .ToLower();


            var results =
                db.Jobs
                .AsNoTracking()
                .Include(x => x.Qualification)
                .Where(x =>
                    x.JobStatus == "Active" &&
                    x.ClosingDate.Date >= DateTime.Today &&
                    x.Qualification != null &&
                    x.Qualification.Name != null &&
                    (
                        x.Qualification.Name
                            .ToLower()
                            .Contains(
                                qualificationSearch)

                        ||

                        qualificationSearch
                            .Contains(
                                x.Qualification.Name
                                    .ToLower())
                    ) &&
                    x.Province.Contains(
                        searchTerm))
                .Select(x =>
                    x.Province)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .Take(10)
                .ToList()
                .Select(x => new
                {
                    value = x
                })
                .ToList();

            return Json(results);
        }


        // =========================================================
        // SEARCH CITY AUTOCOMPLETE
        // =========================================================

        [HttpGet]
        public IActionResult SearchJobCities(
            string? term)
        {
            if (!IsJobseeker())
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(
                    new List<object>());
            }

            string searchTerm =
                term.Trim();

            if (searchTerm.Length < 2)
            {
                return Json(
                    new List<object>());
            }


            string? qualification =
                GetJobseekerQualification();

            if (string.IsNullOrWhiteSpace(
                qualification))
            {
                return Json(
                    new List<object>());
            }

            string qualificationSearch =
                qualification
                    .Trim()
                    .ToLower();


            var results =
                db.Jobs
                .AsNoTracking()
                .Include(x => x.Qualification)
                .Where(x =>
                    x.JobStatus == "Active" &&
                    x.ClosingDate.Date >= DateTime.Today &&
                    x.Qualification != null &&
                    x.Qualification.Name != null &&
                    (
                        x.Qualification.Name
                            .ToLower()
                            .Contains(
                                qualificationSearch)

                        ||

                        qualificationSearch
                            .Contains(
                                x.Qualification.Name
                                    .ToLower())
                    ) &&
                    x.City.Contains(
                        searchTerm))
                .Select(x =>
                    x.City)
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .Take(10)
                .ToList()
                .Select(x => new
                {
                    value = x
                })
                .ToList();

            return Json(results);
        }


        // =========================================================
        // FIND JOBS / VIEW JOBS
        // =========================================================

        [HttpGet]
        public IActionResult Jobs(
            string? title,
            string? province,
            string? city,
            string? sort,
            string? datePosted)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }


            var jobs =
                db.Jobs
                .Include(x => x.Recruiter)
                    .ThenInclude(x =>
                        x.RecruiterProfile)
                .Include(x => x.Qualification)
                .Where(x =>
                    x.JobStatus == "Active" &&
                    x.ClosingDate.Date >= DateTime.Today)
                .AsQueryable();


            // SEARCH BY JOB TITLE

            if (!string.IsNullOrWhiteSpace(title))
            {
                title =
                    title.Trim();

                jobs =
                    jobs.Where(x =>
                        x.JobTitle.Contains(title));
            }


            // SEARCH BY PROVINCE

            if (!string.IsNullOrWhiteSpace(province))
            {
                province =
                    province.Trim();

                jobs =
                    jobs.Where(x =>
                        x.Province.Contains(province));
            }


            // SEARCH BY CITY

            if (!string.IsNullOrWhiteSpace(city))
            {
                city =
                    city.Trim();

                jobs =
                    jobs.Where(x =>
                        x.City.Contains(city));
            }


            // FILTER BY DATE POSTED

            DateTime today =
                DateTime.Today;

            switch (datePosted?.ToLower())
            {
                case "today":

                    jobs =
                        jobs.Where(x =>
                            x.DatePosted.Date ==
                            today);

                    break;

                case "3days":

                    DateTime threeDaysAgo =
                        today.AddDays(-2);

                    jobs =
                        jobs.Where(x =>
                            x.DatePosted.Date >=
                            threeDaysAgo);

                    break;

                case "7days":

                    DateTime sevenDaysAgo =
                        today.AddDays(-6);

                    jobs =
                        jobs.Where(x =>
                            x.DatePosted.Date >=
                            sevenDaysAgo);

                    break;

                case "14days":

                    DateTime fourteenDaysAgo =
                        today.AddDays(-13);

                    jobs =
                        jobs.Where(x =>
                            x.DatePosted.Date >=
                            fourteenDaysAgo);

                    break;

                case "30days":

                    DateTime thirtyDaysAgo =
                        today.AddDays(-29);

                    jobs =
                        jobs.Where(x =>
                            x.DatePosted.Date >=
                            thirtyDaysAgo);

                    break;

                case "any":

                default:

                    break;
            }


            // SORT JOBS

            switch (sort?.ToLower())
            {
                case "oldest":

                    jobs =
                        jobs.OrderBy(x =>
                            x.DatePosted);

                    break;

                case "closing":

                    jobs =
                        jobs.OrderBy(x =>
                            x.ClosingDate);

                    break;

                case "newest":

                default:

                    jobs =
                        jobs.OrderByDescending(x =>
                            x.DatePosted);

                    break;
            }


            // LOAD RESULTS

            var result =
                jobs.ToList();


            // RECOMMENDED JOBS

            ViewBag.RecommendedJobs =
                GetRecommendedJobs(
                    GetUserID());


            // REMEMBER FILTER VALUES

            ViewBag.SelectedDatePosted =
                string.IsNullOrWhiteSpace(datePosted)
                    ? "any"
                    : datePosted;


            return View(result);
        }


        // =========================================================
        // APPLY FOR JOB
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(int id)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();


            // CHECK USER

            var user =
                db.Users
                .FirstOrDefault(x =>
                    x.UserID == userID);

            if (user == null)
            {
                SessionHelper.ClearSession(
                    HttpContext.Session);

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            // CHECK JOB

            var job =
                db.Jobs
                .Include(x => x.Qualification)
                .FirstOrDefault(x =>
                    x.JobID == id &&
                    x.JobStatus == "Active" &&
                    x.ClosingDate.Date >= DateTime.Today);

            if (job == null)
            {
                TempData["Error"] =
                    "This job is no longer available.";

                return RedirectToAction(
                    "Jobs");
            }


            // CHECK DUPLICATE APPLICATION

            var alreadyApplied =
                db.Applications.Any(a =>
                    a.JobID == id &&
                    a.JobseekerID == userID);

            if (alreadyApplied)
            {
                TempData["AlreadyApplied"] =
                    "You have already applied for this job.";

                return RedirectToAction(
                    "JobDetails",
                    new { id = id });
            }


            // CREATE APPLICATION

            var application =
                new JobApplication
                {
                    JobID =
                        id,

                    JobseekerID =
                        userID,

                    ApplicationStatus =
                        "Pending",

                    ApplicationDate =
                        DateTime.Now
                };

            db.Applications.Add(
                application);


            // SAVE APPLICATION

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "Application submitted successfully.";

                return RedirectToAction(
                    "JobDetails",
                    new { id = id });
            }
            catch (DbUpdateException ex)
            {
                var error =
                    ex.InnerException?.Message
                    ?? ex.Message;

                TempData["Error"] =
                    "Unable to submit application: "
                    + error;

                return RedirectToAction(
                    "Jobs");
            }
        }


        // =========================================================
        // APPLICATIONS
        // =========================================================

        [HttpGet]
        public IActionResult Applications()
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();

            var applications =
                db.Applications
                .Include(x => x.Job)
                    .ThenInclude(x => x.Recruiter)
                        .ThenInclude(x =>
                            x.RecruiterProfile)
                .Where(x =>
                    x.JobseekerID == userID)
                .OrderByDescending(x =>
                    x.ApplicationDate)
                .ToList();

            return View(applications);
        }


        // =========================================================
        // NOTIFICATIONS
        // =========================================================

        [HttpGet]
        public IActionResult Notifications()
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();

            var notifications =
                db.Notifications
                .Where(x =>
                    x.UserID == userID)
                .OrderByDescending(x =>
                    x.DateCreated)
                .ToList();

            ViewBag.UnreadNotifications =
                notifications.Count(x =>
                    !x.IsRead);

            return View(notifications);
        }


        // =========================================================
        // NOTIFICATION DETAILS
        // =========================================================

        [HttpGet]
        public IActionResult NotificationDetails(int id)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();


            var notification =
                db.Notifications
                .FirstOrDefault(x =>
                    x.NotificationID == id &&
                    x.UserID == userID);


            if (notification == null)
            {
                TempData["Error"] =
                    "Notification not found.";

                return RedirectToAction(
                    "Notifications");
            }


            if (!notification.IsRead)
            {
                notification.IsRead = true;

                db.SaveChanges();
            }


            return View(notification);
        }


        // =========================================================
        // MARK NOTIFICATION AS READ
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkNotificationAsRead(
            int id)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();

            var notification =
                db.Notifications
                .FirstOrDefault(x =>
                    x.NotificationID == id &&
                    x.UserID == userID);

            if (notification == null)
            {
                TempData["Error"] =
                    "Notification not found.";

                return RedirectToAction(
                    "Notifications");
            }

            notification.IsRead =
                true;

            db.SaveChanges();

            return RedirectToAction(
                "Notifications");
        }


        // =========================================================
        // MARK ALL NOTIFICATIONS READ
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAllNotificationsAsRead()
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();

            var notifications =
                db.Notifications
                .Where(x =>
                    x.UserID == userID &&
                    !x.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead =
                    true;
            }

            db.SaveChanges();

            TempData["Success"] =
                "All notifications marked as read.";

            return RedirectToAction(
                "Notifications");
        }


        // =========================================================
        // PROFILE GET
        // =========================================================

        [HttpGet]
        public IActionResult Profile()
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();

            var user =
                db.Users
                .Include(x => x.Province)
                .Include(x => x.JobseekerProfile)
                .FirstOrDefault(x =>
                    x.UserID == userID);

            if (user == null)
            {
                SessionHelper.ClearSession(
                    HttpContext.Session);

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            ViewBag.Provinces =
                db.Provinces
                .OrderBy(x =>
                    x.ProvinceName)
                .ToList();

            return View(user);
        }


        // =========================================================
        // PROFILE POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User model)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();


            // LOAD REAL USER

            var user =
                db.Users
                .Include(x => x.JobseekerProfile)
                .FirstOrDefault(x =>
                    x.UserID == userID);

            if (user == null)
            {
                SessionHelper.ClearSession(
                    HttpContext.Session);

                return RedirectToAction(
                    "Login",
                    "Account");
            }


            // UPDATE PERSONAL INFORMATION

            if (!string.IsNullOrWhiteSpace(
                model.FullName))
            {
                user.FullName =
                    model.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(
                model.Email))
            {
                string newEmail =
                    model.Email
                        .Trim()
                        .ToLowerInvariant();

                bool emailAlreadyUsed =
                    db.Users.Any(x =>
                        x.UserID != userID &&
                        x.Email.ToLower() ==
                        newEmail);

                if (emailAlreadyUsed)
                {
                    TempData["Error"] =
                        "That email address is already being used by another account.";

                    return RedirectToAction(
                        "Profile");
                }

                user.Email =
                    newEmail;
            }


            // PHONE NUMBER

            user.PhoneNumber =
                model.PhoneNumber?.Trim()
                ?? string.Empty;


            // GENDER

            user.Gender =
                string.IsNullOrWhiteSpace(
                    model.Gender)
                    ? string.Empty
                    : model.Gender.Trim();


            // UPDATE PROVINCE

            if (model.ProvinceID.HasValue)
            {
                bool provinceExists =
                    db.Provinces.Any(x =>
                        x.ProvinceID ==
                        model.ProvinceID.Value);

                if (!provinceExists)
                {
                    TempData["Error"] =
                        "The selected province is invalid.";

                    return RedirectToAction(
                        "Profile");
                }

                user.ProvinceID =
                    model.ProvinceID.Value;
            }
            else
            {
                user.ProvinceID =
                    null;
            }


            // UPDATE JOBSEEKER PROFILE

            var profile =
                user.JobseekerProfile;

            if (profile == null)
            {
                TempData["Error"] =
                    "Your jobseeker profile could not be found.";

                return RedirectToAction(
                    "Profile");
            }


            // UPDATE QUALIFICATION

            string? qualification =
                model.JobseekerProfile?
                    .Qualification;

            if (!string.IsNullOrWhiteSpace(
                qualification))
            {
                profile.Qualification =
                    qualification.Trim();
            }


            // SAVE

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "Profile updated successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message
                    ?? ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(
                "Profile");
        }


        // =========================================================
        // JOB DETAILS
        // =========================================================

        [HttpGet]
        public IActionResult JobDetails(int id)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var job =
                db.Jobs
                .AsNoTracking()
                .Include(x => x.Recruiter)
                    .ThenInclude(x =>
                        x.RecruiterProfile)
                .Include(x => x.Qualification)
                .FirstOrDefault(x =>
                    x.JobID == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }


        // =========================================================
        // UPLOAD CV
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UploadCV(
            IFormFile cv)
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (cv == null ||
                cv.Length == 0)
            {
                TempData["Error"] =
                    "Please select a CV file.";

                return RedirectToAction(
                    "Profile");
            }

            int userID =
                GetUserID();

            string extension =
                Path.GetExtension(
                    cv.FileName)
                .ToLowerInvariant();

            if (extension != ".pdf" &&
                extension != ".doc" &&
                extension != ".docx")
            {
                TempData["Error"] =
                    "Only PDF, DOC, and DOCX files are allowed.";

                return RedirectToAction(
                    "Profile");
            }

            string fileName =
                Guid.NewGuid()
                .ToString()
                + extension;

            string folder =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "cv");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(
                    folder);
            }

            string path =
                Path.Combine(
                    folder,
                    fileName);

            try
            {
                using (FileStream stream =
                    new FileStream(
                        path,
                        FileMode.Create))
                {
                    cv.CopyTo(stream);
                }

                var profile =
                    db.JobseekerProfiles
                    .FirstOrDefault(x =>
                        x.UserID == userID);

                if (profile == null)
                {
                    TempData["Error"] =
                        "Jobseeker profile not found.";

                    return RedirectToAction(
                        "Profile");
                }

                profile.CVFile =
                    fileName;

                db.SaveChanges();

                TempData["Success"] =
                    "CV uploaded successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] =
                    ex.InnerException?.Message
                    ?? ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    ex.Message;
            }

            return RedirectToAction(
                "Profile");
        }


        // =========================================================
        // VIEW CV
        // =========================================================

        [HttpGet]
        public IActionResult ViewCV()
        {
            if (!IsJobseeker())
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int userID =
                GetUserID();


            // GET PROFILE

            var profile =
                db.JobseekerProfiles
                .FirstOrDefault(x =>
                    x.UserID == userID);


            // CHECK PROFILE

            if (profile == null ||
                string.IsNullOrWhiteSpace(
                    profile.CVFile))
            {
                TempData["Error"] =
                    "No CV has been uploaded.";

                return RedirectToAction(
                    "Profile");
            }


            // GET FILE PATH

            string filePath =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads",
                    "cv",
                    profile.CVFile);


            // CHECK FILE EXISTS

            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] =
                    "Your CV file could not be found.";

                return RedirectToAction(
                    "Profile");
            }


            // GET EXTENSION

            string extension =
                Path.GetExtension(
                    profile.CVFile)
                .ToLowerInvariant();


            // PDF

            if (extension == ".pdf")
            {
                return PhysicalFile(
                    filePath,
                    "application/pdf");
            }


            // WORD DOCUMENT

            if (extension == ".doc")
            {
                return PhysicalFile(
                    filePath,
                    "application/msword");
            }

            if (extension == ".docx")
            {
                return PhysicalFile(
                    filePath,
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            }


            // UNSUPPORTED FILE

            TempData["Error"] =
                "The uploaded CV format is not supported.";

            return RedirectToAction(
                "Profile");
        }
    }
}