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
using System.Media;
using System.Threading;

namespace JeuDeBalle
{
    internal class Program
    {
        static void Main()
        {
            Console.ReadLine();
            
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Pour afficher le smiley du player
            Console.CursorVisible = false;
            Console.SetWindowSize(Game.WINDOW_WIDTH,Game.WINDOW_HEIGHT); // Taille de la fenêtre console

            // Initialisation du jeu
            Game game = new Game();
            
            // Démarrage du jeu
            game.StartGame();
            game.EndGame();
            // Simulation d'un tour (ajoute une boucle pour gérer les tours, si besoin)
            while (!game.IsGameOver)
            {              
                game.DisplayScores();
                game.PlayTurn(game.Player1, game.Player2, game.Building2); // Tour du joueur 1

                if (game.IsGameOver) 
                    break;

                game.DisplayScores();
                game.PlayTurn(game.Player2, game.Player1, game.Building1); // Tour du joueur 2

            }


            //Test animation avec un thread séparé à voir si j'ai le temps de continuer après avoir fini le jeu de base

            /*Thread animThread = new Thread(() => Animation(game));
            

            // Simulation des tours
            Thread gameThread = new Thread(() =>
            {                
                while (!game.IsGameOver)
                {
                    game.PlayTurn(game.Player1, game.Player2, game.Building2); // Tour du joueur 1
                    if (game.IsGameOver)
                        break;
                    game.PlayTurn(game.Player2, game.Player1, game.Building1); // Tour du joueur 2
                }
            });

            animThread.Start();
            gameThread.Start();   */
        }

        //Test animation avec un thread séparé à voir si j'ai le temps de continuer après avoir fini le jeu de base
        /*
        static void Animation(Game game)
        {
            // Animation des points de vie
            while (!game.IsGameOver)
            {
                // Afficher les points de vie avec animation colorée                
                DisplayHealth(game.Player1, 1); // Afficher les points de vie de Player 1
                Thread.Sleep(50);
            }
        }

        static void DisplayHealth(Player player, int playerNumber)
        {


            Random random = new Random();
            int i = 0;
            string healthBar = new string('♥', player.LifePoints / 10);

            Array colors = Enum.GetValues(typeof(ConsoleColor));
            ConsoleColor randomColor = (ConsoleColor)colors.GetValue(random.Next(colors.Length));

            while (randomColor == ConsoleColor.Black)
            {
                randomColor = (ConsoleColor)colors.GetValue(random.Next(colors.Length));
            }

            Console.ForegroundColor = randomColor;

            Console.SetCursorPosition(0, playerNumber * 2);
            Console.WriteLine($"Santé: {healthBar} ({player.LifePoints} HP)");

            // Réinitialiser la couleur
            Console.ResetColor();
        }*/

    }
}
