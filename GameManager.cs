/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Fournit des outils pour gérer les actions spécifiques du jeu, comme la destruction des blocs.
*/

using System.Numerics;
using System;

namespace JeuDeBalle
{
    internal class GameManager
    {
        private readonly Game _game;

        public GameManager(Game game)
        {            
            if (game == null)
                game = new Game();

            _game = game;
        }

        public void HandleBallCollisionWithBuilding(Building building, Vector2 ballPosition, Player currentPlayer)
        {
            // Trouver les coordonnées de la case touchée
            int x = (int)(ballPosition.X - building.ConsolePosition.X);
            int y = (int)(ballPosition.Y - building.ConsolePosition.Y); 

            // Vérifier si la case est intacte et la détruire
            if (building.IsBlockIntact(x, y))
            {
                //Console.WriteLine("\nLa balle a touché une case du bâtiment !");
                building.TakeDamage(new Tuple<int, int>(x, y));
                building.Update();
            }
        }
        public void ResetBall(Player currentPlayer)
        {
            _game.Ball.Position = currentPlayer.ConsolePosition;  // Position initiale de la balle
            _game.Ball.Velocity = Vector2.Zero;  // La vitesse de la balle est réinitialisée
            _game.Ball.IsDestroyed = false;  // La balle est considérée comme non détruite
            _game.Ball.LastPosition = null;  // Effacer la dernière position pour qu'elle soit réaffichée à chaque tour
        }
    }
}
