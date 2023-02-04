using System.Collections.Immutable;
using ComradeVanti.CSharpTools;
using TeamShrimp.GGJ23.Networking;

namespace TeamShrimp.GGJ23
{
    public static class Blackboard
    {
        private static IOpt<Game> game = Opt.None<Game>();
        private static IOpt<bool> isHost = Opt.None<bool>();

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

        public static bool IsHost
        {
            get => isHost.DefaultValue(true);
            set => isHost = Opt.Some(value);
        }

        public static IOpt<Connection> EstablishedConnection { get; set; } =
            Opt.None<Connection>();
    }
}