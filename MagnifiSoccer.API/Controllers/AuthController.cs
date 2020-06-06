using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MagnifiSoccer.API.Models;
using MagnifiSoccer.API.Services;
using MagnifiSoccer.Shared.Dtos;
using MagnifiSoccer.Shared.Dtos.AuthDtos;
using MagnifiSoccer.Shared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MagnifiSoccer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IMailService _mailService;
        private IConfiguration _configuration;

        public AuthController(IUserService userService, IMailService mailService, IConfiguration configuration)
        {
            _userService = userService;
            _mailService = mailService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterForDto model)
        {
            if (ModelState.IsValid)
            {
                model.Role = "User";
                var result = await _userService.RegisterUserAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid."); // Status code: 400
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginForDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccess)
                {
                    /* await _mailService.SendMailAsync(loginForDto.Email, $"New login",
                        $"<h1>Hey!, new login to your account noticed.</h1><p>New login to your account at {DateTime.Now}</p>");*/
                    return Ok(result);
                }

                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid."); // Status code: 400
        }

        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            var result = await _userService.ConfirmEmailAsync(userId, token);

            if (result.IsSuccess)
            {
                return Redirect($"{_configuration["AppUrl"]}/confirmEmail.html");
            }

            return BadRequest(result);
        }

        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var result = await _userService.ForgetPasswordAsync(email);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm]ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.ResetPasswordAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }

            return BadRequest("Some properties are not valid.");
        }
        
        [HttpGet("{userId}")]
        public IActionResult GetById(string userId)
        {
            // var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = _userService.GetById(userId);

            if (result != null)
            {
                Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("users")]
        public IActionResult GetAllForSquad()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_userService.GetAllForSquad(userId));
            }

            return BadRequest();
        }
    }
}
