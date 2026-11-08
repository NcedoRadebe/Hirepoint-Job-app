using HirePoint.Data;
using HirePoint.Models;
using HirePoint.Services;
using HirePoint.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HirePoint.Controllers
{
    public class AdminController : Controller
    {
        private readonly HirePointDbContext db;
        private readonly IEmailService emailService;
        private readonly ILogger<AdminController> logger;

        // =========================================================
        // PROTECTED SYSTEM ADMINISTRATOR
        // =========================================================

        private const int SYSTEM_ADMIN_ID = 11;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public AdminController(
            HirePointDbContext context,
            IEmailService emailService,
            ILogger<AdminController> logger)
        {
            db = context;
            this.emailService = emailService;
            this.logger = logger;
        }

        // =========================================================
        // GET CURRENT USER ID
        // =========================================================

        private int GetUserID()
        {
            return HttpContext.Session.GetInt32("UserID") ?? 0;
        }


        // =========================================================
        // GET CURRENT USER
        // =========================================================

        private User? GetCurrentUser()
        {
            int userID = GetUserID();

            if (userID <= 0)
            {
                return null;
            }

            return db.Users
                .FirstOrDefault(x => x.UserID == userID);
        }


        // =========================================================
        // LOAD CREATE ADMIN DATA
        // =========================================================

        private void LoadCreateAdminData()
        {
            ViewBag.Provinces =
                db.Provinces
                    .OrderBy(x => x.ProvinceName)
                    .ToList();
        }


        // =========================================================
        // CHECK SYSTEM ADMINISTRATOR
        // =========================================================

        private bool IsSystemAdministrator(User user)
        {
            return user.UserID == SYSTEM_ADMIN_ID ||
                   string.Equals(
                       user.UserType?.Trim(),
                       "System Administrator",
                       StringComparison.OrdinalIgnoreCase);
        }


        // =========================================================
        // CHECK ADMIN ACCESS
        // =========================================================

        private IActionResult? CheckAdminAccess()
        {
            var user = GetCurrentUser();

            if (user == null)
            {
                HttpContext.Session.Clear();

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // -----------------------------------------------------
            // INACTIVE ACCOUNT
            // -----------------------------------------------------

            if (string.Equals(
                user.Status?.Trim(),
                "Inactive",
                StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Session.Clear();

                TempData["Error"] =
                    "Your account is inactive.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // -----------------------------------------------------
            // SYSTEM ADMINISTRATOR
            // -----------------------------------------------------

            if (IsSystemAdministrator(user))
            {
                return null;
            }

            // -----------------------------------------------------
            // COMPANY ADMIN
            // -----------------------------------------------------

            bool isCompanyAdmin =
                string.Equals(
                    user.UserType?.Trim(),
                    "Admin",
                    StringComparison.OrdinalIgnoreCase);

            if (!isCompanyAdmin)
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            // -----------------------------------------------------
            // COMPANY ADMIN MUST BELONG TO COMPANY
            // -----------------------------------------------------

            if (!user.CompanyID.HasValue)
            {
                TempData["Error"] =
                    "Your administrator account is not assigned to a company.";

                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            return null;
        }


        // =========================================================
        // DASHBOARD
        // =========================================================

        [HttpGet]
        public IActionResult Dashboard()
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            bool isSystemAdmin =
                IsSystemAdministrator(currentUser);

            // =====================================================
            // SYSTEM ADMINISTRATOR
            // =====================================================

            if (isSystemAdmin)
            {
                ViewBag.TotalUsers =
                    db.Users.Count();

                ViewBag.ActiveJobs =
                    db.Jobs.Count(x =>
                        x.JobStatus == "Active");

                ViewBag.Applications =
                    db.Applications.Count();

                ViewBag.Jobseekers =
                    db.Users.Count(x =>
                        x.UserType != null &&
                        x.UserType.ToLower() == "jobseeker");

                ViewBag.Recruiters =
                    db.Users.Count(x =>
                        x.UserType != null &&
                        x.UserType.ToLower() == "recruiter");

                ViewBag.RecentUsers =
                    db.Users
                        .OrderByDescending(x => x.DateCreated)
                        .Take(5)
                        .ToList();

                ViewBag.RecentJobs =
                    db.Jobs
                        .Include(x => x.Company)
                        .Include(x => x.Recruiter)
                        .OrderByDescending(x => x.DatePosted)
                        .Take(5)
                        .ToList();

                return View();
            }

            // =====================================================
            // COMPANY ADMIN
            // =====================================================

            int companyID =
                currentUser.CompanyID!.Value;

            ViewBag.TotalUsers =
                db.Users.Count(x =>
                    x.CompanyID == companyID);

            ViewBag.ActiveJobs =
                db.Jobs.Count(x =>
                    x.CompanyID == companyID &&
                    x.JobStatus == "Active");

            ViewBag.Applications =
                db.Applications.Count(x =>
                    db.Jobs.Any(j =>
                        j.JobID == x.JobID &&
                        j.CompanyID == companyID));

            // Jobseekers are global users.
            ViewBag.Jobseekers = 0;

            ViewBag.Recruiters =
                db.Users.Count(x =>
                    x.CompanyID == companyID &&
                    x.UserType != null &&
                    x.UserType.ToLower() == "recruiter");

            ViewBag.RecentUsers =
                db.Users
                    .Where(x =>
                        x.CompanyID == companyID)
                    .OrderByDescending(x => x.DateCreated)
                    .Take(5)
                    .ToList();

            ViewBag.RecentJobs =
                db.Jobs
                    .Include(x => x.Company)
                    .Include(x => x.Recruiter)
                    .Where(x =>
                        x.CompanyID == companyID)
                    .OrderByDescending(x => x.DatePosted)
                    .Take(5)
                    .ToList();

            return View();
        }


        // =========================================================
        // MANAGE USERS
        // =========================================================

        [HttpGet]
        public IActionResult ManageUsers()
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            IQueryable<User> query =
                db.Users
                    .Include(x => x.Province)
                    .Include(x => x.Company);

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                int companyID =
                    currentUser.CompanyID!.Value;

                query =
                    query.Where(x =>
                        x.CompanyID == companyID);
            }

            var users =
                query
                    .OrderByDescending(x => x.DateCreated)
                    .ToList();

            return View(users);
        }


        // =========================================================
        // EDIT USER - GET
        // =========================================================
        //
        // The administrator can view the user's information.
        // The information itself is NOT editable.
        //
        // Only Status is allowed to be changed.
        // =========================================================

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            // -----------------------------------------------------
            // PROTECT SYSTEM ADMINISTRATOR
            // -----------------------------------------------------

            if (id == SYSTEM_ADMIN_ID)
            {
                TempData["Error"] =
                    "The System Administrator account is protected and cannot be edited.";

                return RedirectToAction(
                    "ManageUsers");
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // -----------------------------------------------------
            // GET USER
            // -----------------------------------------------------

            var user =
                db.Users
                    .Include(x => x.Company)
                    .Include(x => x.Province)
                    .FirstOrDefault(x =>
                        x.UserID == id);

            if (user == null)
            {
                return NotFound();
            }

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    user.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            return View(user);
        }


        // =========================================================
        // EDIT USER - POST
        // =========================================================
        //
        // ADMIN CAN ONLY CHANGE USER STATUS.
        //
        // Full name, email, phone number, gender, user type,
        // company, province, etc. are NOT changed here.
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditUser(
            int UserID,
            string Status)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            // -----------------------------------------------------
            // PROTECT SYSTEM ADMINISTRATOR
            // -----------------------------------------------------

            if (UserID == SYSTEM_ADMIN_ID)
            {
                TempData["Error"] =
                    "The System Administrator account is protected and cannot be modified.";

                return RedirectToAction(
                    "ManageUsers");
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // -----------------------------------------------------
            // FIND USER
            // -----------------------------------------------------

            var existing =
                db.Users
                    .FirstOrDefault(x =>
                        x.UserID == UserID);

            if (existing == null)
            {
                return NotFound();
            }

            // -----------------------------------------------------
            // COMPANY SECURITY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    existing.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            // -----------------------------------------------------
            // ONLY ALLOW ACTIVE OR INACTIVE
            // -----------------------------------------------------

            if (!string.Equals(
                    Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(
                    Status,
                    "Inactive",
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "Invalid account status.";

                return RedirectToAction(
                    "EditUser",
                    new
                    {
                        id = UserID
                    });
            }

            // -----------------------------------------------------
            // UPDATE ONLY STATUS
            // -----------------------------------------------------

            existing.Status =
                Status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase)
                    ? "Active"
                    : "Inactive";

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "User status updated successfully.";

                return RedirectToAction(
                    "ManageUsers");
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The user status could not be updated.";

                return RedirectToAction(
                    "EditUser",
                    new
                    {
                        id = UserID
                    });
            }
        }


        // =========================================================
        // DELETE USER
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteUser(int id)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            int currentAdminID =
                GetUserID();

            if (id == currentAdminID)
            {
                TempData["Error"] =
                    "You cannot delete your own administrator account.";

                return RedirectToAction(
                    "ManageUsers");
            }

            if (id == SYSTEM_ADMIN_ID)
            {
                TempData["Error"] =
                    "The System Administrator account is protected and cannot be deleted.";

                return RedirectToAction(
                    "ManageUsers");
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var user =
                db.Users
                    .FirstOrDefault(x =>
                        x.UserID == id);

            if (user == null)
            {
                TempData["Error"] =
                    "User not found.";

                return RedirectToAction(
                    "ManageUsers");
            }

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    user.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            // -----------------------------------------------------
            // COMPANY ADMIN CANNOT BE DELETED
            // -----------------------------------------------------

            if (string.Equals(
                    user.UserType?.Trim(),
                    "Admin",
                    StringComparison.OrdinalIgnoreCase) &&
                user.CompanyID.HasValue)
            {
                TempData["Error"] =
                    "A company administrator cannot be deleted. Create or assign another administrator before making changes.";

                return RedirectToAction(
                    "ManageUsers");
            }

            // -----------------------------------------------------
            // APPLICATIONS
            // -----------------------------------------------------

            bool hasApplications =
                db.Applications.Any(x =>
                    x.JobseekerID == id);

            if (hasApplications)
            {
                TempData["Error"] =
                    "This user cannot be deleted because they have existing job applications. Deactivate the account instead.";

                return RedirectToAction(
                    "ManageUsers");
            }

            // -----------------------------------------------------
            // JOBS
            // -----------------------------------------------------

            bool hasJobs =
                db.Jobs.Any(x =>
                    x.RecruiterID == id);

            if (hasJobs)
            {
                TempData["Error"] =
                    "This recruiter cannot be deleted because they have job listings. Deactivate the account instead.";

                return RedirectToAction(
                    "ManageUsers");
            }

            // -----------------------------------------------------
            // JOBSEEKER PROFILE
            // -----------------------------------------------------

            var jobseekerProfile =
                db.JobseekerProfiles
                    .FirstOrDefault(x =>
                        x.UserID == id);

            if (jobseekerProfile != null)
            {
                db.JobseekerProfiles.Remove(
                    jobseekerProfile);
            }

            // -----------------------------------------------------
            // RECRUITER PROFILE
            // -----------------------------------------------------

            var recruiterProfile =
                db.RecruiterProfiles
                    .FirstOrDefault(x =>
                        x.UserID == id);

            if (recruiterProfile != null)
            {
                db.RecruiterProfiles.Remove(
                    recruiterProfile);
            }

            // -----------------------------------------------------
            // NOTIFICATIONS
            // -----------------------------------------------------

            var notifications =
                db.Notifications
                    .Where(x =>
                        x.UserID == id)
                    .ToList();

            if (notifications.Any())
            {
                db.Notifications.RemoveRange(
                    notifications);
            }

            db.Users.Remove(user);

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "User deleted successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The user could not be deleted because other records depend on this account.";
            }

            return RedirectToAction(
                "ManageUsers");
        }


        // =========================================================
        // JOB LISTINGS
        // =========================================================

        [HttpGet]
        public IActionResult JobListings()
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            IQueryable<Job> query =
                db.Jobs
                    .Include(x => x.Company)
                    .Include(x => x.Recruiter);

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                int companyID =
                    currentUser.CompanyID!.Value;

                query =
                    query.Where(x =>
                        x.CompanyID == companyID);
            }

            var jobs =
                query
                    .OrderByDescending(x => x.DatePosted)
                    .ToList();

            return View(jobs);
        }


        // =========================================================
        // EDIT JOB - GET
        // =========================================================

        [HttpGet]
        public IActionResult EditJob(int id)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var job =
                db.Jobs
                    .Include(x => x.Company)
                    .Include(x => x.Recruiter)
                    .FirstOrDefault(x =>
                        x.JobID == id);

            if (job == null)
            {
                return NotFound();
            }

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    job.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            ViewBag.Provinces =
                db.Provinces
                    .OrderBy(x => x.ProvinceName)
                    .ToList();

            return View(job);
        }


        // =========================================================
        // EDIT JOB - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditJob(Job job)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var existing =
                db.Jobs
                    .FirstOrDefault(x =>
                        x.JobID == job.JobID);

            if (existing == null)
            {
                return NotFound();
            }

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    existing.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            // -----------------------------------------------------
            // REMOVE PROTECTED / NAVIGATION VALIDATION
            // -----------------------------------------------------

            ModelState.Remove(
                nameof(Job.CompanyID));

            ModelState.Remove(
                nameof(Job.RecruiterID));

            ModelState.Remove(
                nameof(Job.QualificationID));

            ModelState.Remove(
                nameof(Job.Qualification));

            // -----------------------------------------------------
            // BASIC JOB DETAILS
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(job.JobTitle))
            {
                existing.JobTitle =
                    job.JobTitle.Trim();
            }

            if (!string.IsNullOrWhiteSpace(job.JobDescription))
            {
                existing.JobDescription =
                    job.JobDescription.Trim();
            }

            if (!string.IsNullOrWhiteSpace(job.Requirements))
            {
                existing.Requirements =
                    job.Requirements.Trim();
            }

            // -----------------------------------------------------
            // SALARY
            // -----------------------------------------------------

            if (job.Salary != null)
            {
                existing.Salary =
                    job.Salary;
            }

            // -----------------------------------------------------
            // PROVINCE
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(job.Province))
            {
                existing.Province =
                    job.Province.Trim();
            }

            // -----------------------------------------------------
            // CITY
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(job.City))
            {
                existing.City =
                    job.City.Trim();
            }

            // -----------------------------------------------------
            // EMPLOYMENT TYPE
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(job.EmploymentType))
            {
                existing.EmploymentType =
                    job.EmploymentType.Trim();
            }

            // -----------------------------------------------------
            // EXPERIENCE LEVEL
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(job.ExperienceLevel))
            {
                existing.ExperienceLevel =
                    job.ExperienceLevel.Trim();
            }

            // -----------------------------------------------------
            // CLOSING DATE
            // -----------------------------------------------------

            existing.ClosingDate =
                job.ClosingDate;

            // -----------------------------------------------------
            // JOB STATUS
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(job.JobStatus))
            {
                existing.JobStatus =
                    job.JobStatus.Trim();
            }

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "Job updated successfully.";

                return RedirectToAction(
                    "JobListings");
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Provinces =
                    db.Provinces
                        .OrderBy(x => x.ProvinceName)
                        .ToList();

                ViewBag.DbError =
                    ex.InnerException?.Message ??
                    ex.Message;

                ModelState.AddModelError(
                    "",
                    "The job could not be updated.");

                return View(job);
            }
            catch (Exception ex)
            {
                ViewBag.Provinces =
                    db.Provinces
                        .OrderBy(x => x.ProvinceName)
                        .ToList();

                ViewBag.DbError =
                    ex.InnerException?.Message ??
                    ex.Message;

                ModelState.AddModelError(
                    "",
                    "An unexpected error occurred while updating the job.");

                return View(job);
            }
        }


        // =========================================================
        // DELETE JOB
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteJob(int id)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var job =
                db.Jobs
                    .FirstOrDefault(x =>
                        x.JobID == id);

            if (job == null)
            {
                TempData["Error"] =
                    "Job listing not found.";

                return RedirectToAction(
                    "JobListings");
            }

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    job.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            TempData["Error"] =
                "Jobs are not deleted because existing applications may reference them. Deactivate or close the job instead.";

            return RedirectToAction(
                "JobListings");
        }


        // =========================================================
        // DEACTIVATE USER
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeactivateUser(int id)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            if (id == SYSTEM_ADMIN_ID)
            {
                TempData["Error"] =
                    "The System Administrator account is protected and cannot be deactivated.";

                return RedirectToAction(
                    "ManageUsers");
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var user =
                db.Users
                    .FirstOrDefault(x =>
                        x.UserID == id);

            if (user == null)
            {
                TempData["Error"] =
                    "User not found.";

                return RedirectToAction(
                    "ManageUsers");
            }

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    user.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            // -----------------------------------------------------
            // CANNOT DEACTIVATE YOURSELF
            // -----------------------------------------------------

            if (id == currentUser.UserID)
            {
                TempData["Error"] =
                    "You cannot deactivate your own administrator account.";

                return RedirectToAction(
                    "ManageUsers");
            }

            // -----------------------------------------------------
            // DO NOT DEACTIVATE COMPANY ADMIN
            // -----------------------------------------------------

            if (string.Equals(
                    user.UserType?.Trim(),
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "A company administrator cannot be deactivated.";

                return RedirectToAction(
                    "ManageUsers");
            }

            user.Status = "Inactive";

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "User account deactivated successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The user account could not be deactivated.";
            }

            return RedirectToAction(
                "ManageUsers");
        }


        // =========================================================
        // ACTIVATE USER
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActivateUser(int id)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            if (id == SYSTEM_ADMIN_ID)
            {
                TempData["Error"] =
                    "The System Administrator account is protected.";

                return RedirectToAction(
                    "ManageUsers");
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var user =
                db.Users
                    .FirstOrDefault(x =>
                        x.UserID == id);

            if (user == null)
            {
                TempData["Error"] =
                    "User not found.";

                return RedirectToAction(
                    "ManageUsers");
            }

            // -----------------------------------------------------
            // COMPANY ADMIN - OWN COMPANY ONLY
            // -----------------------------------------------------

            if (!IsSystemAdministrator(currentUser))
            {
                if (!currentUser.CompanyID.HasValue ||
                    user.CompanyID != currentUser.CompanyID.Value)
                {
                    return RedirectToAction(
                        "AccessDenied",
                        "Account");
                }
            }

            // -----------------------------------------------------
            // REJECTED RECRUITER CANNOT BE ACTIVATED DIRECTLY
            // -----------------------------------------------------

            if (string.Equals(
                    user.UserType?.Trim(),
                    "Recruiter",
                    StringComparison.OrdinalIgnoreCase) &&
                string.Equals(
                    user.Status?.Trim(),
                    "Rejected",
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] =
                    "A rejected recruiter must be approved through Recruiter Approvals.";

                return RedirectToAction(
                    "ManageUsers");
            }

            user.Status = "Active";

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "User account activated successfully.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The user account could not be activated.";
            }

            return RedirectToAction(
                "ManageUsers");
        }


        // =========================================================
        // VERIFY REPRESENTATIVE
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyRepresentative(int id)
        {
            try
            {
                var accessResult = CheckAdminAccess();
                if (accessResult != null)
                    return accessResult;

                var currentUser = GetCurrentUser();
                if (currentUser == null)
                {
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login", "Account");
                }

                bool isSystemAdmin = IsSystemAdministrator(currentUser);

                var recruiter = await db.Users
                    .Include(x => x.RecruiterProfile)
                    .FirstOrDefaultAsync(x =>
                        x.UserID == id &&
                        x.UserType != null &&
                        x.UserType.ToLower() == "recruiter");

                if (recruiter == null)
                {
                    TempData["ErrorMessage"] = "Recruiter could not be found.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                if (recruiter.IsApproved)
                {
                    TempData["ErrorMessage"] = "This recruiter has already been approved.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                if (!string.Equals(recruiter.Status?.Trim(), "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ErrorMessage"] = "This recruiter is no longer awaiting approval.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                var recruiterProfile = recruiter.RecruiterProfile;
                if (recruiterProfile == null)
                {
                    TempData["ErrorMessage"] = "Recruiter profile could not be found.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                if (string.IsNullOrWhiteSpace(recruiterProfile.RepresentativeName))
                {
                    TempData["ErrorMessage"] = "The representative name is missing.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                if (string.IsNullOrWhiteSpace(recruiterProfile.RepresentativeEmail))
                {
                    TempData["ErrorMessage"] = "The representative email is missing.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                Company? company = null;

                if (isSystemAdmin)
                {
                    string requestedCompany =
                        (recruiterProfile.CompanyName ?? string.Empty).Trim();

                    if (string.IsNullOrWhiteSpace(requestedCompany))
                    {
                        TempData["ErrorMessage"] =
                            "The recruiter's requested company is missing.";
                        return RedirectToAction(nameof(RecruiterApprovals));
                    }

                    company = await db.Companies
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.CompanyName != null &&
                            x.CompanyName.Trim().ToLower() == requestedCompany.ToLower());
                }
                else
                {
                    if (!currentUser.CompanyID.HasValue)
                    {
                        TempData["ErrorMessage"] =
                            "Your administrator account is not assigned to a company.";
                        return RedirectToAction(nameof(RecruiterApprovals));
                    }

                    company = await db.Companies
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x =>
                            x.CompanyID == currentUser.CompanyID.Value);

                    if (company != null &&
                        !string.Equals(
                            recruiterProfile.CompanyName?.Trim(),
                            company.CompanyName?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                    {
                        TempData["ErrorMessage"] =
                            "This recruiter does not belong to your assigned company.";
                        return RedirectToAction(nameof(RecruiterApprovals));
                    }
                }

                if (company == null)
                {
                    TempData["ErrorMessage"] =
                        "The recruiter's requested company could not be found.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
                string token = Convert.ToBase64String(tokenBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");

                string tokenHash = Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(token)));

                recruiterProfile.RepresentativeVerificationTokenHash = tokenHash;
                recruiterProfile.RepresentativeVerificationExpiresAt = DateTime.UtcNow.AddHours(24);
                recruiterProfile.RepresentativeVerificationStatus = "Pending";

                string recruiterName = (recruiter.FullName ?? string.Empty).Trim();
                string representativeName = recruiterProfile.RepresentativeName.Trim();
                string representativeEmail = recruiterProfile.RepresentativeEmail.Trim();
                string companyName = (recruiterProfile.CompanyName ?? company.CompanyName ?? string.Empty).Trim();

                string knowPersonLink = Url.Action(
                    "RepresentativeResponse", "Account",
                    new { id = recruiter.UserID, token, response = "know" },
                    Request.Scheme) ?? string.Empty;

                string doNotKnowPersonLink = Url.Action(
                    "RepresentativeResponse", "Account",
                    new { id = recruiter.UserID, token, response = "dontknow" },
                    Request.Scheme) ?? string.Empty;

                if (string.IsNullOrWhiteSpace(knowPersonLink) ||
                    string.IsNullOrWhiteSpace(doNotKnowPersonLink))
                {
                    TempData["ErrorMessage"] =
                        "The representative verification links could not be created.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                await db.SaveChangesAsync();

                await emailService.SendRepresentativeVerificationEmailAsync(
                    representativeName,
                    representativeEmail,
                    recruiterName,
                    companyName,
                    knowPersonLink,
                    doNotKnowPersonLink);

                TempData["SuccessMessage"] =
                    $"Verification email sent successfully to {representativeEmail}.";

                return RedirectToAction(nameof(RecruiterApprovals));
            }
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                logger.LogError(ex,
                    "SQL error while sending representative verification for recruiter {RecruiterId}", id);

                TempData["ErrorMessage"] =
                    "A database connection error occurred while preparing the verification request. Please try again.";

                return RedirectToAction(nameof(RecruiterApprovals));
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex,
                    "Database update error while sending representative verification for recruiter {RecruiterId}", id);

                TempData["ErrorMessage"] =
                    "The verification request could not be saved to the database.";

                return RedirectToAction(nameof(RecruiterApprovals));
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Unexpected error while sending representative verification for recruiter {RecruiterId}", id);

                TempData["ErrorMessage"] =
                    "The representative verification email could not be sent. Please check the email settings and try again.";

                return RedirectToAction(nameof(RecruiterApprovals));
            }
        }

        // =========================================================
        // ANALYTICS
        // =========================================================

        [HttpGet]
        public IActionResult Analytics()
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            bool isSystemAdmin =
                IsSystemAdministrator(currentUser);

            // =====================================================
            // SYSTEM ADMINISTRATOR ANALYTICS
            // =====================================================

            if (isSystemAdmin)
            {
                ViewBag.TotalUsers =
                    db.Users.Count();

                ViewBag.ActiveUsers =
                    db.Users.Count(x =>
                        x.Status == "Active");

                ViewBag.ActiveJobs =
                    db.Jobs.Count(x =>
                        x.JobStatus == "Active");

                DateTime today =
                    DateTime.UtcNow.Date;

                DateTime tomorrow =
                    today.AddDays(1);

                ViewBag.NewToday =
                    db.Users.Count(x =>
                        x.DateCreated >= today &&
                        x.DateCreated < tomorrow);

                ViewBag.JobSeekers =
                    db.Users.Count(x =>
                        x.UserType == "Jobseeker");

                ViewBag.Employers =
                    db.Users.Count(x =>
                        x.UserType == "Recruiter");

                ViewBag.Provinces =
                    db.Users
                        .Where(x =>
                            x.UserType != "Admin" &&
                            x.ProvinceID != null)
                        .Include(x => x.Province)
                        .GroupBy(x =>
                            x.Province!.ProvinceName)
                        .Select(x => new
                        {
                            Province = x.Key,
                            Total = x.Count()
                        })
                        .OrderByDescending(x => x.Total)
                        .ToList();

                ViewBag.Male =
                    db.Users.Count(x =>
                        x.Gender == "Male");

                ViewBag.Female =
                    db.Users.Count(x =>
                        x.Gender == "Female");

                return View();
            }

            // =====================================================
            // COMPANY ADMIN ANALYTICS
            // =====================================================

            int companyID =
                currentUser.CompanyID!.Value;

            ViewBag.TotalUsers =
                db.Users.Count(x =>
                    x.CompanyID == companyID);

            ViewBag.ActiveUsers =
                db.Users.Count(x =>
                    x.CompanyID == companyID &&
                    x.Status == "Active");

            ViewBag.ActiveJobs =
                db.Jobs.Count(x =>
                    x.CompanyID == companyID &&
                    x.JobStatus == "Active");

            DateTime companyToday =
                DateTime.UtcNow.Date;

            DateTime companyTomorrow =
                companyToday.AddDays(1);

            ViewBag.NewToday =
                db.Users.Count(x =>
                    x.CompanyID == companyID &&
                    x.DateCreated >= companyToday &&
                    x.DateCreated < companyTomorrow);

            ViewBag.JobSeekers = 0;

            ViewBag.Employers =
                db.Users.Count(x =>
                    x.CompanyID == companyID &&
                    x.UserType != null &&
                    x.UserType.ToLower() == "recruiter");

            ViewBag.Provinces =
                db.Users
                    .Where(x =>
                        x.CompanyID == companyID &&
                        x.UserType != "Admin" &&
                        x.ProvinceID != null)
                    .Include(x => x.Province)
                    .GroupBy(x =>
                        x.Province!.ProvinceName)
                    .Select(x => new
                    {
                        Province = x.Key,
                        Total = x.Count()
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList();

            ViewBag.Male =
                db.Users.Count(x =>
                    x.CompanyID == companyID &&
                    x.Gender == "Male");

            ViewBag.Female =
                db.Users.Count(x =>
                    x.CompanyID == companyID &&
                    x.Gender == "Female");

            return View();
        }


        // =========================================================
        // ADMIN PROFILE - GET
        // =========================================================

        [HttpGet]
        public IActionResult Profile()
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            int userID =
                GetUserID();

            var admin =
                db.Users
                    .Include(x => x.Company)
                    .Include(x => x.Province)
                    .FirstOrDefault(x =>
                        x.UserID == userID);

            if (admin == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            return View(admin);
        }


        // =========================================================
        // ADMIN PROFILE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User user)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            int userID =
                GetUserID();

            var existing =
                db.Users
                    .FirstOrDefault(x =>
                        x.UserID == userID);

            if (existing == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(user.FullName))
            {
                existing.FullName =
                    user.FullName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                string newEmail =
                    user.Email
                        .Trim()
                        .ToLowerInvariant();

                bool emailExists =
                    db.Users.Any(x =>
                        x.UserID != existing.UserID &&
                        x.Email != null &&
                        x.Email.ToLower() == newEmail);

                if (emailExists)
                {
                    ModelState.AddModelError(
                        nameof(user.Email),
                        "An account with this email address already exists.");

                    return View(user);
                }

                existing.Email =
                    newEmail;
            }

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                existing.PhoneNumber =
                    user.PhoneNumber.Trim();
            }

            if (!string.IsNullOrWhiteSpace(user.Gender))
            {
                existing.Gender =
                    user.Gender.Trim();
            }

            try
            {
                db.SaveChanges();

                HttpContext.Session.SetString(
                    "UserName",
                    existing.FullName ?? "");

                HttpContext.Session.SetString(
                    "Email",
                    existing.Email ?? "");

                TempData["Success"] =
                    "Profile updated successfully.";

                return RedirectToAction(
                    "Profile");
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The profile could not be updated.";

                return View(user);
            }
        }


        // =========================================================
        // PENDING RECRUITER APPROVALS
        // =========================================================
        //
        // SYSTEM ADMIN:
        //     Can see ALL pending recruiters.
        //
        // COMPANY ADMIN:
        //     Can ONLY see pending recruiters whose requested
        //     company matches the administrator's company.
        //
        // Pending recruiter:
        //
        //     User.CompanyID = NULL
        //     IsApproved = false
        //     Status = Pending
        //
        //     RecruiterProfile contains the company information
        //     supplied during registration.
        //
        // After approval:
        //
        //     User.CompanyID = Admin's CompanyID
        //     IsApproved = true
        //     Status = Active
        // =========================================================

        [HttpGet]
        public IActionResult RecruiterApprovals()
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            IQueryable<User> query =
                db.Users
                    .Include(x => x.Province)
                    .Include(x => x.Company)
                    .Include(x => x.RecruiterProfile)
                    .Where(x =>
                        x.UserType != null &&
                        x.UserType.ToLower() == "recruiter" &&
                        x.IsApproved == false &&
                        x.Status != null &&
                        x.Status.ToLower() == "pending");

            // =====================================================
            // SYSTEM ADMINISTRATOR
            // =====================================================

            if (IsSystemAdministrator(currentUser))
            {
                var allRecruiters =
                    query
                        .OrderBy(x => x.DateCreated)
                        .ToList();

                return View(allRecruiters);
            }

            // =====================================================
            // COMPANY ADMINISTRATOR
            // =====================================================

            if (!currentUser.CompanyID.HasValue)
            {
                TempData["Error"] =
                    "Your administrator account is not assigned to a company.";

                return RedirectToAction(
                    "Dashboard");
            }

            int companyID =
                currentUser.CompanyID.Value;

            var company =
                db.Companies
                    .AsNoTracking()
                    .FirstOrDefault(x =>
                        x.CompanyID == companyID);

            if (company == null)
            {
                TempData["Error"] =
                    "Your company could not be found.";

                return RedirectToAction(
                    "Dashboard");
            }

            string companyName =
                (company.CompanyName ?? string.Empty)
                    .Trim()
                    .ToLower();

            // -----------------------------------------------------
            // MATCH RECRUITER REQUESTED COMPANY
            // -----------------------------------------------------

            query =
                query.Where(x =>
                    x.RecruiterProfile != null &&
                    x.RecruiterProfile.CompanyName != null &&
                    x.RecruiterProfile.CompanyName
                        .Trim()
                        .ToLower() == companyName);

            var recruiters =
                query
                    .OrderBy(x => x.DateCreated)
                    .ToList();

            return View(recruiters);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveRecruiter(int id)
        {
            var accessResult = CheckAdminAccess();
            if (accessResult != null)
                return accessResult;

            var currentUser = GetCurrentUser();
            if (currentUser == null)
                return RedirectToAction("Login", "Account");

            bool isSystemAdmin = IsSystemAdministrator(currentUser);

            // Load recruiter first. System Administrator may have no CompanyID.
            var recruiter = await db.Users
                .Include(x => x.RecruiterProfile)
                .FirstOrDefaultAsync(x =>
                    x.UserID == id &&
                    x.UserType != null &&
                    x.UserType.ToLower() == "recruiter");

            if (recruiter == null)
            {
                TempData["ErrorMessage"] = "Recruiter could not be found.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            if (recruiter.IsApproved)
            {
                TempData["ErrorMessage"] = "This recruiter has already been approved.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            if (!string.Equals(recruiter.Status?.Trim(), "Pending", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "This recruiter is no longer awaiting approval.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            var recruiterProfile = recruiter.RecruiterProfile;
            if (recruiterProfile == null)
            {
                TempData["ErrorMessage"] = "Recruiter profile could not be found.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            if (!string.Equals(
                    recruiterProfile.RepresentativeVerificationStatus,
                    "Verified",
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] =
                    "The company representative has not verified this recruiter.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            if (recruiterProfile.UserID != recruiter.UserID)
            {
                TempData["ErrorMessage"] = "Recruiter profile information is invalid.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            Company? company = null;

            if (isSystemAdmin)
            {
                string requestedCompany =
                    (recruiterProfile.CompanyName ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(requestedCompany))
                {
                    TempData["ErrorMessage"] =
                        "The recruiter's requested company is missing.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                company = await db.Companies
                    .FirstOrDefaultAsync(x =>
                        x.CompanyName != null &&
                        x.CompanyName.Trim().ToLower() == requestedCompany.ToLower());
            }
            else
            {
                if (!currentUser.CompanyID.HasValue)
                {
                    TempData["ErrorMessage"] =
                        "Your administrator account is not assigned to a company.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                int adminCompanyId = currentUser.CompanyID.Value;

                company = await db.Companies
                    .FirstOrDefaultAsync(x => x.CompanyID == adminCompanyId);

                if (company == null)
                {
                    TempData["ErrorMessage"] =
                        "Your assigned company could not be found.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }

                if (!string.Equals(
                        recruiterProfile.CompanyName?.Trim(),
                        company.CompanyName?.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ErrorMessage"] =
                        "This recruiter does not belong to your assigned company.";
                    return RedirectToAction(nameof(RecruiterApprovals));
                }
            }

            if (company == null)
            {
                TempData["ErrorMessage"] =
                    "The recruiter's requested company could not be found.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            var oldTokens = await db.PasswordResetTokens
                .Where(x => x.UserID == recruiter.UserID && x.UsedAt == null)
                .ToListAsync();

            foreach (var oldToken in oldTokens)
                oldToken.UsedAt = DateTime.UtcNow;

            byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);

            string createPasswordToken = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            string tokenHash = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(createPasswordToken)));

            var passwordToken = new PasswordResetToken
            {
                UserID = recruiter.UserID,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(3),
                UsedAt = null
            };

            db.PasswordResetTokens.Add(passwordToken);

            recruiter.CompanyID = company.CompanyID;
            recruiter.IsApproved = true;
            recruiter.Status = "Active";

            string createPasswordLink = Url.Action(
                "CreatePassword",
                "Account",
                new { id = recruiter.UserID, token = createPasswordToken },
                Request.Scheme) ?? string.Empty;

            if (string.IsNullOrWhiteSpace(createPasswordLink))
            {
                passwordToken.UsedAt = DateTime.UtcNow;
                recruiter.CompanyID = null;
                recruiter.IsApproved = false;
                recruiter.Status = "Pending";
                await db.SaveChangesAsync();

                TempData["ErrorMessage"] =
                    "The password creation link could not be created.";
                return RedirectToAction(nameof(RecruiterApprovals));
            }

            await db.SaveChangesAsync();

            string recruiterName = (recruiter.FullName ?? string.Empty).Trim();

            try
            {
                await emailService.SendRecruiterApprovalEmailAsync(
                    recruiterName,
                    recruiter.Email,
                    company.CompanyName,
                    createPasswordLink);

                TempData["SuccessMessage"] =
                    "Recruiter approved successfully. A password creation email has been sent to the recruiter.";
            }
            catch (Exception ex)
            {
                passwordToken.UsedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();

                logger.LogError(
                    ex,
                    "Recruiter {RecruiterId} was approved but password creation email failed.",
                    recruiter.UserID);

                TempData["ErrorMessage"] =
                    "The recruiter was approved, but the password creation email could not be sent. Please try again.";
            }

            return RedirectToAction(nameof(RecruiterApprovals));
        }


        // =========================================================
        // REJECT RECRUITER
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectRecruiter(int id)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            // -----------------------------------------------------
            // SYSTEM ADMINISTRATOR CANNOT REJECT
            // -----------------------------------------------------

            if (IsSystemAdministrator(currentUser))
            {
                TempData["Error"] =
                    "Recruiter approval must be handled by a Company Administrator.";

                return RedirectToAction(
                    "RecruiterApprovals");
            }

            // -----------------------------------------------------
            // COMPANY ADMIN MUST HAVE COMPANY
            // -----------------------------------------------------

            if (!currentUser.CompanyID.HasValue)
            {
                TempData["Error"] =
                    "Your administrator account is not assigned to a company.";

                return RedirectToAction(
                    "RecruiterApprovals");
            }

            int companyID =
                currentUser.CompanyID.Value;

            var company =
                db.Companies
                    .AsNoTracking()
                    .FirstOrDefault(x =>
                        x.CompanyID == companyID);

            if (company == null)
            {
                TempData["Error"] =
                    "Your company could not be found.";

                return RedirectToAction(
                    "RecruiterApprovals");
            }

            string companyName =
                (company.CompanyName ?? string.Empty)
                    .Trim()
                    .ToLower();

            // -----------------------------------------------------
            // FIND RECRUITER
            // -----------------------------------------------------

            var recruiter =
                db.Users
                    .Include(x => x.RecruiterProfile)
                    .FirstOrDefault(x =>
                        x.UserID == id &&
                        x.UserType != null &&
                        x.UserType.ToLower() == "recruiter");

            if (recruiter == null)
            {
                TempData["Error"] =
                    "Recruiter account not found.";

                return RedirectToAction(
                    "RecruiterApprovals");
            }

            // -----------------------------------------------------
            // MUST STILL BE PENDING
            // -----------------------------------------------------

            bool isPending =
                !recruiter.IsApproved &&
                string.Equals(
                    recruiter.Status?.Trim(),
                    "Pending",
                    StringComparison.OrdinalIgnoreCase);

            if (!isPending)
            {
                TempData["Error"] =
                    "This recruiter is no longer awaiting approval.";

                return RedirectToAction(
                    "RecruiterApprovals");
            }

            // -----------------------------------------------------
            // PROFILE MUST EXIST
            // -----------------------------------------------------

            if (recruiter.RecruiterProfile == null)
            {
                TempData["Error"] =
                    "The recruiter's profile could not be found.";

                return RedirectToAction(
                    "RecruiterApprovals");
            }

            // -----------------------------------------------------
            // VERIFY REQUESTED COMPANY
            // -----------------------------------------------------

            string requestedCompany =
                (recruiter.RecruiterProfile.CompanyName ?? string.Empty)
                    .Trim()
                    .ToLower();

            if (string.IsNullOrWhiteSpace(requestedCompany) ||
                !requestedCompany.Equals(
                    companyName,
                    StringComparison.Ordinal))
            {
                TempData["Error"] =
                    "This recruiter did not request to join your company.";

                return RedirectToAction(
                    "RecruiterApprovals");
            }

            // -----------------------------------------------------
            // REJECT
            // -----------------------------------------------------

            recruiter.IsApproved =
                false;

            recruiter.Status =
                "Rejected";

            recruiter.CompanyID =
                null;

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    $"Recruiter '{recruiter.FullName}' has been rejected.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "The recruiter could not be rejected.";
            }

            return RedirectToAction(
                "RecruiterApprovals");
        }


        // =========================================================
        // CREATE ADMIN - GET
        // =========================================================

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null ||
                !IsSystemAdministrator(currentUser))
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            LoadCreateAdminData();

            return View(
                new CreateAdminViewModel
                {
                    UserType = "Admin"
                });
        }


        // =========================================================
        // CREATE ADMIN - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAdmin(
            CreateAdminViewModel model)
        {
            var accessResult = CheckAdminAccess();

            if (accessResult != null)
            {
                return accessResult;
            }

            var currentUser = GetCurrentUser();

            if (currentUser == null ||
                !IsSystemAdministrator(currentUser))
            {
                return RedirectToAction(
                    "AccessDenied",
                    "Account");
            }

            // =====================================================
            // FORCE ADMIN ACCOUNT TYPE
            // =====================================================

            model.UserType =
                "Admin";

            ModelState.Remove(
                nameof(model.UserType));

            // =====================================================
            // MODEL VALIDATION
            // =====================================================

            if (!ModelState.IsValid)
            {
                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // EMAIL
            // =====================================================

            string email =
                (model.Email ?? string.Empty)
                    .Trim()
                    .ToLowerInvariant();

            bool emailExists =
                db.Users.Any(x =>
                    x.Email != null &&
                    x.Email.ToLower() == email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email address already exists.");

                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // PASSWORD
            // =====================================================

            string password =
                model.Password ?? string.Empty;

            // Exactly 8 characters.
            if (password.Length != 8)
            {
                ModelState.AddModelError(
                    nameof(model.Password),
                    "Password must be exactly 8 characters long.");

                LoadCreateAdminData();

                return View(model);
            }

            // Uppercase.
            if (!password.Any(char.IsUpper))
            {
                ModelState.AddModelError(
                    nameof(model.Password),
                    "Password must contain at least 1 uppercase letter.");

                LoadCreateAdminData();

                return View(model);
            }

            // Lowercase.
            if (!password.Any(char.IsLower))
            {
                ModelState.AddModelError(
                    nameof(model.Password),
                    "Password must contain at least 1 lowercase letter.");

                LoadCreateAdminData();

                return View(model);
            }

            // Two numbers.
            if (password.Count(char.IsDigit) < 2)
            {
                ModelState.AddModelError(
                    nameof(model.Password),
                    "Password must contain at least 2 numbers.");

                LoadCreateAdminData();

                return View(model);
            }

            // Special character.
            if (!password.Any(
                c => !char.IsLetterOrDigit(c)))
            {
                ModelState.AddModelError(
                    nameof(model.Password),
                    "Password must contain at least 1 special character.");

                LoadCreateAdminData();

                return View(model);
            }

            // Confirm password.
            string confirmPassword =
                model.ConfirmPassword ?? string.Empty;

            if (password != confirmPassword)
            {
                ModelState.AddModelError(
                    nameof(model.ConfirmPassword),
                    "Passwords do not match.");

                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // PHONE
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                    model.PhoneNumber))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Phone number is required.");

                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // GENDER
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                    model.Gender))
            {
                ModelState.AddModelError(
                    nameof(model.Gender),
                    "Gender is required.");

                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // PROVINCE
            // =====================================================

            if (!model.ProvinceID.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.ProvinceID),
                    "Province is required.");

                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // COMPANY NAME
            // =====================================================

            string companyName =
                (model.CompanyName ?? string.Empty)
                    .Trim();

            if (string.IsNullOrWhiteSpace(
                    companyName))
            {
                ModelState.AddModelError(
                    nameof(model.CompanyName),
                    "Company name is required.");

                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // DUPLICATE COMPANY
            // =====================================================

            bool companyExists =
                db.Companies.Any(x =>
                    x.CompanyName.ToLower() ==
                    companyName.ToLower());

            if (companyExists)
            {
                ModelState.AddModelError(
                    nameof(model.CompanyName),
                    "A company with this name already exists.");

                LoadCreateAdminData();

                return View(model);
            }

            // =====================================================
            // CREATE COMPANY + ADMIN
            // =====================================================

            using var transaction =
                db.Database.BeginTransaction();

            try
            {
                // -------------------------------------------------
                // CREATE COMPANY
                // -------------------------------------------------

                var company =
                    new Company
                    {
                        CompanyName =
                            companyName,

                        CompanyDescription =
                            string.IsNullOrWhiteSpace(
                                model.CompanyDescription)
                                ? null
                                : model.CompanyDescription.Trim(),

                        Website =
                            string.IsNullOrWhiteSpace(
                                model.Website)
                                ? null
                                : model.Website.Trim(),

                        Email =
                            email,

                        PhoneNumber =
                            model.PhoneNumber.Trim(),

                        ProvinceID =
                            model.ProvinceID,

                        DateCreated =
                            DateTime.UtcNow,

                        IsActive =
                            true
                    };

                db.Companies.Add(company);

                db.SaveChanges();

                // -------------------------------------------------
                // CREATE ADMIN
                // -------------------------------------------------

                var newAdmin =
                    new User
                    {
                        FullName =
                            (model.FullName ?? string.Empty)
                                .Trim(),

                        Email =
                            email,

                        PhoneNumber =
                            model.PhoneNumber.Trim(),

                        Gender =
                            model.Gender.Trim(),

                        ProvinceID =
                            model.ProvinceID,

                        CompanyID =
                            company.CompanyID,

                        UserType =
                            "Admin",

                        Status =
                            "Active",

                        IsApproved =
                            true,

                        EmailConfirmed =
                            true,

                        DateCreated =
                            DateTime.UtcNow
                    };

                // -------------------------------------------------
                // IMPORTANT:
                // Use the SAME SHA-256 hashing used by
                // AccountController.Login().
                // -------------------------------------------------

                newAdmin.PasswordHash =
                    HashPassword(password);

                db.Users.Add(newAdmin);

                db.SaveChanges();

                // -------------------------------------------------
                // COMMIT
                // -------------------------------------------------

                transaction.Commit();

                TempData["Success"] =
                    $"Company '{company.CompanyName}' and its Admin account were created successfully.";

                return RedirectToAction(
                    "ManageUsers");
            }
            catch (DbUpdateException)
            {
                transaction.Rollback();

                TempData["Error"] =
                    "The company and Admin account could not be created. Please check the information and try again.";

                LoadCreateAdminData();

                return View(model);
            }
            catch (Exception)
            {
                transaction.Rollback();

                TempData["Error"] =
                    "An unexpected error occurred while creating the company and Admin account.";

                LoadCreateAdminData();

                return View(model);
            }
        }


        // =========================================================
        // PASSWORD HASHING
        // =========================================================
        //
        // Must match AccountController.Login().
        // =========================================================

        private string HashPassword(string password)
        {
            using SHA256 sha =
                SHA256.Create();

            byte[] bytes =
                sha.ComputeHash(
                    Encoding.UTF8.GetBytes(password));

            StringBuilder builder =
                new StringBuilder();

            foreach (byte b in bytes)
            {
                builder.Append(
                    b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}