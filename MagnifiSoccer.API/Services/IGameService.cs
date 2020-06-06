using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MagnifiSoccer.API.Models;
using MagnifiSoccer.Shared.Dtos.GameDtos;
using MagnifiSoccer.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MagnifiSoccer.API.Services
{
    public interface IGameService
    {
        Task<GameManagerResponse> CreateGameAsync(CreateGameForDto model, string userId);
        Task<GameManagerResponse> UpdateGameAsync(UpdateGameForDto model, string userId);
        Task<GameManagerResponse> RemoveGameAsync(RemoveGameForDto model, string userId);
        Game GetGame(int gameId, string userId);
        List<Game> GetListGame(string userId);
        // Task<GameManagerResponse> AutoSquadAsync(AutoSquadForDto model, string userId);
        Task<GameManagerResponse> EditSquadAsync(EditSquadForDto model, string userId);
        Task<GameManagerResponse> InviteUserAsync(string userId, int gameId);
        Task<GameManagerResponse> InviteResponseAsync(InviteResponseForDto model, string userId);
        Task<GameManagerResponse> PlayerRatingAsync(PlayerRatingForDto model, string userId);
        Task<GameManagerResponse> ResultAsync(ResultForDto model, string userId);
        List<Rating> GetListRating(string userId);
    }

    public class GameService : IGameService
    {
        private readonly ApplicationDbContext _context;
        private IMailService _mailService;
        private UserManager<User> _userManager;
        private IConfiguration _configuration;
        public GameService(ApplicationDbContext context, IMailService mailService, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            _mailService = mailService;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<GameManagerResponse> CreateGameAsync(CreateGameForDto model, string userId)
        {
            if (model == null)
                throw new NullReferenceException("Game model is null.");


            if (_context.GroupMembers.FirstOrDefault(p =>
                p.UserId == userId && p.GroupId == model.GroupId && p.Role == "Admin") == null)
                return new GameManagerResponse
                {
                    Message = "Only admins can create.",
                    IsSuccess = false
                };

            await using (_context)
            {
                var game = new Game
                {
                    HomeTeam = "1",
                    AwayTeam = "2",
                    WinnerTeam = "",
                    GameDate = model.GameDate,
                    Location = model.Location,
                    Price = model.Price,
                    GroupId = model.GroupId
                };

                await _context.AddAsync(game);
                await _context.SaveChangesAsync();

                return new GameManagerResponse
                {
                    Message = "Game created successfully.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GameManagerResponse> UpdateGameAsync(UpdateGameForDto model, string userId)
        {
            if (model == null)
                throw new NullReferenceException("Game model is null.");


            await using (_context)
            {
                var userClaim = await _context.GroupMembers.SingleOrDefaultAsync(u =>
                   u.UserId == userId && u.GroupId == model.GroupId && u.Role == "Admin");
                var game = await _context.Games.FindAsync(model.GameId);

                if (game == null || userClaim == null)
                {
                    return new GameManagerResponse
                    {
                        Message = "Game was not found or 403 forbidden.",
                        IsSuccess = false
                    };
                }

                game.GameDate = model.GameDate;
                game.Location = model.Location;
                game.Price = model.Price;
                game.WinnerTeam = model.WinnerTeam;

                _context.Update(game);
                await _context.SaveChangesAsync();

                return new GameManagerResponse
                {
                    Message = "Game updated successfully.",
                    IsSuccess = true
                };
            }
        }

        public async Task<GameManagerResponse> RemoveGameAsync(RemoveGameForDto model, string userId)
        {
            if (model == null || userId == null)
                throw new NullReferenceException("Model is null.");

            await using (_context)
            {
                var userClaim = await _context.GroupMembers.SingleOrDefaultAsync(u =>
                    u.UserId == userId && u.GroupId == model.GroupId && u.Role == "Admin");
                var game = await _context.Games.FindAsync(model.GameId);

                if (userClaim == null || game == null)
                    return new GameManagerResponse
                    {
                        Message = "Game was not found or 403 forbidden.",
                        IsSuccess = false
                    };

                _context.Remove(game);
                await _context.SaveChangesAsync();
                return new GameManagerResponse
                {
                    Message = "The game has been deleted.",
                    IsSuccess = true
                };
            }
        }

        public Game GetGame(int gameId, string userId)
        {
            using (_context)
            {
                var game = _context.Games.FirstOrDefault(g => g.Id == gameId);
                var group = _context.GroupMembers.Where(u => u.UserId == userId);
                return @group.Any(u => u.GroupId == game.GroupId) ? game : null;
            }
        }

        public List<Game> GetListGame(string userId)
        {
            using (_context)
            {
                var game = new List<Game>();
                var myGames = _context.GamePlayers.Where(p => p.UserId == userId).ToList();
                var isAdmin = _context.GroupMembers.FirstOrDefault(p => p.UserId == userId && p.Role == "Admin");
                if (isAdmin!=null)
                {
                    var temp = _context.Games.SingleOrDefault(p =>
                        p.GroupId == isAdmin.GroupId && p.GamePlayers.Count == 0);
                    if (temp != null)
                    {
                        game.Add(temp);
                    }
                }

                foreach (var g in myGames)
                {
                    game.Add(_context.Games.Find(g.GameId));
                }

                foreach (var g in game)
                {
                    var players = _context.GamePlayers.Where(p => p.GameId == g.Id).ToList();
                    foreach (var player in players)
                    {
                        var temp = _userManager.FindByIdAsync(player.UserId).Result;
                        player.User = new User { FirstName = temp.FirstName, LastName = temp.LastName };
                    }
                    g.GamePlayers = players;
                    g.Group = _context.Groups.Find(g.GroupId);
                }

                return game.OrderByDescending(o => o.GameDate).ToList();
            }
        }

        //public async Task<GameManagerResponse> AutoSquadAsync(AutoSquadForDto model, string userId)
        //{
        //    if (model.Capacity % 2 != 0)
        //    {
        //        return new GameManagerResponse { Message = "Capacity must be an even number", IsSuccess = false };
        //    }
        //    await using (_context)
        //    {
        //        var game = await _context.Games.FindAsync(model.GameId);
        //        var userClaim = await _context.UserClaims.SingleOrDefaultAsync(u =>
        //            u.UserId == userId && u.ClaimType == game.GroupId && u.ClaimValue == "Admin");

        //        if (userClaim == null || game == null)
        //            return new GameManagerResponse
        //            {
        //                Message = "Game was not found or 403 forbidden.",
        //                IsSuccess = false
        //            };

        //        var groups = new List<Group>();
        //        for (var i = 0; i < model.GroupId.Count; i++)
        //        {
        //            groups.AddRange(_context.Groups.Where(g => g.Id == model.GroupId[i]));
        //        }

        //        var groupUsers = new List<IdentityUserClaim<string>>();
        //        foreach (var group in groups)
        //        {
        //            groupUsers.AddRange(_context.UserClaims.Where(u => u.ClaimType == group.Id));
        //        }

        //        var players = new List<User>();
        //        foreach (var user in groupUsers)
        //        {
        //            if (players.Any(p => p.Id == user.UserId))
        //            {
        //                continue;
        //            }
        //            players.Add(await _context.Users.FirstOrDefaultAsync(u => u.Id == user.UserId));
        //        }

        //        var userRating = rating.GroupBy(g => g.Id).Select(g => new
        //        {
        //            g.Key,
        //            Rating = g.Average(r => r.Rating),
        //            Position = g.Select(r => r.Position).ToList()
        //        }).OrderByDescending(o => o.Rating).ToList();

        //        var oldSquad = _context.GamePlayers.Where(p => p.GameId == model.GameId).ToList();
        //        _context.RemoveRange(oldSquad);

        //        var squad = new List<GamePlayer>();
        //        for (var i = 0; i < model.Capacity; i++)  // GELİŞTİR
        //        {
        //            var player = userRating[i];
        //            var result = player.Position.GroupBy(p => p).OrderBy(g => g.Count()).Select(g => g.Key).ToList();
        //            var mostFrequent = result.Last();
        //            squad.Add(new GamePlayer
        //            {
        //                UserId = player.Key,
        //                GameId = model.GameId,
        //                Team = ((i % 2) + 1).ToString(),
        //                Position = mostFrequent
        //            });
        //        }

        //        await _context.AddRangeAsync(squad);
        //        await _context.SaveChangesAsync();
        //        return new GameManagerResponse { Message = "Automatic squad created.", IsSuccess = true };
        //    }
        //}

        public async Task<GameManagerResponse> EditSquadAsync(EditSquadForDto model, string userId)
        {
            await using (_context)
            {
                var oldSquad = _context.GamePlayers.Where(p => p.GameId == model.GameId).ToList();
                if (oldSquad.Any())
                {
                    _context.RemoveRange(oldSquad);
                }

                var squad = new List<GamePlayer>();
                var users = new List<string>();
                var positions = new List<int>();

                foreach (var u in model.Team1Users)
                    users.Add(u);

                foreach (var p in model.Team1Positions)
                    positions.Add(p);

                foreach (var u in model.Team2Users)
                    users.Add(u);

                foreach (var p in model.Team2Positions)
                    positions.Add(p);

                var team = 1;
                for (var i = 0; i < model.Team1Positions.Count + model.Team2Positions.Count; i++)
                {
                    if (i == model.Team1Positions.Count)
                        team = 2;

                    squad.Add(new GamePlayer { UserId = users[i], Position = positions[i], GameId = model.GameId, Team = team.ToString() });
                }

                await _context.AddRangeAsync(squad);
                await _context.SaveChangesAsync();
                return new GameManagerResponse { Message = "Squad edited.", IsSuccess = true };
            }
        }

        public async Task<GameManagerResponse> InviteUserAsync(string userId, int gameId)
        {
            await using (_context)
            {
                var game = await _context.Games.SingleOrDefaultAsync(g => g.Id == gameId);
                var gamePlayers = _context.GamePlayers.Where(u => u.GameId == game.Id).ToList();

                foreach (var player in gamePlayers)
                {
                    var user = _context.Users.Find(player.UserId);
                    await _mailService.SendMailAsync(user.Email, "A new event.",
                        $"<h1>Congratulations!</h1><p>You have been invited to a new event. Event information:{game.GameDate} {game.Location}.</p>");
                }
                return new GameManagerResponse
                {
                    IsSuccess = true,
                    Message = "Sent to email successfully!"
                };
            }
        }

        public async Task<GameManagerResponse> InviteResponseAsync(InviteResponseForDto model, string userId)
        {
            await using (_context)
            {
                var gamePlayer = await _context.GamePlayers.SingleOrDefaultAsync(u => u.UserId == userId && u.GameId == model.GameId);
                gamePlayer.InviteResponse = model.Response;
                _context.Update(gamePlayer);
                await _context.SaveChangesAsync();
                return new GameManagerResponse
                {
                    IsSuccess = true,
                    Message = $"Invite response: {model.Response}."
                };
            }
        }

        public async Task<GameManagerResponse> PlayerRatingAsync(PlayerRatingForDto model, string userId)
        {
            await using (_context)
            {
                var game = await _context.Games.SingleOrDefaultAsync(g => g.Id == model.GameId);
                if (game.GameDate == null && game.GameDate > DateTime.Now)
                {
                    return new GameManagerResponse { Message = "You cannot rate before the match is completed.", IsSuccess = false };
                }

                var gamePlayer = _context.GamePlayers.Where(u => u.GameId == model.GameId).ToList();
                var index = 0;
                var currentUserTeam = gamePlayer.SingleOrDefault(p => p.UserId == userId)?.Team;

                if (_context.Ratings.Any(p => p.GameId == model.GameId && p.VoterUserId == userId))
                {
                    return new GameManagerResponse { Message = "You have already rated.", IsSuccess = false };
                }

                foreach (var playerId in model.UserId)
                {
                    if (playerId == userId)
                    {
                        index++;
                        continue;
                    }
                    var player = gamePlayer.FirstOrDefault(p => p.UserId == playerId);
                    if (player == null || currentUserTeam != player.Team) continue;
                    var rating = new Rating { UserId = playerId, VoterUserId = userId, GameId = model.GameId, RatingValue = model.Rating[index] };
                    index++;
                    _context.Add(rating);
                }
                await _context.SaveChangesAsync();
                return new GameManagerResponse { Message = "Players have been successfully rated.", IsSuccess = true };

            }
        }

        public async Task<GameManagerResponse> ResultAsync(ResultForDto model, string userId)
        {
            await using (_context)
            {
                var game = _context.Games.Find(model.GameId);
                if (_context.GroupMembers.FirstOrDefault(u =>
                    u.GroupId == game.GroupId && u.UserId == userId && u.Role == "Admin") == null)
                {
                    return new GameManagerResponse { Message = "You are not authorized to do this.", IsSuccess = false };
                }

                if (game.GameDate.Value > DateTime.Now)
                {
                    return new GameManagerResponse { Message = "Match have been played yet.", IsSuccess = false };
                }


                game.WinnerTeam = model.WinnerTeam;
                var gamePlayers = _context.GamePlayers.Where(p => p.GameId == model.GameId && p.InviteResponse == true).ToList();
                var index = 0;
                foreach (var player in model.Players)
                {
                    var temp = gamePlayers.FirstOrDefault(u => u.GameId == model.GameId && u.UserId == player);
                    temp.Attended = model.Attendees[index];
                    if (model.Attendees[index])
                    {
                        var user = await _userManager.FindByIdAsync(temp.UserId);
                        user.Remainder -= game.Price.Value;
                    }

                    _context.Update(temp);
                    index++;
                }

                await _context.SaveChangesAsync();
                return new GameManagerResponse { Message = "The process has been completed.", IsSuccess = true };
            }
        }

        public List<Rating> GetListRating(string userId)
        {
            using (_context)
            {
                var gameIdList = _context.GamePlayers.Where(p => p.UserId == userId).ToList();
                var games = new List<Game>();
                var rating = new List<Rating>();

                foreach (var game in gameIdList)
                {
                    games.Add(_context.Games.Find(game.GameId));
                }

                foreach (var game in games)
                {
                    rating.AddRange(_context.Ratings.Where(p => p.GameId == game.Id));
                }

                return rating;
            }
        }
    }
}