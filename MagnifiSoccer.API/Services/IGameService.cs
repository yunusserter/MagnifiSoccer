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
        Task<GameManagerResponse> UpdateGameSentEmail(string userId, int gameId);
        List<Game> NoticeAsync(string userId);
        Game GetGameForthComing(string userId);
        List<GamePlayer> GetListGameRating(string userId);
        List<GamePlayer> GetLastGameForGraph(string userId);
    }

    public class GameService : IGameService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;
        public IConfiguration Configuration { get; }

        public GameService(ApplicationDbContext context, IMailService mailService, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            _mailService = mailService;
            _userManager = userManager;
            Configuration = configuration;
        }

        public async Task<GameManagerResponse> CreateGameAsync(CreateGameForDto model, string userId)
        {
            if (model == null)
                throw new NullReferenceException("Game model is null.");


            if (_context.GroupMembers.FirstOrDefault(p =>
                p.UserId == userId && p.GroupId == model.GroupId && p.Role == "Admin") == null)
                return new GameManagerResponse
                {
                    Message = "Sadece grup yöneticisi oyun oluşturabilir.",
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
                    Message = "Oyun başarıyla oluşturuldu.",
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
                        Message = "Oyun bulunamadı veya yetkiniz yok.",
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
                    Message = "Oyun başarıyla düzenlendi.",
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
                var players = _context.GamePlayers.Where(x => x.GameId == model.GameId).ToList();

                if (userClaim == null || game == null)
                    return new GameManagerResponse
                    {
                        Message = "Oyun bulunamadı veya yetkiniz yok.",
                        IsSuccess = false
                    };

                foreach (var player in players)
                {
                    var user = _userManager.FindByIdAsync(player.UserId).Result;
                    await _mailService.SendMailAsync(user.Email, "Davetli olduğunuz oyun iptal edildi.",
                        $"<h1>Davetli olduğunuz oyun iptal oldu!</h1><p>İptal edilen oyun bilgileri: {game.GameDate} {game.Location}.</p>");
                }

                _context.RemoveRange(players);
                _context.Remove(game);
                await _context.SaveChangesAsync();
                return new GameManagerResponse
                {
                    Message = "Oyun başarıyla silindi.",
                    IsSuccess = true
                };
            }
        }

        public Game GetGame(int gameId, string userId)
        {
            using (_context)
            {
                var game = _context.Games.Include(x => x.GamePlayers).FirstOrDefault(g => g.Id == gameId);
                if (game == null)
                {
                    return null;
                }
                var group = _context.GroupMembers.Where(u => u.UserId == userId && u.Role == "Admin");

                return @group.Any(u => u.GroupId == game.GroupId) ? game : null;
            }
        }

        public List<Game> GetListGame(string userId)
        {
            var game = new List<Game>();
            var myGames = _context.GamePlayers.Where(p => p.UserId == userId).ToList();
            var isAdmin = _context.GroupMembers.Where(p => p.UserId == userId && p.Role == "Admin").ToList();
            foreach (var temp in isAdmin.Select(g => _context.Games.Include(x => x.Group.GroupMembers).Where(p =>
                  p.GroupId == g.GroupId && p.GamePlayers.SingleOrDefault(x => x.UserId == userId) == null).ToList()))
            {
                game.AddRange(temp);
            }

            foreach (var g in myGames)
            {
                game.Add(_context.Games.Include(x => x.Group.GroupMembers).FirstOrDefault(x => x.Id == g.GameId));
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
                g.Group = _context.Groups.FirstOrDefault(x => x.Id == g.GroupId);
            }

            return game.OrderByDescending(o => o.GameDate).ToList();
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

                var users = model.Team1Users.ToList();
                var positions = model.Team1Positions.ToList();
                users.AddRange(model.Team2Users);
                positions.AddRange(model.Team2Positions);

                var team = 1;
                for (var i = 0; i < model.Team1Positions.Count + model.Team2Positions.Count; i++)
                {
                    if (i == model.Team1Positions.Count)
                        team = 2;

                    squad.Add(new GamePlayer { UserId = users[i], Position = positions[i], GameId = model.GameId, Team = team.ToString(), InviteResponse = oldSquad.Find(x => x.UserId == users[i])?.InviteResponse });
                }

                await _context.AddRangeAsync(squad);
                await _context.SaveChangesAsync();
                return new GameManagerResponse { Message = "Kadro düzenlendi.", IsSuccess = true };
            }
        }

        public async Task<GameManagerResponse> InviteUserAsync(string userId, int gameId)
        {
            await using (_context)
            {
                var url = "magnifisoccer-fc484.web.app/games";
                string position = null;
                var game = await _context.Games.SingleOrDefaultAsync(g => g.Id == gameId);
                var gamePlayers = _context.GamePlayers.Where(u => u.GameId == game.Id && u.InviteResponse == null).ToList();

                foreach (var player in gamePlayers)
                {
                    switch (player.Position)
                    {
                        case 0: position = "KALECİ"; break;
                        case 1: position = "DEFANS"; break;
                        case 2: position = "ORTA SAHA"; break;
                        case 3: position = "FORVET"; break;
                    }

                    var user = await _context.Users.FindAsync(player.UserId);
                    await _mailService.SendMailAsync(user.Email, "Yeni bir oyuna davet edildiniz",
                        $"<h1>Tebrikler!</h1><p>Yeni bir oyuna davet edildiniz. Katılım durumunuzu lütfen <a href='{url}'> buraya tıklayarak</a> belirtiniz. Oyun bilgileri: {position} {game.GameDate} {game.Location}.</p>");
                }
                return new GameManagerResponse
                {
                    IsSuccess = true,
                    Message = "E-posta gönderimi başarılı."
                };
            }
        }

        public async Task<GameManagerResponse> UpdateGameSentEmail(string userId, int gameId)
        {
            await using (_context)
            {
                var game = await _context.Games.FindAsync(gameId);

                var players = _context.GamePlayers.Where(x => x.GameId == gameId).ToList();

                string position = null;
                foreach (var player in players)
                {
                    switch (player.Position)
                    {
                        case 0: position = "KALECİ"; break;
                        case 1: position = "DEFANS"; break;
                        case 2: position = "ORTA SAHA"; break;
                        case 3: position = "FORVET"; break;
                    }

                    var user = _userManager.FindByIdAsync(player.UserId).Result;
                    await _mailService.SendMailAsync(user.Email, "Davetli olduğunuz oyunda değişiklik oldu",
                        $"<h1>Davetli olduğunuz oyunda değişiklik oldu!</h1><p>Oyun bilgileri: {position} {game.GameDate} {game.Location}.</p>");
                }
                return new GameManagerResponse
                {
                    Message = "Gönderildi.",
                    IsSuccess = true
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
                    Message = $"Katılım durumu: {model.Response}."
                };
            }
        }

        public async Task<GameManagerResponse> PlayerRatingAsync(PlayerRatingForDto model, string userId)
        {
            await using (_context)
            {
                if (model.Rating.Count != model.UserId.Count || model.Rating.Count == 0)
                {
                    return new GameManagerResponse { Message = "Eksik giriş. Lütfen kontrol ediniz.", IsSuccess = false };
                }

                var game = await _context.Games.SingleOrDefaultAsync(g => g.Id == model.GameId);
                if (game.GameDate == null && game.GameDate > DateTime.Now)
                {
                    return new GameManagerResponse { Message = "Oyun oynanmadan oylama yapılamaz.", IsSuccess = false };
                }

                var gamePlayer = _context.GamePlayers.Where(u => u.GameId == model.GameId).ToList();
                var index = 0;
                var currentUserTeam = gamePlayer.SingleOrDefault(p => p.UserId == userId)?.Team;

                if (_context.Ratings.Any(p => p.GameId == model.GameId && p.VoterUserId == userId))
                {
                    return new GameManagerResponse { Message = "Tekrar oy veremezsiniz.", IsSuccess = false };
                }

                foreach (var rat in model.Rating)
                {
                    if (rat < 4 || rat > 10)
                    {
                        return new GameManagerResponse { Message = "Lütfen 4-10 aralığında puanlayınız.", IsSuccess = false };
                    }
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
                    await _context.SaveChangesAsync();
                    var allRating = _context.Ratings.Where(x => x.GameId == model.GameId && x.UserId == playerId).Select(x => x.RatingValue).ToList();
                    var gameTemp = _context.GamePlayers
                        .SingleOrDefault(x => x.GameId == model.GameId && x.UserId == playerId);
                    if (gameTemp == null) continue;
                    gameTemp.GameRating = allRating.Sum() / allRating.Count;
                    _context.Update(gameTemp);
                    await _context.SaveChangesAsync();
                    var user = await _context.Users.FindAsync(playerId);
                    user.OverAllRating = _context.GamePlayers.Where(x => x.UserId == playerId && x.GameRating != 0).Average(x => x.GameRating);
                    _context.Update(user);
                }

                await _context.SaveChangesAsync();
                return new GameManagerResponse { Message = "Oyuncular başarıyla oylandı.", IsSuccess = true };

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
                    return new GameManagerResponse { Message = "Bu işlem için yetkiniz yok.", IsSuccess = false };
                }

                if (game.GameDate.Value > DateTime.Now)
                {
                    return new GameManagerResponse { Message = "Oyun henüz oynanmadı.", IsSuccess = false };
                }


                game.WinnerTeam = model.WinnerTeam;

                if (model.Attendees != null && model.Players != null)
                {
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
                }


                await _context.SaveChangesAsync();
                return new GameManagerResponse { Message = "Oyun düzenleme başarılı.", IsSuccess = true };
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

        public List<Game> NoticeAsync(string userId)
        {
            var games = _context.GamePlayers.Where(x => x.UserId == userId && x.InviteResponse == null).ToList();
            var result = new List<Game>();
            foreach (var game in games)
            {
                result.Add(_context.Games.Find(game.GameId));
            }

            return result.OrderByDescending(o => o.GameDate).ToList();
        }

        public Game GetGameForthComing(string userId)
        {
            using (_context)
            {
                var currentDate = DateTime.Now;
                var games = _context.GamePlayers.Include(x => x.Game)
                    .Where(x => x.UserId == userId && x.Game.GameDate.Value > currentDate).OrderBy(o => o.Game.GameDate).ToList();

                if (games.Count > 0)
                {
                    foreach (var gamePlayer in games[0].Game.GamePlayers)
                    {
                        var temp = _userManager.FindByIdAsync(gamePlayer.UserId).Result;
                        gamePlayer.User = new User { FirstName = temp.FirstName, LastName = temp.LastName };
                    }
                    return games[0].Game;
                }

                return null;
            }
        }

        public List<GamePlayer> GetListGameRating(string userId)
        {
            using (_context)
            {
                var myGroups = _context.GroupMembers.Where(x => x.UserId == userId).ToList();
                var groupMembers = new List<GroupMember>();
                foreach (var g in myGroups)
                {
                    groupMembers.AddRange(_context.GroupMembers.Where(x => x.GroupId == g.GroupId));
                }
                var gamePlayers = new List<GamePlayer>();
                foreach (var g in groupMembers)
                {
                    gamePlayers.AddRange(_context.GamePlayers.Include(x => x.User).Where(x => x.UserId == g.UserId && x.GameRating > 0));
                }

                return gamePlayers.Distinct().ToList();
            }
        }

        public List<GamePlayer> GetLastGameForGraph(string userId)
        {
            using (_context)
            {
                var games = _context.GamePlayers.Include(x => x.Game)
                    .Where(x => x.UserId == userId && x.GameRating > 0).OrderBy(o => o.Game.GameDate).ToList();

                return games.Count > 0 ? games : null;
            }
        }

    }

}