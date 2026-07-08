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
            double minRating,
            CancellationToken cancellationToken);
    }
}
