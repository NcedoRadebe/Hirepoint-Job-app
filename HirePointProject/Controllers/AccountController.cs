using HirePoint.Data;
using HirePoint.Helpers;
using HirePoint.Models;
using HirePoint.Services;
using HirePoint.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HirePoint.Controllers
{
    public class AccountController : Controller
    {
        private readonly HirePointDbContext db;
        private readonly IEmailService emailService;

    public AccountController(
        HirePointDbContext context,
        IEmailService emailService)
        {
            db = context;
            this.emailService = emailService;
        }


        // =========================================================
        // LOGIN - GET
        // =========================================================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        // =========================================================
        // LOGIN - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            SessionHelper.ClearSession(HttpContext.Session);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email =
                (model.Email ?? string.Empty)
                    .Trim()
                    .ToLowerInvariant();

            // Do NOT trim passwords.
            string enteredPassword =
                model.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(enteredPassword))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Email or password is incorrect.");

                return View(model);
            }

            var user =
                db.Users
                    .AsNoTracking()
                    .FirstOrDefault(x =>
                        x.Email.ToLower() == email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Email or password is incorrect.");

                return View(model);
            }

            string passwordHash =
                HashPassword(enteredPassword);

            bool passwordCorrect =
                string.Equals(
                    user.PasswordHash?.Trim(),
                    passwordHash,
                    StringComparison.OrdinalIgnoreCase);

            if (!passwordCorrect)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Email or password is incorrect.");

                return View(model);
            }


            // =====================================================
            // RECRUITER APPROVAL
            // =====================================================

            string userType =
                (user.UserType ?? string.Empty).Trim();

            bool isRecruiter =
                userType.Equals(
                    "Recruiter",
                    StringComparison.OrdinalIgnoreCase);

            if (isRecruiter && !user.IsApproved)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your recruiter account is awaiting approval by a Company Administrator.");

                return View(model);
            }


            // =====================================================
            // ACCOUNT STATUS
            // =====================================================

            string accountStatus =
                (user.Status ?? string.Empty).Trim();

            if (!accountStatus.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (isRecruiter &&
                    accountStatus.Equals(
                        "Rejected",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Your recruiter application has been rejected by the Company Administrator.");

                    return View(model);
                }

                if (isRecruiter &&
                    accountStatus.Equals(
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Your recruiter account is awaiting approval by a Company Administrator.");

                    return View(model);
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Your account is not active.");

                return View(model);
            }


            // =====================================================
            // EMAIL VERIFICATION
            // =====================================================

            if (!user.EmailConfirmed)
            {
                TempData["Error"] =
                    "Please verify your email before logging in.";

                return RedirectToAction(
                    "VerifyEmail",
                    new
                    {
                        email = user.Email
                    });
            }


            // =====================================================
            // CREATE SESSION
            // =====================================================

            SessionHelper.SetUserSession(
                HttpContext.Session,
                user.UserID,
                user.UserType,
                user.FullName);


            // =====================================================
            // ADMIN
            // =====================================================

            if (userType.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(
                    "Dashboard",
                    "Admin");
            }


            // =====================================================
            // RECRUITER
            // =====================================================

            if (isRecruiter)
            {
                return RedirectToAction(
                    "Dashboard",
                    "Recruiter");
            }


            // =====================================================
            // JOBSEEKER
            // =====================================================

            if (userType.Equals(
                    "Jobseeker",
                    StringComparison.OrdinalIgnoreCase))
            {
                // -------------------------------------------------
                // CHECK WHETHER THE JOBSEEKER HAS SELECTED
                // ANY JOB INTERESTS.
                // -------------------------------------------------

                bool hasInterests =
                    db.JobseekerInterests
                        .AsNoTracking()
                        .Any(x =>
                            x.JobseekerId == user.UserID);

                // -------------------------------------------------
                // FIRST LOGIN / NO INTERESTS
                // -------------------------------------------------
                //
                // Send the jobseeker to the separate Interests page
                // before allowing access to the dashboard.
                //
                // -------------------------------------------------

                if (!hasInterests)
                {
                    return RedirectToAction(
                        "Interests",
                        "Jobseeker");
                }

                // -------------------------------------------------
                // INTERESTS ALREADY EXIST
                // -------------------------------------------------

                return RedirectToAction(
                    "Dashboard",
                    "Jobseeker");
            }


            // =====================================================
            // INVALID ACCOUNT TYPE
            // =====================================================

            SessionHelper.ClearSession(
                HttpContext.Session);

            ModelState.AddModelError(
                string.Empty,
                "Invalid account type.");

            return View(model);
        }


        // =========================================================
        // REGISTER - GET
        // =========================================================

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel();

            LoadProvinces(model);

            return View(model);
        }


        // =========================================================
        // REGISTER - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadProvinces(model);
                return View(model);
            }


            // =====================================================
            // PROVINCE
            // =====================================================

            if (!model.ProvinceID.HasValue ||
                model.ProvinceID.Value <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.ProvinceID),
                    "Please select a province.");

                LoadProvinces(model);
                return View(model);
            }


            // =====================================================
            // GENDER
            // =====================================================

            if (string.IsNullOrWhiteSpace(model.Gender))
            {
                ModelState.AddModelError(
                    nameof(model.Gender),
                    "Please select your gender.");

                LoadProvinces(model);
                return View(model);
            }


            // =====================================================
            // PHONE NUMBER
            // =====================================================

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                ModelState.AddModelError(
                    nameof(model.PhoneNumber),
                    "Phone number is required.");

                LoadProvinces(model);
                return View(model);
            }


            // =====================================================
            // ACCOUNT TYPE
            // =====================================================

            if (string.IsNullOrWhiteSpace(model.UserType))
            {
                ModelState.AddModelError(
                    nameof(model.UserType),
                    "Please select an account type.");

                LoadProvinces(model);
                return View(model);
            }


            bool isRecruiter =
                model.UserType.Equals(
                    "Recruiter",
                    StringComparison.OrdinalIgnoreCase);

            bool isJobseeker =
                model.UserType.Equals(
                    "Jobseeker",
                    StringComparison.OrdinalIgnoreCase);

            if (!isRecruiter && !isJobseeker)
            {
                ModelState.AddModelError(
                    nameof(model.UserType),
                    "Please select Jobseeker or Recruiter.");

                LoadProvinces(model);
                return View(model);
            }


            // =====================================================
            // JOBSEEKER QUALIFICATION
            // =====================================================

            if (isJobseeker)
            {
                string qualification =
                    (model.Qualification ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(qualification))
                {
                    ModelState.AddModelError(
                        nameof(model.Qualification),
                        "Qualification is required.");

                    LoadProvinces(model);
                    return View(model);
                }

                model.Qualification = qualification;
            }


            // =====================================================
            // RECRUITER COMPANY INFORMATION
            // =====================================================

            if (isRecruiter &&
                string.IsNullOrWhiteSpace(model.CompanyName))
            {
                ModelState.AddModelError(
                    nameof(model.CompanyName),
                    "Company name is required for recruiters.");

                LoadProvinces(model);
                return View(model);
            }


            try
            {
                // =================================================
                // NORMALISE EMAIL
                // =================================================

                string email =
                    (model.Email ?? string.Empty)
                        .Trim()
                        .ToLowerInvariant();


                // =================================================
                // CHECK EMAIL
                // =================================================

                bool emailExists =
                    db.Users
                        .AsNoTracking()
                        .Any(x =>
                            x.Email.ToLower() == email);

                if (emailExists)
                {
                    ModelState.AddModelError(
                        nameof(model.Email),
                        "An account with this email already exists.");

                    LoadProvinces(model);
                    return View(model);
                }


                // =================================================
                // PASSWORD
                // =================================================

                // Do NOT trim passwords.
                string password =
                    model.Password ?? string.Empty;

                if (string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError(
                        nameof(model.Password),
                        "Password is required.");

                    LoadProvinces(model);
                    return View(model);
                }


                // =================================================
                // CREATE USER
                // =================================================

                User user = new User
                {
                    FullName =
                        (model.FullName ?? string.Empty).Trim(),

                    Email =
                        email,

                    PasswordHash =
                        HashPassword(password),

                    PhoneNumber =
                        (model.PhoneNumber ?? string.Empty).Trim(),

                    Gender =
                        (model.Gender ?? string.Empty).Trim(),

                    ProvinceID =
                        model.ProvinceID.Value,

                    UserType =
                        model.UserType.Trim(),

                    Status =
                        isRecruiter
                            ? "Pending"
                            : "Active",

                    IsApproved =
                        isRecruiter
                            ? false
                            : true,

                    CompanyID =
                        null,

                    EmailConfirmed =
                        false,

                    DateCreated =
                        DateTime.UtcNow
                };


                // =================================================
                // SAVE USER
                // =================================================

                db.Users.Add(user);

                db.SaveChanges();


                // =================================================
                // CREATE RECRUITER PROFILE
                // =================================================

                if (isRecruiter)
                {
                    RecruiterProfile profile =
                        new RecruiterProfile
                        {
                            UserID =
                                user.UserID,

                            CompanyName =
                                model.CompanyName?
                                    .Trim()
                                    ?? string.Empty,

                            CompanyDescription =
                                string.IsNullOrWhiteSpace(
                                    model.CompanyDescription)
                                    ? null
                                    : model.CompanyDescription.Trim(),

                            Website =
                                string.IsNullOrWhiteSpace(
                                    model.Website)
                                    ? null
                                    : model.Website.Trim()
                        };

                    db.RecruiterProfiles.Add(profile);
                }


                // =================================================
                // CREATE JOBSEEKER PROFILE
                // =================================================

                if (isJobseeker)
                {
                    JobseekerProfile profile =
                        new JobseekerProfile
                        {
                            UserID =
                                user.UserID,

                            Qualification =
                                model.Qualification!.Trim(),

                            Skills =
                                string.Empty,

                            Education =
                                string.Empty,

                            Experience =
                                string.Empty
                        };

                    db.JobseekerProfiles.Add(profile);
                }


                // =================================================
                // SAVE PROFILE
                // =================================================

                db.SaveChanges();


                // =================================================
                // SUCCESS MESSAGE
                // =================================================

                if (isRecruiter)
                {
                    TempData["Success"] =
                        "Your recruiter account has been created successfully. " +
                        "Please verify your email. After verification, " +
                        "a Company Administrator must approve your recruiter account before you can sign in.";
                }
                else
                {
                    TempData["Success"] =
                        "Your account has been created successfully. " +
                        "Please verify your email.";
                }

                return RedirectToAction(
                    "VerifyEmail",
                    new
                    {
                        email = user.Email
                    });
            }
            catch (DbUpdateException)
            {
                TempData["Error"] =
                    "We could not create your account. " +
                    "Please check your information and try again.";

                LoadProvinces(model);

                return View(model);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "An unexpected error occurred while creating your account.";

                LoadProvinces(model);

                return View(model);
            }
        }


        // =========================================================
        // FORGOT PASSWORD - GET
        // =========================================================

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(
                new ForgotPasswordViewModel());
        }


        // =========================================================
        // FORGOT PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email =
                (model.Email ?? string.Empty)
                    .Trim()
                    .ToLowerInvariant();

            var user =
                db.Users.FirstOrDefault(
                    x =>
                        x.Email != null &&
                        x.Email.ToLower() == email);

            if (user == null)
            {
                TempData["Success"] =
                    "If an account exists for that email address, " +
                    "a password reset link has been sent.";

                return RedirectToAction("ForgotPassword");
            }

            string accountStatus =
                (user.Status ?? string.Empty).Trim();

            if (!accountStatus.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                TempData["Success"] =
                    "If an account exists for that email address, " +
                    "a password reset link has been sent.";

                return RedirectToAction("ForgotPassword");
            }

            if (user.UserType != null &&
                user.UserType.Equals(
                    "Recruiter",
                    StringComparison.OrdinalIgnoreCase) &&
                !user.IsApproved)
            {
                TempData["Success"] =
                    "If an account exists for that email address, " +
                    "a password reset link has been sent.";

                return RedirectToAction("ForgotPassword");
            }

            if (!user.EmailConfirmed)
            {
                TempData["Success"] =
                    "If an account exists for that email address, " +
                    "a password reset link has been sent.";

                return RedirectToAction("ForgotPassword");
            }

            var oldTokens =
                db.PasswordResetTokens
                    .Where(x =>
                        x.UserID == user.UserID &&
                        x.UsedAt == null)
                    .ToList();

            db.PasswordResetTokens.RemoveRange(oldTokens);

            byte[] tokenBytes =
                RandomNumberGenerator.GetBytes(32);

            string rawToken =
                Convert.ToBase64String(tokenBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");

            string tokenHash =
                Convert.ToHexString(
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(rawToken)));

            var resetToken =
                new PasswordResetToken
                {
                    UserID = user.UserID,
                    TokenHash = tokenHash,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    CreatedAt = DateTime.UtcNow,
                    UsedAt = null
                };

            db.PasswordResetTokens.Add(resetToken);

            db.SaveChanges();

            string resetLink =
                Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        email = user.Email,
                        token = rawToken
                    },
                    Request.Scheme)!;

            try
            {
                await emailService.SendPasswordResetEmailAsync(
                    user.Email,
                    resetLink);

                TempData["Success"] =
                    "If an account exists for that email address, " +
                    "a password reset link has been sent.";
            }
            catch (Exception ex)
            {
                resetToken.UsedAt =
                    DateTime.UtcNow;

                db.SaveChanges();

                TempData["Error"] =
                    "Email error: " + ex.Message;
            }

            return RedirectToAction("ForgotPassword");
        }


        // =========================================================
        // RESET PASSWORD - GET
        // =========================================================

        [HttpGet]
        public IActionResult ResetPassword(
            string email,
            string token)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token))
            {
                return View(
                    "ResetPasswordExpired");
            }

            string tokenHash =
                Convert.ToHexString(
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(token)));

            var resetToken =
                db.PasswordResetTokens
                    .Include(x => x.User)
                    .FirstOrDefault(
                        x =>
                            x.TokenHash == tokenHash &&
                            x.UsedAt == null &&
                            x.ExpiresAt > DateTime.UtcNow &&
                            x.User.Email == email);

            if (resetToken == null)
            {
                return View(
                    "ResetPasswordExpired");
            }

            var model =
                new ResetPasswordViewModel
                {
                    Email =
                        email,

                    Token =
                        token
                };

            return View(model);
        }


        // =========================================================
        // RESET PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string tokenHash =
                Convert.ToHexString(
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(
                            model.Token)));

            var resetToken =
                db.PasswordResetTokens
                    .Include(x => x.User)
                    .FirstOrDefault(
                        x =>
                            x.TokenHash == tokenHash &&
                            x.UsedAt == null &&
                            x.ExpiresAt > DateTime.UtcNow &&
                            x.User.Email == model.Email);

            if (resetToken == null)
            {
                return View(
                    "ResetPasswordExpired");
            }

            var user =
                resetToken.User;

            user.PasswordHash =
                HashPassword(
                    model.NewPassword);

            resetToken.UsedAt =
                DateTime.UtcNow;

            db.SaveChanges();

            TempData["Success"] =
                "Your password has been reset successfully. " +
                "You can now login with your new password.";

            return RedirectToAction(
                "Login");
        }


        // =========================================================
        // VERIFY EMAIL - GET
        // =========================================================

        [HttpGet]
        public IActionResult VerifyEmail(
            string? email = null)
        {
            ViewBag.Email = email;

            return View();
        }


        // =========================================================
        // VERIFY EMAIL - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyEmail(
            VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email =
                (model.Email ?? string.Empty)
                    .Trim()
                    .ToLowerInvariant();

            var user =
                db.Users.FirstOrDefault(
                    x =>
                        x.Email.ToLower() == email);

            if (user == null)
            {
                TempData["Error"] =
                    "We could not find an account with that email address.";

                return View(model);
            }

            if (user.EmailConfirmed)
            {
                TempData["Success"] =
                    "Your email is already verified.";

                return RedirectToAction(
                    "Login");
            }

            user.EmailConfirmed =
                true;

            db.SaveChanges();


            // =====================================================
            // RECRUITER APPROVAL
            // =====================================================

            if (user.UserType != null &&
                user.UserType.Equals(
                    "Recruiter",
                    StringComparison.OrdinalIgnoreCase) &&
                !user.IsApproved)
            {
                TempData["Success"] =
                    "Your email has been verified successfully. " +
                    "Your recruiter account is now awaiting approval by a Company Administrator.";

                return RedirectToAction(
                    "Login");
            }


            // =====================================================
            // SUCCESS
            // =====================================================

            TempData["Success"] =
                "Your email has been verified successfully. " +
                "You can now login.";

            return RedirectToAction(
                "Login");
        }


        // =========================================================
        // LOGOUT
        // =========================================================

        [HttpGet]
        public IActionResult Logout()
        {
            SessionHelper.ClearSession(
                HttpContext.Session);

            return RedirectToAction(
                "Login");
        }


        // =========================================================
        // LOAD PROVINCES
        // =========================================================

        private void LoadProvinces(
            RegisterViewModel model)
        {
            model.Provinces =
                db.Provinces
                    .AsNoTracking()
                    .OrderBy(x =>
                        x.ProvinceName)
                    .Select(x =>
                        new SelectListItem
                        {
                            Value =
                                x.ProvinceID.ToString(),

                            Text =
                                x.ProvinceName
                        })
                    .ToList();
        }


        // =========================================================
        // PASSWORD HASH
        // =========================================================

        private string HashPassword(
            string password)
        {
            using SHA256 sha =
                SHA256.Create();

            byte[] bytes =
                sha.ComputeHash(
                    Encoding.UTF8.GetBytes(
                        password));

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
