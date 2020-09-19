using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MagnifiSoccer.API.Services;
using MagnifiSoccer.Shared.Dtos.GameDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagnifiSoccer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("create")]
        public IActionResult CreateGame([FromBody] CreateGameForDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _gameService.CreateGameAsync(model, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpPut("update")]
        public IActionResult UpdateGame([FromBody] UpdateGameForDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _gameService.UpdateGameAsync(model, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpDelete("remove")]
        public IActionResult RemoveGame([FromBody] RemoveGameForDto model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                var group = _gameService.RemoveGameAsync(model, userId);

                if (group.Result.IsSuccess)
                {
                    return Ok(group.Result);
                }

                return BadRequest(group.Result);
            }

            return BadRequest("Some properties are not valid.");
        }

        [HttpGet("{gameId}")]
        public IActionResult GetGame(int gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.GetGame(gameId, userId));
            }

            return BadRequest();
        }

        [HttpGet("forthComing")]
        public IActionResult GetGameForthComing(int gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.GetGameForthComing(userId));
            }

            return BadRequest();
        }

        [HttpGet]
        public IActionResult GetListGame()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.GetListGame(userId));
            }

            return BadRequest();
        }

        [HttpGet("rating")]
        public IActionResult GetListRating()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.GetListRating(userId));
            }

            return BadRequest();
        }


        [HttpGet("gameRating")]
        public IActionResult GetListGameRating()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.GetListGameRating(userId));
            }

            return BadRequest();
        }

        //[HttpPost("autoSquad")]
        //public IActionResult AutoSquad([FromBody]AutoSquadForDto model)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    if (userId != null)
        //    {
        //        return Ok(_gameService.AutoSquadAsync(model, userId).Result);
        //    }

        //    return BadRequest();
        //}

        [HttpPut("editSquad")]
        public IActionResult EditSquad([FromBody] EditSquadForDto model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId != null)
                {
                    return Ok(_gameService.EditSquadAsync(model, userId).Result);
                }
                return BadRequest();
            }
            return BadRequest("Some properties are not valid.");

        }

        [HttpGet("invite/{gameId}")]
        public IActionResult InviteUser(int gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.InviteUserAsync(userId, gameId).Result);
            }

            return BadRequest();
        }

        [HttpGet("updateSentMail/{gameId}")]
        public IActionResult UpdateSentMail(int gameId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.UpdateGameSentEmail(userId, gameId).Result);
            }

            return BadRequest();
        }

        [HttpPost("inviteResponse")]
        public IActionResult InviteResponseUser([FromBody]InviteResponseForDto model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId != null)
                {
                    return Ok(_gameService.InviteResponseAsync(model, userId).Result);
                }

                return BadRequest();
            }
            return BadRequest("Some properties are not valid.");
        }

        [HttpPost("playerRating")]
        public IActionResult PlayerRating([FromBody] PlayerRatingForDto model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId != null)
                {
                    return Ok(_gameService.PlayerRatingAsync(model, userId).Result);
                }

                return BadRequest();
            }
            return BadRequest("Some properties are not valid.");
        }

        [HttpPut("result")]
        public IActionResult Result([FromBody]ResultForDto model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                if (userId != null)
                {
                    return Ok(_gameService.ResultAsync(model, userId).Result);
                }

                return BadRequest();
            }
            return BadRequest("Some properties are not valid.");
        }


        [HttpGet("notice")]
        public IActionResult Notice()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.NoticeAsync(userId));
            }

            return BadRequest();
        }

        [HttpGet("getLastGameForGraph")]
        public IActionResult GetLastGameForGraph()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (userId != null)
            {
                return Ok(_gameService.GetLastGameForGraph(userId));
            }

            return BadRequest();
        }
    }
}