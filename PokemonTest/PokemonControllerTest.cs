namespace PokemonTest;

using Microsoft.AspNetCore.Mvc;
using Pokemon.Server.Controllers;

[TestClass]
public class PokemonControllerTest
{
    [TestMethod]
    public void TestPokemonFightRule()
    {
        Pokemon p1 = new Pokemon
        {
            Id = 1,
            Type = "fire",
        };
        Pokemon p2 = new Pokemon
        {
            Id = 2,
            Type = "water",
        };
        Pokemon.Fight(p1, p2, new Dictionary<int, int> { { 1, 200 }, { 2, 100 } });
        Assert.AreEqual(p1.Wins, 0);
        Assert.AreEqual(p1.Losses, 1);
        Assert.AreEqual(p2.Wins, 1);
        Assert.AreEqual(p2.Losses, 0);
    }

    [TestMethod]
    public void TestPokemonFightBaseExperience()
    {
        Pokemon p1 = new Pokemon
        {
            Id = 1,
            Type = "poison",
        };
        Pokemon p2 = new Pokemon
        {
            Id = 2,
            Type = "dark",
        };
        Pokemon.Fight(p1, p2, new Dictionary<int, int> { { 1, 200 }, { 2, 100 } });
        Assert.AreEqual(p1.Wins, 1);
        Assert.AreEqual(p1.Losses, 0);
        Assert.AreEqual(p2.Wins, 0);
        Assert.AreEqual(p2.Losses, 1);
    }

    [TestMethod]
    public void TestPokemonFightTie()
    {
        Pokemon p1 = new Pokemon
        {
            Id = 1,
            Type = "water",
        };
        Pokemon p2 = new Pokemon
        {
            Id = 2,
            Type = "bug",
        };
        Pokemon.Fight(p1, p2, new Dictionary<int, int> { { 1, 200 }, { 2, 200 } });
        Assert.AreEqual(p1.Wins, 0);
        Assert.AreEqual(p1.Losses, 0);
        Assert.AreEqual(p1.Ties, 1);
        Assert.AreEqual(p2.Wins, 0);
        Assert.AreEqual(p2.Losses, 0);
        Assert.AreEqual(p2.Ties, 1);
    }

    [TestMethod]
    public async Task TestGetPokemon()
    {
        PokemonController controller = new PokemonController();
        (Pokemon pokemon, int baseExperience) = await controller.GetPokemon(1);
        Assert.AreEqual(pokemon.Id, 1);
        Assert.AreEqual(pokemon.Name, "bulbasaur");
        Assert.AreEqual(pokemon.Type, "grass");
        Assert.AreEqual(baseExperience, 64);

        //invalid id should throw an error
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() => controller.GetPokemon(-1));
    }

    [TestMethod]
    public async Task TestGetSuccess()
    {
        PokemonController controller = new PokemonController();
        var result = await controller.Get("wins", "desc");
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(okResult.StatusCode, 200);
        Assert.IsInstanceOfType(okResult.Value, typeof(List<Pokemon>));
    }

    [TestMethod]
    public async Task TestGetFail()
    {
        PokemonController controller = new PokemonController();
        var result = await controller.Get(null);
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        Assert.AreEqual(((BadRequestObjectResult)result).Value, "sortBy parameter is required");

        result = await controller.Get("a");
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        Assert.AreEqual(((BadRequestObjectResult)result).Value, "sortBy parameter is invalid");

        result = await controller.Get("losses", "ascending");
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        Assert.AreEqual(((BadRequestObjectResult)result).Value, "sortDirection parameter is invalid");
    }
}