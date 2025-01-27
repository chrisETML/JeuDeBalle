/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Interface pour les objets dommageable
*/


namespace JeuDeBalle
{
    internal interface IDamageable
    {
        /// <summary>
        /// Prototype de méthode qui permet de faire des dégats à l'objet
        /// </summary>
        /// <param name="damageData">Données lié au dégats subi sur l'objet</param>
        void TakeDamage(object damageData);
    }
}
