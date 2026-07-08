using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using TokenLifecycle.Application.UseCases.SearchMovies;

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

        [HttpGet("search")]
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
    }
}
