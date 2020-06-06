using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MagnifiSoccer.API.Services;
using MagnifiSoccer.Shared.Dtos;
using MagnifiSoccer.Shared.Dtos.GroupDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagnifiSoccer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("{groupId}")]
        public IActionResult GetGroup(string groupId)
        {
            return Ok(_groupService.GetGroupAsync(groupId));
        }

        [HttpGet]
        public IActionResult GetListGroup()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // System.Threading.Thread.Sleep(5000);
            if (userId != null)
            {
                return Ok(_groupService.GetListGroupAsync(userId));
            }

            return BadRequest();
        }

        [HttpGet("all")]
        public IActionResult GetAllGroupForSearch()
        {
            return Ok(_groupService.GetAllGroupForSearch());
        }

        [HttpPost("create")]
        public IActionResult CreateGroup([FromForm]GroupForDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.CreateGroupAsync(model, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpPut("update")]
        public IActionResult UpdateGroup([FromForm]GroupForDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.UpdateGroupAsync(model, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpGet("leave/{groupId}")]
        public IActionResult LeaveGroup(string groupId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.LeaveGroupAsync(groupId, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpDelete("kick")]
        public IActionResult KickMember([FromBody]KickMemberGroupForDto model)
        {
            var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.KickGroupAsync(adminUserId, model);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpPut("editMember")]
        public IActionResult EditMember([FromBody]EditMemberGroupForDto model)
        {
            var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.EditMemberAsync(model, adminUserId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpDelete("removeGroup")]
        public IActionResult RemoveGroup([FromBody]RemoveGroupForDto model)
        {
            var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.RemoveGroupAsync(adminUserId, model);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpPost("join")]
        public IActionResult JoinGroup([FromBody]JoinGroupForDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.JoinGroupAsync(model, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpPost("invite")]
        public IActionResult InviteGroup([FromBody]JoinGroupForDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _groupService.InviteGroupAsync(model, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }
    }
}