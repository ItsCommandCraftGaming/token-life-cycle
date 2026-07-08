using MediatR;

namespace TokenLifecycle.Application.UseCases.SearchMovies
{
    public class SearchMoviesRequest : IRequest<SearchMoviesResponse>
    {
        public string SearchTerm { get; set; } = "space travel action";
        public int MinYear { get; set; } = 1995;
        public double MinRating { get; set; } = 7.5;
    }
}
