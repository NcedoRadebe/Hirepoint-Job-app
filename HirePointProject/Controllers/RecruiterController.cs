using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HirePoint.Data;
using HirePoint.Models;
using HirePoint.Services;

namespace HirePoint.Controllers
{
    public class RecruiterController : Controller
    {
        private readonly HirePointDbContext db;
        private readonly IAIEmailService aiEmailService;

        public RecruiterController(
            HirePointDbContext context,
            IAIEmailService aiEmailService)
        {
            db = context;
            this.aiEmailService = aiEmailService;
        }

        // =========================================================
        // CURRENT USER
        // =========================================================

        private int GetUserID()
        {
            return HttpContext.Session.GetInt32("UserID") ?? 0;
        }

        private User? GetCurrentRecruiter()
        {
            int userID = GetUserID();

            if (userID <= 0)
                return null;

            return db.Users
                .FirstOrDefault(x => x.UserID == userID);
        }

        private bool IsRecruiter()
        {
            var user = GetCurrentRecruiter();

            return user != null &&
                   string.Equals(
                       user.UserType?.Trim(),
                       "Recruiter",
                       StringComparison.OrdinalIgnoreCase);
        }

        // =========================================================
        // FORM OPTIONS
        // =========================================================

        private void LoadProvinces()
        {
            ViewBag.Provinces = db.Provinces
                .AsNoTracking()
                .OrderBy(x => x.ProvinceName)
                .ToList();
        }

        private void LoadEmploymentTypes()
        {
            ViewBag.EmploymentTypes = new List<string>
            {
                "Full Time",
                "Part Time",
                "Internship"
            };
        }

        private void LoadExperienceLevels()
        {
            ViewBag.ExperienceLevels = new List<string>
            {
                "0-1 Years",
                "1-3 Years",
                "2-4 Years",
                "3-5 Years",
                "4-6 Years",
                "5-7 Years",
                "7-10 Years",
                "10+ Years"
            };
        }

        private void LoadJobStatuses()
        {
            ViewBag.JobStatuses = new List<string>
            {
                "Active",
                "Closed"
            };
        }

        private void LoadJobFormOptions()
        {
            LoadProvinces();
            LoadEmploymentTypes();
            LoadExperienceLevels();
            LoadJobStatuses();
        }

        // =========================================================
        // QUALIFICATION
        // =========================================================

        private Qualification? GetOrCreateQualification(
            string? qualificationName)
        {
            if (string.IsNullOrWhiteSpace(qualificationName))
                return null;

            string name = qualificationName.Trim();

            var existingQualification = db.Qualifications
                .FirstOrDefault(x =>
                    x.Name.ToLower() == name.ToLower());

            if (existingQualification != null)
                return existingQualification;

            var qualification = new Qualification
            {
                Name = name
            };

            db.Qualifications.Add(qualification);
            db.SaveChanges();

            return qualification;
        }

        // =========================================================
        // QUALIFICATION VALIDATION
        // =========================================================

        private void ValidateQualification(
            string? qualification)
        {
            // Qualification is OPTIONAL.
            if (string.IsNullOrWhiteSpace(qualification))
                return;

            string name = qualification.Trim();

            if (name.Length < 2)
            {
                ModelState.AddModelError(
                    "Qualification",
                    "Qualification must be at least 2 characters long.");
            }

            if (name.Length > 150)
            {
                ModelState.AddModelError(
                    "Qualification",
                    "Qualification cannot exceed 150 characters.");
            }
        }

        // =========================================================
        // COMPANY PROFILE
        // =========================================================

        private RecruiterProfile? GetCompanyProfile(int userID)
        {
            return db.RecruiterProfiles
                .AsNoTracking()
                .FirstOrDefault(x => x.UserID == userID);
        }

        // =========================================================
        // GET RECRUITER COMPANY ID
        // =========================================================

        private int? GetRecruiterCompanyID(int userID)
        {
            var user = db.Users
                .AsNoTracking()
                .FirstOrDefault(x => x.UserID == userID);

            if (user?.CompanyID.HasValue == true &&
                user.CompanyID.Value > 0)
            {
                return user.CompanyID.Value;
            }

            var profile = db.RecruiterProfiles
                .AsNoTracking()
                .FirstOrDefault(x => x.UserID == userID);

            if (profile == null ||
                string.IsNullOrWhiteSpace(profile.CompanyName))
            {
                return null;
            }

            string companyName =
                profile.CompanyName.Trim();

            var company = db.Companies
                .AsNoTracking()
                .FirstOrDefault(x =>
                    x.CompanyName.ToLower() ==
                    companyName.ToLower());

            return company?.CompanyID;
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        [HttpGet]
        public IActionResult Dashboard()
        {
            if (!IsRecruiter())
                return RedirectToAction("Login", "Account");

            int userID = GetUserID();

            var recruiter = db.Users
                .AsNoTracking()
                .FirstOrDefault(x =>
                    x.UserID == userID);

            if (recruiter == null)
                return RedirectToAction("Login", "Account");

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue)
            {
                TempData["Error"] =
                    "Your recruiter account is not linked to a company. Please contact your Company Administrator.";

                return RedirectToAction(
                    "Login",
                    "Account");
            }

            int recruiterCompanyID =
                companyID.Value;

            ViewBag.UserName =
                recruiter.FullName;

            ViewBag.Company =
                GetCompanyProfile(userID);

            ViewBag.TotalJobs =
                db.Jobs.Count(x =>
                    x.RecruiterID == userID &&
                    x.CompanyID == recruiterCompanyID);

            ViewBag.ActiveJobs =
                db.Jobs.Count(x =>
                    x.RecruiterID == userID &&
                    x.CompanyID == recruiterCompanyID &&
                    x.JobStatus != null &&
                    x.JobStatus.ToLower() == "active");

            ViewBag.TotalApplications =
                db.Applications.Count(x =>
                    x.Job != null &&
                    x.Job.RecruiterID == userID &&
                    x.Job.CompanyID == recruiterCompanyID);

            // Pending card removed.
            // Accepted/Rejected counters are also no longer
            // required by the dashboard.

            ViewBag.RecentJobs =
                db.Jobs
                    .AsNoTracking()
                    .Where(x =>
                        x.RecruiterID == userID &&
                        x.CompanyID == recruiterCompanyID)
                    .OrderByDescending(x => x.DatePosted)
                    .Take(5)
                    .ToList();

            return View();
        }

        // =========================================================
        // POST JOB - GET
        // =========================================================

        [HttpGet]
        public IActionResult PostJob()
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            ViewBag.Company =
                GetCompanyProfile(userID);

            ViewBag.Qualification = "";

            LoadJobFormOptions();

            return View(new Job
            {
                JobStatus = "Active",

                // Default closing date:
                // 30 days from today.
                ClosingDate =
                    DateTime.Today.AddDays(30)
            });
        }

        // =========================================================
        // POST JOB - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostJob(
            Job job,
            string? qualification)
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            if (userID <= 0)
                return RedirectToAction(
                    "Login",
                    "Account");

            // Qualification is optional.
            ModelState.Remove(nameof(Job.QualificationID));
            ModelState.Remove(nameof(Job.Qualification));

            ModelState.Remove(nameof(Job.Skills));
            ModelState.Remove(nameof(Job.JobID));
            ModelState.Remove(nameof(Job.RecruiterID));
            ModelState.Remove(nameof(Job.Recruiter));
            ModelState.Remove(nameof(Job.CompanyID));
            ModelState.Remove(nameof(Job.Company));
            ModelState.Remove(nameof(Job.DatePosted));
            ModelState.Remove(nameof(Job.JobStatus));

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue ||
                companyID.Value <= 0)
            {
                ViewBag.Company =
                    GetCompanyProfile(userID);

                ViewBag.Qualification =
                    qualification;

                LoadJobFormOptions();

                ModelState.AddModelError(
                    "",
                    "Your recruiter account is not linked to a company. Please contact your Company Administrator.");

                return View(job);
            }

            qualification =
                qualification?.Trim();

            job.JobTitle =
                job.JobTitle?.Trim() ?? "";

            job.EmploymentType =
                job.EmploymentType?.Trim() ?? "";

            job.ExperienceLevel =
                job.ExperienceLevel?.Trim() ?? "";

            job.Province =
                job.Province?.Trim() ?? "";

            job.City =
                job.City?.Trim() ?? "";

            job.Salary =
                job.Salary?.Trim() ?? "";

            job.JobDescription =
                job.JobDescription?.Trim() ?? "";

            job.Requirements =
                job.Requirements?.Trim() ?? "";

            job.Skills =
                job.JobDescription.Length > 2000
                    ? job.JobDescription.Substring(0, 2000)
                    : job.JobDescription;

            ValidateQualification(
                qualification);

            ValidateJob(
                job,
                false);

            if (!ModelState.IsValid)
            {
                ViewBag.Company =
                    GetCompanyProfile(userID);

                ViewBag.Qualification =
                    qualification;

                LoadJobFormOptions();

                return View(job);
            }

            try
            {
                var qualificationEntity =
                    GetOrCreateQualification(
                        qualification);

                job.Qualification =
                    qualificationEntity;

                job.QualificationID =
                    qualificationEntity?.QualificationID;

                job.JobID = 0;

                job.RecruiterID =
                    userID;

                job.CompanyID =
                    companyID.Value;

                job.DatePosted =
                    DateTime.Now;

                job.JobStatus =
                    "Active";

                db.Jobs.Add(job);

                db.SaveChanges();

                TempData["Success"] =
                    "Job published successfully.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Company =
                    GetCompanyProfile(userID);

                ViewBag.Qualification =
                    qualification;

                LoadJobFormOptions();

                string error =
                    ex.InnerException?.Message ??
                    ex.Message;

                ModelState.AddModelError(
                    "",
                    "The job could not be published. Database error: " +
                    error);

                return View(job);
            }
            catch (Exception ex)
            {
                ViewBag.Company =
                    GetCompanyProfile(userID);

                ViewBag.Qualification =
                    qualification;

                LoadJobFormOptions();

                ModelState.AddModelError(
                    "",
                    "The job could not be published. " +
                    ex.Message);

                return View(job);
            }
        }

        // =========================================================
        // MANAGE JOBS / MY JOBS
        // =========================================================

        [HttpGet]
        public IActionResult ManageJobs()
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            if (userID <= 0)
                return RedirectToAction(
                    "Login",
                    "Account");

            var recruiter = db.Users
                .AsNoTracking()
                .FirstOrDefault(x =>
                    x.UserID == userID);

            if (recruiter == null)
                return RedirectToAction(
                    "Login",
                    "Account");

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue)
            {
                TempData["Error"] =
                    "Your recruiter account is not linked to a company.";

                return RedirectToAction(
                    nameof(Dashboard));
            }

            int recruiterCompanyID =
                companyID.Value;

            ViewBag.UserName =
                recruiter.FullName;

            ViewBag.Company =
                GetCompanyProfile(userID);

            var jobs =
                db.Jobs
                    .AsNoTracking()
                    .Where(x =>
                        x.RecruiterID == userID &&
                        x.CompanyID == recruiterCompanyID)
                    .OrderByDescending(x => x.DatePosted)
                    .ToList();

            // Counters removed.
            // The view should display the jobs directly.

            return View(jobs);
        }

        // =========================================================
        // EDIT JOB - GET
        // =========================================================

        [HttpGet]
        public IActionResult EditJob(int id)
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            if (userID <= 0)
                return RedirectToAction(
                    "Login",
                    "Account");

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue)
            {
                TempData["Error"] =
                    "Your recruiter account is not linked to a company.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }

            int recruiterCompanyID =
                companyID.Value;

            var job =
                db.Jobs
                    .AsNoTracking()
                    .Include(x => x.Qualification)
                    .FirstOrDefault(x =>
                        x.JobID == id &&
                        x.RecruiterID == userID &&
                        x.CompanyID == recruiterCompanyID);

            if (job == null)
            {
                TempData["Error"] =
                    "Job not found or you do not have permission to edit this job.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }

            ViewBag.Company =
                GetCompanyProfile(userID);

            ViewBag.Qualification =
                job.Qualification?.Name ?? "";

            LoadJobFormOptions();

            return View(job);
        }

        // =========================================================
        // EDIT JOB - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditJob(
            Job model,
            string? qualification)
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            if (userID <= 0)
                return RedirectToAction(
                    "Login",
                    "Account");

            if (model.JobID <= 0)
            {
                TempData["Error"] =
                    "Invalid job ID.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue)
            {
                TempData["Error"] =
                    "Your recruiter account is not linked to a company.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }

            int recruiterCompanyID =
                companyID.Value;

            var job =
                db.Jobs
                    .FirstOrDefault(x =>
                        x.JobID == model.JobID &&
                        x.RecruiterID == userID &&
                        x.CompanyID == recruiterCompanyID);

            if (job == null)
            {
                TempData["Error"] =
                    "Job not found or you do not have permission to edit this job.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }

            // Qualification remains optional.
            ModelState.Remove(
                nameof(Job.QualificationID));

            ModelState.Remove(
                nameof(Job.Qualification));

            ModelState.Remove(
                nameof(Job.Skills));

            ModelState.Remove(
                nameof(Job.JobID));

            ModelState.Remove(
                nameof(Job.RecruiterID));

            ModelState.Remove(
                nameof(Job.Recruiter));

            ModelState.Remove(
                nameof(Job.CompanyID));

            ModelState.Remove(
                nameof(Job.Company));

            ModelState.Remove(
                nameof(Job.DatePosted));

            qualification =
                qualification?.Trim();

            model.JobTitle =
                model.JobTitle?.Trim() ?? "";

            model.EmploymentType =
                model.EmploymentType?.Trim() ?? "";

            model.ExperienceLevel =
                model.ExperienceLevel?.Trim() ?? "";

            model.Province =
                model.Province?.Trim() ?? "";

            model.City =
                model.City?.Trim() ?? "";

            model.Salary =
                model.Salary?.Trim() ?? "";

            model.JobDescription =
                model.JobDescription?.Trim() ?? "";

            model.Requirements =
                model.Requirements?.Trim() ?? "";

            model.JobStatus =
                model.JobStatus?.Trim() ?? "";

            model.Skills =
                model.JobDescription.Length > 2000
                    ? model.JobDescription.Substring(0, 2000)
                    : model.JobDescription;

            ModelState.Remove(
                nameof(Job.Skills));

            ValidateQualification(
                qualification);

            ValidateJob(
                model,
                true);

            if (!ModelState.IsValid)
            {
                model.JobID =
                    job.JobID;

                model.RecruiterID =
                    job.RecruiterID;

                model.CompanyID =
                    job.CompanyID;

                model.DatePosted =
                    job.DatePosted;

                ViewBag.Company =
                    GetCompanyProfile(userID);

                ViewBag.Qualification =
                    qualification;

                LoadJobFormOptions();

                return View(model);
            }

            try
            {
                var qualificationEntity =
                    GetOrCreateQualification(
                        qualification);

                job.Qualification =
                    qualificationEntity;

                job.QualificationID =
                    qualificationEntity?.QualificationID;

                job.JobTitle =
                    model.JobTitle;

                job.EmploymentType =
                    model.EmploymentType;

                job.ExperienceLevel =
                    model.ExperienceLevel;

                job.Province =
                    model.Province;

                job.City =
                    model.City;

                job.Salary =
                    model.Salary;

                job.ClosingDate =
                    model.ClosingDate;

                job.JobDescription =
                    model.JobDescription;

                job.Requirements =
                    model.Requirements;

                job.Skills =
                    model.JobDescription.Length > 2000
                        ? model.JobDescription.Substring(0, 2000)
                        : model.JobDescription;

                job.JobStatus =
                    string.Equals(
                        model.JobStatus,
                        "Closed",
                        StringComparison.OrdinalIgnoreCase)
                        ? "Closed"
                        : "Active";

                db.SaveChanges();

                TempData["Success"] =
                    job.JobStatus == "Closed"
                        ? "Job closed successfully."
                        : "Job updated successfully.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }
            catch (DbUpdateException ex)
            {
                ViewBag.Company =
                    GetCompanyProfile(userID);

                ViewBag.Qualification =
                    qualification;

                LoadJobFormOptions();

                string error =
                    ex.InnerException?.Message ??
                    ex.Message;

                ModelState.AddModelError(
                    "",
                    "The job could not be updated. Database error: " +
                    error);

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Company =
                    GetCompanyProfile(userID);

                ViewBag.Qualification =
                    qualification;

                LoadJobFormOptions();

                ModelState.AddModelError(
                    "",
                    "The job could not be updated. " +
                    ex.Message);

                return View(model);
            }
        }

        // =========================================================
        // DELETE JOB - NOT ALLOWED
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteJob(int id)
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            if (userID <= 0)
                return RedirectToAction(
                    "Login",
                    "Account");

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue)
            {
                TempData["Error"] =
                    "Your recruiter account is not linked to a company.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }

            int recruiterCompanyID =
                companyID.Value;

            var job =
                db.Jobs
                    .AsNoTracking()
                    .FirstOrDefault(x =>
                        x.JobID == id &&
                        x.RecruiterID == userID &&
                        x.CompanyID == recruiterCompanyID);

            if (job == null)
            {
                TempData["Error"] =
                    "Job not found or you do not have permission to manage this job.";

                return RedirectToAction(
                    nameof(ManageJobs));
            }

            TempData["Error"] =
                "Jobs cannot be deleted. Please edit the job and change its status to Closed.";

            return RedirectToAction(
                nameof(ManageJobs));
        }

        // =========================================================
        // JOB VALIDATION
        // =========================================================

        private void ValidateJob(
            Job job,
            bool includeStatus)
        {
            if (string.IsNullOrWhiteSpace(
                    job.JobTitle))
            {
                ModelState.AddModelError(
                    nameof(Job.JobTitle),
                    "Job title is required.");
            }

            var employmentTypes =
                new[]
                {
                    "Full Time",
                    "Part Time",
                    "Internship"
                };

            if (!employmentTypes.Contains(
                    job.EmploymentType,
                    StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    nameof(Job.EmploymentType),
                    "Please select a valid employment type.");
            }

            var experienceLevels =
                new[]
                {
                    "0-1 Years",
                    "1-3 Years",
                    "2-4 Years",
                    "3-5 Years",
                    "4-6 Years",
                    "5-7 Years",
                    "7-10 Years",
                    "10+ Years"
                };

            if (!experienceLevels.Contains(
                    job.ExperienceLevel,
                    StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    nameof(Job.ExperienceLevel),
                    "Please select a valid experience level.");
            }

            if (string.IsNullOrWhiteSpace(
                    job.Province))
            {
                ModelState.AddModelError(
                    nameof(Job.Province),
                    "Province is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    job.City))
            {
                ModelState.AddModelError(
                    nameof(Job.City),
                    "City is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    job.Salary))
            {
                ModelState.AddModelError(
                    nameof(Job.Salary),
                    "Salary is required.");
            }

            if (string.IsNullOrWhiteSpace(
                    job.JobDescription))
            {
                ModelState.AddModelError(
                    nameof(Job.JobDescription),
                    "Job description is required.");
            }
            else if (job.JobDescription.Length < 20)
            {
                ModelState.AddModelError(
                    nameof(Job.JobDescription),
                    "Job description must be at least 20 characters.");
            }
            else if (job.JobDescription.Length > 5000)
            {
                ModelState.AddModelError(
                    nameof(Job.JobDescription),
                    "Job description cannot exceed 5000 characters.");
            }

            if (string.IsNullOrWhiteSpace(
                    job.Requirements))
            {
                ModelState.AddModelError(
                    nameof(Job.Requirements),
                    "Job requirements are required.");
            }
            else if (job.Requirements.Length < 5)
            {
                ModelState.AddModelError(
                    nameof(Job.Requirements),
                    "Job requirements must be at least 5 characters.");
            }
            else if (job.Requirements.Length > 3000)
            {
                ModelState.AddModelError(
                    nameof(Job.Requirements),
                    "Job requirements cannot exceed 3000 characters.");
            }

            // =====================================================
            // CLOSING DATE
            // =====================================================
            // User requested the date to be greater than yesterday.
            // Therefore today is valid, tomorrow is valid, etc.
            // Any date before today is rejected.

            if (job.ClosingDate == default(DateTime))
            {
                ModelState.AddModelError(
                    nameof(Job.ClosingDate),
                    "Closing date is required.");
            }
            else if (job.ClosingDate.Date < DateTime.Today)
            {
                bool closingJob =
                    includeStatus &&
                    string.Equals(
                        job.JobStatus,
                        "Closed",
                        StringComparison.OrdinalIgnoreCase);

                if (!closingJob)
                {
                    ModelState.AddModelError(
                        nameof(Job.ClosingDate),
                        "Closing date must be today or a future date.");
                }
            }

            if (includeStatus)
            {
                var statuses =
                    new[]
                    {
                        "Active",
                        "Closed"
                    };

                if (!statuses.Contains(
                        job.JobStatus,
                        StringComparer.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        nameof(Job.JobStatus),
                        "Please select Active or Closed.");
                }
            }
        }

        // =========================================================
        // APPLICANTS
        // =========================================================

        [HttpGet]
        public IActionResult Applicants()
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue)
            {
                TempData["Error"] =
                    "Your recruiter account is not linked to a company.";

                return RedirectToAction(
                    nameof(Dashboard));
            }

            int recruiterCompanyID =
                companyID.Value;

            var applications =
                db.Applications
                    .Include(x => x.Job)
                    .Include(x => x.Jobseeker)
                    .Where(x =>
                        x.Job != null &&
                        x.Job.RecruiterID == userID &&
                        x.Job.CompanyID == recruiterCompanyID)
                    .OrderByDescending(
                        x => x.ApplicationDate)
                    .ToList();

            ViewBag.Company =
                GetCompanyProfile(userID);

            return View(applications);
        }

        // =========================================================
        // GENERATE AI MESSAGE
        // =========================================================
        //
        // This replaces:
        // Schedule Interview
        //
        // It DOES NOT change the application status.
        // It simply generates a personalised message that the
        // recruiter can use when communicating with the applicant.
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateMessage(
            int id,
            string? messageType)
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            int? companyID =
                GetRecruiterCompanyID(userID);

            if (!companyID.HasValue)
            {
                TempData["Error"] =
                    "Your recruiter account is not linked to a company.";

                return RedirectToAction(
                    nameof(Applicants));
            }

            int recruiterCompanyID =
                companyID.Value;

            var application =
                db.Applications
                    .Include(x => x.Job)
                    .Include(x => x.Jobseeker)
                    .FirstOrDefault(x =>
                        x.ApplicationID == id &&
                        x.Job != null &&
                        x.Job.RecruiterID == userID &&
                        x.Job.CompanyID == recruiterCompanyID);

            if (application == null)
            {
                TempData["Error"] =
                    "Application could not be found.";

                return RedirectToAction(
                    nameof(Applicants));
            }

            string applicantName =
                application.Jobseeker?.FullName ??
                "Applicant";

            string jobTitle =
                application.Job?.JobTitle ??
                "the position";

            string status =
                application.ApplicationStatus ??
                "Pending";

            messageType =
                string.IsNullOrWhiteSpace(messageType)
                    ? "Application Update"
                    : messageType.Trim();

            string recruiterMessage =
                messageType.ToLowerInvariant()
                switch
                {
                    "followup" =>
                        "Please provide a professional follow-up message to the applicant regarding their application.",

                    "shortlist" =>
                        "Please provide a professional message informing the applicant that their application is being considered for the next stage.",

                    "accepted" =>
                        "Please provide a professional message informing the applicant that their application has been successful and explaining the next steps.",

                    "rejected" =>
                        "Please provide a professional and respectful message informing the applicant that their application was unsuccessful.",

                    _ =>
                        "Please provide a professional personalised message to the applicant regarding their job application."
                };

            string aiMessage;

            try
            {
                aiMessage =
                    await aiEmailService
                        .GenerateStatusMessageAsync(
                            applicantName,
                            jobTitle,
                            status,
                            recruiterMessage);
            }
            catch
            {
                aiMessage =
                    $"Dear {applicantName},\n\n" +
                    $"Thank you for your application for the {jobTitle} position. " +
                    "We appreciate your interest and will be in touch regarding the next steps.\n\n" +
                    "Kind regards,\n" +
                    "Recruitment Team";
            }

            // Save the generated message so that it is available
            // from the application record.
            application.DecisionMessage =
                aiMessage;

            application.DecisionDate =
                DateTime.Now;

            try
            {
                db.SaveChanges();

                TempData["GeneratedMessage"] =
                    aiMessage;

                TempData["Success"] =
                    "AI message generated successfully.";
            }
            catch
            {
                TempData["Error"] =
                    "The AI message was generated but could not be saved.";
            }

            return RedirectToAction(
                nameof(Applicants));
        }

        // =========================================================
        // NOTIFICATION
        // =========================================================

        private void CreateNotification(
            int? userID,
            string title,
            string message)
        {
            if (!userID.HasValue ||
                userID.Value <= 0)
            {
                return;
            }

            var notification =
                new Notification
                {
                    UserID = userID.Value,
                    Title = title,
                    Message = message,
                    IsRead = false,
                    DateCreated = DateTime.Now
                };

            db.Notifications.Add(
                notification);
        }

        // =========================================================
        // PROFILE - GET
        // =========================================================

        [HttpGet]
        public IActionResult Profile()
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            var profile =
                db.RecruiterProfiles
                    .FirstOrDefault(x =>
                        x.UserID == userID);

            if (profile == null)
            {
                profile =
                    new RecruiterProfile
                    {
                        UserID = userID
                    };

                db.RecruiterProfiles.Add(
                    profile);

                db.SaveChanges();
            }

            return View(profile);
        }

        // =========================================================
        // PROFILE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(
            RecruiterProfile model)
        {
            if (!IsRecruiter())
                return RedirectToAction(
                    "Login",
                    "Account");

            int userID = GetUserID();

            ModelState.Remove(
                nameof(RecruiterProfile.UserID));

            ModelState.Remove(
                nameof(RecruiterProfile.User));

            model.CompanyName =
                model.CompanyName?.Trim() ??
                "";

            model.CompanyDescription =
                string.IsNullOrWhiteSpace(
                    model.CompanyDescription)
                    ? null
                    : model.CompanyDescription.Trim();

            model.Website =
                string.IsNullOrWhiteSpace(
                    model.Website)
                    ? null
                    : model.Website.Trim();

            if (string.IsNullOrWhiteSpace(
                    model.CompanyName))
            {
                ModelState.AddModelError(
                    nameof(
                        RecruiterProfile.CompanyName),
                    "Company name is required.");
            }

            if (!ModelState.IsValid)
            {
                model.UserID =
                    userID;

                return View(model);
            }

            var profile =
                db.RecruiterProfiles
                    .FirstOrDefault(x =>
                        x.UserID == userID);

            if (profile == null)
            {
                profile =
                    new RecruiterProfile
                    {
                        UserID = userID
                    };

                db.RecruiterProfiles.Add(
                    profile);
            }

            profile.CompanyName =
                model.CompanyName;

            profile.CompanyDescription =
                model.CompanyDescription;

            profile.Website =
                model.Website;

            try
            {
                db.SaveChanges();

                TempData["Success"] =
                    "Company profile updated successfully.";

                return RedirectToAction(
                    nameof(Profile));
            }
            catch (DbUpdateException ex)
            {
                ViewBag.DbError =
                    ex.InnerException?.Message ??
                    ex.Message;

                ModelState.AddModelError(
                    "",
                    "The company profile could not be saved.");

                model.UserID =
                    userID;

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.DbError =
                    ex.InnerException?.Message ??
                    ex.Message;

                ModelState.AddModelError(
                    "",
                    "An unexpected error occurred while saving the profile.");

                model.UserID =
                    userID;

                return View(model);
            }
        }
    }
}

