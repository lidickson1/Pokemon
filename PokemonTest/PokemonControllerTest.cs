namespace PokemonTest;

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
}