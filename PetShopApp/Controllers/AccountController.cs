using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using PetShopApp.DTOs;

namespace PetShopApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _rolemanager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> rolemanager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _rolemanager = rolemanager;
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if(!ModelState.IsValid)
            {
                return View(registerDto);
            }
            User user = new User()
            {
                Name = registerDto.Name,
                Surname = registerDto.Surname,
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
           var result= await _userManager.CreateAsync(user,registerDto.Password);
            if(!result.Succeeded)
            {
                foreach(var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _userManager.AddToRoleAsync(user, "User");
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(!ModelState.IsValid)
            {
                return View(loginDto);
            }
            var user = await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail);
            if(user == null)
            {
                user=await _userManager.FindByNameAsync(loginDto.UsernameOrEmail);
                if(user == null)
                {
                    ModelState.AddModelError("", "Username/Email or password is not valid!");
                    return View();
                }
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password,true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Try agin later!");
                return View();
            }
            if(!result.Succeeded)
            {
                ModelState.AddModelError("", "Username/Email or password is not valid!");
                return View();
            }
            await _signInManager.SignInAsync(user, loginDto.IsRemember);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> CreateRole()
        {
            IdentityRole identityRole = new IdentityRole("Admin");
            IdentityRole identityRole1 = new IdentityRole("User");
            await _rolemanager.CreateAsync(identityRole);
            await _rolemanager.CreateAsync(identityRole1);
            return Ok();


        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
