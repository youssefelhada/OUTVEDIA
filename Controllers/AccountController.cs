
using System.ComponentModel.DataAnnotations;
using System.Net;
using final_project_depi.Models;
using final_project_depi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace final_project_depi.Controllers
{
 
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser>signInManager,IConfiguration configuration)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;

        }
        public IActionResult Register  ()
        {
            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index","Home");
            }
            return View();


        }
        [HttpPost]
        public async Task< IActionResult >Register (RegisterDto registerDto)
        {
            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {
                  return View(registerDto);
            }

            
            // create a new account and authenticate the user
            var user = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.Email, // UserName will be used to authenticate the user
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.Now,
            };
             var result = await userManager.CreateAsync(user, registerDto.Password);


              if (result.Succeeded)
            {
                // successful user registration
                await userManager.AddToRoleAsync(user, "client");

                // sign in the new user
                await signInManager.SignInAsync(user, false);

                return RedirectToAction("Index", "Home");
            }
            // registration failed => show registration errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerDto);


        }

        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>  Login(LoginDto loginDto )
        {
            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid) {

                return View(loginDto);


            }


            var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.Remeberme,false);
            if (result.Succeeded) { 
            return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid Login Attempt";
            }
            return View(loginDto);
        }

        [Authorize]
        public async Task<IActionResult>  Profile()
        {
            var appUser=await userManager.GetUserAsync(User);
            if (appUser == null) {
                return RedirectToAction("Index", "Home");

            }
            var ProfileDto = new ProfileDto()
            {
FirstName = appUser.FirstName,
LastName = appUser.LastName,
Email= appUser.Email??"",
PhoneNumber= appUser.PhoneNumber,
Address= appUser.Address,   

            };
            return View(ProfileDto);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {

            if (!ModelState.IsValid)
            {
              
                ViewBag.ErrorMessage = "Please Fil All the required Fields With Valid Values";
                return View(profileDto);
            }

            var appuser=await userManager.GetUserAsync(User);
            if (appuser == null)
            {
                return RedirectToAction("Index", "Home");


            }
            appuser.FirstName = profileDto.FirstName;
            appuser.LastName = profileDto.LastName;
            appuser.Email = profileDto.Email ?? "";
            appuser.PhoneNumber = profileDto.PhoneNumber;
            appuser.Address = profileDto.Address;
            var result=await userManager.UpdateAsync(appuser);
            if (result.Succeeded) {


                ViewBag.SuccessMessage = "Profile Updated Successfully!";


            }
            else
            {
                ViewBag.ErrorMessage = "Unable to update the profile:" + result.Errors.First().Description;


            }



            return View(profileDto);
        }

        [Authorize]
        public IActionResult Password()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill in all fields correctly.";
                return View();
            }

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await userManager.ChangePasswordAsync(appUser, passwordDto.CurrentPassword, passwordDto.NewPassword);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password Updated Successfully!";
            }
            else
            {
                ViewBag.ErrorMessage = "Error: " + result.Errors.First().Description;
            }

            return View();
        }

        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
        }
        public IActionResult ForgetPassword()
        {

            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index", "Home");
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword([Required,EmailAddress]string email)
        {
            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index", "Home");
            }
            ViewBag.Email = email;
            if (!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email Address";
                return View();
            }
            var user=await userManager.FindByEmailAsync(email);
            if (user != null) {
                var token=await userManager.GeneratePasswordResetTokenAsync(user);
                string reseturl = Url.ActionLink("ResetPassword", "Account", new { token }) ?? "Url Error";
                string senderName = configuration["BrevoSettings:SenderName"] ?? "";
                string senderEmail = configuration["BrevoSettings:senderEmail"] ?? "";
                string username=user.FirstName + " " +user.LastName;
                string subject = "Password Reset";
                string Message = "Dear" + username + ", \n\n" +
                    reseturl + "\n\n" + "Best Regrads";

                EmailSender.SendEmail(senderName, senderEmail, username, email, Message, subject);

            }
            ViewBag.successMessage = "Please Check Your Email Account And Click On The Password Reset Link!";
            return View();

        }

        public IActionResult ResetPassowrd(string? token)
        {

            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index", "Home");
            }

            if(token == null)
            {

                return RedirectToAction("Index", "Home");
            }
            return View();

        }

        [HttpPost]
        public async Task<IActionResult>  ResetPassowrd(string? token , PasswordResetDto model)
        {

            if (signInManager.IsSignedIn(User))
            {

                return RedirectToAction("Index", "Home");
            }

            if (token == null)
            {

                return RedirectToAction("Index", "Home");
            }
            if (!ModelState.IsValid)
            {

                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                ViewBag.ErrorMessage = "Token Not Valid!";
                return View(model);
            }


            var result = await userManager.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password Reset Successfully";
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }


            return View(model);

        }


    }
}