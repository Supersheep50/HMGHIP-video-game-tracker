using Microsoft.AspNetCore.Authorization.Infrastructure;
using PlayedGames.Components;


namespace PlayedGames.Modals
{
    public class Games
    {

        public string Name { get; set; }
        public string Platform { get; set; }

        public string Genre { get; set; }
        public string Developer { get; set; }

    }
}