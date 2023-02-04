using System.Collections.Immutable;

namespace TeamShrimp.GGJ23
{
    public record Game(
        IImmutableDictionary<Team, string> PlayerNamesByTeam,
        int MapSize,
        int MapSeed);
}