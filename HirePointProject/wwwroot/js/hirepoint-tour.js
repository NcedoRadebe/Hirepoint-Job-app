/* ============================================================
   HIREPOINT TOUR SYSTEM
   ============================================================ */

(function (window, document) {

    "use strict";


    // ============================================================
    // CONFIGURATION
    // ============================================================

    const TOUR_STORAGE_KEY = "hirepointTourState";


    // ============================================================
    // PAGE ROUTES
    // ============================================================

    const JOBSEEKER_PAGE_ROUTES = {

        JobseekerDashboard: "/Jobseeker/Dashboard",

        JobseekerJobs: "/Jobseeker/Jobs",

        JobseekerApplications: "/Jobseeker/Applications",

        JobseekerNotifications: "/Jobseeker/Notifications",

        JobseekerProfile: "/Jobseeker/Profile"

    };


    const RECRUITER_PAGE_ROUTES = {

        RecruiterDashboard: "/Recruiter/Dashboard",

        RecruiterManageJobs: "/Recruiter/ManageJobs",

        RecruiterApplicants: "/Recruiter/Applicants",

        RecruiterProfile: "/Recruiter/Profile",

        // Contextual pages only
        RecruiterPostJob: "/Recruiter/PostJob",

        RecruiterEditJob: "/Recruiter/EditJob",

        RecruiterGenerateMessage: "/Recruiter/GenerateMessage",

        RecruiterGenerateMessagePage: "/Recruiter/GenerateMessagePage"

    };


    const ADMIN_PAGE_ROUTES = {

        AdminDashboard: "/Admin/Dashboard",

        AdminAnalytics: "/Admin/Analytics",

        AdminRecruiterApprovals: "/Admin/RecruiterApprovals",

        AdminJobListings: "/Admin/JobListings",

        AdminManageUsers: "/Admin/ManageUsers",

        AdminProfile: "/Admin/Profile"

    };


    // ============================================================
    // AUTOMATIC TOUR ORDER
    // ============================================================

    const JOBSEEKER_PAGE_ORDER = [

        "JobseekerDashboard",

        "JobseekerJobs",

        "JobseekerApplications",

        "JobseekerNotifications",

        "JobseekerProfile"

    ];


    const RECRUITER_PAGE_ORDER = [

        "RecruiterDashboard",

        "RecruiterManageJobs",

        "RecruiterApplicants",

        "RecruiterProfile"

    ];


    const ADMIN_PAGE_ORDER = [

        "AdminDashboard",

        "AdminAnalytics",

        "AdminRecruiterApprovals",

        "AdminJobListings",

        "AdminManageUsers",

        "AdminProfile"

    ];


    // ============================================================
    // JOBSEEKER TOUR
    // ============================================================

    const JOBSEEKER_TOUR = {


        JobseekerDashboard: [

            {
                selector: "#dashboardHeader",
                title: "Dashboard",
                text: "Welcome to your HirePoint jobseeker dashboard. This is your main overview of your job search activity."
            },

            {
                selector: "#dashboardStats",
                title: "Dashboard Statistics",
                text: "These statistics provide a quick overview of your applications, available opportunities, and job search activity."
            },

            {
                selector: "#recommendedJobs",
                title: "Recommended Jobs",
                text: "These jobs are recommended based on your profile and selected interests."
            },

            {
                selector: "#updateInterestsLink",
                title: "Update Interests",
                text: "Use this option to update the interests that help HirePoint provide relevant job recommendations."
            },

            {
                selector: "#restartJobseekerTour",
                title: "Restart Tour",
                text: "You can use this option to restart the complete jobseeker tour whenever you need help navigating HirePoint."
            }

        ],


        JobseekerJobs: [

            {
                selector: "#findJobsHeader",
                title: "Find Jobs",
                text: "This is where you can search and browse available job opportunities."
            },

            {
                selector: "#jobSearchFilters",
                title: "Search Filters",
                text: "Use these filters to narrow down jobs by title, province, city, date posted, and other available criteria."
            },

            {
                selector: "#jobTitleInput",
                title: "Job Title",
                text: "Enter a job title, keyword, or position you are looking for."
            },

            {
                selector: "#jobProvinceInput",
                title: "Province",
                text: "Select or enter a province to focus your job search on a specific area."
            },

            {
                selector: "#jobCityInput",
                title: "City",
                text: "Use the city field to make your location search more specific."
            },

            {
                selector: "#datePostedFilter",
                title: "Date Posted",
                text: "Filter opportunities according to when they were posted."
            },

            {
                selector: "#sortFilter",
                title: "Sort Jobs",
                text: "Use the sorting option to control how available jobs are displayed."
            },

            {
                selector: "#availableJobs",
                title: "Available Jobs",
                text: "Browse the available job opportunities and select a job to view more details."
            },

            {
                selector: ".tour-job-view",
                title: "View Job",
                text: "Use the job action to open an opportunity and view its full details."

            }

        ],


        JobseekerApplications: [

            {
                selector: ".applications-header",
                title: "Applications",
                text: "This page lets you keep track of the jobs you have applied for."
            },

            {
                selector: ".applications-card-header",
                title: "Application History",
                text: "Your submitted applications are organised here so you can monitor their progress."
            },

            {
                selector: ".applications-table",
                title: "Applications Table",
                text: "This table contains the jobs you have applied for together with important application information."
            },

            {
                selector: [
                    ".application-status",
                    ".applications-empty"
                ],
                title: "Application Status",
                text: "The status shows the current state of your application. If there are no applications, HirePoint will display an appropriate empty-state message."
            }

        ],


        JobseekerNotifications: [

            {
                selector: ".notifications-header",
                title: "Notifications",
                text: "This page contains important notifications and updates relating to your HirePoint account."
            },

            {
                selector: [
                    ".notifications-card",
                    ".notifications-empty"
                ],
                title: "Notification Centre",
                text: "Your notifications are displayed here. If there are no notifications, an empty-state message is shown."
            },

            {
                selector: ".notification-item",
                title: "Notification",
                text: "Each notification contains information about an event or update associated with your account."
            }

        ],


        JobseekerProfile: [

            {
                selector: ".profile-header",
                title: "My Profile",
                text: "This is your profile page where you can manage your personal account information."
            },

            {
                selector: ".profile-card-header",
                title: "Account Information",
                text: "This section contains the information associated with your HirePoint account."
            },

            {
                selector: ".profile-save-btn",
                title: "Save Changes",
                text: "Use this button to save changes made to your profile."
            },

            {
                selector: ".cv-upload-area",
                title: "CV Upload",
                text: "This area allows you to manage your CV and upload your current document."
            },

            {
                selector: ".profile-upload-btn",
                title: "Upload CV",
                text: "Use this button to upload a CV to your HirePoint profile."
            },

            {
                selector: ".profile-cv-btn",
                title: "CV Actions",
                text: "Use the available CV action to view or manage your uploaded CV."
            }

        ]

    };


    // ============================================================
    // RECRUITER TOUR
    // ============================================================

    const RECRUITER_TOUR = {


        RecruiterDashboard: [

            {
                selector: ".dashboard-header",
                title: "Recruiter Dashboard",
                text: "Welcome to your recruiter dashboard. This is your main overview of recruitment activity."
            },

            {
                selector: ".stat-card",
                title: "Recruitment Statistics",
                text: "These cards provide a quick overview of your jobs, applications, and recruitment activity."
            },

            {
                selector: ".dashboard-card",
                title: "Dashboard Information",
                text: "These sections provide additional information about your recruitment activity."
            },

            {
                selector: ".dashboard-table",
                title: "Job Information",
                text: "This table provides information about your posted jobs and their current activity."
            },

            {
                selector: ".active-status",
                title: "Active Jobs",
                text: "This status indicates jobs that are currently active and available to jobseekers."
            },

            {
                selector: ".company-name",
                title: "Company",
                text: "The company name identifies the organisation associated with the recruiter account."
            },

            {
                selector: ".company-info",
                title: "Company Information",
                text: "This section provides additional company information associated with the recruiter."
            },

            {
                selector: ".empty-dashboard-message",
                title: "Dashboard",
                text: "If there is no current activity, this area explains that there is nothing to display."
            }

        ],


        RecruiterManageJobs: [

            {
                selector: ".manage-header",
                title: "Manage Jobs",
                text: "This page allows you to view and manage the jobs you have posted."
            },

            {
                selector: ".jobs-card-header",
                title: "Your Jobs",
                text: "This section contains the job opportunities associated with your recruiter account."
            },

            {
                selector: ".job-item",
                title: "Job Listing",
                text: "Each job listing contains important information about the opportunity."
            },

            {
                selector: ".job-title",
                title: "Job Title",
                text: "This identifies the position you have posted."
            },

            {
                selector: ".job-info",
                title: "Job Information",
                text: "This area contains information such as the job location and other details."
            },

            {
                selector: ".job-description",
                title: "Job Description",
                text: "The job description provides details about the opportunity."
            },

            {
                selector: ".closing-date",
                title: "Closing Date",
                text: "This shows the date on which the job opportunity closes."
            },

            {
                selector: ".status-badge",
                title: "Job Status",
                text: "This badge shows whether the job is active or closed."
            },

            {
                selector: ".actions-area",
                title: "Job Actions",
                text: "Use the available actions to manage your posted job."
            },

            {
                selector: ".edit-btn",
                title: "Edit Job",
                text: "Use Edit to change information about an existing job."
            },

            {
                selector: ".empty-jobs",
                title: "No Jobs",
                text: "If you have not posted any jobs, this area explains that there are currently no jobs to manage."
            }

        ],


        RecruiterApplicants: [

            {
                selector: ".page-header",
                title: "Applicants",
                text: "This page allows you to review applications submitted for your jobs."
            },

            {
                selector: ".total-applications",
                title: "Total Applications",
                text: "This displays the total number of applications currently available for review."
            },

            {
                selector: [
                    ".applicants-card",
                    ".empty-applications"
                ],
                title: "Applications",
                text: "This section contains applications submitted by jobseekers for your opportunities."
            },

            {
                selector: ".applicants-table",
                title: "Applicants Table",
                text: "The table provides applicant information and the current status of each application."
            },

            {
                selector: ".applicant-avatar",
                title: "Applicant",
                text: "The applicant information identifies the jobseeker who submitted the application."
            },

            {
                selector: ".status-badge",
                title: "Application Status",
                text: "This badge shows the current status of the applicant's application."
            },

            {
                selector: ".applicant-actions",
                title: "Applicant Actions",
                text: "Available actions allow you to manage or respond to an applicant."
            },

            {
                selector: ".btn-generate-message",
                title: "Generate Message",
                text: "Use this option to generate an application-related message for the applicant."
            },

            {
                selector: ".empty-applications",
                title: "No Applications",
                text: "If there are no applications, this area explains that there are currently no applicants to review."
            }

        ],


        RecruiterProfile: [

            {
                selector: ".profile-page",
                title: "Recruiter Profile",
                text: "This is your recruiter profile where you can manage your account information."
            },

            {
                selector: ".profile-header",
                title: "Profile Overview",
                text: "This section identifies the profile page and provides information about managing your account."
            },

            {
                selector: ".profile-card",
                title: "Profile Information",
                text: "This card contains the information associated with your recruiter account."
            },

            {
                selector: ".profile-card-header",
                title: "Account Information",
                text: "This section contains your account information."
            },

            {
                selector: ".profile-save-btn",
                title: "Save Changes",
                text: "Use this button to save changes made to your recruiter profile."
            }

        ],


        // ========================================================
        // CONTEXTUAL: POST JOB
        // ========================================================

        RecruiterPostJob: [

            {
                selector: ".page-heading",
                title: "Post a Job",
                text: "Use this page to create and publish a new job opportunity."
            },

            {
                selector: ".job-form-wrapper",
                title: "Job Form",
                text: "Enter the details of the position you want to advertise."
            },

            {
                selector: ".form-top",
                title: "Job Details",
                text: "This section introduces the job posting form and provides information about completing it."
            },

            {
                selector: ".job-form-body",
                title: "Job Information",
                text: "Complete the available fields with accurate information about the job."
            },

            {
                selector: ".form-section",
                title: "Form Sections",
                text: "The job form is organised into sections to make entering information easier."
            },

            {
                selector: ".form-label",
                title: "Form Fields",
                text: "These labels identify the information required for the job posting."
            },

            {
                selector: ".form-control",
                title: "Enter Information",
                text: "Enter the relevant job information into these fields."
            },

            {
                selector: ".form-footer",
                title: "Publish Job",
                text: "Review the information you entered before publishing the job."
            },

            {
                selector: ".publish-button",
                title: "Publish",
                text: "Use Publish to submit the completed job posting."
            },

            {
                selector: ".cancel-button",
                title: "Cancel",
                text: "Use Cancel if you want to leave the form without publishing the job."
            }

        ],


        // ========================================================
        // CONTEXTUAL: EDIT JOB
        // ========================================================

        RecruiterEditJob: [

            {
                selector: ".job-form-header",
                title: "Edit Job",
                text: "This page allows you to update an existing job posting."
            },

            {
                selector: ".job-form-card",
                title: "Job Information",
                text: "The job information is displayed here and can be updated where permitted."
            },

            {
                selector: ".job-form-card-header",
                title: "Job Details",
                text: "This section identifies the information being edited."
            },

            {
                selector: ".job-section-title",
                title: "Job Sections",
                text: "The form is divided into sections to make editing the job easier."
            },

            {
                selector: ".job-form-control",
                title: "Edit Information",
                text: "Update the job information using these form fields."
            },

            {
                selector: ".job-actions",
                title: "Save or Cancel",
                text: "When you have finished editing, save your changes or cancel the operation."
            },

            {
                selector: ".job-save-btn",
                title: "Save Changes",
                text: "Use this button to save your updated job information."
            },

            {
                selector: ".job-cancel-btn",
                title: "Cancel",
                text: "Use Cancel to leave the editing page without saving changes."
            }

        ],


        // ========================================================
        // CONTEXTUAL: GENERATE APPLICATION MESSAGE
        // ========================================================

        RecruiterGenerateMessage: [

            {
                selector: ".container.py-4",
                title: "Generate Message",
                text: "This page allows you to create a message relating to an applicant's application."
            },

            {
                selector: ".card-header",
                title: "Application Message",
                text: "This section contains the tools used to generate and review the applicant message."
            },

            {
                selector: ".card-body",
                title: "Message Details",
                text: "Enter or review the information used to create the application message."
            },

            {
                selector: ".form-control",
                title: "Message Input",
                text: "Use the available field to provide or edit the message information."
            },

            {
                selector: ".form-text",
                title: "Guidance",
                text: "Additional guidance is provided here to help you complete the message."
            },

            {
                selector: ".btn-primary",
                title: "Generate",
                text: "Use this action to generate the application message."
            },

            {
                selector: ".btn-success",
                title: "Confirm",
                text: "Use this action when you are ready to confirm the generated message."
            },

            {
                selector: ".btn-outline-primary",
                title: "Return",
                text: "Use this option to return to the previous application-related page."
            }

        ],


        // ========================================================
        // CONTEXTUAL: GENERATE MESSAGE PAGE
        // ========================================================

        RecruiterGenerateMessagePage: [

            {
                selector: ".container.py-4",
                title: "Generate Message",
                text: "This page helps you create an appropriate message for an applicant."
            },

            {
                selector: ".card",
                title: "Message Generator",
                text: "The message generator contains the information and actions needed to create the message."
            },

            {
                selector: ".card-body",
                title: "Message Details",
                text: "Review the applicant information and complete the message details."
            },

            {
                selector: ".form-control",
                title: "Message Field",
                text: "Use this field to enter or adjust the message content."
            },

            {
                selector: ".btn-primary",
                title: "Generate Message",
                text: "Use this button to generate the message."
            },

            {
                selector: ".btn-success",
                title: "Confirm Message",
                text: "Confirm the generated message when you are satisfied with the content."
            }

        ]

    };


    // ============================================================
    // ADMIN TOUR
    // ============================================================

    const ADMIN_TOUR = {


        // ========================================================
        // ADMIN DASHBOARD
        // ========================================================

        AdminDashboard: [

            {
                selector: ".dashboard-header",
                title: "Admin Dashboard",
                text: "Welcome to the HirePoint administrator dashboard. This is your central overview of platform activity."
            },

            {
                selector: ".stat-card",
                title: "Platform Statistics",
                text: "These cards provide a quick overview of important HirePoint platform statistics."
            },

            {
                selector: ".dashboard-card",
                title: "Dashboard Information",
                text: "These sections provide additional information about activity across the platform."
            },

            {
                selector: ".dashboard-table",
                title: "Platform Activity",
                text: "This area provides information that helps administrators monitor platform activity."
            }

        ],


        // ========================================================
        // ANALYTICS
        // ========================================================

        AdminAnalytics: [

            {
                selector: ".analytics-header",
                title: "Analytics",
                text: "This section gives you an overview of activity and statistics across the HirePoint platform."
            },

            {
                selector: ".analytics-stat-card",
                title: "Platform Statistics",
                text: "These cards show key platform figures such as total users, active users, active jobs, and new users today."
            },

            {
                selector: ".analytics-card",
                title: "Analytics Details",
                text: "These sections provide additional information, including users by province, gender distribution, and user types."
            },

            {
                selector: ".analytics-table",
                title: "Users by Province",
                text: "This table shows how registered users are distributed across provinces."
            },

            {
                selector: ".analytics-info-item",
                title: "User Information",
                text: "These items show additional user statistics such as gender and account type."
            }

        ],


        // ========================================================
        // RECRUITER APPROVALS
        // ========================================================

        AdminRecruiterApprovals: [

            {
                selector: ".recruiter-approvals-header",
                title: "Recruiter Approvals",
                text: "This page allows administrators to review recruiters who are waiting for approval."
            },

            {
                selector: ".recruiter-approvals-notice",
                title: "Administrator Notice",
                text: "This section explains the verification responsibilities available to the system administrator."
            },

            {
                selector: ".recruiter-approvals-card",
                title: "Pending Recruiter",
                text: "Each card contains recruiter, company, and representative information that must be reviewed."
            },

            {
                selector: ".recruiter-approvals-body",
                title: "Recruiter Details",
                text: "Review the recruiter information, company information, and company representative details here."
            },

            {
                selector: ".recruiter-approvals-body form[action*='VerifyRepresentative']",
                title: "Representative Verification",
                text: "Use this action to send or resend the verification email to the company representative."
            },

            {
                selector: ".recruiter-approvals-body form[action*='ApproveRecruiter']",
                title: "Approve Recruiter",
                text: "A recruiter can be approved after the company representative verification has been completed."
            },

            {
                selector: ".recruiter-approvals-body form[action*='RejectRecruiter']",
                title: "Reject Recruiter",
                text: "This action allows the administrator to reject the pending recruiter."
            }

        ],


        // ========================================================
        // JOB LISTINGS
        // ========================================================

        AdminJobListings: [

            {
                selector: ".job-listings-header",
                title: "Job Listings",
                text: "This page allows administrators to view and manage jobs posted on HirePoint."
            },

            {
                selector: ".job-listings-card-header",
                title: "All Job Listings",
                text: "This section contains all jobs currently registered on the platform."
            },

            {
                selector: ".job-listings-table thead",
                title: "Job Information",
                text: "The table displays the company, job name, salary, posting date, location, status, and available actions."
            },

            {
                selector: ".job-title",
                title: "Job Details",
                text: "These fields identify the company and job title."
            },

            {
                selector: ".active-job-status, .other-job-status",
                title: "Job Status",
                text: "This indicates whether a job is currently active or has another status."
            },

            {
                selector: ".edit-job-button",
                title: "Edit Job",
                text: "Use this action to open a job and make changes to its information."
            },

            {
                selector: ".empty-job-listings",
                title: "No Job Listings",
                text: "If there are no jobs, this area explains that no jobs are currently available."
            }

        ],


        // ========================================================
        // MANAGE USERS
        // ========================================================

        AdminManageUsers: [

            {
                selector: ".admin-page-header",
                title: "Manage Users",
                text: "This page allows administrators to view and manage registered HirePoint users."
            },

            {
                selector: ".total-users-badge",
                title: "Total Users",
                text: "This displays the total number of registered users on the platform."
            },

            {
                selector: ".users-card",
                title: "Registered Users",
                text: "This table contains the registered HirePoint users and their account information."
            },

            {
                selector: ".users-table thead",
                title: "User Information",
                text: "The table shows each user's name, email, account type, province, status, and available actions."
            },

            {
                selector: ".user-name",
                title: "User",
                text: "This identifies the registered user."
            },

            {
                selector: ".admin-blue-badge",
                title: "Account Type",
                text: "This shows whether the account belongs to an administrator, recruiter, or jobseeker."
            },

            {
                selector: ".status-active, .status-other",
                title: "Account Status",
                text: "This displays the current status of the user's account."
            },

            {
                selector: [
                    ".edit-user-button",
                    ".protected-user-badge"
                ],
                title: "User Actions",
                text: "Administrators can edit users when permitted. Protected system administrator accounts are identified separately."
            }

        ],


        // ========================================================
        // ADMIN PROFILE
        // ========================================================

        AdminProfile: [

            {
                selector: ".profile-page",
                title: "Admin Profile",
                text: "This is your administrator profile where you can manage your personal account information."
            },

            {
                selector: ".profile-header",
                title: "Profile Overview",
                text: "This section identifies the profile page and explains that you can update your personal account information."
            },

            {
                selector: ".profile-card-header",
                title: "Account Information",
                text: "This section contains the personal information associated with your administrator account."
            },

            {
                selector: ".profile-user-id",
                title: "User ID",
                text: "Your unique HirePoint user ID is displayed here for account identification."
            },

            {
                selector: ".profile-form .form-control",
                title: "Profile Information",
                text: "Use these fields to update your name, email address, phone number, and gender."
            },

            {
                selector: ".profile-save-button",
                title: "Save Changes",
                text: "Click Save Changes to submit your updated profile information."
            }

        ]

    };


    // ============================================================
    // STATE
    // ============================================================

    let currentTourType = null;

    let currentPage = null;

    let currentStepIndex = 0;

    let tourActive = false;

    let automaticTour = false;

    let overlay = null;

    let tooltip = null;

    let highlightedElement = null;


    // ============================================================
    // PAGE DETECTION
    // ============================================================

    function detectCurrentPage() {

        const path = window.location.pathname
            .replace(/\/+$/, "")
            .toLowerCase();


        const allRoutes = Object.assign(
            {},
            JOBSEEKER_PAGE_ROUTES,
            RECRUITER_PAGE_ROUTES,
            ADMIN_PAGE_ROUTES
        );


        for (const page in allRoutes) {

            const route = allRoutes[page]
                .replace(/\/+$/, "")
                .toLowerCase();


            if (path === route) {

                return page;

            }

        }


        return null;

    }


    // ============================================================
    // TOUR TYPE
    // ============================================================

    function getTourTypeForPage(page) {

        if (!page) {
            return null;
        }


        if (page.indexOf("Jobseeker") === 0) {
            return "jobseeker";
        }


        if (page.indexOf("Recruiter") === 0) {
            return "recruiter";
        }


        if (page.indexOf("Admin") === 0) {
            return "admin";
        }


        return null;

    }


    // ============================================================
    // GET TOUR COLLECTION
    // ============================================================

    function getTourCollection(type) {

        switch (type) {

            case "jobseeker":
                return JOBSEEKER_TOUR;

            case "recruiter":
                return RECRUITER_TOUR;

            case "admin":
                return ADMIN_TOUR;

            default:
                return null;

        }

    }


    // ============================================================
    // GET PAGE ORDER
    // ============================================================

    function getPageOrder(type) {

        switch (type) {

            case "jobseeker":
                return JOBSEEKER_PAGE_ORDER;

            case "recruiter":
                return RECRUITER_PAGE_ORDER;

            case "admin":
                return ADMIN_PAGE_ORDER;

            default:
                return [];

        }

    }


    // ============================================================
    // GET ROUTES
    // ============================================================

    function getRoutes(type) {

        switch (type) {

            case "jobseeker":
                return JOBSEEKER_PAGE_ROUTES;

            case "recruiter":
                return RECRUITER_PAGE_ROUTES;

            case "admin":
                return ADMIN_PAGE_ROUTES;

            default:
                return {};

        }

    }


    // ============================================================
    // SELECTOR RESOLUTION
    // ============================================================

    function resolveSelector(selector) {

        if (!selector) {
            return null;
        }


        // Multiple possible selectors
        if (Array.isArray(selector)) {

            for (let i = 0; i < selector.length; i++) {

                const element = resolveSelector(selector[i]);

                if (element) {
                    return element;
                }

            }

            return null;

        }


        if (typeof selector !== "string") {
            return null;
        }


        try {

            return document.querySelector(selector);

        }
        catch (error) {

            console.warn(
                "HirePoint Tour: Invalid selector:",
                selector
            );

            return null;

        }

    }


    // ============================================================
    // GET CURRENT TOUR
    // ============================================================

    function getCurrentTour() {

        if (!currentTourType || !currentPage) {
            return null;
        }


        const collection = getTourCollection(currentTourType);

        if (!collection) {
            return null;
        }


        return collection[currentPage] || null;

    }


    // ============================================================
    // FILTER VALID STEPS
    // ============================================================

    function getValidSteps(page) {

        const collection = getTourCollection(currentTourType);

        if (!collection || !collection[page]) {
            return [];
        }


        return collection[page].filter(function (step) {

            return resolveSelector(step.selector) !== null;

        });

    }


    // ============================================================
    // CREATE OVERLAY
    // ============================================================

    function createOverlay() {

        removeOverlay();


        overlay = document.createElement("div");

        overlay.id = "hirepoint-tour-overlay";


        Object.assign(
            overlay.style,
            {

                position: "fixed",

                top: "0",

                left: "0",

                right: "0",

                bottom: "0",

                width: "100%",

                height: "100%",

                background: "rgba(0, 0, 0, 0.58)",

                zIndex: "99998",

                pointerEvents: "auto"

            }
        );


        document.body.appendChild(overlay);

    }


    // ============================================================
    // CREATE TOOLTIP
    // ============================================================

    function createTooltip() {

        removeTooltip();


        tooltip = document.createElement("div");

        tooltip.id = "hirepoint-tour-tooltip";


        Object.assign(
            tooltip.style,
            {

                position: "fixed",

                width: "360px",

                maxWidth: "calc(100vw - 30px)",

                background: "#ffffff",

                color: "#183b56",

                borderRadius: "12px",

                padding: "20px",

                boxShadow: "0 12px 35px rgba(0,0,0,0.25)",

                zIndex: "100000",

                fontFamily: "inherit",

                lineHeight: "1.5",

                boxSizing: "border-box"

            }
        );


        document.body.appendChild(tooltip);

    }


    // ============================================================
    // REMOVE OVERLAY
    // ============================================================

    function removeOverlay() {

        const existing = document.getElementById(
            "hirepoint-tour-overlay"
        );


        if (existing) {
            existing.remove();
        }


        overlay = null;

    }


    // ============================================================
    // REMOVE TOOLTIP
    // ============================================================

    function removeTooltip() {

        const existing = document.getElementById(
            "hirepoint-tour-tooltip"
        );


        if (existing) {
            existing.remove();
        }


        tooltip = null;

    }


    // ============================================================
    // REMOVE HIGHLIGHT
    // ============================================================

    function removeHighlight() {

        if (!highlightedElement) {
            return;
        }


        highlightedElement.style.removeProperty(
            "position"
        );

        highlightedElement.style.removeProperty(
            "z-index"
        );

        highlightedElement.style.removeProperty(
            "box-shadow"
        );


        highlightedElement = null;

    }


    // ============================================================
    // HIGHLIGHT ELEMENT
    // ============================================================

    function highlightElement(element) {

        removeHighlight();


        highlightedElement = element;


        const computedPosition =
            window.getComputedStyle(element).position;


        if (
            computedPosition === "static"
        ) {

            element.style.position = "relative";

        }


        element.style.zIndex = "99999";


        element.style.boxShadow =
            "0 0 0 4px rgba(40, 120, 173, 0.95), 0 0 25px rgba(40, 120, 173, 0.55)";


        element.scrollIntoView({

            behavior: "smooth",

            block: "center",

            inline: "nearest"

        });

    }


    // ============================================================
    // POSITION TOOLTIP
    // ============================================================

    function positionTooltip(element) {

        if (!tooltip || !element) {
            return;
        }


        const rect = element.getBoundingClientRect();

        const tooltipRect = tooltip.getBoundingClientRect();

        const margin = 15;


        let top;

        let left;


        // Prefer below
        if (
            rect.bottom + tooltipRect.height + margin
            <= window.innerHeight
        ) {

            top = rect.bottom + margin;

        }

        // Otherwise above
        else if (
            rect.top - tooltipRect.height - margin
            >= 0
        ) {

            top = rect.top -
                tooltipRect.height -
                margin;

        }

        // Center vertically if neither works
        else {

            top =
                Math.max(
                    margin,
                    (window.innerHeight -
                        tooltipRect.height) / 2
                );

        }


        left =
            rect.left +
            (rect.width / 2) -
            (tooltipRect.width / 2);


        left = Math.max(
            margin,
            Math.min(
                left,
                window.innerWidth -
                tooltipRect.width -
                margin
            )
        );


        top = Math.max(
            margin,
            Math.min(
                top,
                window.innerHeight -
                tooltipRect.height -
                margin
            )
        );


        tooltip.style.top = top + "px";

        tooltip.style.left = left + "px";

    }


    // ============================================================
    // RENDER STEP
    // ============================================================

    function renderCurrentStep() {

        if (!tourActive) {
            return;
        }


        const steps = getValidSteps(currentPage);


        // No valid steps on this page
        if (!steps.length) {

            handlePageWithNoSteps();

            return;

        }


        // Make sure index is valid
        if (
            currentStepIndex < 0
        ) {

            currentStepIndex = 0;

        }


        if (
            currentStepIndex >= steps.length
        ) {

            currentStepIndex =
                steps.length - 1;

        }


        const step =
            steps[currentStepIndex];


        const element =
            resolveSelector(step.selector);


        if (!element) {

            nextStep();

            return;

        }


        createOverlay();

        createTooltip();

        highlightElement(element);


        const title =
            step.title || "HirePoint Tour";


        const text =
            step.text || "";


        const isFirst =
            currentStepIndex === 0;


        const isLast =
            currentStepIndex === steps.length - 1;


        tooltip.innerHTML = "";


        const titleElement =
            document.createElement("div");


        Object.assign(
            titleElement.style,
            {

                fontSize: "18px",

                fontWeight: "700",

                marginBottom: "8px",

                color: "#183b56"

            }
        );


        titleElement.textContent = title;


        tooltip.appendChild(titleElement);


        const textElement =
            document.createElement("div");


        Object.assign(
            textElement.style,
            {

                fontSize: "14px",

                color: "#52616b",

                marginBottom: "18px"

            }
        );


        textElement.textContent = text;


        tooltip.appendChild(textElement);


        // Progress
        const progress =
            document.createElement("div");


        Object.assign(
            progress.style,
            {

                fontSize: "12px",

                color: "#7b8790",

                marginBottom: "15px"

            }
        );


        progress.textContent =
            "Step " +
            (currentStepIndex + 1) +
            " of " +
            steps.length;


        tooltip.appendChild(progress);


        // Buttons container
        const buttons =
            document.createElement("div");


        Object.assign(
            buttons.style,
            {

                display: "flex",

                alignItems: "center",

                justifyContent: "space-between",

                gap: "8px"

            }
        );


        // Previous
        const previousButton =
            document.createElement("button");


        previousButton.type = "button";

        previousButton.textContent = "Previous";


        Object.assign(
            previousButton.style,
            {

                border: "1px solid #d7e7f0",

                background: "#ffffff",

                color: "#52616b",

                borderRadius: "7px",

                padding: "7px 12px",

                cursor: isFirst
                    ? "not-allowed"
                    : "pointer",

                opacity: isFirst ? "0.45" : "1"

            }
        );


        previousButton.disabled = isFirst;


        previousButton.addEventListener(
            "click",
            previousStep
        );


        buttons.appendChild(previousButton);


        // Right side
        const rightButtons =
            document.createElement("div");


        Object.assign(
            rightButtons.style,
            {

                display: "flex",

                gap: "8px"

            }
        );


        // Skip
        const skipButton =
            document.createElement("button");


        skipButton.type = "button";

        skipButton.textContent = "Skip";


        Object.assign(
            skipButton.style,
            {

                border: "none",

                background: "transparent",

                color: "#7b8790",

                borderRadius: "7px",

                padding: "7px 10px",

                cursor: "pointer"

            }
        );


        skipButton.addEventListener(
            "click",
            skipTour
        );


        rightButtons.appendChild(skipButton);


        // Next / Finish
        const nextButton =
            document.createElement("button");


        nextButton.type = "button";


        nextButton.textContent =
            isLast ? "Finish" : "Next";


        Object.assign(
            nextButton.style,
            {

                border: "1px solid #2878ad",

                background: "#2878ad",

                color: "#ffffff",

                borderRadius: "7px",

                padding: "7px 14px",

                cursor: "pointer",

                fontWeight: "600"

            }
        );


        nextButton.addEventListener(
            "click",
            nextStep
        );


        rightButtons.appendChild(nextButton);


        buttons.appendChild(rightButtons);


        tooltip.appendChild(buttons);


        requestAnimationFrame(function () {

            positionTooltip(element);

        });

    }


    // ============================================================
    // WINDOW RESIZE
    // ============================================================

    function repositionTooltip() {

        if (
            !tourActive ||
            !tooltip ||
            !highlightedElement
        ) {
            return;
        }


        positionTooltip(
            highlightedElement
        );

    }


    window.addEventListener(
        "resize",
        repositionTooltip
    );


    window.addEventListener(
        "scroll",
        repositionTooltip,
        true
    );


    // ============================================================
    // NEXT STEP
    // ============================================================

    function nextStep() {

        if (!tourActive) {
            return;
        }


        const steps =
            getValidSteps(currentPage);


        if (!steps.length) {

            handlePageWithNoSteps();

            return;

        }


        if (
            currentStepIndex <
            steps.length - 1
        ) {

            currentStepIndex++;

            renderCurrentStep();

            return;

        }


        // Current page is complete
        completeCurrentPage();

    }


    // ============================================================
    // PREVIOUS STEP
    // ============================================================

    function previousStep() {

        if (!tourActive) {
            return;
        }


        if (
            currentStepIndex > 0
        ) {

            currentStepIndex--;

            renderCurrentStep();

        }

    }


    // ============================================================
    // COMPLETE CURRENT PAGE
    // ============================================================

    function completeCurrentPage() {

        const pageOrder =
            getPageOrder(currentTourType);


        const currentIndex =
            pageOrder.indexOf(currentPage);


        // Manual/contextual tour
        if (
            !automaticTour ||
            currentIndex === -1
        ) {

            finishTour();

            return;

        }


        const nextPage =
            pageOrder[currentIndex + 1];


        // Entire automatic tour finished
        if (!nextPage) {

            finishTour();

            return;

        }


        const routes =
            getRoutes(currentTourType);


        const nextRoute =
            routes[nextPage];


        if (!nextRoute) {

            finishTour();

            return;

        }


        saveTourState({

            active: true,

            automatic: true,

            type: currentTourType,

            currentPage: nextPage,

            stepIndex: 0

        });


        tourActive = false;


        removeHighlight();

        removeOverlay();

        removeTooltip();


        window.location.href =
            nextRoute;

    }


    // ============================================================
    // PAGE WITH NO STEPS
    // ============================================================

    function handlePageWithNoSteps() {

        if (
            automaticTour
        ) {

            completeCurrentPage();

        }
        else {

            finishTour();

        }

    }


    // ============================================================
    // SKIP TOUR
    // ============================================================

    function skipTour() {

        finishTour();

    }


    // ============================================================
    // FINISH TOUR
    // ============================================================

    function finishTour() {

        tourActive = false;

        automaticTour = false;

        currentTourType = null;

        currentPage = null;

        currentStepIndex = 0;


        removeHighlight();

        removeOverlay();

        removeTooltip();


        clearTourState();

    }


    // ============================================================
    // START CURRENT PAGE TOUR
    // ============================================================

    function startPageTour(page, type) {

        if (!page) {
            return false;
        }


        currentPage = page;

        currentTourType =
            type || getTourTypeForPage(page);


        currentStepIndex = 0;

        tourActive = true;

        automaticTour = false;


        const tour =
            getTourCollection(currentTourType);


        if (
            !tour ||
            !tour[currentPage]
        ) {

            finishTour();

            return false;

        }


        renderCurrentStep();

        return true;

    }


    // ============================================================
    // START FULL TOUR
    // ============================================================

    function startFullTour(type) {

        const pageOrder =
            getPageOrder(type);


        if (!pageOrder.length) {
            return false;
        }


        const firstPage =
            pageOrder[0];


        const routes =
            getRoutes(type);


        const currentDetectedPage =
            detectCurrentPage();


        // Full tour always begins on the dashboard
        if (
            currentDetectedPage !== firstPage
        ) {

            saveTourState({

                active: true,

                automatic: true,

                type: type,

                currentPage: firstPage,

                stepIndex: 0

            });


            window.location.href =
                routes[firstPage];


            return true;

        }


        currentTourType = type;

        currentPage = firstPage;

        currentStepIndex = 0;

        tourActive = true;

        automaticTour = true;


        saveTourState({

            active: true,

            automatic: true,

            type: type,

            currentPage: firstPage,

            stepIndex: 0

        });


        renderCurrentStep();


        return true;

    }


    // ============================================================
    // START JOBSEEKER FULL TOUR
    // ============================================================

    function startFullJobseekerTour() {

        return startFullTour(
            "jobseeker"
        );

    }


    // ============================================================
    // START RECRUITER FULL TOUR
    // ============================================================

    function startFullRecruiterTour() {

        return startFullTour(
            "recruiter"
        );

    }


    // ============================================================
    // START ADMIN FULL TOUR
    // ============================================================

    function startFullAdminTour() {

        return startFullTour(
            "admin"
        );

    }


    // ============================================================
    // RESTART FULL TOUR
    // ============================================================

    function restartFullTour(type) {

        clearTourState();


        const pageOrder =
            getPageOrder(type);


        if (!pageOrder.length) {
            return false;
        }


        const firstPage =
            pageOrder[0];


        const routes =
            getRoutes(type);


        const currentPageDetected =
            detectCurrentPage();


        if (
            currentPageDetected !== firstPage
        ) {

            saveTourState({

                active: true,

                automatic: true,

                type: type,

                currentPage: firstPage,

                stepIndex: 0

            });


            window.location.href =
                routes[firstPage];


            return true;

        }


        return startFullTour(type);

    }


    // ============================================================
    // RESTART JOBSEEKER
    // ============================================================

    function restartJobseekerTour() {

        return restartFullTour(
            "jobseeker"
        );

    }


    // ============================================================
    // RESTART RECRUITER
    // ============================================================

    function restartRecruiterTour() {

        return restartFullTour(
            "recruiter"
        );

    }


    // ============================================================
    // RESTART ADMIN
    // ============================================================

    function restartAdminTour() {

        return restartFullTour(
            "admin"
        );

    }


    // ============================================================
    // SAVE TOUR STATE
    // ============================================================

    function saveTourState(state) {

        try {

            sessionStorage.setItem(
                TOUR_STORAGE_KEY,
                JSON.stringify(state)
            );

        }
        catch (error) {

            console.warn(
                "HirePoint Tour: Unable to save tour state.",
                error
            );

        }

    }


    // ============================================================
    // GET TOUR STATE
    // ============================================================

    function getTourState() {

        try {

            const state =
                sessionStorage.getItem(
                    TOUR_STORAGE_KEY
                );


            if (!state) {
                return null;
            }


            return JSON.parse(state);

        }
        catch (error) {

            return null;

        }

    }


    // ============================================================
    // CLEAR TOUR STATE
    // ============================================================

    function clearTourState() {

        try {

            sessionStorage.removeItem(
                TOUR_STORAGE_KEY
            );

        }
        catch (error) {

            // Ignore storage errors

        }

    }


    // ============================================================
    // RESUME AUTOMATIC TOUR
    // ============================================================

    function resumeAutomaticTour() {

        const state =
            getTourState();


        if (
            !state ||
            !state.active ||
            !state.automatic ||
            !state.type
        ) {

            return false;

        }


        const detectedPage =
            detectCurrentPage();


        const pageOrder =
            getPageOrder(state.type);


        const expectedPage =
            state.currentPage;


        /*
         * IMPORTANT:
         * Only resume when we arrived at the page that the
         * previous tour step expected.
         */
        if (
            detectedPage !== expectedPage
        ) {

            return false;

        }


        if (
            pageOrder.indexOf(expectedPage) === -1
        ) {

            clearTourState();

            return false;

        }


        currentTourType =
            state.type;


        currentPage =
            expectedPage;


        currentStepIndex =
            Number.isInteger(state.stepIndex)
                ? state.stepIndex
                : 0;


        automaticTour = true;

        tourActive = true;


        /*
         * Allow the page DOM to finish loading before
         * attempting to locate tour elements.
         */
        setTimeout(function () {

            renderCurrentStep();

        }, 250);


        return true;

    }


    // ============================================================
    // ACTIVATE FULL TOUR
    // ============================================================

    function activateFullTour() {

        const page =
            detectCurrentPage();


        const type =
            getTourTypeForPage(page);


        if (!type) {
            return false;
        }


        return startFullTour(type);

    }


    // ============================================================
    // START TOUR FOR CURRENT PAGE
    // ============================================================

    function startCurrentPageTour() {

        const page =
            detectCurrentPage();


        const type =
            getTourTypeForPage(page);


        if (!page || !type) {
            return false;
        }


        return startPageTour(
            page,
            type
        );

    }


    // ============================================================
    // DOM READY
    // ============================================================

    document.addEventListener(
        "DOMContentLoaded",
        function () {

            /*
             * First priority:
             * Resume an automatic tour that navigated here.
             */
            if (
                resumeAutomaticTour()
            ) {

                return;

            }

        }
    );


    // ============================================================
    // PUBLIC API
    // ============================================================

    window.HirePointTour = {


        // --------------------------------------------------------
        // General
        // --------------------------------------------------------

        start: function () {

            return startCurrentPageTour();

        },


        restart: function () {

            return activateFullTour();

        },


        next: function () {

            return nextStep();

        },


        previous: function () {

            return previousStep();

        },


        skip: function () {

            return skipTour();

        },


        finish: function () {

            return finishTour();

        },


        activateFullTour: function () {

            return activateFullTour();

        },


        isActive: function () {

            return tourActive;

        },


        getCurrentPage: function () {

            return currentPage;

        },


        getSteps: function () {

            return getCurrentTour() || [];

        },


        // --------------------------------------------------------
        // Jobseeker
        // --------------------------------------------------------

        startFullJobseekerTour: function () {

            return startFullJobseekerTour();

        },


        restartJobseekerTour: function () {

            return restartJobseekerTour();

        },


        // --------------------------------------------------------
        // Recruiter
        // --------------------------------------------------------

        startFullRecruiterTour: function () {

            return startFullRecruiterTour();

        },


        restartRecruiter: function () {

            return restartRecruiterTour();

        },


        restartRecruiterTour: function () {

            return restartRecruiterTour();

        },


        // --------------------------------------------------------
        // Admin
        // --------------------------------------------------------

        startFullAdminTour: function () {

            return startFullAdminTour();

        },


        restartAdmin: function () {

            return restartAdminTour();

        },


        restartAdminTour: function () {

            return restartAdminTour();

        },


        // --------------------------------------------------------
        // Manual page tours
        // --------------------------------------------------------

        startPageTour: function (page) {

            const type =
                getTourTypeForPage(page);


            return startPageTour(
                page,
                type
            );

        }

    };


})(window, document);