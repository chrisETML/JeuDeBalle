/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Classe représentant un joueur 
*/

using System;
using System.Numerics;

namespace JeuDeBalle
{
    internal class Player : IDamageable, ICollidable
    {
        /// <summary>
        /// Point de vie
        /// </summary>
        public Byte LifePoints { get; set; } = 3;

        /// <summary>
        /// Point de score
        /// </summary>
        public int PlayerScore { get; set; } = 0;

        /// <summary>
        /// Position du joueur dans la console
        /// </summary>
        public Vector2 ConsolePosition { get; private set; }

        /// <summary>
        /// Le dessin qui représente le joueur
        /// </summary>
        public readonly string[] CARACTER =
        {
            " \u263A",
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
        public Player(int x, int y, ConsoleColor consoleColor) 
        {
            ConsolePosition = new Vector2(x, y);
            ConsoleColor = consoleColor;
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
        }
        
        /// <summary>
        /// Méthode générique pour ajouter des points de vie au joueur
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="amount">Nombre de point de vie récupéré</param>
        public void Heal<T>(T amount) where T : IConvertible => LifePoints += Convert.ToByte(amount);
                
        public bool CheckCollision(Vector2 position)
        {
            float collisionRadius = 1.0f; // Rayon fictif pour détecter la collision
            return Vector2.Distance(ConsolePosition, position) <= collisionRadius;
        }
    }
}
