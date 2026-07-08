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
            _movieCollection = db.GetCollection<BsonDocument>("movies");
        }

        public async Task<List<MovieSearchResult>> SearchMoviesAsync(
            string searchTerm,
            int minYear,
            double minRating,
            CancellationToken cancellationToken)
        {
            var searchStage = new BsonDocument
            {
                { "$search", new BsonDocument
                    {
                        { "index", "atlas_search" },
                        { "compound", new BsonDocument
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
                                },
                                { "should", new BsonArray
                                    {
                                        new BsonDocument
                                        {
                                            { "text", new BsonDocument
                                                {
                                                    { "query", "Sci-Fi" },
                                                    { "path", "genres" }
                                                }
                                            }
                                        }
                                    }
                                },
                                { "mustNot", new BsonArray
                                    {
                                        new BsonDocument
                                        {
                                            { "text", new BsonDocument
                                                {
                                                    { "query", "Horror" },
                                                    { "path", "genres" }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var matchStage = new BsonDocument
            {
                { "$match", new BsonDocument
                    {
                        { "year", new BsonDocument { { "$gte", minYear } } },
                        { "imdb.rating", new BsonDocument { { "$gte", minRating } } }
                    }
                }
            };

            var projectStage = new BsonDocument
            {
                { "$project", new BsonDocument
                    {
                        { "_id", 0 },
                        { "title", 1 },
                        { "genres", 1 },
                        { "year", 1 },
                        { "imdb.rating", 1 },
                        { "score", new BsonDocument { { "$meta", "searchScore" } } }
                    }
                }
            };

            var limitStage = new BsonDocument
            {
                { "$limit", 5 }
            };

            var pipeline = new[] { searchStage, matchStage, projectStage, limitStage };

            return await _movieCollection
                .Aggregate<MovieSearchResult>(pipeline, cancellationToken: cancellationToken)
                .ToListAsync(cancellationToken);
        }
    }
}
