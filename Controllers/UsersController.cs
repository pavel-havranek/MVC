﻿using MagistriMVC.Models;
using MagistriMVC.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MagistriMVC.Controllers {
    public class UsersController : Controller {
        private UserManager<AppUser> userManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private IPasswordValidator<AppUser> passwordValidator;
        public UsersController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator) {
            this.userManager = userManager;
            this.passwordHasher = passwordHasher;
            this.passwordValidator = passwordValidator;

        }
        public IActionResult Index() {
            return View(userManager.Users);
        }
        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserVM user) {
            if (ModelState.IsValid) {
                AppUser appUser = new AppUser {
                    UserName = user.Name,
                    Email = user.Email
                };
                //pokus o zápis nového uživatele do databáze
                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        public async Task<IActionResult> Edit(string id) {
            AppUser userToEdit = await userManager.FindByIdAsync(id);
            if (userToEdit == null)
                return View("NotFound");
            else
                return View(userToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password) {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null) {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(password)) {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, password);
                    if (validPass.Succeeded) {
                        user.PasswordHash = passwordHasher.HashPassword(user, password); 
                    }
                    else {
                        Errors(validPass);
                    }
                }
                else
                    ModelState.AddModelError("", "Password cannot be empty");
                if (!string.IsNullOrEmpty(email) && validPass.Succeeded) {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }
        private void Errors(IdentityResult result) {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        public async Task<IActionResult> Delete(string id) {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null) {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }
    }
}
