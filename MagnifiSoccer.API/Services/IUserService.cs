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
using Microsoft.EntityFrameworkCore;
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
        Task<UserManagerResponse> ResetPasswordIsLogin(ResetPasswordForDto model, string userId);
        Task<UserManagerResponse> ChangeUser(UserForDto model, string userId);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
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
                    Message = "Şifreler eşleşmiyor.",
                    IsSuccess = false
                };

            var user = _context.Users.SingleOrDefaultAsync(x => x.UserName == registerForDto.UserName).Result;
            if (user != null)
            {
                return new UserManagerResponse
                {
                    Message = "Kullanıcı adı kullanımda.",
                    IsSuccess = false
                };
            }

            var identityUser = new User
            {
                Email = registerForDto.Email,
                UserName = registerForDto.UserName,
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
                    $"<h1>MagnifiSoccer'e hoş geldin.</h1><p>E-posta aktivasyonu için <a href='{url}'>buraya tıklayınız.</a></p>");

                return new UserManagerResponse
                {
                    Message = "Kayıt başarılı! Giriş yapabilirsiniz.",
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
                    Message = "Bu e-posta adresine sahip bir kullanıcı yok.",
                    IsSuccess = false
                };

            var result = await _userManager.CheckPasswordAsync(user, loginForDto.Password);
            if (!result)
                return new UserManagerResponse
                {
                    Message = "Geçersiz şifre.",
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
                OverAllRating = user.OverAllRating,
                UserName = user.UserName
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
                    Message = "Kullanıcı bulunamadı."
                };

            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "E-posta aktivasyonu başarılı!"
                };


            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "E-posta aktivasyonu başarısız!",
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
                    Message = "E-posta ile ilişkilendirilmiş kullanıcı yok."
                };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = WebEncoders.Base64UrlEncode(encodedToken);

            string url = $"{_configuration["AppUrl"]}/resetPassword?email={email}&token={validToken}";

            await _mailService.SendMailAsync(email, "Şifre yenileme",
                $"<h1>Şifre yenileme.</h1> <p>Şifrenizi yenilemek için <a href='{url}'>buraya tıklayınız.</p>");

            return new UserManagerResponse
            {
                IsSuccess = true,
                Message = "Şifre yenileme bağlantısı e-posta adresinize gönderildi!"
            };
        }

        public async Task<UserManagerResponse> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "E-posta ile ilişkilendirilmiş kullanıcı yok."
                };

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Şifreler eşleşmiyor. Kontrol ediniz."
                };
            }
            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ResetPasswordAsync(user, normalToken, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Şifre yenileme başarılı."
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Bir şeyler yanlış gitti.",
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

        public async Task<UserManagerResponse> ResetPasswordIsLogin(ResetPasswordForDto model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };

            if (!LoginUserAsync(new LoginForDto { Email = user.Email, Password = model.CurrentPassword }).Result.IsSuccess)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Geçerli şifre yanlış. Lütfen kontrol ediniz."
                };
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Şifreler eşleşmiyor. Kontrol ediniz."
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (result.Succeeded)
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = "Şifre yenileme başarılı."
                };

            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Bir şeyler yanlış gitti.",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<UserManagerResponse> ChangeUser(UserForDto model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Kullanıcı bulunamadı."
                };

            var userName = _context.Users.SingleOrDefaultAsync(x => x.UserName == model.UserName).Result;
            if (userName != null && userName.UserName != model.UserName)
            {
                return new UserManagerResponse
                {
                    IsSuccess = false,
                    Message = "Kullanıcı adı kullanılıyor."
                };
            }

            if (model.FirstName != null && user.FirstName != model.FirstName) user.FirstName = model.FirstName;
            if (model.LastName != null && user.LastName != model.LastName) user.LastName = model.LastName;
            if (model.UserName != null && user.UserName != model.UserName) user.UserName = model.UserName;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                IdentityOptions _options = new IdentityOptions();

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Email, user.Email),
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
                    OverAllRating = user.OverAllRating,
                    UserName = user.UserName
                };
                return new UserManagerResponse
                {
                    IsSuccess = true,
                    Message = tokenAsString,
                    User = currentUser,
                    ExpireDate = token.ValidTo
                };
            }
            return new UserManagerResponse
            {
                IsSuccess = false,
                Message = "Bir şeyler yanlış gitti.",
                Errors = result.Errors.Select(e => e.Description)
            };
        }
    }
}
