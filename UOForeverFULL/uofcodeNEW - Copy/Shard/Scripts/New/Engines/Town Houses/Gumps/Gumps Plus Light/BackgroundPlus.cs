#region References

using Server.Gumps;

#endregion

namespace Knives.TownHouses
{
    public class BackgroundPlus : GumpBackground
    {
        private bool Override { get; set; }

        public BackgroundPlus(int x, int y, int width, int height, int back) : base(x, y, width, height, back)
        {
            Override = true;
        }

        public BackgroundPlus(int x, int y, int width, int height, int back, bool over)
            : base(x, y, width, height, back)
        {
            Override = over;
        }
    }
}