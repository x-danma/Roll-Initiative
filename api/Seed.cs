using Api.Models;

namespace Api.Seed
{
    public static class Seed
    {
        public static ICollection<Game> Data =>
            new[]
            {
            new Game(12345678)
            {
                Characters = new (){
                    new Character("andarel"),
                    new Character("bethar")
            }
        }
        };
    }
}