/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Classe qui coordonne le jeu dans son ensemble.
Contient les joueurs, les bâtiments, et d'autres objets importants comme la balle.
Gère les débuts, les fins et le déroulement général du jeu.
*/

using System;
using System.Collections.Generic;
using System.Numerics;

namespace JeuDeBalle
{
    internal class Game
    {
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public Building Building1 { get; set; }
        public Building Building2 { get; set; }
        public Ball Ball { get; set; }
        public bool IsGameOver { get; private set; }
        public static List<IUpdatable> Updatables { get; private set; } = new List<IUpdatable>();
        public static List<IDamageable> Damageables { get; private set; } = new List<IDamageable>();
        public static List<ICollidable> Collidables { get; private set; } = new List<ICollidable>();

        private GameManager _gameManager;

        private Random randPower = new Random();
        private Random randAngle = new Random();

        public Game()
        {
            Player1 = new Player(10,10, ConsoleColor.Red);
            Player2 = new Player(100,10, ConsoleColor.Cyan);

            Building1 = new Building(width: 5, height: 5, owner: Player1, true);
            Building2 = new Building(width: 5, height: 5, owner: Player2, false);

            Ball = new Ball();

            _gameManager = new GameManager(this);

            IsGameOver = false;
        }

        public void StartGame()
        {
            IsGameOver = false;
            Console.WriteLine("Le jeu commence !");
            // Affiche les joueurs     
            DisplayPlayers();
            foreach (IUpdatable updatable in Updatables)
                updatable.Update();

        }

        public void EndGame()
        {
            IsGameOver = true;
            Console.WriteLine("Le jeu est terminé !");
        }

        public void DisplayScores()
        {
            Console.WriteLine($"Score Joueur 1: {Player1.PlayerScore}");
            Console.WriteLine($"Score Joueur 2: {Player2.PlayerScore}");
        }

        public void PlayTurn(Player currentPlayer, Player opponent, Building opponentBuilding)
        {
            // Réinitialiser la balle avant de commencer un tour
            _gameManager.ResetBall(currentPlayer);

            //Console.WriteLine($"{(currentPlayer == Player1 ? "Joueur 1" : "Joueur 2")}, choisissez un angle (en degrés) :");
            if (!float.TryParse(Console.ReadLine(), out float angle))
            {
                Console.WriteLine("Angle invalide. Veuillez réessayer.");
                return;
            }

            //Console.WriteLine("Choisissez une force (entre 1 et 10) :");
            if (!float.TryParse(Console.ReadLine(), out float force) || force < 1 || force > 10)
            {
                Console.WriteLine("Force invalide. Veuillez réessayer.");
                return;
            }

            // Lancer la balle depuis la position du joueur
            Ball.Launch(angle, force, currentPlayer.ConsolePosition);

            // Simuler le mouvement de la balle
            SimulateBall(currentPlayer, opponent, opponentBuilding);
        }
        private void DisplayPlayers()
        {
            // Affiche le Joueur 1
            Console.SetCursorPosition((int)Player1.ConsolePosition.X, (int)Player1.ConsolePosition.Y);
            foreach (string item in Player1.CARACTER)
            {
                Console.SetCursorPosition((int)Player1.ConsolePosition.X, Console.CursorTop);
                Console.WriteLine(item);
            }

            // Affiche le Joueur 2
            Console.SetCursorPosition((int)Player2.ConsolePosition.X, (int)Player2.ConsolePosition.Y);
            foreach (string item in Player2.CARACTER)
            {
                Console.SetCursorPosition((int)Player2.ConsolePosition.X, Console.CursorTop);
                Console.WriteLine(item);
            }
        }
        
        private void SimulateBall(Player currentPlayer, Player opponent, Building opponentBuilding)
        {
            while (!Ball.IsDestroyed)
            {
                //Console.Clear();                

                // Met à jour et affiche la balle
                Ball.Update();
                Ball.Display();

                // Vérifie les collisions avec tous les objets dans Collidables
                foreach (ICollidable collidable in Collidables)
                {
                    if (collidable is Building building)
                    {                        
                        // Vérifier si la balle touche le bâtiment
                        if (building.CheckCollision(Ball.Position))
                        {
                            // Verifier que le building et collisionnable et dommageable
                            if (Collidables.Contains(building) && Damageables.Contains(building))
                            {
                                if (building.Owner == Player1)
                                {
                                    Vector2 gridPosition = Ball.Position;
                                    _gameManager.HandleBallCollisionWithBuilding(building, Ball.Position, currentPlayer); // Gérer la collision et la destruction de la case
                                    Ball.Destroy();
                                    building.Update();
                                }
                                else if (building.Owner == Player2)
                                {
                                    Vector2 gridPosition = Ball.Position;                                   

                                    _gameManager.HandleBallCollisionWithBuilding(building, Ball.Position, currentPlayer); // Gérer la collision et la destruction de la case
                                    Ball.Destroy();
                                    building.Update();
                                }
                                break;  // Arrêter la boucle dès qu'il y a collision
                            }
                        }
                    }
                    else if (collidable is Player player)
                    {
                        // Vérifier si la balle touche un joueur
                        if (player.CheckCollision(Ball.Position))
                        {
                            if (Collidables.Contains(player) && Damageables.Contains(player))
                            {                                
                                player.TakeDamage(1);
                                currentPlayer.Heal(1);
                                break;  // Arrêter la boucle dès qu'il y a collision
                            }
                        }
                    }
                }

                // Vérifie si la balle sort du terrain
                if (Ball.Position.X < 0 || Ball.Position.X >= Console.WindowWidth || Ball.Position.Y >= Console.WindowHeight)
                {
                    break;
                }

                // Pause pour ralentir l'animation
                System.Threading.Thread.Sleep(50);
            }

        }
    }
}
