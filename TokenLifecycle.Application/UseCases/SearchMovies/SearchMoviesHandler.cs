using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TokenLifecycle.Domain.Abstractions;

namespace TokenLifecycle.Application.UseCases.SearchMovies
{
    public class SearchMoviesHandler : IRequestHandler<SearchMoviesRequest, SearchMoviesResponse>
    {
        private readonly IMovieRepository _movieRepository;

        public SearchMoviesHandler(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<SearchMoviesResponse> Handle(SearchMoviesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var movies = await _movieRepository.SearchMoviesAsync(
                    request.SearchTerm,
                    request.MinYear,
                    request.MaxYear,
                    request.MinRating,
                    request.MaxRating,
                    request.GenreShould,
                    request.GenreMustNot,
                    request.MinRuntime,
                    request.Limit,
                    cancellationToken);

                return new SearchMoviesResponse
                {
                    Movies = movies,
                    Message = "Movies loaded successfully."
                };
            }
            catch (Exception ex)
            {
                return new SearchMoviesResponse
                {
                    Message = $"Error retrieving movies: {ex.Message}"
                };
            }
        }
    }
}
