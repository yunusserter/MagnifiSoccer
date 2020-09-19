using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MagnifiSoccer.API.Models;
using MagnifiSoccer.Shared.Dtos;
using MagnifiSoccer.Shared.Dtos.GroupDtos;
using MagnifiSoccer.Shared.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MagnifiSoccer.API.Services
{
    public interface IGroupService
    {
        Group GetGroup(string groupId);
        List<Group> GetListGroup(string userId);
        Task<GroupManagerResponse> CreateGroupAsync(GroupForDto model, string userId);
        Task<GroupManagerResponse> UpdateGroupAsync(GroupForDto model, string userId);
        Task<GroupManagerResponse> LeaveGroupAsync(string groupId, string userId);
        Task<GroupManagerResponse> KickGroupAsync(string userId, KickMemberGroupForDto model);
        Task<GroupManagerResponse> EditMemberAsync(EditMemberGroupForDto model1, string adminUserId);
        Task<GroupManagerResponse> RemoveGroupAsync(string userId, RemoveGroupForDto model);
        Task<GroupManagerResponse> JoinGroupAsync(JoinGroupForDto model, string userId);
        Task<GroupManagerResponse> InviteGroupAsync(JoinGroupForDto model, string userId);
        List<Group> GetGroupsForSearch(string searchText);
    }

    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly Cloudinary _cloudinary;

        public GroupService(ApplicationDbContext context,
            UserManager<User> userManager,
            IWebHostEnvironment environment
            )
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;

            Account account = new Account("dxxtg8kme", "623915428118857", "RRux-bnTIn90MuKKDiab25Zsic0");
            _cloudinary = new Cloudinary(account);
        }

        public async Task<GroupManagerResponse> CreateGroupAsync(GroupForDto model, string userId)
        {
            if (model == null)
                throw new NullReferenceException("Group model is null.");


            await using (_context)
            {
                var file = model.File;
                var uploadResult = new ImageUploadResult();

                if (file != null)
                {
                    await using (var steam = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.Name, steam)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                    model.Url = uploadResult.Url.ToString();
                    model.PublicId = uploadResult.PublicId;
                }
                

                var group = new Group
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupName = model.GroupName,
                    PhotoUrl = model?.Url
                };
                var user = await _userManager.FindByIdAsync(userId);
                var groupMember = new GroupMember
                {
                    GroupId = group.Id,
                    UserId = user.Id,
                    Role = "Admin",
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                await _context.AddAsync(groupMember);
                await _context.AddAsync(group);
                await _context.SaveChangesAsync();

                return new GroupManagerResponse
                {
                    Message = "Grup oluşturuldu.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GroupManagerResponse> UpdateGroupAsync(GroupForDto model, string userId)
        {
            if (model == null)
                throw new NullReferenceException("Group model is null.");

            await using (_context)
            {
                var group = await _context.Groups.FindAsync(model.GroupId);
                var userClaim = await _context.GroupMembers.SingleOrDefaultAsync(u =>
                    u.GroupId == model.GroupId && u.UserId == userId && u.Role == "Admin");

                if (group == null || userClaim == null)
                {
                    return new GroupManagerResponse
                    {
                        Message = "Grup bulunamadı veya bu işlem için yetkiniz yok.",
                        IsSuccess = false
                    };
                }

                if (model.GroupName != null)
                {
                    group.GroupName = model.GroupName;
                }

                var file = model.File;
                var uploadResult = new ImageUploadResult();

                if (file != null)
                {
                    await using (var steam = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.Name, steam)
                        };
                        uploadResult = _cloudinary.Upload(uploadParams);
                    }
                    model.Url = uploadResult.Url.ToString();
                    model.PublicId = uploadResult.PublicId;
                    group.PhotoUrl = model.Url.ToString();
                }

                _context.Update(group);
                await _context.SaveChangesAsync();

                return new GroupManagerResponse
                {
                    Message = "Grup düzenlendi.",
                    IsSuccess = true
                };

            }
        }

        public async Task<GroupManagerResponse> LeaveGroupAsync(string groupId, string userId)
        {
            if (groupId == null || userId == null)
                throw new NullReferenceException("Group parameters are null.");


            await using (_context)
            {
                var group = await _context.Groups.FindAsync(groupId);
                var groupAdmins = _context.GroupMembers.Where(u => u.GroupId == groupId && u.Role == "Admin");
                var groupUsers = _context.GroupMembers.Where(u => u.GroupId == groupId && u.Role == "User");


                if (groupAdmins.FirstOrDefault(p => p.UserId == userId) != null) // For admins
                {
                    if (groupAdmins.Count() == 1) // Just an admin
                    {
                        var groupAdmin = await groupAdmins.SingleOrDefaultAsync(p => p.UserId == userId);
                        _context.Remove(groupAdmin);
                        _context.Remove(group);

                    }
                    else
                    {
                        var groupAdmin = await groupAdmins.SingleOrDefaultAsync(p => p.UserId == userId);
                        _context.Remove(groupAdmin);
                    }
                }
                else // For users
                {
                    var groupUser = await groupUsers.SingleOrDefaultAsync(p => p.UserId == userId);
                    _context.Remove(groupUser);
                }

                await _context.SaveChangesAsync();

                return new GroupManagerResponse
                {
                    Message = "Gruptan başarıyla ayrıldınız.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GroupManagerResponse> KickGroupAsync(string adminUserId, KickMemberGroupForDto model)
        {
            if (model == null)
                throw new NullReferenceException("Model is null.");


            await using (_context)
            {
                var group = await _context.Groups.FindAsync(model.GroupId);
                var groupMember =
                    await _context.GroupMembers.SingleOrDefaultAsync(u =>
                        u.UserId == model.UserId && u.GroupId == model.GroupId);

                if (_context.GroupMembers.SingleOrDefaultAsync(u =>
                        u.UserId == adminUserId && u.GroupId == model.GroupId).Result.Role != "Admin" ||
                    model.UserId == adminUserId)
                    return new GroupManagerResponse
                    {
                        Message = "Bu işlem için yetkiniz yok.",
                        IsSuccess = false
                    };


                if (groupMember == null || group == null)
                    return new GroupManagerResponse
                    {
                        Message = "Grupta böyle bir kişi yok veya böyle bir grup yok.",
                        IsSuccess = false
                    };

                _context.Remove(groupMember);
                await _context.SaveChangesAsync();
                return new GroupManagerResponse
                {
                    Message = "Üye gruptan çıkarıldı.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GroupManagerResponse> EditMemberAsync(EditMemberGroupForDto model, string adminUserId)
        {
            if (model == null)
                throw new NullReferenceException("Model is null.");


            await using (_context)
            {
                var group = await _context.Groups.FindAsync(model.GroupId);
                var groupMember =
                    await _context.GroupMembers.SingleOrDefaultAsync(u =>
                        u.UserId == model.UserId && u.GroupId == model.GroupId);

                if (_context.GroupMembers.SingleOrDefaultAsync(u =>
                        u.UserId == adminUserId && u.GroupId == model.GroupId).Result.Role != "Admin" ||
                    model.UserId == adminUserId)
                    return new GroupManagerResponse
                    {
                        Message = "Bu işlem için yetkiniz yok.",
                        IsSuccess = false
                    };

                if (groupMember == null || group == null)
                    return new GroupManagerResponse
                    {
                        Message = "Grupta böyle bir kişi yok veya böyle bir grup yok.",
                        IsSuccess = false
                    };

                groupMember.Role = model.Role;
                _context.Update(groupMember);
                await _context.SaveChangesAsync();
                return new GroupManagerResponse
                {
                    Message =
                        $"{groupMember.FirstName} {groupMember.LastName} {(groupMember.Role == "Admin" ? "yönetici" : "üye")} olarak düzenlendi.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GroupManagerResponse> RemoveGroupAsync(string userId, RemoveGroupForDto model)
        {
            if (model == null || userId == null)
                throw new NullReferenceException("Model is null.");

            await using (_context)
            {
                var games = await _context.Games.Where(p => p.GroupId == model.GroupId).ToListAsync();
                var group = await _context.Groups.FindAsync(model.GroupId);
                var members = _context.GroupMembers.Where(p => p.GroupId == group.Id).ToList();

                if (_context.GroupMembers.SingleOrDefaultAsync(u =>
                    u.UserId == userId && u.GroupId == model.GroupId).Result.Role != "Admin")
                    return new GroupManagerResponse
                    {
                        Message = "You are not authorized to do this.",
                        IsSuccess = false
                    };
                _context.RemoveRange(games);
                _context.RemoveRange(members);
                _context.Remove(group);
                await _context.SaveChangesAsync();
                return new GroupManagerResponse
                {
                    Message = "Grup silindi.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GroupManagerResponse> JoinGroupAsync(JoinGroupForDto model, string userId)
        {
            if (model == null || userId == null)
                throw new NullReferenceException("Model is null.");

            await using (_context)
            {
                if (await _context.GroupMembers.FirstOrDefaultAsync(u =>
                    u.UserId == userId && u.GroupId == model.GroupId) != null)
                {
                    return new GroupManagerResponse
                    {
                        Message = "Gruba zaten üyesiniz.",
                        IsSuccess = false
                    };
                }

                var user = await _userManager.FindByIdAsync(userId);
                var newUser = new GroupMember
                {
                    GroupId = model.GroupId,
                    Role = "User",
                    UserId = userId,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                await _context.AddAsync(newUser);
                await _context.SaveChangesAsync();
                return new GroupManagerResponse
                {
                    Message = "Gruba katıldınız.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GroupManagerResponse> InviteGroupAsync(JoinGroupForDto model, string userId)
        {
            if (model == null || userId == null)
                throw new NullReferenceException("Model is null.");

            await using (_context)
            {
                var inviteUser = await _context.Users.SingleOrDefaultAsync(u => u.UserName == model.UserName);
                if (inviteUser == null)
                {
                    return new GroupManagerResponse
                    {
                        Message = "Kullanıcı bulunamadı.",
                        IsSuccess = false
                    };
                }

                if (await _context.GroupMembers.FirstOrDefaultAsync(u =>
                    u.UserId == inviteUser.Id && u.GroupId == model.GroupId) != null)
                {
                    return new GroupManagerResponse
                    {
                        Message = "Kullanıcı gruba zaten üye.",
                        IsSuccess = false
                    };
                }

                var user = await _userManager.FindByIdAsync(inviteUser.Id);
                var member = new GroupMember
                {
                    GroupId = model.GroupId,
                    Role = "User",
                    UserId = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                };

                await _context.AddAsync(member);
                await _context.SaveChangesAsync();
                return new GroupManagerResponse
                {
                    Message = $"{member.FirstName} {member.LastName} gruba alındı.",
                    IsSuccess = true
                };
            }
        }

        public Group GetGroup(string groupId)
        {
            using (_context)
            {
                var group = _context.Groups.Include(x => x.GroupMembers).SingleOrDefault(x => x.Id == groupId);
                if (group == null)
                    return null;

                foreach (var g in group.GroupMembers)
                {
                    g.User = new User { OverAllRating = _context.Users.Find(g.UserId).OverAllRating };
                }

                return group;
            }
        }

        public List<Group> GetListGroup(string userId)
        {
            using (_context)
            {
                var myGroups = _context.GroupMembers.Where(g
                        => g.UserId == userId)
                    .Select(m => m.GroupId).ToList();

                var groups = new List<Group>();

                foreach (var myGroup in myGroups)
                {
                    groups.Add(new Group
                    {
                        Id = myGroup,
                        GroupMembers = _context.GroupMembers.Where(p => p.GroupId == myGroup).ToList(),
                        GroupName = _context.Groups.Find(myGroup).GroupName,
                        PhotoUrl = _context.Groups.Find(myGroup).PhotoUrl,
                        Games = _context.Games.Where(p => p.GroupId == myGroup).ToList()
                    });
                }

                return groups;
            }
        }

        public List<Group> GetGroupsForSearch(string searchText)
        {
            using (_context)
            {
                var query = _context.Groups.Where(x => x.GroupName.Contains(searchText));
                return query.ToList();
            }

        }
    }
}