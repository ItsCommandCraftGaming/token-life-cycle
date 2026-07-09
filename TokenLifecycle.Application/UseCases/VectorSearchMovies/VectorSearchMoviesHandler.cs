using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TokenLifecycle.Domain.Abstractions;

namespace TokenLifecycle.Application.UseCases.VectorSearchMovies
{
    public class VectorSearchMoviesHandler : IRequestHandler<VectorSearchMoviesRequest, VectorSearchMoviesResponse>
    {
        private readonly IMovieRepository _movieRepository;

        public VectorSearchMoviesHandler(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<VectorSearchMoviesResponse> Handle(VectorSearchMoviesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var movies = await _movieRepository.VectorSearchMoviesAsync(
                    request.Query,
                    request.Limit,
                    cancellationToken);

                return new VectorSearchMoviesResponse
                {
                    Movies = movies,
                    Message = "Movies loaded successfully via vector search."
                };
            }
            catch (Exception ex)
            {
                return new VectorSearchMoviesResponse
                {
                    Message = $"Error retrieving movies via vector search: {ex.Message}"
                };
            }
        }
    }
}
