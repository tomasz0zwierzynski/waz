using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ConsoleApplication1
{
    internal class Program
    {
        // Główny panel gry
        // klasa ta trzyma dane na temat
        // mapy gry
        public class GamePanel
        {
            public String[,] area = new String[60, 10];
        }
        // Tworzymy Punkt oraz jego współrzędne  X, Y
        public class Punkt
        {
            
            public int IleRazyPowstal = 0;
            public int xx = 10;
            public int xy = 8;
            public Point startPoint = new Point(6, 6);
            public Boolean Jest = true;
            // Punkty
            public void NowaPunktacja()
            {
                IleRazyPowstal = 0;
            }
            // Rysuje punkt do zdobycia w randowmowym miejscu mapy
            // Za każdym razem gdy powstaje punkt do zdobycia, 
            // punkt zdobyty się zwiększa
            public void RysujeKolejnyPunkt()
            {
                Random rnd = new Random();

                int poczatek = 0;
                int koniec = 60;
                int wylosowana = rnd.Next(poczatek, koniec);

                xx = wylosowana;

                poczatek = 0;
                koniec = 10;
                wylosowana = rnd.Next(poczatek, koniec);
                xy = wylosowana;
               
                IleRazyPowstal++;

            }
            public Boolean Contains(Point punkt2)
            {
                return punkt2.X==xx && punkt2.Y==xy;
            }
        }

        // Klasa węża
        public class Snake
        {
            /// Konstruktor węża:
            /// Wykonuje się gdy tworzymy nowego wężą np:
            /// Snake snake = new Snake();
            public Boolean OgonRosnij = false;
            public Snake()
            {
                // Dodaje głowę wężowi
                // Z punktu startowego
                this.tail.Add(this.startPoint);

                // Dla długości węża "startTileLength"
                for (int i = 1; i <= this.startTileLenght; i++)
                {
                    // Dodaje kolejne części ogona
                    this.tail.Add(this.getPointBehind(this.tail.Last()));
                }
            }

            // Enum z kierunkami ruchu węża
            public enum directions
            {
                Up,
                Right,
                Down,
                Left
            }

            // Punkty węża
            public int score = 0;

            // Aktualny kierunek
            public int direction = 0;

            // Aktualna prędkość węża
            public float Speed = 500;

            public float PokazywanSzybkosc = 1;

            // Lista punktów (części ogonu węża)
            public List<Point> tail = new List<Point>();

            // Punkt startowy węża
            public Point startPoint = new Point(5, 5);

            // Ilość części ogona przy stworzeniu
            public int startTileLenght = 3;

            // Zwraca punkt (X, Y) pozycji głowy węża
            public Point getHeadPosition()
            {
                return tail.First();
            }

            // Zwraca pozycje (do tyłu) względem podanej pozycji do punktu
            // biorąc pod uwagę kierunek węża
            public Point getPointBehind(Point point)
            {
                Point pointBehind;
                switch (this.direction)
                {
                    case (int)(Snake.directions.Up):
                        pointBehind = new Point(point.X, point.Y + 1);
                        break;

                    case (int)(Snake.directions.Right):
                        pointBehind = new Point(point.X - 1, point.Y);
                        break;

                    case (int)(Snake.directions.Down):
                        pointBehind = new Point(point.X, point.Y - 1);
                        break;

                    case (int)(Snake.directions.Left):
                        pointBehind = new Point(point.X + 1, point.Y);
                        break;

                    default:
                        pointBehind = new Point(-1, -1);
                        break;
                }

                return pointBehind;
            }

            // Zwraca pozycje (do przodu) względem podanej pozycji do punktu
            // biorąc pod uwagę kierunek węża
            public Point getPointForward(Point point)
            {
                Point pointBehind;
                switch (this.direction)
                {
                    case (int)(Snake.directions.Up):
                        pointBehind = new Point(point.X, point.Y - 1);
                        break;

                    case (int)(Snake.directions.Right):
                        pointBehind = new Point(point.X + 1, point.Y);
                        break;

                    case (int)(Snake.directions.Down):
                        pointBehind = new Point(point.X, point.Y + 1);
                        break;

                    case (int)(Snake.directions.Left):
                        pointBehind = new Point(point.X - 1, point.Y);
                        break;

                    default:
                        pointBehind = new Point(-1, -1);
                        break;
                }

                return pointBehind;
            }

            // Sprawdza czy kolejny ruch węża nie koliduje
            // ze ścianami lub z samym wężem
            public Boolean isSafeMove(GamePanel panel)
            {
                // Sprawdza pozycję kolejnego ruchu węża
                Point nextStep = this.getPointForward(this.getHeadPosition());

                // Sprawdza czy ta pozycja nie koliduje z wężem
                if (this.tail.Contains(nextStep))
                {
                    return false;
                }

                // Sprawdza czy wąż nie wyjedzie za obszar mapy
                if ((nextStep.X > 59 || nextStep.X < 0) || (nextStep.Y > 9 || nextStep.Y < 0))
                {
                    return false;
                }

                return true;
            }

            // Rusza węża zależnie od jego kierunku, zwiększa gdy minie punkt
            public void move()
            {
                this.tail.Insert(0, this.getPointForward(this.tail.First()));
                if (OgonRosnij)
                {
                    OgonRosnij = false;
                }
                else
                {
                    this.tail.Remove(this.tail.Last());
                }
            }
        }

        // Odświeża ekran
        // czyści konsolę następnie nanosi na nią
        // pozycję z panelu gry (GamePanel)
        public static void refreshScreen(GamePanel gamePanel)
        {
            Console.Clear();
            StringBuilder stringBuilder = new StringBuilder();

            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 60; x++)
                {
                    stringBuilder.Append(gamePanel.area[x, y]);

                    if (x == 59)
                    {
                        stringBuilder.Append(Environment.NewLine);
                    }
                }
            }

            Console.WriteLine(stringBuilder);
        }

        // Generuje mapęna podstawie pozycji węża
        public static void initializeMap(GamePanel panel, Snake snake, Punkt punkt)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 60; x++)
                {
                    if (snake.tail.Contains(new Point(x, y)))
                    {
                        panel.area[x, y] = "O";
                    }
                    else if (punkt.Contains(new Point(x, y)) && punkt.Jest)
                    {
                        panel.area[x, y] = "X";
                    }
                    else
                    {
                        panel.area[x, y] = "-";
                    }
                }
            }
        }

        public static void Main(string[] args)
        {
            GamePanel panel = new GamePanel();
            Snake snake = new Snake();
            Punkt punkt = new Punkt();
            DateTime tijd = DateTime.Now;
            DateTime tijd2 = DateTime.Now;
            Boolean buttonPressed = false;
            Boolean gameOver = false;
            initializeMap(panel, snake, punkt);

            while (true)
            {
                // Jeśli gameOver
                // czyści konsolę
                // czeka na input gracza
                // gdy znajdzie input
                // resetuje pozycję węża
                // oraz punktację
                if (gameOver)
                {

                    Console.WriteLine("Game Over!!");
                    Console.ReadKey();
                    gameOver = false;
                    snake = new Snake();
                    punkt.NowaPunktacja();
                }

                // Odświeża grę
                // Wypisuje zdibyte punkty oraz prędkość węża
                tijd = DateTime.Now;
                buttonPressed = false;
                initializeMap(panel, snake, punkt);
                refreshScreen(panel);
                Console.WriteLine("Zdobyte Punkty {0}", punkt.IleRazyPowstal);
                Console.WriteLine("Prędkość Węża {0}", snake.PokazywanSzybkosc);

                while (true)
                {
                    
                    tijd2 = DateTime.Now;
                    if (tijd2.Subtract(tijd).TotalMilliseconds > snake.Speed)
                    {
                        break;
                    }

                    // Sprawdza ktory klawisz jest wcisniety
                    // i czy klawisz nie jest przeciwny do kierunku ruchu
                    // węża
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo pressedKey = Console.ReadKey(true);

                        if (pressedKey.Key.Equals(ConsoleKey.UpArrow) &&
                            snake.direction != (int)Snake.directions.Down && buttonPressed == false)
                        {
                            snake.direction = (int)Snake.directions.Up;
                            buttonPressed = true;
                        }

                        if (pressedKey.Key.Equals(ConsoleKey.DownArrow) &&
                            snake.direction != (int)Snake.directions.Up && buttonPressed == false)
                        {
                            snake.direction = (int)Snake.directions.Down;
                            buttonPressed = true;
                        }

                        if (pressedKey.Key.Equals(ConsoleKey.LeftArrow) &&
                            snake.direction != (int)Snake.directions.Right && buttonPressed == false)
                        {
                            snake.direction = (int)Snake.directions.Left;
                            buttonPressed = true;
                        }

                        if (pressedKey.Key.Equals(ConsoleKey.RightArrow) &&
                            snake.direction != (int)Snake.directions.Left && buttonPressed == false)
                        {
                            snake.direction = (int)Snake.directions.Right;
                            buttonPressed = true;
                        }
                    }
                }

                // Gdy wąż może się bezpiecznie poruszyć
                // rusza węża
                // a jeśli koliduje z czymś (patrz isSafeMove())
                // GameOver
                if (snake.isSafeMove(panel))
                {
                    snake.move();
                    // Sprawdza czy zdobyl punkt
                    Point pun = snake.getHeadPosition();
                    // Zmienia predkosc, rosnie, rysuje kolejny punkt gdy zdobyl punkt 
                    if (pun.X == punkt.xx && pun.Y == punkt.xy) 
                    {
                        snake.OgonRosnij = true;
                        punkt.RysujeKolejnyPunkt();
                        snake.Speed /= (float )1.1;
                        snake.PokazywanSzybkosc *= (float)1.1;
                    }
                }
                else
                {
                    gameOver = true;
                }

            }
        }
    }
}