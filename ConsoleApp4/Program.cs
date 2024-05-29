using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApp4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Menu();
        }
        static void Menu()
        {
            Console.Clear();
            Console.WriteLine("Bienvenido al juego de Snake!");
            Console.WriteLine("1. Iniciar juego");
            Console.WriteLine("2. Salir");

            char key = Console.ReadKey().KeyChar;

            switch (key)
            {
                case '1':
                    StartGame();
                    break;
                case '2':
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opción no válida. Presiona una tecla para volver al menú.");
                    Console.ReadKey();
                    Menu();
                    break;
            }
        }


        static void StartGame()
        {
            Console.Clear();
            Console.WriteLine("Iniciando juego...");

            SnakeGame snakeGame = new SnakeGame(); // Inicia una nueva instancia del juego de Snake
            snakeGame.Start(); // Inicia el juego

            Console.WriteLine("Presiona cualquier tecla para volver al menú.");
            Console.ReadKey();
            Menu(); // Vuelve al menú después de que el juego termina
        }

        class SnakeGame
        {
            private int width = 30;                                 // Ancho del tablero
            private int height = 15;                                // Alto del tablero
            private int score = 0;                                  // Puntaje inicial del jugador
            private bool gameOver = false;                          // Variable que indica si el juego ha terminado
            private Queue<Position> snake = new Queue<Position>();  // Cola para almacenar las posiciones de la serpiente
            private Position food;                                  // Posición de la comida en el tablero
            private Random random = new Random();                   // Generador de números aleatorios
            private int speed = 300;                                // Velocidad inicial del juego en milisegundos (controla la velocidad de la serpiente)

            private ConsoleKey currentDirection = ConsoleKey.RightArrow; // Dirección inicial de la serpiente

            // Estructura que representa una posición en el tablero
            private class Position
            {
                public int X { get; set; }
                public int Y { get; set; }

                public Position(int x, int y)
                {
                    X = x;
                    Y = y;
                }
            }

            public void Start()
            {
                Initialize(); // Inicializa el juego (coloca la serpiente y la comida en el tablero)

                while (!gameOver)
                {
                    ProcessInput();         // Procesa la entrada del usuario
                    Update();               // Actualiza el estado del juego (mover la serpiente, detectar colisiones, etc.)
                    Draw();                 // Dibuja el tablero y los elementos del juego en la consola
                    Thread.Sleep(speed);    // Pausa el juego durante un corto período de tiempo para controlar la velocidad
                }

                Console.SetCursorPosition(0, height + 3);
                Console.WriteLine("¡Game Over! Puntaje final: " + score); // Muestra el puntaje final cuando el juego termina
            }

            private void Initialize()
            {
                snake.Clear();
                snake.Enqueue(new Position(width / 2, height / 2)); // Agrega la cabeza de la serpiente al centro del tablero

                food = GenerateNewFoodPosition();   // Genera una nueva posición para la comida

                score = 0;                          // Reinicia el puntaje del jugador
                gameOver = false;                   // Reinicia la variable de fin de juego

                Console.Clear();
                DrawBorders();                      // Dibuja los bordes del tablero en la consola
            }

            private void DrawBorders()
            {
                // Dibuja los bordes superior e inferior del tablero
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("+" + new string('-', width) + "+");

                for (int y = 0; y < height; y++)
                {
                    // Dibuja los bordes laterales del tablero
                    Console.Write("|" + new string(' ', width) + "|");
                    if (y < height - 1) Console.WriteLine(); // Cambia de línea excepto en la última fila
                }

                // Dibuja el borde inferior del tablero
                Console.WriteLine();
                Console.WriteLine("+" + new string('-', width) + "+");
            }

            private void Draw()
            {
                // Dibuja la comida en el tablero
                Console.SetCursorPosition(food.X + 1, food.Y + 1);
                Console.Write("■");

                // Dibuja la serpiente en el tablero
                foreach (var position in snake)
                {
                    if (position.Equals(snake.Last()))
                    {
                        // Dibuja la cabeza de la serpiente
                        Console.SetCursorPosition(position.X + 1, position.Y + 1);
                        Console.Write("☻");
                    }
                    else
                    {
                        // Dibuja el cuerpo de la serpiente
                        Console.SetCursorPosition(position.X + 1, position.Y + 1);
                        Console.Write("O");
                    }
                }

                // Borra la cola de la serpiente si no ha crecido
                if (snake.Count > score + 1)
                {
                    var tail = snake.Dequeue();
                    Console.SetCursorPosition(tail.X + 1, tail.Y + 1);
                    Console.Write(" ");
                }

                // Muestra el puntaje justo debajo del borde inferior
                Console.SetCursorPosition(0, height + 2);
                Console.Write("Puntuacion: " + score);
            }

            // Función que procesa las teclas presionadas por el usuario
            private void ProcessInput()
            {
                // Procesa las teclas presionadas por el usuario y actualiza la variable currentDirection.
                while (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (currentDirection != ConsoleKey.DownArrow)
                                currentDirection = key.Key;
                            break;
                        case ConsoleKey.DownArrow:
                            if (currentDirection != ConsoleKey.UpArrow)
                                currentDirection = key.Key;
                            break;
                        case ConsoleKey.LeftArrow:
                            if (currentDirection != ConsoleKey.RightArrow)
                                currentDirection = key.Key;
                            break;
                        case ConsoleKey.RightArrow:
                            if (currentDirection != ConsoleKey.LeftArrow)
                                currentDirection = key.Key;
                            break;
                        case ConsoleKey.Escape:
                            gameOver = true;
                            break;
                    }
                }
            }

            // Función que actualiza el estado del juego
            private void Update()
            {
                // Obtener la posición de la cabeza de la serpiente
                Position head = snake.Last();
                // Inicializar la siguiente posición de la serpiente
                Position nextPosition = null;

                // Calcular la siguiente posición en función de la dirección actual de la serpiente
                switch (currentDirection)
                {
                    // Si la serpiente va hacia arriba, la siguiente posición será una fila arriba de la cabeza actual
                    case ConsoleKey.UpArrow:
                        nextPosition = new Position(head.X, head.Y - 1);
                        break;
                    // Si la serpiente va hacia abajo, la siguiente posición será una fila abajo de la cabeza actual
                    case ConsoleKey.DownArrow:
                        nextPosition = new Position(head.X, head.Y + 1);
                        break;
                    // Si la serpiente va hacia la izquierda, la siguiente posición será una columna a la izquierda de la cabeza actual
                    case ConsoleKey.LeftArrow:
                        nextPosition = new Position(head.X - 1, head.Y);
                        break;
                    // Si la serpiente va hacia la derecha, la siguiente posición será una columna a la derecha de la cabeza actual
                    case ConsoleKey.RightArrow:
                        nextPosition = new Position(head.X + 1, head.Y);
                        break;
                }

                // Verificar si la serpiente ha alcanzado los bordes del tablero y ajustar la posición en consecuencia para aparecer por el otro lado
                if (nextPosition.X < 0)
                    nextPosition.X = width - 1;
                
                else if (nextPosition.X >= width)
                    nextPosition.X = 0;
                

                if (nextPosition.Y < 0)
                    nextPosition.Y = height - 1;
                
                else if (nextPosition.Y >= height)
                    nextPosition.Y = 0;
                

                // Verificar si la serpiente colisiona consigo misma
                if (IsSnakeCollidingWithItself(nextPosition))
                {
                    gameOver = true; // Si la serpiente colisiona consigo misma, el juego termina
                    return; // Salir del método para detener la ejecución adicional
                }

                // Agregar la siguiente posición a la cola de la serpiente
                snake.Enqueue(nextPosition);

                // Verificar si la serpiente ha alcanzado la comida
                if (nextPosition.X == food.X && nextPosition.Y == food.Y)
                {
                    score++; // Incrementar el puntaje del jugador
                    food = GenerateNewFoodPosition(); // Generar una nueva posición para la comida

                    // Incrementar la velocidad gradualmente cuando la serpiente come más
                    if (speed > 50) // Limitar la velocidad mínima
                    {
                        speed -= 20; // Ajustar este valor según la sensibilidad deseada
                    }
                }
            }

            // Función que genera una nueva posición para la comida
            private Position GenerateNewFoodPosition()
            {
                // Inicializa una nueva posición para la comida
                Position newPosition;
                // Genera aleatoriamente una posición dentro del área del tablero
                do
                {
                    // Genera coordenadas X e Y aleatorias dentro del rango del ancho y alto del tablero
                    newPosition = new Position(random.Next(0, width), random.Next(0, height));
                } while (snake.Contains(newPosition)); // Repite el proceso si la nueva posición coincide con alguna parte del cuerpo de la serpiente
                return newPosition; // Devuelve la nueva posición generada para la comida
            }

            // Función que verifica si la serpiente colisiona consigo misma
            private bool IsSnakeCollidingWithItself(Position nextPosition)
            {
                // Itera sobre todas las partes del cuerpo de la serpiente
                foreach (var part in snake)
                {
                    // Comprueba si la siguiente posición coincide con alguna parte del cuerpo de la serpiente
                    if (nextPosition.X == part.X && nextPosition.Y == part.Y)
                    {
                        return true; // Devuelve verdadero si hay una colisión entre la siguiente posición y alguna parte del cuerpo de la serpiente
                    }
                }
                return false; // Devuelve falso si no hay colisión entre la siguiente posición y ninguna parte del cuerpo de la serpiente
            }
        }
    }
}

