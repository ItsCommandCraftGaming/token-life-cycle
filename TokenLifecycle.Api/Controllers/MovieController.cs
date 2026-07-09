using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TokenLifecycle.Application.UseCases.SearchMovies;
using TokenLifecycle.Application.UseCases.VectorSearchMovies;

namespace TokenLifecycle.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MovieController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Search movies from MongoDB.
        /// </summary>
        /// <param name="searchTerm">Allowed for: USER &amp; ADMIN</param>
        /// <param name="minYear">Allowed for: USER &amp; ADMIN</param>
        /// <param name="maxYear">Allowed for: ADMIN ONLY</param>
        /// <param name="minRating">Allowed for: USER &amp; ADMIN</param>
        /// <param name="maxRating">Allowed for: ADMIN ONLY</param>
        /// <param name="genreShould">Allowed for: ADMIN ONLY (Fixed to 'Sci-Fi' for USER)</param>
        /// <param name="genreMustNot">Allowed for: ADMIN ONLY (Fixed to 'Horror' for USER)</param>
        /// <param name="minRuntime">Allowed for: ADMIN ONLY</param>
        /// <param name="limit">Allowed for: ADMIN ONLY (Fixed to 5 for USER)</param>
        /// <param name="cancellationToken"></param>
        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> Search(
            [FromQuery] string searchTerm = "space travel action",
            [FromQuery] int minYear = 1995,
            [FromQuery] int? maxYear = null,
            [FromQuery] double minRating = 7.5,
            [FromQuery] double? maxRating = null,
            [FromQuery] string genreShould = "Sci-Fi",
            [FromQuery] string genreMustNot = "Horror",
            [FromQuery] int? minRuntime = null,
            [FromQuery] int limit = 5,
            CancellationToken cancellationToken = default)
        {
            // Extragem rolul din Claims
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            bool isAdmin = roleClaim == "Admin" || roleClaim == "2" || User.IsInRole("Admin") || User.IsInRole("2");

            if (!isAdmin)
            {
                // Dacă userul normal încearcă să folosească/modifice orice alt filtru în afară de cele 3 permise, returnăm 403 Unauthorized
                if (maxYear != null ||
                    maxRating != null ||
                    genreShould != "Sci-Fi" ||
                    genreMustNot != "Horror" ||
                    minRuntime != null ||
                    limit != 5)
                {
                    return StatusCode(403, new { message = "Unauthorized: Only Admins can use advanced filters." });
                }
            }

            var request = new SearchMoviesRequest
            {
                SearchTerm = searchTerm,
                MinYear = minYear,
                MaxYear = maxYear,
                MinRating = minRating,
                MaxRating = maxRating,
                GenreShould = genreShould,
                GenreMustNot = genreMustNot,
                MinRuntime = minRuntime,
                Limit = limit
            };

            var response = await _mediator.Send(request, cancellationToken);

            if (response.Movies == null || (response.Movies.Count == 0 && !string.IsNullOrEmpty(response.Message) && response.Message.StartsWith("Error")))
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response);
        }

        /// <summary>
        /// Search movies using Vector Search (semantic similarity).
        /// </summary>
        /// <param name="query">The search query to vectorize.</param>
        /// <param name="limit">The maximum number of results to return (Default: 5).</param>
        /// <param name="cancellationToken"></param>
        [HttpGet("vector-search")]
        [Authorize]
        public async Task<IActionResult> VectorSearch(
            [FromQuery] string query = "journey through the universe and stars",
            [FromQuery] int limit = 5,
            CancellationToken cancellationToken = default)
        {
            var request = new VectorSearchMoviesRequest
            {
                Query = query,
                Limit = limit
            };

            var response = await _mediator.Send(request, cancellationToken);

            if (response.Movies == null || (response.Movies.Count == 0 && !string.IsNullOrEmpty(response.Message) && response.Message.StartsWith("Error")))
            {
                return BadRequest(new { message = response.Message });
            }

            return Ok(response);
        }
    }
}
