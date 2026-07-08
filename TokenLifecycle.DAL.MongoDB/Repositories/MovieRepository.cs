using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TokenLifecycle.Domain.Abstractions;
using TokenLifecycle.Domain.DTOs;

namespace TokenLifecycle.DAL.MongoDB.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IMongoCollection<BsonDocument> _movieCollection;

        public MovieRepository(Database.Database db)
        {
            _movieCollection = db.GetCollectionFromDatabase<BsonDocument>("sample_mflix", "movies");
        }

        public async Task<List<MovieSearchResult>> SearchMoviesAsync(
            string searchTerm,
            int minYear,
            int? maxYear,
            double minRating,
            double? maxRating,
            string genreShould,
            string genreMustNot,
            int? minRuntime,
            int limit,
            CancellationToken cancellationToken)
        {
            // Build the compound search stage
            var compound = new BsonDocument
            {
                { "must", new BsonArray
                    {
                        new BsonDocument
                        {
                            { "text", new BsonDocument
                                {
                                    { "query", searchTerm },
                                    { "path", new BsonArray { "plot", "fullplot", "title" } }
                                }
                            }
                        }
                    }
                }
            };

            if (!string.IsNullOrWhiteSpace(genreShould))
            {
                compound.Add("should", new BsonArray
                {
                    new BsonDocument
                    {
                        { "text", new BsonDocument
                            {
                                { "query", genreShould },
                                { "path", "genres" }
                            }
                        }
                    }
                });
            }

            if (!string.IsNullOrWhiteSpace(genreMustNot))
            {
                compound.Add("mustNot", new BsonArray
                {
                    new BsonDocument
                    {
                        { "text", new BsonDocument
                            {
                                { "query", genreMustNot },
                                { "path", "genres" }
                            }
                        }
                    }
                });
            }

            var searchStage = new BsonDocument
            {
                { "$search", new BsonDocument
                    {
                        { "index", "atlas_search" },
                        { "compound", compound }
                    }
                }
            };

            // Build match stage dynamically
            var matchFilters = new BsonDocument();

            // Year filter
            var yearFilter = new BsonDocument { { "$gte", minYear } };
            if (maxYear.HasValue)
            {
                yearFilter.Add("$lte", maxYear.Value);
            }
            matchFilters.Add("year", yearFilter);

            // Rating filter
            var ratingFilter = new BsonDocument { { "$gte", minRating } };
            if (maxRating.HasValue)
            {
                ratingFilter.Add("$lte", maxRating.Value);
            }
            matchFilters.Add("imdb.rating", ratingFilter);

            // Min runtime filter
            if (minRuntime.HasValue)
            {
                matchFilters.Add("runtime", new BsonDocument { { "$gte", minRuntime.Value } });
            }

            var matchStage = new BsonDocument { { "$match", matchFilters } };

            var projectStage = new BsonDocument
            {
                { "$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "title", 1 },
                        { "genres", 1 },
                        { "year", 1 },
                        { "imdb.rating", 1 },
                        { "imdb.votes", 1 },
                        { "plot", 1 },
                        { "fullplot", 1 },
                        { "poster", 1 },
                        { "cast", 1 },
                        { "directors", 1 },
                        { "runtime", 1 },
                        { "score", new BsonDocument { { "$meta", "searchScore" } } }
                    }
                }
            };

            var limitStage = new BsonDocument
            {
                { "$limit", limit > 0 ? limit : 5 }
            };

            var pipeline = new[] { searchStage, matchStage, projectStage, limitStage };

            return await _movieCollection
                .Aggregate<MovieSearchResult>(pipeline, cancellationToken: cancellationToken)
                .ToListAsync(cancellationToken);
        }
    }
}
