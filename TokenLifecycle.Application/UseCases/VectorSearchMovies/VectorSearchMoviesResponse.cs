using System.Collections.Generic;
using TokenLifecycle.Domain.DTOs;

namespace TokenLifecycle.Application.UseCases.VectorSearchMovies
{
    public class VectorSearchMoviesResponse
    {
        public List<MovieSearchResult> Movies { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
