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
        public int Width { get; private set; }    // Largeur du batiment
        public int Height { get; private set; }   // Hauteur du batiment
        private bool[,] grid;              // Tableau 2D pour représenter les cases (1 = case intacte, 0 = case détruite)
        private const char CELLFORM = '■'; // Forme d'un carré du bâtiment        
        public Vector2 ConsolePosition { get; private set; } // Position haut gauche du batiment
        public Player Owner { get; private set; } // Propriétaire du bâtiment
        private const Byte SPACE_BETWEEN = 13;

        public Building(int width, int height, Player owner, bool positionBuildingLeft)
        {
            Width = width;
            Height = height;
            Owner = owner;
            ConsolePosition = new Vector2(Owner.ConsolePosition.X + (positionBuildingLeft == true ? SPACE_BETWEEN : -SPACE_BETWEEN), Owner.ConsolePosition.Y - 2);
            grid = new bool[height, width];
            
            // Créer le rectangle pour le bâtiment            
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
        public void DisplayBuilding()
        {
            Console.SetCursorPosition((int)ConsolePosition.X, (int)ConsolePosition.Y);

            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                    Console.Write(grid[i, j] ? CELLFORM.ToString() : " ");

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
                        // Calculer la position de la case (en prenant en compte le décalage du bâtiment)
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

        public void TakeDamage(object damageData)
        {
            if (damageData is Tuple<int, int> position)
            {
                int x = position.Item1;
                int y = position.Item2;

                DestroyBlock(x, y);
            }
        }

        /// <summary>
        /// Mettre à jour l'affichage du bâtiment
        /// </summary>
        public void Update()
        {
            DisplayBuilding();
        }
    }
}
