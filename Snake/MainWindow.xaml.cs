using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic.Devices;
using NAudio.Wave;
using System.IO;
using System.Windows.Threading;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WaveOutEvent waveOut;
        private AudioFileReader audioFileReader;
        private DispatcherTimer timer;
        private int seconds;

        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food },
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 },
            { Direction.Right, 90 },
            { Direction.Down, 180 },
            { Direction.Left, 270 }
        };

        private readonly int rows = 40;
        private readonly int cols = 50;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int highScore = 0;
        public MainWindow()
        {
            // Initialize the timer
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Set the interval to 1 second
            timer.Tick += OnTimerTick;

            // Initial seconds value
            seconds = 0;

            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
            string fileName = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "highscore.txt"); 
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName);
                highScore = int.Parse(sr.ReadLine());
                sr.Close();
                HighScoreText.Text = $"High Score: {highScore}";
            }
            else
            {
                StreamWriter sw = new StreamWriter(fileName);
                sw.WriteLine(highScore);
                sw.Close();
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            // Calculate minutes and remaining seconds
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;

            // Update the TextBlock with the formatted time
            timeTextBlock.Text = $"Time: {minutes:D2}:{remainingSeconds:D2}";

            // Increment the seconds counter
            seconds++;
        }


        private async Task RunGame()
        {
            timer.Start();
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangedDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangedDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangedDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangedDirection(Direction.Down);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;

            for(int r=0; r < rows; r++)
            {
                for(int c=0; c < cols; c++)
                {
                    Image image = new System.Windows.Controls.Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"SCORE {gameState.Score}";

            if (gameState.Score == 69)
            {
                var audioFile = new AudioFileReader("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\ding-sound-effect_1.mp3");
                var waveOut = new WaveOut();
                waveOut.Init(audioFile);
                waveOut.Play();
            }

            if (gameState.Score == 50 )
            {
                var audioFile = new AudioFileReader("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\japanese-eas-alarm.mp3");
                var waveOut = new WaveOut();
                waveOut.Init(audioFile);
                waveOut.Play();
            }

            if (gameState.Score == 150)
            {
                var audioFile = new AudioFileReader("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\israel-eas-alarm.mp3");
                var waveOut = new WaveOut();
                waveOut.Init(audioFile);
                waveOut.Play();
            }

            if (gameState.Score == 100)
            {
                var audioFile = new AudioFileReader("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\daft-punk-robot-rock-official-audio.mp3");
                var waveOut = new WaveOut();
                waveOut.Init(audioFile);
                waveOut.Play();
            }
        }

        private void DrawGrid()
        {
            for (int r=0; r < rows;r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            var audioFile = new AudioFileReader("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\femur-breaker-[AudioTrimmer.com].mp3");
            var waveOut = new WaveOut();
            waveOut.Init(audioFile);
            waveOut.Play();
            List<Position> positions = new List<Position>(gameState.SnakePositions());
            for (int i = 0; i < positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i ==0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            var audioFile = new AudioFileReader("C:\\Users\\802630ctc\\source\\repos\\Snake\\Snake\\Assets\\nnnnn_luSV5Gb.mp3");
            var waveOut = new WaveOut();
            waveOut.Init(audioFile);
            waveOut.Play();
            seconds = 0;
            for (int i = 3; i >= 1; i--) 
            { 
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            if (gameState.Score > highScore)
            {
                highScore = gameState.Score;
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\highscore.txt");
                sw.WriteLine(highScore);
                sw.Close();
            }

            HighScoreText.Text = $"High Score: {highScore}";

            timer.Stop();
            await DrawDeadSnake();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverlayText.Text = "PRESS ANY KEY TO START";
        }

    }
}


