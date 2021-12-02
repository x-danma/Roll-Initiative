namespace Api.Models
{
    public class Game
    {
        public int GameId { get; set; }

        public Game(int gameId)
        {
            GameId = gameId;
        }

        public List<Character> Characters { get; set; } = new();
    }

    public class Character
    {
        public string Name { get; set; }

        public Character(string name)
        {
            Name = name;
        }

        public int RollValue { get; set; }
    }
}