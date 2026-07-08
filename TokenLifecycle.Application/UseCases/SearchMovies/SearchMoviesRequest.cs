using MediatR;

namespace TokenLifecycle.Application.UseCases.SearchMovies
{
    public class SearchMoviesRequest : IRequest<SearchMoviesResponse>
    {
        public string SearchTerm { get; set; } = "space travel action";
        public int MinYear { get; set; } = 1995;
        public int? MaxYear { get; set; }
        public double MinRating { get; set; } = 7.5;
        public double? MaxRating { get; set; }
        public string GenreShould { get; set; } = "Sci-Fi";
        public string GenreMustNot { get; set; } = "Horror";
        public int? MinRuntime { get; set; }
        public int Limit { get; set; } = 5;
    }
}
