﻿using BoardGamesApi.Data;
using BoardGamesApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BoardGamesApi.Controllers
{
    /// <summary>
    /// Games endpoint of Board Games API.
    /// </summary>
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [Authorize]
    [Route("api/games")]
    [Route("api/v{api-version:apiVersion}/games")]
    public class GamesController : Controller
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ILogger<GamesController> _logger;

        public GamesController(
            IGamesRepository gamesRepository, ILogger<GamesController> logger)
        {
            _gamesRepository = gamesRepository;
            _logger = logger;
        }

        /// <summary>
        /// Delete the game with the given id.
        /// </summary>
        /// <param name="id">Id of the game to delete.</param>
        /// <response code="200">Game successfully deleted</response>
        /// <response code="404">Game with the given ID not found.</response>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            _logger.LogDebug($"Deleting game with id {id}");

            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var game = _gamesRepository.GetById(id);
            if (game == null)
                return NotFound();

            _gamesRepository.Delete(id);

            return Ok();
        }

        /// <summary>
        /// Get one page of games.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        /// <remarks>If you omit <c>page</c> and <c>size</c> query parameters, you'll get the first page with 10 games.</remarks>
        /// <response code="200">Returns a page of games.</response>
        /// <response code="404">Game with the given id not found.</response>
        [HttpGet]
        public ActionResult<PagedList<Game>> GetAll(int page = 1, int size = 10)
        {
            _logger.LogDebug("Getting one page of games");

            var games = _gamesRepository.GetPage(page, size);

            return games;
        }

        /// <summary>
        /// Get a single game by id.
        /// </summary>
        /// <param name="id">Id of the game to retrieve.</param>
        /// <response code="200">Returns the game with the given id.</response>
        /// <response code="404">Game with the given id not found.</response>
        [HttpGet("{id}")]
        public ActionResult<Game> GetById(string id)
        {
            _logger.LogDebug($"Getting a game with id {id}");

            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var game = _gamesRepository.GetById(id);

            if (game == null)
                return NotFound();

            return game;
        }

        /// <summary>
        /// Create a new game from the supplied data.
        /// </summary>
        /// <param name="model">Data to create the game from.</param>
        /// <response code="200">Returns the created game.</response>
        /// <response code="400">Supplied data is not valid.</response>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult<Game> Post(GameInput model)
        {
            _logger.LogDebug($"Creating a new game with title \"{model.Title}\"");

            var game = new Game();
            model.MapToGame(game);

            _gamesRepository.Create(game);

            return CreatedAtAction(
                nameof(GamesController.GetById), "games", new { id = game.Id }, game);
        }

        /// <summary>
        /// Updates the game with the given id.
        /// </summary>
        /// <param name="id">Id of the game to update.</param>
        /// <param name="model">Data to update the game from.</param>
        /// <response code="200">Returns the updated game.</response>
        /// <response code="400">Supplied data is not valid.</response>
        /// <response code="404">Game with the given id not found.</response>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public ActionResult<Game> Put(string id, GameInput model)
        {
            _logger.LogDebug($"Updating a game with id {id}");

            var game = _gamesRepository.GetById(id);

            if (game == null)
                return NotFound();

            model.MapToGame(game);

            _gamesRepository.Update(game);

            return game;
        }
    }
}
