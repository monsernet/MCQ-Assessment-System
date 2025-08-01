﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using AuthSystem.Areas.Identity.Data;
using MCQInterviews.Repositories.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRepository _userRepository;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager,
            IUserRepository userRepository
            )
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    // Check if user is active before sign-in
                    if (user.IsActive == 1)
                    {
                        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                        if (result.Succeeded)
                        {
                            // Save user login details
                            await _userRepository.LogUserLogin(user.Id);

                            // Update the Last Login Date
                            user.LastLoginDate = DateTime.UtcNow;
                            await _userManager.UpdateAsync(user);

                            _logger.LogInformation("User logged in.");
                            if (user != null)
                            {
                                // Return Page 
                                bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                                bool isEditor = await _userManager.IsInRoleAsync(user, "Editor");

                                if (isAdmin || isEditor)
                                {
                                    return RedirectToAction("Index", "AdminDashboard");
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                            return LocalRedirect(returnUrl);
                        }
                        if (result.RequiresTwoFactor)
                        {
                            return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Your account is inactive. Please contact support.");
                        return Page();
                    }
                }
                else
                {
                    // Handle invalid username
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();

            ////returnUrl ??= Url.Content("~/");

            ////ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ////if (ModelState.IsValid)
            ////{
            ////    // This doesn't count login failures towards account lockout
            ////    // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            ////    var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
            ////    if (result.Succeeded)
            ////    {
            ////        // Save user login details
            ////        var user = await _userManager.FindByEmailAsync(Input.Email);
            ////        await _userRepository.LogUserLogin(user.Id);

            ////        //Update the Last Login Date
            ////        user.LastLoginDate = DateTime.UtcNow;
            ////        await _userManager.UpdateAsync(user);

            ////        _logger.LogInformation("User logged in.");
            ////        if (user != null)
            ////        {
            ////            // Return Page 
            ////            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            ////            bool isEditor = await _userManager.IsInRoleAsync(user, "Editor");

            ////            if (isAdmin || isEditor)
            ////            {
            ////                return RedirectToAction("Index", "AdminDashboard");
            ////            }
            ////            else
            ////            {
            ////                return RedirectToAction("Index", "Home");
            ////            }
            ////        }
            ////        return LocalRedirect(returnUrl);
            ////    }
            ////    if (result.RequiresTwoFactor)
            ////    {
            ////        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            ////    }
            ////    if (result.IsLockedOut)
            ////    {
            ////        _logger.LogWarning("User account locked out.");
            ////        return RedirectToPage("./Lockout");
            ////    }
            ////    else
            ////    {
            ////        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            ////        return Page();
            ////    }
            ////}

            ////// If we got this far, something failed, redisplay form
            ////return Page();
        }
    }
}
