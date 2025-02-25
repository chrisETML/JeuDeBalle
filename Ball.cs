/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Classe pour la balle du jeu
*/

using System;
using System.Media;
using System.Numerics;

namespace JeuDeBalle
{
    internal class Ball : IUpdatable
    {
        /// <summary>
        /// Position de la balle
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Vitesse de la balle 
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Gravité constante
        /// </summary>
        private const float GRAVITY = 0.5f;

        /// <summary>
        /// Pas de temps pour la simulation
        /// </summary>
        private const float TIME_STEP = 0.6f;

        /// <summary>
        /// Représente la balle
        /// </summary>
        private const char BALL_FORM = '●';

        /// <summary>
        /// Stocker la dernière position pour l'effacement
        /// </summary>
        public Vector2? LastPosition { get; set; }

        /// <summary>
        /// Savoir si la balle est détruite lorsqu'elle a fini son parcours
        /// </summary>
        public bool IsDestroyed { get; set; }

        /// <summary>
        /// Constructeur de la balle
        /// </summary>
        public Ball()
        {
            Position = Vector2.Zero;
            Velocity = Vector2.Zero;
            LastPosition = null;

            Game.Updatables.Add(this);
        }

        /// <summary>
        /// Mise à jour de la position de la balle
        /// </summary>
        public void Update()
        {
            LastPosition = Position;

            // Met à jour la position en fonction de la vitesse
            Position += Velocity * TIME_STEP;

            // Applique la gravité à la vitesse verticale
            Velocity = new Vector2(Velocity.X, Velocity.Y + GRAVITY * TIME_STEP);
        }

        /// <summary>
        /// Affiche la balle à sa nouvelle position
        /// </summary>
        public void Display(Player player)
        {
            // Efface la balle à la dernière position
            if (LastPosition.HasValue)
            {
                Console.SetCursorPosition((int)LastPosition.Value.X, (int)LastPosition.Value.Y);
                Console.Write(' ');
            }

            // Affiche la balle à la nouvelle position
            Console.SetCursorPosition((int)Position.X, (int)Position.Y);
            Console.ForegroundColor = player.ConsoleColor;
            Console.Write(BALL_FORM);
            Console.ResetColor();
        }

        /// <summary>
        /// Lance la balle avec un angle, une force et une position de départ, audio aléatoire pour le tir
        /// </summary>
        /// <param name="angle">Angle de tir en degrés</param>
        /// <param name="force">Force de tir</param>
        /// <param name="startPosition">Position de départ de la balle</param>
        public void Launch(float angle, float force, Vector2 startPosition)
        {
            string soundFile = Environment.CurrentDirectory;

            Random random = new Random();
            switch (random.Next(0, 2))
            {
                case 0:
                    soundFile += @"\sounds\shot.wav";
                    break;
                case 1:
                    soundFile += @"\sounds\shot1.wav";
                    break;
                default:
                    break;
            }

            using (SoundPlayer sound = new SoundPlayer(soundFile))
            {
                sound.Load();
                sound.Play();
            }

            // Définir la position initiale
            Position = startPosition;
            LastPosition = null; // Réinitialiser la dernière position

            // Convertir l'angle en radians
            float radians = (float)(Math.PI * angle / 180f);

            // Calculer la vitesse initiale
            Velocity = new Vector2(
                (float)(force * Math.Cos(radians)), // Vx
                (float)(-force * Math.Sin(radians)) // Vy (négatif car Y augmente vers le bas)
            );
        }

        /// <summary>
        /// Détruire la balle
        /// </summary>
        public void Destroy()
        {
            IsDestroyed = true;
            Console.SetCursorPosition((int)Position.X, (int)Position.Y);
            Console.Write(' ');
        }

        /// <summary>
        /// Simule le mouvement de la balle en vérifiant les collisions avec les bâtiments et les joueurs.
        /// Cette méthode continue jusqu'à ce que la balle soit détruite ou que certaines conditions de sortie soient remplies.
        /// </summary>
        /// <param name="currentPlayer">Le joueur qui a son tour</param>
        /// <param name="opponent">L'adversaire du joueur actuel.</param>
        /// <param name="opponentBuilding">Le bâtiment de l'adversaire</param>
        /// <param name="gameManager">Le gestionnaire de jeu qui gère la logique</param>
        public void SimulateBall(Player currentPlayer, Player opponent, Building opponentBuilding, GameManager gameManager)
        {
            while (!IsDestroyed)
            {
                // Met à jour et affiche la balle                
                Update();
                Display(currentPlayer);

                // Vérifie les collisions avec tous les objets dans Collidables
                foreach (ICollidable collidable in Game.Collidables)
                {                    
                    if (collidable is Building building)
                    {
                        // Vérifier si la balle touche le bâtiment
                        if (building.CheckCollision(Position))
                        {
                            // Verifier que le building et collisionnable et dommageable
                            if (Game.Damageables.Contains(building))
                            {
                                // Si le bâtiment appartient à currentPlayer ou opponent
                                if (building.Owner == currentPlayer || building.Owner == opponent)
                                {
                                    gameManager.HandleBallCollisionWithBuilding(building, Position, currentPlayer);
                                    Destroy(); // Détruire la balle après la collision
                                    building.Update();
                                }
                            }
                        }
                    }
                    else if (collidable is Player playerCollide)
                    {
                        // Vérifier si la balle touche un joueur
                        if (playerCollide.CheckCollision(Position))
                        {
                            if (Game.Collidables.Contains(playerCollide))
                            {
                                gameManager.HandleBallCollisionWithPlayer(playerCollide, currentPlayer);
                                Destroy(); // Détruire la balle après la collision
                                break;  // Arrêter la boucle dès qu'il y a collision
                            }
                        }
                    }
                }

                // Vérifie si la balle sort du terrain et audio
                if (Position.Y >= Game.GroundPosition.Y)
                {                    
                    string soundFile = Environment.CurrentDirectory + @"\sounds\tomber.wav";
                    
                    using (SoundPlayer sound = new SoundPlayer(soundFile))
                    {
                        sound.Load();
                        sound.Play();
                    }
                    break;
                }
                   

                System.Threading.Thread.Sleep(50);// Pause pour ralentir l'animation   
            }
        }
    }
}
