using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TokenLifecycle.Domain.DTOs;

namespace TokenLifecycle.Domain.Abstractions
{
    public interface IMovieRepository
    {
        Task<List<MovieSearchResult>> SearchMoviesAsync(
            string searchTerm,
            int minYear,
            int? maxYear,
            double minRating,
            double? maxRating,
            string genreShould,
            string genreMustNot,
            int? minRuntime,
            int limit,
            CancellationToken cancellationToken);

        Task<List<MovieSearchResult>> VectorSearchMoviesAsync(
            string query,
            int limit,
            CancellationToken cancellationToken);
    }
}
