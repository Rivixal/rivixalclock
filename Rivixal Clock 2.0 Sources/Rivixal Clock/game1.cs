using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rivixal_Clock
{
    public partial class game1 : Form
    {
        private Timer gameTimer;
        private List<Rectangle> fallingObjects;
        private Rectangle player;
        private int playerSpeed = 10;
        private int objectSpeed = 5;
        private Random random;
        private bool gameOver = false;

        public game1()
        {
            InitializeGame();
            this.Load += new System.EventHandler(this.Game1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Game1_KeyDown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Game1_Paint);
            this.ResumeLayout(false);
        }

        private void InitializeGame()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Black;

            gameTimer = new Timer();
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;

            fallingObjects = new List<Rectangle>();
            player = new Rectangle(this.ClientSize.Width / 2, this.ClientSize.Height - 50, 50, 50);
            random = new Random();

            gameTimer.Start();
        }

        private void Game1_Load(object sender, EventArgs e)
        {
        }

        private void Game1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!gameOver)
            {
                switch (e.KeyCode)
                {
                    case Keys.W:
                        player.Y -= playerSpeed;
                        if (player.Y < 0) player.Y = 0;
                        break;
                    case Keys.A:
                        player.X -= playerSpeed;
                        if (player.X < 0) player.X = 0;
                        break;
                    case Keys.S:
                        player.Y += playerSpeed;
                        if (player.Y + player.Height > this.ClientSize.Height) player.Y = this.ClientSize.Height - player.Height;
                        break;
                    case Keys.D:
                        player.X += playerSpeed;
                        if (player.X + player.Width > this.ClientSize.Width) player.X = this.ClientSize.Width - player.Width;
                        break;
                    case Keys.Escape:
                        this.Close();
                        break;
                }
            }
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                UpdateFallingObjects();
                CheckCollisions();
                Invalidate();
            }
        }

        private void UpdateFallingObjects()
        {
            for (int i = 0; i < fallingObjects.Count; i++)
            {
                fallingObjects[i] = new Rectangle(fallingObjects[i].X, fallingObjects[i].Y + objectSpeed, fallingObjects[i].Width, fallingObjects[i].Height);
                if (fallingObjects[i].Y > this.ClientSize.Height)
                {
                    fallingObjects.RemoveAt(i);
                    i--;
                }
            }

            if (random.Next(100) < 10)
            {
                int x = random.Next(this.ClientSize.Width - 50);
                fallingObjects.Add(new Rectangle(x, 0, 50, 50));
            }
        }

        private void CheckCollisions()
        {
            foreach (var obj in fallingObjects)
            {
                if (player.IntersectsWith(obj))
                {
                    gameTimer.Stop();
                    gameOver = true;
                    Invalidate(); // Перерисовываем форму для отображения "Game Over"
                    break;
                }
            }
        }

        private void Game1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (gameOver)
            {
                string gameOverText = "Game Over";
                Font font = new Font("lucida console", 40);
                SizeF textSize = g.MeasureString(gameOverText, font);
                PointF textLocation = new PointF((this.ClientSize.Width - textSize.Width) / 2, (this.ClientSize.Height - textSize.Height) / 2);

                g.DrawString(gameOverText, font, Brushes.Red, textLocation);
                this.Close();
            }
            else
            {
                using (Brush brush = new SolidBrush(Color.Blue))
                {
                    g.FillRectangle(brush, player);
                }

                using (Brush brush = new SolidBrush(Color.Red))
                {
                    foreach (var obj in fallingObjects)
                    {
                        g.FillRectangle(brush, obj);
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
