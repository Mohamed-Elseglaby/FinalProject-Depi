using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;


namespace FinalProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }




        // /Account/Register
        public IActionResult Register()
        {
            return View(new ApplicationUser());
        }

        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser user, string password)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(user.UserName) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                ViewBag.status = 1;
                ViewBag.message = "All fields are required.";
                return View(user);
            }
            if (!IsValidEmail(user.Email))
            {
                ViewBag.status = 1;
                ViewBag.message = "Email format is not valid.";
                return View(user);
            }
            // Validate password format
            if (!IsValidPassword(password))
            {
                ViewBag.status = 1;
                ViewBag.message = "Password must be at least 8 characters long, include one capital letter, and one special character.";
                return View(user);
            }

            // Validate phone number format if needed (optional)
            if (!IsValidPhoneNumber(user.PhoneNumber))
            {
                ViewBag.status = 1;
                ViewBag.message = "Phone number format is not valid.";
                return View(user);
            }

            // Check if the email or username already exists
            var existingUserByEmail = await _userManager.FindByEmailAsync(user.Email);
            var existingUserByUsername = await _userManager.FindByNameAsync(user.UserName);

            if (existingUserByEmail != null)
            {
                ViewBag.status = 1;
                ViewBag.message = "An account with this email already exists.";
                return View(user);
            }

            if (existingUserByUsername != null)
            {
                ViewBag.status = 1;
                ViewBag.message = "An account with this username already exists.";
                return View(user);
            }

            // Attempt to create the user
            user.PasswordHash = password; // Use plain password for creating user
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("Student"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Student"));
                }

                // Assign the user to the "Student" role
                await _userManager.AddToRoleAsync(user, "Student");

                 return Redirect("/Account/Login");
            }

            // If user creation failed
            ViewBag.status = 1;
            ViewBag.message = "User registration failed. Please try again.";
            return View(user);
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return phoneNumber.All(char.IsDigit) && phoneNumber.Length >= 11;
        }

        private bool IsValidPassword(string password)
        {
            var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*[!@#$%^&*(),.?""{}|<>]).{8,}$");
            return passwordRegex.IsMatch(password);
        }
        private bool IsValidEmail(string email)
        {
            // Regular expression for validating an email address
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }

        // /Account/Login
        public IActionResult Login()
        {
            return View(new ApplicationUser());
        }

        [HttpPost]
        public async Task<IActionResult> Login(ApplicationUser user)
        {
            // Check if either the email or username field is filled
            if (string.IsNullOrWhiteSpace(user.UserName) && string.IsNullOrWhiteSpace(user.Email))
            {
                ViewBag.status = 1;
                ViewBag.message = "Please enter your username or email.";
                return View(user);
            }

            // Check password field
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                ViewBag.status = 1;
                ViewBag.message = "Password cannot be empty.";
                return View(user);
            }

            // Check if user exists by their username or email
            ApplicationUser existingUser = null;
            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                existingUser = await _userManager.FindByNameAsync(user.UserName);
            }
            else if (!string.IsNullOrWhiteSpace(user.Email))
            {
                existingUser = await _userManager.FindByEmailAsync(user.Email);
            }

            if (existingUser == null)
            {
                ViewBag.status = 1; // failure
                ViewBag.message = "User not registered. Please sign up first.";
                return View(user);
            }

            // Check password match
            var result = await _signInManager.PasswordSignInAsync(existingUser.UserName, user.PasswordHash, false, false);
            if (result.Succeeded)
            {
               
                    // Check the user's roles
                    var roles = await _userManager.GetRolesAsync(existingUser);

                    // Redirect based on the role
                    if (roles.Contains("Instructor"))
                    {
                        return RedirectToAction("InstructorProfile", "Instructor");
                    }
                    else if (roles.Contains("Student"))
                    {
                        return RedirectToAction("Profile", "Student");
                    }
                    else if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                // If no specific role is found, redirect to a general home page
                return RedirectToAction("Index", "Home");
                }
            

            // If login fails (incorrect password or other issue)
            ViewBag.status = 1;
            ViewBag.message = "Username, Email or Password is not correct.";
            return View(user);
        }


        // /Account/LogOut
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Account/Login");
        }

        // Authorization methods (unchanged)
        public IActionResult AddRole() => View();

        [HttpPost]
        public async Task<IActionResult> AddRole(string RoleName)
        {
            IdentityRole role = new IdentityRole { Name = RoleName };
            var result = await _roleManager.CreateAsync(role);
            return View();
        }

        public IActionResult AssignRole() => View();

        [HttpPost]
        public async Task<IActionResult> AssignRole(string RoleName, string UserName)
        {
            var user = await _userManager.FindByNameAsync(UserName);
            var result = await _userManager.AddToRoleAsync(user, RoleName);
            return View();
        }
    }
}

