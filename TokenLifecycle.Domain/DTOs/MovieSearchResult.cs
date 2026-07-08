using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace TokenLifecycle.Domain.DTOs
{
    public class ImdbInfo
    {
        [BsonElement("rating")]
        public double? Rating { get; set; }

        [BsonElement("votes")]
        public int? Votes { get; set; }
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

        [BsonElement("plot")]
        public string? Plot { get; set; }

        [BsonElement("fullplot")]
        public string? FullPlot { get; set; }

        [BsonElement("poster")]
        public string? Poster { get; set; }

        [BsonElement("cast")]
        public List<string>? Cast { get; set; }

        [BsonElement("directors")]
        public List<string>? Directors { get; set; }

        [BsonElement("runtime")]
        public int? Runtime { get; set; }

        [BsonElement("score")]
        public double Score { get; set; }
    }
}
