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
using System.Media;
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
        private readonly GameManager _gameManager;

        /// <summary>
        /// Position du sol
        /// </summary>
        public static Vector2 GroundPosition { get; private set; } = new Vector2(0,36);

        /// <summary>
        /// Indique si c'est le premier tour, permet de gérer le placement pour l'affichage de l'angle du joueur
        /// </summary>
        private static bool firstTurn = true;

        /// <summary>
        /// Largeur de la fenetre du jeu
        /// </summary>
        public static readonly Byte WINDOW_WIDTH = 150;

        /// <summary>
        /// Hauteur de la fenetre du jeu
        /// </summary>
        public static readonly Byte WINDOW_HEIGHT = 40;

        /// <summary>
        /// Constructeur de la classe Game
        /// </summary>
        public Game()
        {
            Player1 = new Player(x:30, y:33, ConsoleColor.DarkRed, angleMin:30, angleMax:60);
            Player2 = new Player(x:80, y:33, ConsoleColor.DarkCyan, angleMin:120, angleMax:150);

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
            string soundFile = Environment.CurrentDirectory + @"\sounds\intro.wav";

            using (SoundPlayer sound = new SoundPlayer(soundFile))
            {
                sound.Load();
                sound.Play();
            }

            IsGameOver = false;

            // Affichage du sol
            Console.ForegroundColor = ConsoleColor.Green;            
            Console.SetCursorPosition((int)GroundPosition.X, (int)GroundPosition.Y + 3);
            Console.WriteLine("---------------------------------------------------" +
                "-------------------------------------------------------------------------------------------------");

            Console.SetCursorPosition((int)GroundPosition.X, (int)GroundPosition.Y - 3);
            Console.WriteLine("---------------------------------------------------" +
                "-------------------------------------------------------------------------------------------------");
            Console.ResetColor();
            
            DisplayPlayers();
            DisplayScores();

            //Affiche tous les objet updatable
            foreach (IUpdatable updatable in Updatables)           
                updatable.Update();
                
        }

        /// <summary>
        /// Fin du jeu
        /// </summary>
        public void EndGame()
        {
            string soundFile = Environment.CurrentDirectory + @"\sounds\end.wav";

            using (SoundPlayer sound = new SoundPlayer(soundFile))
            {
                sound.Load();
                sound.PlayLooping();
            }

            IsGameOver = true;

            DisplayScores();

            _gameManager.EndGameAnimation();
        }

        /// <summary>
        /// Affichage du score
        /// </summary>
        public void DisplayScores()
        {            
            Console.SetCursorPosition((int)Player1.ConsolePosition.X - 2, (int)Player1.ConsolePosition.Y + 4);
            Console.ForegroundColor = Player1.ConsoleColor;
            Console.Write("Score : ");
            Console.ResetColor();
            Console.Write(Player1.PlayerScore);

            Console.SetCursorPosition((int)Player1.ConsolePosition.X - 2, (int)Player1.ConsolePosition.Y + 3);
            Console.ForegroundColor = Player1.ConsoleColor;
            Console.Write("PV : ");
            Console.ResetColor();
            Console.Write(Player1.LifePoints);

            Console.SetCursorPosition((int)Player2.ConsolePosition.X - 2, (int)Player2.ConsolePosition.Y + 4);
            Console.ForegroundColor = Player2.ConsoleColor;
            Console.Write("Score : ");
            Console.ResetColor();
            Console.Write(Player2.PlayerScore);

            Console.SetCursorPosition((int)Player2.ConsolePosition.X - 2, (int)Player2.ConsolePosition.Y + 3);
            Console.ForegroundColor = Player2.ConsoleColor;
            Console.Write("PV : ");
            Console.ResetColor();
            Console.Write(Player2.LifePoints);
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

            bool angleChosen = false; // Indique si l'angle a été choisi
            bool forceChosen = false; // Indique si la force a été choisie            

            int angle = 0;
            int force = 0;

            // Choisir l'angle et la force
            while (!forceChosen)
            {                
                if (!angleChosen)
                {
                    angle = _gameManager.AngleSystem(MIN_ANGLE: currentPlayer.ANGLE_MIN, currentPlayer.ANGLE_MAX, firstTurn);
                    angleChosen = true;
                }                   
                else if(angleChosen && !forceChosen)
                {
                    force = _gameManager.RunHorizontalAnimation();
                    forceChosen = true;
                }
                              
            }
            firstTurn = !firstTurn;

            // Lancer la balle depuis la position du joueur
            Ball.Launch(angle, force, currentPlayer.ConsolePosition);

            // Simuler le mouvement de la balle
            Ball.SimulateBall(currentPlayer, opponent, opponentBuilding, _gameManager);

            if(Player1.LifePoints == 0 || Player2.LifePoints == 0 || !Building1.IsBuildingIntact() || !Building2.IsBuildingIntact())
                EndGame();
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
