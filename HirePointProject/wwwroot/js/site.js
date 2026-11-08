```javascript
// =========================================================
// HIREPOINT SITE JAVASCRIPT
// Help Centre + User Guide + Tooltips
// =========================================================


document.addEventListener("DOMContentLoaded", function () {

    // =====================================================
    // HELP DROPDOWN
    // =====================================================

    const helpButton =
        document.getElementById("hpHelpButton");

    const helpDropdown =
        document.getElementById("hpHelpDropdown");


    if (helpButton && helpDropdown) {

        helpButton.addEventListener("click", function (event) {

            event.stopPropagation();

            const isOpen =
                helpDropdown.classList.contains("show");

            helpDropdown.classList.toggle(
                "show",
                !isOpen
            );

            helpButton.classList.toggle(
                "active",
                !isOpen
            );

            helpButton.setAttribute(
                "aria-expanded",
                (!isOpen).toString()
            );

        });


        document.addEventListener("click", function (event) {

            if (!helpDropdown.contains(event.target) &&
                !helpButton.contains(event.target)) {

                helpDropdown.classList.remove("show");

                helpButton.classList.remove("active");

                helpButton.setAttribute(
                    "aria-expanded",
                    "false"
                );
            }

        });

    }


    // =====================================================
    // USER GUIDE
    // =====================================================

    const tourOverlay =
        document.getElementById("hirePointTour");

    const startGuideButton =
        document.getElementById("startGuideButton");

    const resetGuideButton =
        document.getElementById("resetGuideButton");

    const closeTourButton =
        document.getElementById("closeTourButton");

    const skipTourButton =
        document.getElementById("tourSkipButton");

    const previousTourButton =
        document.getElementById("tourPreviousButton");

    const nextTourButton =
        document.getElementById("tourNextButton");


    if (!tourOverlay) {
        return;
    }


    // =====================================================
    // TOUR CONTENT
    // =====================================================

    const commonSteps = [

        {
            icon: "bi-stars",
            title: "Welcome to HirePoint",
            description:
                "HirePoint helps connect jobseekers, recruiters and administrators through one recruitment platform."
        },

        {
            icon: "bi-speedometer2",
            title: "Your Dashboard",
            description:
                "The dashboard gives you an overview of the information and actions that are most important to your account."
        },

        {
            icon: "bi-briefcase",
            title: "Jobs",
            description:
                "Use the Jobs area to find available opportunities or manage vacancies, depending on your account type."
        },

        {
            icon: "bi-file-earmark-text",
            title: "Applications",
            description:
                "Applications allow recruitment information to move through the recruitment process and help users track progress."
        },

        {
            icon: "bi-bell",
            title: "Notifications",
            description:
                "Notifications keep you informed about important updates, application activity and other system events."
        },

        {
            icon: "bi-question-circle",
            title: "Help is always available",
            description:
                "You can open Help from the top-right corner at any time to access the User Guide, Help Centre, FAQs and Troubleshooting."
        }

    ];


    const jobseekerSteps = [

        {
            icon: "bi-person-vcard",
            title: "Complete Your Profile",
            description:
                "Keep your personal details, qualifications and profile information up to date so recruiters can understand your background."
        },

        {
            icon: "bi-search",
            title: "Find Jobs",
            description:
                "Use Find Jobs to search available opportunities and review job requirements before submitting an application."
        },

        {
            icon: "bi-file-earmark-check",
            title: "Track Applications",
            description:
                "The Applications area lets you monitor applications and view their current status."
        },

        {
            icon: "bi-bell",
            title: "Notifications",
            description:
                "Check notifications for important updates about your applications and recruitment activity."
        },

        {
            icon: "bi-question-circle",
            title: "Get Help Anytime",
            description:
                "The Help button in the top-right corner gives you access to the User Guide, Help Centre, FAQs and troubleshooting information."
        }

    ];


    const recruiterSteps = [

        {
            icon: "bi-speedometer2",
            title: "Your Recruiter Dashboard",
            description:
                "Your dashboard provides an overview of your recruitment activities and gives you quick access to important tasks."
        },

        {
            icon: "bi-plus-circle",
            title: "Post a Job",
            description:
                "Use Post Job to create a vacancy and provide candidates with the information and requirements for the position."
        },

        {
            icon: "bi-briefcase",
            title: "Manage Your Jobs",
            description:
                "My Jobs allows you to review and manage vacancies that you have created."
        },

        {
            icon: "bi-people",
            title: "Manage Applicants",
            description:
                "Applicants allows you to review candidates and manage their progress through the recruitment process."
        },

        {
            icon: "bi-question-circle",
            title: "Get Help Anytime",
            description:
                "The Help button in the top-right corner provides access to the User Guide, Help Centre, FAQs and troubleshooting."
        }

    ];


    const adminSteps = [

        {
            icon: "bi-speedometer2",
            title: "Your Admin Dashboard",
            description:
                "The administrator dashboard provides an overview of HirePoint and gives you access to system management functions."
        },

        {
            icon: "bi-people",
            title: "Manage Users",
            description:
                "Manage user accounts and maintain the information required to keep the recruitment platform organised."
        },

        {
            icon: "bi-person-check",
            title: "Recruiter Approvals",
            description:
                "Review recruiter requests and manage the approval process for recruiter accounts."
        },

        {
            icon: "bi-bar-chart",
            title: "Analytics",
            description:
                "Analytics provides information that can help administrators understand activity within HirePoint."
        },

        {
            icon: "bi-question-circle",
            title: "Get Help Anytime",
            description:
                "The Help button in the top-right corner provides access to the User Guide, Help Centre, FAQs and troubleshooting."
        }

    ];


    let tourSteps =
        commonSteps;

    const userType =
        window.hirePointUserType || "";


    if (userType.toLowerCase() === "jobseeker") {

        tourSteps =
            jobseekerSteps;

    }
    else if (userType.toLowerCase() === "recruiter") {

        tourSteps =
            recruiterSteps;

    }
    else if (userType.toLowerCase() === "admin") {

        tourSteps =
            adminSteps;

    }


    // =====================================================
    // TOUR STATE
    // =====================================================

    let currentStep = 0;


    const TOUR_STORAGE_KEY =
        "hirepoint_user_guide_completed";


    // =====================================================
    // ELEMENTS
    // =====================================================

    const stepNumber =
        document.getElementById("tourStepNumber");

    const totalSteps =
        document.getElementById("tourTotalSteps");

    const tourIcon =
        document.getElementById("tourIcon");

    const tourTitle =
        document.getElementById("tourTitle");

    const tourDescription =
        document.getElementById("tourDescription");

    const tourDots =
        document.getElementById("tourDots");


    totalSteps.textContent =
        tourSteps.length;


    // =====================================================
    // RENDER TOUR
    // =====================================================

    function renderTourStep() {

        const step =
            tourSteps[currentStep];


        stepNumber.textContent =
            currentStep + 1;


        tourIcon.innerHTML =
            `< i class="bi ${step.icon}" ></i > `;


        tourTitle.textContent =
            step.title;


        tourDescription.textContent =
            step.description;


        // -----------------------------
        // DOTS
        // -----------------------------

        tourDots.innerHTML = "";


        tourSteps.forEach(
            function (_, index) {

                const dot =
                    document.createElement("span");

                dot.className =
                    "hp-tour-dot";


                if (index === currentStep) {

                    dot.classList.add("active");

                }


                tourDots.appendChild(dot);

            }
        );


        // -----------------------------
        // PREVIOUS BUTTON
        // -----------------------------

        previousTourButton.style.display =
            currentStep === 0
                ? "none"
                : "inline-block";


        // -----------------------------
        // NEXT BUTTON
        // -----------------------------

        if (currentStep === tourSteps.length - 1) {

            nextTourButton.innerHTML =
                `Finish < i class="bi bi-check-lg" ></i > `;

        }
        else {

            nextTourButton.innerHTML =
                `Next < i class="bi bi-arrow-right" ></i > `;

        }

    }


    // =====================================================
    // OPEN TOUR
    // =====================================================

    function openTour() {

        currentStep = 0;

        renderTourStep();

        tourOverlay.classList.add("show");

        tourOverlay.setAttribute(
            "aria-hidden",
            "false"
        );

        document.body.style.overflow =
            "hidden";

    }


    // =====================================================
    // CLOSE TOUR
    // =====================================================

    function closeTour(markCompleted) {

        tourOverlay.classList.remove("show");

        tourOverlay.setAttribute(
            "aria-hidden",
            "true"
        );

        document.body.style.overflow =
            "";


        if (markCompleted) {

            localStorage.setItem(
                TOUR_STORAGE_KEY,
                "true"
            );

        }

    }


    // =====================================================
    // NEXT
    // =====================================================

    function nextStep() {

        if (currentStep <
            tourSteps.length - 1) {

            currentStep++;

            renderTourStep();

        }
        else {

            closeTour(true);

        }

    }


    // =====================================================
    // PREVIOUS
    // =====================================================

    function previousStep() {

        if (currentStep > 0) {

            currentStep--;

            renderTourStep();

        }

    }


    // =====================================================
    // BUTTON EVENTS
    // =====================================================

    if (startGuideButton) {

        startGuideButton.addEventListener(
            "click",
            openTour
        );

    }


    if (resetGuideButton) {

        resetGuideButton.addEventListener(
            "click",
            function () {

                localStorage.removeItem(
                    TOUR_STORAGE_KEY
                );

                openTour();

            }
        );

    }


    if (closeTourButton) {

        closeTourButton.addEventListener(
            "click",
            function () {

                closeTour(true);

            }
        );

    }


    if (skipTourButton) {

        skipTourButton.addEventListener(
            "click",
            function () {

                closeTour(true);

            }
        );

    }


    if (previousTourButton) {

        previousTourButton.addEventListener(
            "click",
            previousStep
        );

    }


    if (nextTourButton) {

        nextTourButton.addEventListener(
            "click",
            nextStep
        );

    }


    // =====================================================
    // ESCAPE KEY
    // =====================================================

    document.addEventListener(
        "keydown",
        function (event) {

            if (event.key === "Escape" &&
                tourOverlay.classList.contains("show")) {

                closeTour(true);

            }

        }
    );


    // =====================================================
    // FIRST-TIME TOUR
    // =====================================================

    const tourCompleted =
        localStorage.getItem(
            TOUR_STORAGE_KEY
        );


    const isUserGuidePage =
        window.location.pathname
            .toLowerCase()
            .includes("/help/userguide");


    /*
       Only automatically open the tour on normal
       authenticated pages.

       The User Guide page has its own Start Tour button.
    */

    if (!tourCompleted &&
        !isUserGuidePage) {

        setTimeout(
            function () {

                openTour();

            },
            700
        );

    }


    // =====================================================
    // TOOLTIP SUPPORT
    // =====================================================

    document
        .querySelectorAll(".hp-tooltip")
        .forEach(
            function (tooltip) {

                tooltip.setAttribute(
                    "tabindex",
                    "0"
                );

            }
        );

});
```
