using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MagnifiSoccer.API.Models;
using MagnifiSoccer.Shared.Dtos;
using MagnifiSoccer.Shared.Dtos.GroupDtos;
using MagnifiSoccer.Shared.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;

namespace MagnifiSoccer.API.Services
{
    public interface IGroupService
    {
        Group GetGroupAsync(string groupId);
        List<Group> GetListGroupAsync(string userId);
        List<Group> GetAllGroupForSearch();
        Task<GroupManagerResponse> CreateGroupAsync(GroupForDto model, string userId);
        Task<GroupManagerResponse> UpdateGroupAsync(GroupForDto model, string userId);
        Task<GroupManagerResponse> LeaveGroupAsync(string groupId, string userId);
        Task<GroupManagerResponse> KickGroupAsync(string userId, KickMemberGroupForDto model);
        Task<GroupManagerResponse> EditMemberAsync(EditMemberGroupForDto model1, string adminUserId);
        Task<GroupManagerResponse> RemoveGroupAsync(string userId, RemoveGroupForDto model);
        Task<GroupManagerResponse> JoinGroupAsync(JoinGroupForDto model, string userId);
        Task<GroupManagerResponse> InviteGroupAsync(JoinGroupForDto model, string userId);
    }

    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _context;
        private UserManager<User> _userManager;
        private IWebHostEnvironment _environment;
        public GroupService(ApplicationDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<GroupManagerResponse> CreateGroupAsync(GroupForDto model, string userId)
        {
            if (model == null)
                throw new NullReferenceException("Group model is null.");


            await using (_context)
            {
                if (model.PhotoUrl.Length > 0)
                {
                    if (!Directory.Exists(_environment.WebRootPath + "..\\..\\..\\magnifisoccer-reactjs\\Upload\\"))
                    {
                        Directory.CreateDirectory(_environment.WebRootPath + "..\\..\\..\\magnifisoccer-reactjs\\Upload\\");
                    }

                    await using FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "..\\..\\..\\magnifisoccer-reactjs\\Upload\\" + model.PhotoUrl.FileName);
                    await model.PhotoUrl.CopyToAsync(fileStream);
                    fileStream.Flush();
                    var group = new Group
                    {
                        Id = Guid.NewGuid().ToString(),
                        GroupName = model.GroupName,
                        PhotoUrl = model.PhotoUrl.FileName
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
                        Message = "Group created successfully.",
                        IsSuccess = true
                    };


                }
                return new GroupManagerResponse
                {
                    Message = "Group not created.",
                    IsSuccess = false
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
                        Message = "Group was not found or 403 forbidden.",
                        IsSuccess = false
                    };
                }

                if (!Directory.Exists(_environment.WebRootPath + "..\\..\\..\\magnifisoccer-reactjs\\Upload\\"))
                {
                    Directory.CreateDirectory(_environment.WebRootPath + "..\\..\\..\\magnifisoccer-reactjs\\Upload\\");
                }

                if (model.GroupName!=null)
                {
                    group.GroupName = model.GroupName;
                }

                if (model.PhotoUrl!=null)
                {
                    await using FileStream fileStream = System.IO.File.Create(_environment.WebRootPath + "..\\..\\..\\magnifisoccer-reactjs\\Upload\\" + model.PhotoUrl.FileName);
                    await model.PhotoUrl.CopyToAsync(fileStream);
                    fileStream.Flush();
                    group.PhotoUrl = model.PhotoUrl.FileName;
                }

                _context.Update(group);
                await _context.SaveChangesAsync();

                return new GroupManagerResponse
                {
                    Message = "Group updated successfully.",
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
                    Message = "Left from group successfully.",
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
                    u.UserId == adminUserId && u.GroupId == model.GroupId).Result.Role != "Admin" || model.UserId == adminUserId)
                    return new GroupManagerResponse
                    {
                        Message = "You are not authorized to do this.",
                        IsSuccess = false
                    };


                if (groupMember == null || group == null)
                    return new GroupManagerResponse
                    {
                        Message = "There is no such person in the group or no such group.",
                        IsSuccess = false
                    };

                _context.Remove(groupMember);
                await _context.SaveChangesAsync();
                return new GroupManagerResponse
                {
                    Message = "Member was expelled from the group.",
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
                    await _context.GroupMembers.SingleOrDefaultAsync(u => u.UserId == model.UserId && u.GroupId == model.GroupId);

                if (_context.GroupMembers.SingleOrDefaultAsync(u =>
                    u.UserId == adminUserId && u.GroupId == model.GroupId).Result.Role != "Admin" || model.UserId == adminUserId)
                    return new GroupManagerResponse
                    {
                        Message = "You are not authorized to do this.",
                        IsSuccess = false
                    };

                if (groupMember == null || group == null)
                    return new GroupManagerResponse
                    {
                        Message = "There is no such person in the group or no such group.",
                        IsSuccess = false
                    };

                groupMember.Role = model.Role;
                _context.Update(groupMember);
                await _context.SaveChangesAsync();
                return new GroupManagerResponse
                {
                    Message = "Edited group member successfully.",
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
                    Message = "The group has been deleted.",
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
                if (await _context.GroupMembers.FirstOrDefaultAsync(u => u.UserId == userId && u.GroupId == model.GroupId) != null)
                {
                    return new GroupManagerResponse
                    {
                        Message = "You are already a member of the group.",
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
                    Message = "You have joined the group.",
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
                var inviteUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
                if (await _context.GroupMembers.FirstOrDefaultAsync(u => u.UserId == inviteUser.Id && u.GroupId == model.GroupId) != null)
                {
                    return new GroupManagerResponse
                    {
                        Message = "User are already a member of the group.",
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
                    Message = "User have joined the group.",
                    IsSuccess = true
                };
            }
        }

        public Group GetGroupAsync(string groupId)
        {
            using (_context)
            {
                return _context.Groups.SingleOrDefault(g => g.Id == groupId);
            }
        }

        public List<Group> GetListGroupAsync(string userId)
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

        public List<Group> GetAllGroupForSearch()
        {
            using (_context)
            {
                var groups = _context.Groups.ToList();
                return groups;
            }
        }
    }
}