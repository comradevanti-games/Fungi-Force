using System.Collections.Immutable;
using ComradeVanti.CSharpTools;

namespace TeamShrimp.GGJ23
{
    public static class Blackboard
    {
        private static IOpt<Game> game = Opt.None<Game>();

        private static readonly Game TestGame = new Game(
            ImmutableDictionary<Team, string>.Empty
                .Add(Team.Red, "Red")
                .Add(Team.Blue, "Blue"),
            10, 123);

        public static Game Game
        {
            get => game.DefaultValue(TestGame);
            set => game = Opt.Some(value);
        }

        public static bool IsHost { get; set; }
    }
}