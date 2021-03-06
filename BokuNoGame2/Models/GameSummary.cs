using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BokuNoGame2.Models
{
    public class GameSummary
    {
        public int Id { get; set; }
        public string GameName { get; set; }
        public int? Rate { get; set; }
        public Genre Genre { get; set; }
        public string GenreWrapper { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public string UserId { get; set; }
        public Catalog Catalog { get; set; }
        public int CatalogId { get; set; }
    }
}