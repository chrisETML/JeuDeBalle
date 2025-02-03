/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Programme principal du jeu de balle 
Le projet consiste à créer un jeu avec 2 personnages, 2 bâtiments, un score, des points de vies, et une balle qui est lancée par les 2 joueurs. 
Si la balle touche le bâtiment adverse, le bâtiment perds un carré et commence à perdre sa forme rectangulaire. 
Si la balle touche le joueur il perd un point de vie, et le joueur qui a tiré gagne un point de vie. Les joueurs peuvent tirer la balle en choisissant un angle et la puissance du tir.
*/

using System;

namespace JeuDeBalle
{
    internal class Program
    {
        static void Main()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8; // Pour afficher le smiley du player
            Console.CursorVisible = false;
            Console.SetWindowSize(150,40); // Taille de la fenêtre console

            ConsoleKey press = ConsoleKey.Spacebar;            

            // Initialisation du jeu
            Game game = new Game();

            // Démarrage du jeu
            game.StartGame();

            // Simulation d'un tour (ajoute une boucle pour gérer les tours, si besoin)
            while (!game.IsGameOver)
            {              
                game.PlayTurn(game.Player1, game.Player2, game.Building2); // Tour du joueur 1
                if (game.IsGameOver) 
                    break;
                game.PlayTurn(game.Player2, game.Player1, game.Building1); // Tour du joueur 2
            }

            // Fin du jeu
            Console.WriteLine("Le jeu est terminé !");
        }
    }
}
