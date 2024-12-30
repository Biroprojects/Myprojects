// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Az email mező kötelező.")]
        [EmailAddress(ErrorMessage = "Kérlek, adj meg egy érvényes email címet.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A jelszó mező kötelező.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Emlékezz rám?")]
        public bool RememberMe { get; set; }
    }

    public async Task OnGetAsync(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        returnUrl ??= Url.Content("~/");

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
            // Első lépés: próbálkozz a felhasználónévvel
            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                // Ha a felhasználónévvel nem sikerült, próbálkozz az e-mail cím ellenőrzésével
                var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                }
            }

            if (result.Succeeded)
            {
                _logger.LogInformation("A felhasználó bejelentkezett.");
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("A felhasználói fiók zárolva van.");
                ModelState.AddModelError(string.Empty, "A felhasználói fiók zárolva van. Kérlek, próbálkozz később.");
                return Page();
            }
            else
            {
                // További részletek a hiba okáról
                ModelState.AddModelError(string.Empty, "Érvénytelen bejelentkezési adatok.");
                return Page();
            }
        }

        // Ha a ModelState érvénytelen
        return Page();
    }
}