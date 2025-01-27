/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Interface pour les objets collisionable
*/

using System.Numerics;

namespace JeuDeBalle
{
    internal interface ICollidable
    {
        /// <summary>
        /// Vérifier la collision avec une position
        /// </summary>
        /// <param name="position">Position à vérifier</param>
        /// <returns>Vrai si on touche un objet</returns>
        bool CheckCollision(Vector2 position);
    }
}
