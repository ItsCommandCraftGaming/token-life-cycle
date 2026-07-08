using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TokenLifecycle.Domain.DTOs
{
    public class ImdbInfo
    {
        [BsonElement("rating")]
        public double? Rating { get; set; }
    }

    public class MovieSearchResult
    {
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("genres")]
        public List<string> Genres { get; set; } = new();

        [BsonElement("year")]
        public int? Year { get; set; }

        [BsonElement("imdb")]
        public ImdbInfo? Imdb { get; set; }

        [BsonElement("score")]
        public double Score { get; set; }
    }
}
