using System.Collections.Generic;
using TokenLifecycle.Domain.DTOs;

namespace TokenLifecycle.Application.UseCases.SearchMovies
{
    public class SearchMoviesResponse
    {
        public List<MovieSearchResult> Movies { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
