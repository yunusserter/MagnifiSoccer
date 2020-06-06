using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MagnifiSoccer.API.Models;
using MagnifiSoccer.Shared.Dtos;
using MagnifiSoccer.Shared.Dtos.AuthDtos;
using MagnifiSoccer.Shared.Responses;
using MagnifiSoccer.Shared.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MagnifiSoccer.API.Services
{
    public interface IUserService
    {
        Task<UserManagerResponse> RegisterUserAsync(RegisterForDto registerForDto);
        Task<UserManagerResponse> LoginUserAsync(LoginForDto loginForDto);
        Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token);
        Task<UserManagerResponse> ForgetPasswordAsync(string email);
        Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
        User GetById(string userId);
        List<GroupMember> GetAllForSquad(string userId);
    }

    public class UserService : IUserService
    {
        private UserManager<User> _userManager;
        private IConfiguration _configuration;
        private IMailService _mailService;
        private readonly ApplicationDbContext _context;

        public UserService(UserManager<User> userManager, IConfiguration configuration, IMailService mailService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _context = context;
        }

        public async Task<UserManagerResponse> RegisterUserAsync(RegisterForDto registerForDto)
        {
            if (registerForDto == null)
                throw new NullReferenceException("Register model is null.");

            if (registerForDto.Password != registerForDto.ConfirmPassword)
                return new UserManagerResponse
                {
                    Message = "Confirm password doesn't match the password.",
                    IsSuccess = false
                };

            var identityUser = new User
            {
                Email = registerForDto.Email,
                UserName = registerForDto.Email,
                FirstName = registerForDto.FirstName,
                LastName = registerForDto.LastName
            };

            var result = await _userManager.CreateAsync(identityUser, registerForDto.Password);

            if (result.Succeeded)
            {
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                var encodedEmailToken = Encoding.UTF8.GetBytes(confirmEmailToken);
                var validEmailToken = WebEncoders.Base64UrlEncode(encodedEmailToken);
                string url =
                    $"{_configuration["AppUrl"]}/api/auth/confirmEmail?userId={identityUser.Id}&token={validEmailToken}";

                await _mailService.SendMailAsync(identityUser.Email, "Confirm your email",
                    $"<h1>Welcome to MagnifiSoccer</h1><p>Please confirm your email by <a href='{url}'>Click here.</a></p>");

                return new UserManagerResponse
                {
                    Message = "User created successfully.",
                    IsSuccess = true
                };
            }

            return new UserManagerResponse
            {
                Message = result.Errors.Select(e => e.Description).Last(),
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> LoginUserAsync(LoginForDto loginForDto)
        {
            var user = await _userManager.FindByEmailAsync(loginForDto.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    Message = "There is no user with that e-mail address.",
                    IsSuccess = false
                };

            var result = await _userManager.CheckPasswordAsync(user, loginForDto.Password);
            if (!result)
                return new UserManagerResponse
                {
                    Message = "Invalid password.",
                    IsSuccess = false
                };

            IdentityOptions _options = new IdentityOptions();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, loginForDto.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["AuthSettings:Issuer"],
                audience: _configuration["AuthSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            var currentUser = new
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                Remainder = user.Remainder,
                OverAllRating = user.OverAllRating
            };

            return new UserManagerResponse
            {
                User = currentUser,
                Message = tokenAsString,
                IsSuccess = true,
                ExpireDate = token.ValidTo
            };
        }

        public async Task<UserManagerResponse> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new UserManagerResponse()
                {
                    IsSuccess = false,
                    Message = "User not found."
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Email confirmed succussfully!"
                };


            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Email did not confirm!",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email"
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/resetPassword?email={email}&token={validToken}";

            await _mailService.SendMailAsync(email, "Reset Password",
                $"<h1>Follow the instructions to reset your password.</h1> <p>To reset your password <a href='{url}'>click here</p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Reset password URL has been sent to email successfully!"
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "No user associated with email."
                };

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Password doesn't match its confirmation"
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Password has been reset successfully!"
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Something went wrong.",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public User GetById(string userId)
        {
            var user = _userManager.FindByIdAsync(userId);
            return user.Result;
        }

        public List<GroupMember> GetAllForSquad(string userId)
        {
            using (_context)
            {
                var groups = _context.GroupMembers.Where(p => p.UserId == userId).ToList();
                var tempUsers = new List<GroupMember>();
                var users = new List<GroupMember>();

                foreach (var group in groups)
                {
                    tempUsers.AddRange(_context.GroupMembers.Where(p => p.GroupId == group.GroupId));
                }

                foreach (var user in tempUsers)
                {
                    if (users.Find(p => p.UserId == user.UserId) == null)
                    {
                        users.Add(user);
                    }
                }
                return users;
            }
        }
    }
}
