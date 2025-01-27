/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Classe pour la balle du jeu
*/

using System;
using System.Numerics;

namespace JeuDeBalle
{
    internal class Ball : IUpdatable
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        private const float GRAVITY = 0.5f;   // Gravité constante
        private const float TIME_STEP = 0.2f;  // Pas de temps pour la simulation
        private const char BALL_FORM = '●';   // Représente la balle
        public Vector2? LastPosition { get; set; } // Stocker la dernière position pour l'effacement
        public bool IsDestroyed { get; set; } // Savoir si la balle est détruite lorsqu'elle a fini son parcours
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
        public void Display()
        {
            // Efface la balle à la dernière position
            if (LastPosition.HasValue)
            {
                Console.SetCursorPosition((int)LastPosition.Value.X, (int)LastPosition.Value.Y);
                Console.Write(' ');
            }

            // Affiche la balle à la nouvelle position
            Console.SetCursorPosition((int)Position.X, (int)Position.Y);
            Console.Write(BALL_FORM); 
        }

        /// <summary>
        /// Lance la balle avec un angle, une force et une position de départ
        /// </summary>
        /// <param name="angle">Angle de tir en degrés</param>
        /// <param name="force">Force de tir</param>
        /// <param name="startPosition">Position de départ de la balle</param>
        public void Launch(float angle, float force, Vector2 startPosition)
        {
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

        public void Destroy()
        {
            IsDestroyed = true;
            Console.SetCursorPosition((int)Position.X, (int)Position.Y);
            Console.Write(' ');
        }
    }
}
