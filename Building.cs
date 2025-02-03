/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Classe représentant un bâtiment
*/

using System;
using System.Numerics;

namespace JeuDeBalle
{
    internal class Building : ICollidable, IDamageable, IUpdatable
    {

        /// <summary>
        /// Représente la largeur du bâtiment.
        /// </summary>
        public int Width { get; private set; }    

        /// <summary>
        /// Représente la hauteur du bâtiment.
        /// </summary>
        public int Height { get; private set; }  

        /// <summary>
        /// Tableau 2D représentant les cases du bâtiment.
        /// Chaque case peut être intacte (true) ou détruite (false).
        /// </summary>
        private bool[,] grid;             

        /// <summary>
        /// Représente le caractère utilisé pour afficher une case du bâtiment dans la console.
        /// </summary>
        private const char CELLFORM = '■'; 

        /// <summary>
        /// Position du coin supérieur gauche du bâtiment sur la console.
        /// </summary>
        public Vector2 ConsolePosition { get; private set; } 

        /// <summary>
        /// Propriétaire du bâtiment.
        /// </summary>
        public Player Owner { get; private set; } 

        /// <summary>
        /// Espace entre les bâtiments et les joueurs sur l'axe x.
        /// </summary>
        private const Byte SPACE_BETWEEN = 13;

        /// <summary>
        /// Constructeur de la classe Building.    
        /// </summary>
        /// <param name="width">La largeur du bâtiment.</param>
        /// <param name="height">La hauteur du bâtiment.</param>
        /// <param name="owner">Le joueur qui possède ce bâtiment.</param>
        /// <param name="positionBuildingLeft">Indique si le bâtiment doit être positionné à gauche ou à droite du joueur.</param>
        public Building(int width, int height, Player owner, bool positionBuildingLeft)
        {
            Width = width;
            Height = height;
            Owner = owner;
            ConsolePosition = new Vector2(Owner.ConsolePosition.X + (positionBuildingLeft == true ? SPACE_BETWEEN : -SPACE_BETWEEN), Owner.ConsolePosition.Y - 2);
            grid = new bool[height, width];
            
            // Créer un rectangle pour le bâtiment            
            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                    grid[i, j] = true;
            }

            Game.Collidables.Add(this);
            Game.Damageables.Add(this);
            Game.Updatables.Add(this);
        }

        /// <summary>
        /// Méthode pour détruire une case à une position spécifique
        /// </summary>
        /// <param name="x">Axe x de la grille</param>
        /// <param name="y">Axe y de la grille</param>
        public void DestroyBlock(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                grid[y, x] = false;
        }

        /// <summary>
        /// Méthode pour vérifier si une case est intacte
        /// </summary>
        /// <param name="x">Axe x de la grille</param>
        /// <param name="y">Axe y de la grille</param>
        /// <returns>True si la case est intacte</returns>
        public bool IsBlockIntact(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return grid[y, x];

            return false;
        }

        /// <summary>
        /// Afficher l'état du bâtiment sous forme de grille dans la console
        /// </summary>
        public void Update()
        {
            Console.SetCursorPosition((int)ConsolePosition.X, (int)ConsolePosition.Y);

            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    Console.ForegroundColor = Owner.ConsoleColor;
                    Console.Write(grid[i, j] ? CELLFORM.ToString() : " ");
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.SetCursorPosition((int)ConsolePosition.X, Console.CursorTop);
            }
        }

        /// <summary>
        /// Vérifier si la balle touche le bâtiment
        /// </summary>
        /// <param name="ballPosition">Position de la balle</param>
        /// <returns>True si la balle touche le bâtiment</returns>
        public bool CheckCollision(Vector2 ballPosition)
        {
            // Vérifiez chaque case du bâtiment
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (grid[y, x])  // Si la case est intacte
                    {
                        // Calculer la position de la case
                        float cellX = ConsolePosition.X + x;
                        float cellY = ConsolePosition.Y + y;

                        // Vérifier si la balle touche une case du bâtiment
                        if (ballPosition.X >= cellX && ballPosition.X < cellX + 1 && ballPosition.Y >= cellY && ballPosition.Y < cellY + 1)                        
                            return true;                       
                    }
                }
            }
            return false; // La balle ne touche pas le bâtiment
        }

        /// <summary>
        /// Endommager une case du batiment
        /// </summary>
        /// <param name="damageData">Position à endommager</param>
        public void TakeDamage(object damageData)
        {
            if (damageData is Tuple<int, int> position)
            {
                int x = position.Item1;
                int y = position.Item2;

                DestroyBlock(x, y);
            }
        }        
    }
}
