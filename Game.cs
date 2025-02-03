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
        /// <summary>
        /// Joueur 1
        /// </summary>
        public Player Player1 { get; set; }

        /// <summary>
        /// Joueur 2
        /// </summary>
        public Player Player2 { get; set; }

        /// <summary>
        /// Bâtiment du joueur 1
        /// </summary>
        public Building Building1 { get; set; }

        /// <summary>
        /// Bâtiment du joueur 2
        /// </summary>
        public Building Building2 { get; set; }

        /// <summary>
        /// Balle qui traverse le jeu
        /// </summary>
        public Ball Ball { get; set; }

        /// <summary>
        /// Est ce que le jeu est terminé
        /// </summary>
        public bool IsGameOver { get; private set; }

        /// <summary>
        /// Liste des objets pouvant être mis à jour
        /// </summary>
        public static List<IUpdatable> Updatables { get; private set; } = new List<IUpdatable>();

        /// <summary>
        /// Liste des objets dommageable
        /// </summary>
        public static List<IDamageable> Damageables { get; private set; } = new List<IDamageable>();

        /// <summary>
        /// Liste des objets collisionnable
        /// </summary>
        public static List<ICollidable> Collidables { get; private set; } = new List<ICollidable>();

        /// <summary>
        /// Le gestionnaire de jeu qui gère la logique
        /// </summary>
        private GameManager _gameManager;

        /// <summary>
        /// Position du sol
        /// </summary>
        public static Vector2 GroundPosition { get; private set; } = new Vector2(0,36);

        /// <summary>
        /// Constructeur de la classe Game
        /// </summary>
        public Game()
        {
            Player1 = new Player(10,33, ConsoleColor.Red);
            Player2 = new Player(40,33, ConsoleColor.Cyan);

            Building1 = new Building(width: 5, height: 5, owner: Player1, true);
            Building2 = new Building(width: 5, height: 5, owner: Player2, false);

            Ball = new Ball();

            _gameManager = new GameManager(this);

            IsGameOver = false;
        }

        /// <summary>
        /// Début du jeu, affichage des objets
        /// </summary>
        public void StartGame()
        {
            IsGameOver = false;
            Console.WriteLine($"Le jeu commence ! PV:{Player1.LifePoints}");

            // Affichage du sol
            Console.ForegroundColor = ConsoleColor.Green;            
            Console.SetCursorPosition((int)GroundPosition.X, (int)GroundPosition.Y + 3);
            Console.WriteLine("---------------------------------------------------" +
                "-------------------------------------------------------------------------------------------------");

            Console.SetCursorPosition((int)GroundPosition.X, (int)GroundPosition.Y - 3);
            Console.WriteLine("---------------------------------------------------" +
                "-------------------------------------------------------------------------------------------------");
            Console.ResetColor();

            // Affiche les joueurs     
            DisplayPlayers();
            
            //Affiche tous les objet updatable
            foreach (IUpdatable updatable in Updatables)
                updatable.Update();                      
        }

        /// <summary>
        /// Fin du jeu
        /// </summary>
        public void EndGame()
        {
            IsGameOver = true;
            Console.WriteLine("Le jeu est terminé !");
        }

        /// <summary>
        /// Affichage du score
        /// </summary>
        public void DisplayScores()
        {
            Console.WriteLine($"Score Joueur 1: {Player1.PlayerScore}");
            Console.WriteLine($"Score Joueur 2: {Player2.PlayerScore}");
        }

        /// <summary>
        /// Gestion des tours 
        /// </summary>
        /// <param name="currentPlayer">Le joueur qui joue actuellement.</param>
        /// <param name="opponent">L'adversaire du joueur actuel.</param>
        /// <param name="opponentBuilding">Le bâtiment du joueur adverse</param>
        public void PlayTurn(Player currentPlayer, Player opponent, Building opponentBuilding)
        {
            // Réinitialiser la balle avant de commencer un tour
            _gameManager.ResetBall(currentPlayer);

            #region debug
            if (!float.TryParse(Console.ReadLine(), out float angle))
            {
                Console.WriteLine("Angle invalide. Veuillez réessayer.");
                return;
            }
            
            if (!float.TryParse(Console.ReadLine(), out float force) || force < 1 || force > 10)
            {
                Console.WriteLine("Force invalide. Veuillez réessayer.");
                return;
            }
            #endregion

            // Lancer la balle depuis la position du joueur
            Ball.Launch(angle, force, currentPlayer.ConsolePosition);

            // Simuler le mouvement de la balle
            Ball.SimulateBall(currentPlayer, opponent, opponentBuilding, _gameManager);
        }

        /// <summary>
        /// Afficher les joueurs sur la console
        /// </summary>
        private void DisplayPlayers()
        {
            // Affiche le Joueur 1
            Console.SetCursorPosition((int)Player1.ConsolePosition.X, (int)Player1.ConsolePosition.Y);
            foreach (string item in Player1.CHARACTER)
            {
                Console.SetCursorPosition((int)Player1.ConsolePosition.X, Console.CursorTop);
                Console.ForegroundColor = Player1.ConsoleColor;
                Console.WriteLine(item);
            }

            // Affiche le Joueur 2
            Console.SetCursorPosition((int)Player2.ConsolePosition.X, (int)Player2.ConsolePosition.Y);
            foreach (string item in Player2.CHARACTER)
            {
                Console.SetCursorPosition((int)Player2.ConsolePosition.X, Console.CursorTop);
                Console.ForegroundColor = Player2.ConsoleColor;
                Console.WriteLine(item);
                Console.ResetColor();
            }
        } 
    }
}
