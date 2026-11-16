using Saga.Sim;

namespace Saga.Game
{
    // Free for all: just fleets. Capital ship: each side also has a slow, heavily armed flagship; lose it, lose
    // the battle. Base defence: each side has an immobile home base with defence platforms; lose it, lose.
    public enum SkirmishMode { FreeForAll, CapitalShip, BaseDefence }

    // What battle to start: a skirmish (Mission < 0) or a campaign mission.
    public class BattleConfig
    {
        public int Mission = -1;
        public bool Quick;   // 8 v 8
        public bool Small;   // 4 v 4
        public string Map = "asteroid_field";
        public string Sky = "skybox5";
        public Difficulty Difficulty = Difficulty.Normal;
        public SkirmishMode Mode = SkirmishMode.FreeForAll;
    }
}
