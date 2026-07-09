using MediatR;

namespace TokenLifecycle.Application.UseCases.VectorSearchMovies
{
    public class VectorSearchMoviesRequest : IRequest<VectorSearchMoviesResponse>
    {
        public string Query { get; set; } = "journey through the universe and stars";
        public int Limit { get; set; } = 5;
    }
}
