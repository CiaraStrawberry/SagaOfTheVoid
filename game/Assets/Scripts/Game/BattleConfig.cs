using Saga.Sim;

namespace Saga.Game
{
    // What battle to start: a skirmish (Mission < 0) or a campaign mission.
    public class BattleConfig
    {
        public int Mission = -1;
        public bool Quick;
        public string Map = "asteroid_field";
        public string Sky = "skybox5";
        public Difficulty Difficulty = Difficulty.Normal;
    }
}
