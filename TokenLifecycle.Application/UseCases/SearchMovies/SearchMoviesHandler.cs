using MediatR;
using TokenLifecycle.Domain.Abstractions;

namespace TokenLifecycle.Application.UseCases.SearchMovies
{
    public class SearchMoviesHandler : IRequestHandler<SearchMoviesRequest, SearchMoviesResponse>
    {
        private readonly IMovieRepository _movieRepository;

        public SearchMoviesHandler(IMovieRepository movieRepository)
        {
            this._movieRepository = movieRepository;
        }

        public async Task<SearchMoviesResponse> Handle(SearchMoviesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var movies = await _movieRepository.SearchMoviesAsync(
                    request.SearchTerm,
                    request.MinYear,
                    request.MinRating,
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
