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
        /// <summary>
        /// Instance du jeu
        /// </summary>
        private readonly Game _game;

        /// <summary>
        /// Constructeur pour le game manager
        /// </summary>
        /// <param name="game">instance du jeu</param>
        public GameManager(Game game)
        {            
            if (game == null)
                game = new Game();

            _game = game;
        }

        /// <summary>
        /// Gère la collision entre la balle et un bâtiment.
        /// Vérifie si la balle touche une case intacte du bâtiment et inflige des dégâts.
        /// </summary>
        /// <param name="building">Le bâtiment avec lequel la balle entre en collision.</param>
        /// <param name="ballPosition">La position actuelle de la balle.</param>
        /// <param name="currentPlayer">Le joueur qui joue actuellement.</param>
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

        /// <summary>
        /// Gère la collision entre la balle et un joueur.
        /// Vérifie si la balle touche un caractère du joueur et inflige des dégâts.
        /// </summary>
        /// <param name="player">Le joueur avec lequel la balle entre en collision.</param>
        /// <param name="currentPlayer">Le joueur qui joue actuellement.</param>
        public void HandleBallCollisionWithPlayer(Player player, Player currentPlayer)
        {
            // Vérifier si le joueur fait partie des objets qui peuvent subir des dégâts
            if (Game.Damageables.Contains(player))
            {
                player.TakeDamage(1);
                currentPlayer.Heal(1);
            }
        }
        /// <summary>
        /// Remet à zéro les valeur pour la balle 
        /// </summary>
        /// <param name="currentPlayer">Récupérer la position du joueur</param>
        public void ResetBall(Player currentPlayer)
        {
            _game.Ball.Position = currentPlayer.ConsolePosition;  // Position initiale de la balle
            _game.Ball.Velocity = Vector2.Zero;  // La vitesse de la balle est réinitialisée
            _game.Ball.IsDestroyed = false;  // La balle est considérée comme non détruite
            _game.Ball.LastPosition = null;  // Effacer la dernière position pour qu'elle soit réaffichée à chaque tour
        }
    }
}
