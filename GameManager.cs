/*
Entreprise : ETML
Auteur : Christopher Ristic 
Date : 17.01.2025
Description : Fournit des outils pour gérer les actions spécifiques du jeu, comme la gestion de collision, les animations.
*/

using System.Numerics;
using System;
using System.Threading;
using System.Media;
using System.Collections.Generic;
using System.Linq;

namespace JeuDeBalle
{
    internal class GameManager
    {
        /// <summary>
        /// Instance du jeu
        /// </summary>
        private readonly Game _game;

        /// <summary>
        /// Nombre de points affiché pour les angles
        /// </summary>
        private const int NUM_POINTS = 5;

        /// <summary>
        /// Constructeur pour le game manager
        /// </summary>
        /// <param name="game">instance du jeu</param>
        public GameManager(Game game)
        {
            if (game == null)
                game = new Game();

            _game = game;
        }

        /// <summary>
        /// Gère la collision entre la balle et un bâtiment.
        /// Vérifie si la balle touche une case intacte du bâtiment et inflige des dégâts.
        /// </summary>
        /// <param name="building">Le bâtiment avec lequel la balle entre en collision.</param>
        /// <param name="ballPosition">La position actuelle de la balle.</param>
        /// <param name="currentPlayer">Le joueur qui joue actuellement.</param>
        public void HandleBallCollisionWithBuilding(Building building, Vector2 ballPosition, Player currentPlayer)
        {
            try
            {
                // Trouver les coordonnées de la case touchée
                int x = (int)(ballPosition.X - building.ConsolePosition.X);
                int y = (int)(ballPosition.Y - building.ConsolePosition.Y);

                // Vérifier si la case est intacte et la détruire
                if (building.IsBlockIntact(x, y))
                {
                    string soundFile = Environment.CurrentDirectory + @"\sounds\touch.wav";

                    using (SoundPlayer sound = new SoundPlayer(soundFile))
                    {
                        sound.Load();
                        sound.Play();
                    }


                    building.TakeDamage(new Tuple<int, int>(x, y));
                    building.Update();
                    if (building.Owner == currentPlayer)
                        currentPlayer.PlayerScore -= 5;
                    else
                        currentPlayer.PlayerScore += 5;
                }
            }
            finally
            {

            }
        }

        /// <summary>
        /// Gère la collision entre la balle et un joueur.
        /// Vérifie si la balle touche un caractère du joueur et inflige des dégâts.
        /// </summary>
        /// <param name="opponent">Le joueur avec lequel la balle entre en collision.</param>
        /// <param name="currentPlayer">Le joueur qui joue actuellement.</param>
        public void HandleBallCollisionWithPlayer(Player opponent, Player currentPlayer)
        {
            // Vérifier si le joueur fait partie des objets qui peuvent subir des dégâts
            if (Game.Damageables.Contains(opponent))
            {
                string soundFile = Environment.CurrentDirectory + @"\sounds\touch.wav";

                using (SoundPlayer sound = new SoundPlayer(soundFile))
                {
                    sound.Load();
                    sound.Play();
                }
                opponent.TakeDamage(1);
                currentPlayer.Heal(1);
                currentPlayer.PlayerScore += 10;
            }
        }
        /// <summary>
        /// Remet à zéro les valeur pour la balle 
        /// </summary>
        /// <param name="currentPlayer">Récupérer la position du joueur</param>
        public void ResetBall(Player currentPlayer)
        {
            _game.Ball.Position = currentPlayer.ConsolePosition;  // Position initiale de la balle
            _game.Ball.Velocity = Vector2.Zero;  // La vitesse de la balle est réinitialisée
            _game.Ball.IsDestroyed = false;  // La balle est considérée comme non détruite
            _game.Ball.LastPosition = null;  // Effacer la dernière position pour qu'elle soit réaffichée à chaque tour
        }

        /// <summary>
        /// Animation pour la force
        /// </summary>
        public int RunHorizontalAnimation()
        {
            int forceValue = 1;
            const int maxValue = 6;
            const int minValue = 1;
            const int positionY = 10;
            int positionXAcutal = 10;
            const char CHARACTER = '▌';
            bool isSpacePressed = false;

            // Boucle principale pour gérer l'animation
            while (true)
            {
                // Effacer l'ancienne valeur en réinitialisant la position du curseur
                Console.SetCursorPosition(positionXAcutal, positionY);
                Console.Write("                                             "); // Effacer les caractères précédemment affiché

                // Replacer le curseur à la position actuelle et afficher la nouvelle valeur
                Console.SetCursorPosition(positionXAcutal, positionY);
                for (int i = 0; i < forceValue; ++i)
                {
                    Console.ForegroundColor = (forceValue <= 3) ? ConsoleColor.Green : 
                                              (forceValue > 3 && forceValue <= 6) ? ConsoleColor.Yellow: ConsoleColor.Red ;

                    Console.Write(CHARACTER);
                    Console.ResetColor();
                }

                // Afficher la valeur actuelle à côté de la barre
                Console.SetCursorPosition(positionXAcutal + forceValue + 1, positionY);
                Console.Write($" [{forceValue}]");

                // Vérifier les touches pressées
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(intercept: true).Key;

                    // Si la touche espace est pressée et qu'elle ne l'était pas déjà
                    if (key == ConsoleKey.Spacebar && !isSpacePressed)
                        isSpacePressed = true;

                    // Si l'espace est relâché
                    else if (key == ConsoleKey.Spacebar && isSpacePressed)
                    {
                        break;
                    }
                }

                // Si on clique espace, incrémente la valeur
                if (isSpacePressed)
                {
                    ++forceValue;
                    if (forceValue > maxValue)
                        forceValue = minValue; // Réinitialiser à la valeur minimale après avoir atteint la valeur maximale
                }
                Thread.Sleep(100); 
            }
            return forceValue;
        }
        
        /// <summary>
        /// Système qui gère l'angle de tir de la balle
        /// </summary>
        public int AngleSystem(int MIN_ANGLE, int MAX_ANGLE, bool firstTurn)
        {
            bool isSpacePressed = false;
            int _actualAngle = MIN_ANGLE;

            while (true)
            {
                // Afficher les points à la verticale, du bas vers le haut
                for (int i = 0; i < NUM_POINTS; ++i)
                {
                    if (firstTurn)
                    {
                        // Calculer la position verticale, en partant du bas vers le haut
                        int verticalPosition = (int)_game.Player1.ConsolePosition.Y - 5 + (NUM_POINTS - 1 - i);

                        // Affichage du côté gauche
                        Console.SetCursorPosition((int)_game.Player1.ConsolePosition.X + 9 - (i + 1), verticalPosition);

                        // Vérifier si l'angle est dans la plage de ce point pour l'affichage
                        if (_actualAngle >= MIN_ANGLE + (MAX_ANGLE - MIN_ANGLE) / (NUM_POINTS - 1) * i &&
                            _actualAngle < MIN_ANGLE + (MAX_ANGLE - MIN_ANGLE) / (NUM_POINTS - 1) * (i + 1))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write('o');
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write('.');
                        }  
                    }
                    else
                    {
                        // Calculer la position verticale, en partant du bas vers le haut
                        int verticalPosition = (int)_game.Player2.ConsolePosition.Y - 11 + NUM_POINTS + 1 + i;

                        // Affichage du côté gauche
                        Console.SetCursorPosition((int)_game.Player2.ConsolePosition.X - (i + 1), verticalPosition);

                        // Vérifier si l'angle est dans la plage de ce point pour l'affichage
                        if (_actualAngle >= MIN_ANGLE + (MAX_ANGLE - MIN_ANGLE) / (NUM_POINTS - 1) * i &&
                            _actualAngle < MIN_ANGLE + (MAX_ANGLE - MIN_ANGLE) / (NUM_POINTS - 1) * (i + 1))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write('o');
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write('.');
                        }                            
                    }
                    
                }
                if (_actualAngle > MAX_ANGLE)
                    _actualAngle = MIN_ANGLE + (_actualAngle - MAX_ANGLE);

                if (_actualAngle < MIN_ANGLE)
                    _actualAngle = MAX_ANGLE - (MIN_ANGLE - _actualAngle);

                Console.ForegroundColor = ConsoleColor.White;

                if (firstTurn)
                {
                    Console.SetCursorPosition((int)_game.Player1.ConsolePosition.X - 2, (int)_game.Player1.ConsolePosition.Y + 5);
                    Console.WriteLine($"Angle: {_actualAngle}°");
                }
                else
                {
                    Console.SetCursorPosition((int)_game.Player2.ConsolePosition.X - 2, (int)_game.Player2.ConsolePosition.Y + 5);
                    Console.WriteLine($"Angle: {_actualAngle}°");
                }

                // Si la touche espace est pressée et qu'elle ne l'était pas déjà
                if (Console.KeyAvailable)
                {
                    ConsoleKey key = Console.ReadKey(intercept: true).Key;
                    if (key == ConsoleKey.Spacebar)
                    {
                        break;
                    }
                }

                if (isSpacePressed)
                {                    
                    if (_actualAngle > MAX_ANGLE) 
                        _actualAngle = MIN_ANGLE + (_actualAngle - MAX_ANGLE);

                    if (_actualAngle < MIN_ANGLE) 
                        _actualAngle = MAX_ANGLE - (MIN_ANGLE - _actualAngle);
                }

                // Variation aléatoire de l'angle à l'intérieur de la plage
                double variation = 1 + new Random().NextDouble() * 2.0 - 1.0;  // Variation entre -1 et 1
                _actualAngle += (int)((MAX_ANGLE - MIN_ANGLE) / (NUM_POINTS - 1) * variation);

                Thread.Sleep(300);     
            }          
            return _actualAngle;
        }
        /// <summary>
        /// Etoile pour l'animation de fin de jeu
        /// </summary>
        class Star
        {
            public int X { get; set; }
            public int Y { get; set; }
            public char CHARACTER { get; private set; } = '*';
        }

        /// <summary>
        /// Animation de fin de jeu
        /// </summary>
        public void EndGameAnimation()
        {            
            string gameOver = @"
      ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
     ░  ██████   █████  ███    ███ ███████     ██████  ██    ██ ███████ █████  ░
     ░ ██       ██   ██ ████  ████ ██         ██    ██ ██    ██ ██      ██  ██ ░
     ░ ██   ███ ███████ ██ ████ ██ █████      ██    ██ ██    ██ █████   ████   ░
     ░ ██    ██ ██   ██ ██  ██  ██ ██         ██    ██  ██  ██  ██      ██  ██ ░
     ░  ██████  ██   ██ ██      ██ ███████     ██████    ████   ███████ ██  ██ ░
      ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░
  ";

            string[] lines = gameOver.Split('\n');

            int gameOverLines = lines.Length;
            int gameOverWidth = lines.Max(l => l.Length);
            int gameOverStartRow = (Game.WINDOW_HEIGHT - 20 - gameOverLines) / 2;
            int gameOverStartCol = (Game.WINDOW_WIDTH - 10 - gameOverWidth) / 2;

            int width = Game.WINDOW_WIDTH;
            int height = Game.WINDOW_HEIGHT - 20;
            int starCount = (width * height) / 50;
            Random rand = new Random();

            // Initialisation des étoiles
            List<Star> stars = new List<Star>();
            for (int i = 0; i < starCount; i++)
            {
                stars.Add(new Star { X = rand.Next(0, width), Y = rand.Next(0, height) });
            }

            Console.CursorVisible = false;

            // Boucle d'animation
            while (!Console.KeyAvailable)
            {
                // Pour chaque étoile, effacer son ancien caractère, mettre à jour sa position, puis l'afficher
                foreach (Star star in stars)
                {
                    // Effacer l'ancienne position en y écrivant un espace
                    Console.SetCursorPosition(star.X, star.Y);
                    Console.Write(" ");

                    // Mise à jour de la position
                    --star.X;
                    if (star.X < 0)
                    {
                        star.X = width - 1;
                        star.Y = rand.Next(0, height);
                    }

                    // Afficher l'étoile dans sa nouvelle position avec une couleur aléatoire (jaune ou blanc)
                    Console.ForegroundColor = (rand.Next(0, 2) == 0) ? ConsoleColor.DarkYellow : ConsoleColor.White;
                    Console.SetCursorPosition(star.X, star.Y);
                    Console.Write(star.CHARACTER);
                }

                // Redessiner le texte "Game Over" par-dessus les étoiles pour éviter que les étoiles n'effacent le texte
                Console.ResetColor();
                for (int i = 0; i < lines.Length; ++i)
                {
                    int row = gameOverStartRow + i;
                    if (row >= 0 && row < height)
                    {
                        Console.SetCursorPosition(gameOverStartCol, row);
                        Console.Write(lines[i]);
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
