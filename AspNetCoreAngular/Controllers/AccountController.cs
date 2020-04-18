using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AspNetCoreAngular.Models;
using AspNetCoreAngular.Helpers;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace AspNetCoreAngular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly AppSettings _appSettings;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager , IOptions<AppSettings> appSettings)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;

        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel formdata)
        {
            List<string> errorList = new List<string>();

            var user = new IdentityUser
            {
                Email = formdata.Email,
                UserName = formdata.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, formdata.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");

                return Ok(
                    new
                    {
                        username = user.UserName,
                        email = user.Email,
                        status = 1,
                        message = "registration successful"

                    });

            }

            else {


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    errorList.Add(error.Description);

                }
                     
                        
              }



            return BadRequest(new JsonResult(errorList));


            


        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Login ([FromBody]LoginViewModel formdata)
        {
            var user = await _userManager.FindByNameAsync(formdata.UserName);
            var roles = await _userManager.GetRolesAsync(user);
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret));
            var tokenExpiryTime = Convert.ToDouble(_appSettings.ExpireTime);
            

            if(user != null && await _userManager.CheckPasswordAsync(user , formdata.Password))
            {
                var tokenHsndler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                     {
                         new Claim(JwtRegisteredClaimNames.Sub, formdata.UserName),
                         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                         new Claim(ClaimTypes.NameIdentifier, user.Id),
                         new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                         new Claim("LoggedOn", DateTime.Now.ToString())
                     }),

                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _appSettings.Site,
                    Audience = _appSettings.Audience,
                    Expires = DateTime.UtcNow.AddMinutes(tokenExpiryTime)
                     
                };

                var token = tokenHsndler.CreateToken(tokenDescriptor);

                return Ok(new { token = tokenHsndler.WriteToken(token), expiration = token.ValidTo, username = user.UserName, userRole = roles.FirstOrDefault() });

            }

            ModelState.AddModelError("", "Username or Password was not Found");
            return Unauthorized(new { LoginError = "Please check Login or password" });

        }
    }
}
