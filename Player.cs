/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Classe représentant un joueur 
*/

using System;
using System.Media;
using System.Numerics;

namespace JeuDeBalle
{
    internal class Player : IDamageable, ICollidable
    {
        /// <summary>
        /// Point de vie
        /// </summary>
        public Byte LifePoints { get; set; } = 2;

        /// <summary>
        /// Point de score
        /// </summary>
        public int PlayerScore { get; set; } = 0;

        /// <summary>
        /// Position du joueur dans la console
        /// </summary>
        public Vector2 ConsolePosition { get; private set; }
        
        /// <summary>
        /// Angle minimum de tir
        /// </summary>
        public readonly int ANGLE_MIN;

        /// <summary>
        /// Angle maximum de tir
        /// </summary>
        public readonly int ANGLE_MAX;

        /// <summary>
        /// Le dessin qui représente le joueur
        /// </summary>
        public readonly string[] CHARACTER =
        {
            "●\u263A",
            @"-|-",
            @"´ `"
        };

        /// <summary>
        /// Couleur du joueur
        /// </summary>
        public ConsoleColor ConsoleColor {  get; private set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        /// <param name="consoleColor">Couleur du joueur</param>
        public Player(int x, int y, ConsoleColor consoleColor, int angleMin, int angleMax) 
        {
            ConsolePosition = new Vector2(x, y);
            ConsoleColor = consoleColor;
            ANGLE_MAX = angleMax;
            ANGLE_MIN = angleMin;

            Game.Collidables.Add(this);
            Game.Damageables.Add(this);            
        }

        /// <summary>
        /// Le joueur prendra des dégats
        /// </summary>
        /// <param name="damageData">Nombre de dégats subi par le joueur</param>
        public void TakeDamage(object damageData)
        {
            if (damageData is int damageAmount)
                LifePoints -= (Byte)damageAmount;

            if(LifePoints == 1)
            {
                string soundFile = Environment.CurrentDirectory + @"\sounds\lowHP.wav"; 

                using (SoundPlayer sound = new SoundPlayer(soundFile))
                {
                    sound.Load();
                    sound.Play();
                }
            }
        }
        
        /// <summary>
        /// Méthode générique pour ajouter des points de vie au joueur
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="amount">Nombre de point de vie récupéré</param>
        public void Heal<T>(T amount) where T : IConvertible => LifePoints += Convert.ToByte(amount);

        /// <summary>
        /// Verifie si le joueur collisionne avec un autre objet
        /// </summary>
        /// <param name="ballPosition">Position de la balle</param>
        /// <returns>Vrai si collision avec le joueur</returns>
        public bool CheckCollision(Vector2 ballPosition)
        {
            // On parcourt chaque caractère du joueur en fonction de sa forme
            for (int dy = 0; dy < CHARACTER.Length; ++dy) // Hauteur de la forme du joueur
            {
                for (int dx = 0; dx < CHARACTER[dy].Length; ++dx) // Largeur de la forme du joueur
                {
                    // Calcul de la position de chaque caractère (case) du joueur
                    int x = (int)(ConsolePosition.X + dx);
                    int y = (int)(ConsolePosition.Y + dy);

                    // Vérifier si la balle touche cette case
                    if (Convert.ToInt32(ballPosition.X) >= x && Convert.ToInt32(ballPosition.X) < x + 1 
                        && Convert.ToInt32(ballPosition.Y) >= y && Convert.ToInt32(ballPosition.Y) < y + 1)
                        return true; // Collision avec le joueur
                }
            }
            return false;
        }
    }
}
