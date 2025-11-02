using Microsoft.AspNetCore.Authorization.Infrastructure;
using PlayedGames.Components;


namespace PlayedGames.Modals
{
    public class Element
    {

        public string Name { get; set; }
        public string Molar { get; set; }

        public int Number { get; set; }
        public string Sign { get; set; }
        public int Position { get; set; }

        public double MolarMass { get; set; }

        public int Nr { get; set; }
    }
}