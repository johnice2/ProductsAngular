using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProductsAngular.Models;

namespace ProductsAngular.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("action")]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel formdata)
        {
            List<string> errorList = new List<string>();
            var user = new IdentityUser
            {
                Email = formdata.Email,
                UserName = formdata.Username,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, formdata.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                //sending vonfimation email
                return Ok(new { username = user.UserName, email = user.Email, status = 1, message = "Registration Succesfull"});

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);
                }
            }

            return BadRequest(new JsonResult(errorList));


        }

        [HttpPost("action")]
        public async Task<ActionResult> Login([FromBody]LoginViewModel formdata)
        {
            var user = await _userManager.FindByNameAsync(formdata.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, formdata.Password))
            {

            }
            ModelState.AddModelError("", "UserName/password was not found");
            return Unauthorized(new { loginError = "Please check the login credentials - invalid UserName/Password was entered" });

        }
    }
}
