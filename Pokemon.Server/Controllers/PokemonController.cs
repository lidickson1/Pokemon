using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AngularApp1.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly HttpClient client = new() { BaseAddress = new Uri($"https://pokeapi.co") }; //reuse client
        public PokemonController()
        {
            Console.WriteLine("Hello Pokemon!");
        }

        private async Task<(List<Pokemon>, Dictionary<int, int>)> FetchPokemons()
        {
            var ids = Enumerable.Range(1, 151) //generate random ids from 1 - 151
                .OrderBy(x => Random.Shared.Next()) //shuffle them
                .Take(8).ToList();
            List<Pokemon> pokemons = new List<Pokemon>();
            Dictionary<int, int> baseExperiences = new Dictionary<int, int>();
            foreach (int id in ids)
            {
                try
                {
                    using HttpResponseMessage response = await client.GetAsync($"api/v2/pokemon/{id}");
                    response.EnsureSuccessStatusCode();
                    string responseString = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(responseString);
                    Pokemon pokemon = new Pokemon
                    {
                        Id = (int)json["id"]!,
                        Name = (string)json["name"]!,
                        Type = (string)json.SelectToken("types[0].type.name")!,
                    };
                    baseExperiences[pokemon.Id] = (int)json["base_experience"]!;
                    // Console.WriteLine(pokemon);
                    pokemons.Add(pokemon);
                }
                catch (HttpRequestException e)
                {
                    Console.Error.WriteLine($"Error occurred with pokemon id {id}:");
                    Console.Error.WriteLine(e);
                }
            }
            return (pokemons, baseExperiences);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? sortBy, string? sortDirection = "desc")
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return BadRequest("sortBy parameter is required");
            }
            HashSet<string> sortOptions = new HashSet<string> { "wins", "losses", "ties", "name", "id" };
            if (!sortOptions.Contains(sortBy))
            {
                return BadRequest("sortBy parameter is invalid");
            }

            if (sortDirection != "asc" && sortDirection != "desc")
            {
                return BadRequest("sortDirection parameter is invalid");
            }

            (List<Pokemon> pokemons, Dictionary<int, int> baseExperiences) = await FetchPokemons();

            //round robin
            for (int i = 0; i < pokemons.Count; i++)
            {
                for (int j = i + 1; j < pokemons.Count; j++)
                {
                    Pokemon.Fight(pokemons[i], pokemons[j], baseExperiences);
                    // Console.WriteLine(pokemons[i]);
                    // Console.WriteLine(pokemons[j]);
                }
            }

            //sort dynamically based on the sortBy (e.g. "wins" => the "Wins" property)
            var property = typeof(Pokemon).GetProperty(char.ToUpper(sortBy[0]) + sortBy.Substring(1))!;
            pokemons.Sort((x, y) =>
            {
                int compare = ((IComparable)property.GetValue(x)!).CompareTo((IComparable)property.GetValue(y)!);
                if (sortDirection == "desc")
                {
                    compare *= -1;
                }
                return compare;
            });

            return Ok(pokemons);
        }
    }

    public class Pokemon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public override string ToString()
        {
            return $"Id = {Id}, Name = {Name}, Type = {Type}, Wins = {Wins}, Losses = {Losses}, Ties = {Ties}";
        }
        private static List<(string, string)> rules = new List<(string, string)> {
                ("water", "fire"),
                ("fire", "grass"),
                ("grass", "electric"),
                ("electric", "water"),
                ("ghost", "psychic"),
                ("psychic", "fighting"),
                ("fighting", "dark"),
                ("dark", "ghost"),
            };

        public static void Fight(Pokemon p1, Pokemon p2, Dictionary<int, int> baseExperiences)
        {
            foreach ((string, string) rule in rules)
            {
                if (p1.Type == rule.Item1 && p2.Type == rule.Item2)
                {
                    p1.Wins++;
                    p2.Losses++;
                    return;
                }
                else if (p1.Type == rule.Item2 && p2.Type == rule.Item1)
                {
                    p2.Wins++;
                    p1.Losses++;
                    return;
                }
            }
            if (baseExperiences[p1.Id] > baseExperiences[p2.Id])
            {
                p1.Wins++;
                p2.Losses++;
            }
            else if (baseExperiences[p1.Id] < baseExperiences[p2.Id])
            {
                p2.Wins++;
                p1.Losses++;
            }
            else
            {
                p1.Ties++;
                p2.Ties++;
            }
        }
    }
}
