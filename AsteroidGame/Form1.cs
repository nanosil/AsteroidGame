/*
 * Programmer Fernando
 * Date: 2/16/2012
 * Time: 7:28 PM
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Threading;
using System.Windows.Forms;
namespace AsteroidGame
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public static ComponentResourceManager resources = new ComponentResourceManager(typeof(MainForm));
        Bitmap ship;
        Bitmap background;
        Rectangle rec;
        Rectangle resetRec;
        Random r = new Random();
        int score = 0, shotCount = 0, lifes = 3, level = 1, levelcount = 0, lifecount = 0,
            bLcount = 0, fps = 0, framecount = 0, start = Environment.TickCount, diff = 0;
        bool goingRight = false, goingLeft = false, gointUp = false, goingDown = false, gameover = false,
        showingLevel = false, levelingUp = false, killed = false, clicked = false, paused = false;
        Astroid[] astroids = new Astroid[5];
        Bullet[] bullets;
        Bullet[] bulletsR = new Bullet[5];
        Bullet[] bulletsL = new Bullet[5];
        BufferedGraphics grafx;
        Font scoreFont = new Font("Arial", 17);
        Font gameoverFont = new Font("Algerian", 30);
        SolidBrush brush2 = new SolidBrush(Color.Red);
        SolidBrush brush3 = new SolidBrush(Color.Yellow);
        SolidBrush brush4 = new SolidBrush(Color.Green);
        Life life;
        Sphere sphere;
        Ammo ammo;
        SoundPlayer explosound;
        SoundPlayer shotSound;
        System.Windows.Forms.Timer powerUpTimer = new System.Windows.Forms.Timer();

        void MainFormLoad(object sender, System.EventArgs e)
        {
            int num = (20 / Level) - diff;
            if (num <= 0)
                num = 1;
            bullets = new Bullet[num];
            button1.BringToFront();
            button1.Location = new Point((this.Width / 2) - 50, (this.Height / 2) - 40);
            numericUpDown1.Location = new Point((this.Width / 2) + 25, (this.Height / 2) + 20);
            label1.Location = new Point((this.Width / 2) - 90, (this.Height / 2) + 22);
            pictureBox1.Image = ((Bitmap)(resources.GetObject("background")));
            this.BackgroundImageLayout = ImageLayout.Stretch;
            life = new Life(r, this.Size) { speed = 0 };
            sphere = new Sphere(r, this.Size) { speed = 0 };
            ammo = new Ammo(r, this.Size) { speed = 0 };
            powerUpTimer.Interval = r.Next(2000, 20000);
            powerUpTimer.Tick += new EventHandler(powerUpTimer_Tick);
            this.Icon = ((Icon)(resources.GetObject("App-package-games-arcade")));
            resetRec = new Rectangle(this.Size.Width - 200, this.ClientRectangle.Y + 20, 150, 50);
            background = ((Bitmap)(resources.GetObject("background")));
            ship = ((Bitmap)(resources.GetObject("spaceShip")));
            rec = new Rectangle(0, this.Height - 100, 50, 50);
            grafx = BufferedGraphicsManager.Current.Allocate(pictureBox1.CreateGraphics(), this.ClientRectangle);

            for (int x = 0; x < astroids.Length; x++)
                astroids[x] = new Astroid(r, this.Size, Level, diff);
            for (int x = 0; x < bulletsL.Length; x++)
            {
                bulletsL[x] = new Bullet("L", this.Size);
                bulletsR[x] = new Bullet("R", this.Size);
            }
            for (int x = 0; x < bullets.Length; x++)
                bullets[x] = new Bullet();
            explosound = new SoundPlayer((System.IO.Stream)resources.GetObject("Explosionn sound"));
            shotSound = new SoundPlayer((System.IO.Stream)resources.GetObject("Shooting sound"));
        }
        void powerUpTimer_Tick(object o, EventArgs e)
        {
            powerUpTimer.Interval = r.Next(2000, (20000 / Level) + (diff * 300));
            GC.Collect();
            if (Level >= 2 && r.Next(1, 4) == 2 &&
                (life.rec.Y > this.Size.Height || life.rec.Y <= -70))
                life = new Life(r, this.Size);
            if (Level >= 3 && r.Next(1, 3) == 2 && !sphere.alive &&
                (sphere.rec.Y > this.Size.Height || sphere.rec.Y <= -70))
                sphere = new Sphere(r, this.Size);
            if (Level >= 4 && r.Next(1, 4) == 2 && !ammo.alive &&
                (ammo.rec.Y > this.Size.Height || ammo.rec.Y <= -70))
                ammo = new Ammo(r, this.Size);
        }
        bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;
                if (paused)
                {
                    numericUpDown1.Show();
                    numericUpDown1.Focus();
                    label1.Show();
                    timer1.Enabled = false;
                    powerUpTimer.Enabled = false;
                    fps = 0;
                    draw();
                    Cursor.Hide();
                }
                else
                {
                    this.Focus();
                    numericUpDown1.Hide();
                    label1.Hide();
                    timer1.Enabled = true;
                    powerUpTimer.Enabled = true;
                    Cursor.Show();
                }
            }
        }
        public int Level
        {
            get { return level; }
            set
            {
                if (value != level)
                {
                    showingLevel = true;
                    level = value;
                }
            }
        }
        void ShowReset()
        {
            grafx.Graphics.FillRectangle(brush4, resetRec);
            ControlPaint.DrawBorder3D(grafx.Graphics, resetRec,Border3DStyle.Bump);
            grafx.Graphics.DrawString("Reset", gameoverFont, brush2, resetRec.Location);
            if (clicked)
            {
                score = 0; shotCount = 0; lifes = 3; Level = 1; levelcount = 0; lifecount = 0; bLcount = 0;
                goingRight = false; goingLeft = false; gointUp = false; goingDown = false; gameover = false;
                showingLevel = false; levelingUp = false; killed = false; clicked = false; paused = false;
                LevelUp();
                life = new Life(r, this.Size) { speed = 0 };
                sphere = new Sphere(r, this.Size) { speed = 0 };
                ammo = new Ammo(r, this.Size) { speed = 0 };
                rec = new Rectangle(0, this.Height - 100, 50, 50);
                grafx = BufferedGraphicsManager.Current.Allocate(pictureBox1.CreateGraphics(), this.ClientRectangle);
                background = ((Bitmap)(resources.GetObject("background")));
                ship = ((Bitmap)(resources.GetObject("spaceShip")));
                astroids = new Astroid[5];
                for (int x = 0; x < astroids.Length; x++)
                    astroids[x] = new Astroid(r, this.Size, Level, diff);
                for (int x = 0; x < bulletsL.Length; x++)
                {
                    bulletsL[x] = new Bullet("L", this.Size);
                    bulletsR[x] = new Bullet("R", this.Size);
                }
                bullets = new Bullet[(20 - (diff * 2)) + 1];
                for (int x = 0; x < bullets.Length; x++)
                    bullets[x] = new Bullet();
                Cursor.Hide();
            }
        }
        void LevelUp()
        {
            Cursor.Hide();
            switch (Level)
            {
                case 2:
                    background = ((Bitmap)(resources.GetObject("background2")));
                    break;
                case 3:
                    background = ((Bitmap)(resources.GetObject("background3"))); ;
                    break;
                case 4:
                    background = ((Bitmap)(resources.GetObject("background4")));
                    break;
                case 5:
                    background = ((Bitmap)(resources.GetObject("background5")));
                    break;
                case 6:
                    background = ((Bitmap)(resources.GetObject("background6")));
                    break;
                case 7:
                    background = ((Bitmap)(resources.GetObject("background7")));
                    break;
                case 8:
                    background = ((Bitmap)(resources.GetObject("background8")));
                    break;
                case 9:
                    background = ((Bitmap)(resources.GetObject("background9")));
                    break;
                case 10:
                    background = ((Bitmap)(resources.GetObject("background10")));
                    break;
            }
            int num = (20 / Level) - diff;
            if (num <= 0)
                num = 1;
            bullets = new Bullet[num];
            for (int x = 0; x < bullets.Length; x++)
                bullets[x] = new Bullet();
            astroids = new Astroid[5 * Level];
            for (int x = 0; x < astroids.Length; x++)
                astroids[x] = new Astroid(r, this.Size, Level, diff);
        }
        void Shoot()
        {
            //so you cannot shoot when paused
            if (Paused || !button1.IsDisposed || gameover)
                return;
            shotSound.Play();
            if (shotCount < bullets.Length)
            {
                bullets[shotCount].rec.Location = rec.Location;
                bullets[shotCount].rec.X += 20;
                shotCount++;
            }
            else
            {
                bullets[0].rec.Location = rec.Location;
                bullets[0].rec.X += 20;
                shotCount = 1;
            }
            if (ammo.alive)
            {
                if (bLcount < bulletsL.Length)
                {
                    bulletsL[bLcount].rec.Location = rec.Location;
                    bulletsL[bLcount].rec.Y += 20;
                    bulletsR[bLcount].rec.Location = rec.Location;
                    bulletsR[bLcount].rec.Y += 20;
                    bulletsR[bLcount].rec.X += 20;
                    bLcount++;
                }
                else
                {
                    bulletsL[0].rec.Location = rec.Location;
                    bulletsL[0].rec.Y += 20;
                    bulletsR[0].rec.Location = rec.Location;
                    bulletsR[0].rec.Y += 20;
                    bLcount = 1;
                }
            }
        }
        void Timer1Tick(object sender, EventArgs e)
        {
            if (goingRight)
                rec.X += 10 - diff;
            else if (goingLeft)
                rec.X -= 10 - diff;
            if (gointUp)
                rec.Y -= 10 - diff;
            else if (goingDown)
                rec.Y += 10 - diff;
            if (score >= 10 && score < 20)
                Level = 2;
            else if (score >= 20 && score < 35)
                Level = 3;
            else if (score >= 35 && score < 50)
                Level = 4;
            else if (score >= 50 && score < 70)
                Level = 5;
            else if (score >= 70 && score < 100)
                Level = 6;
            else if (score >= 100 && score < 150)
                Level = 7;
            else if (score >= 150 && score < 200)
                Level = 8;
            else if (score >= 200 && score < 260)
                Level = 9;
            else if (score >= 260)
                Level = 10;
            //to draw astrods
            for (int x = 0; x < astroids.Length; x++)
            {
                astroids[x].rec.Y += astroids[x].speed;
                if (astroids[x].rec.Y > this.Size.Height)
                {
                    astroids[x] = new Astroid(r, this.Size, Level, diff);
                    continue; //makes it a little faster
                }
                //To see if an asteriod got shot
                for (int y = 0; y < bullets.Length; y++)
                {
                    if (astroids[x].rec.IntersectsWith(bullets[y].rec) && astroids[x].alive)
                    {
                        astroids[x].alive = false;
                        explosound.Play();
                        astroids[x].speed = 0;
                        bullets[y] = new Bullet();
                        score++;
                    }
                }
                //if armmored check if shot
                if (ammo.alive)
                {
                    //for bullets going left
                    for (int y = 0; y < bulletsL.Length; y++)
                    {
                        if (astroids[x].rec.IntersectsWith(bulletsL[y].rec) && astroids[x].alive)
                        {
                            astroids[x].alive = false;
                            explosound.Play();
                            astroids[x].speed = 0;
                            bulletsL[y] = new Bullet("L", this.Size);
                            score++;
                        }
                        //for bullets going right
                        if (astroids[x].rec.IntersectsWith(bulletsR[y].rec) && astroids[x].alive)
                        {
                            astroids[x].alive = false;
                            explosound.Play();
                            astroids[x].speed = 0;
                            bulletsR[y] = new Bullet("R", this.Size);
                            score++;
                        }
                    }
                }
                if (astroids[x].rec.IntersectsWith(rec) && astroids[x].alive && !sphere.alive && !gameover)
                {
                    killed = true;
                    astroids[x].alive = false;
                    explosound.Play();
                    astroids[x].speed = 0;
                    lifes--;
                }
                //if astriod is not alive then show explosion
                if (!astroids[x].alive)
                {
                    astroids[x].count++;
                    if (astroids[x].count > 8)
                        astroids[x] = new Astroid(r, this.Size, Level, diff);
                    else
                    {
                        astroids[x].rec.Width += 6;
                        astroids[x].rec.Height = astroids[x].rec.Width;
                        astroids[x].rec.Y -= 3;
                        astroids[x].rec.X -= 3;
                        astroids[x].bitImage = ((Bitmap)(resources.GetObject("explosion")));
                    }
                }
            }
            // if killed then show explosion
            if (killed)
            {
                if (lifecount < 8)
                {
                    lifecount++;
                    ship = ((Bitmap)(resources.GetObject("explosion")));
                    rec.Height += 5;
                    rec.Width += 5;
                }
                else
                {
                    ship = ((Bitmap)(resources.GetObject("spaceShip")));
                    lifecount = 0;
                    rec.Height = 50;
                    rec.Width = 50;
                    killed = false;
                }
            }
            //to move ammo bulets
            if (ammo.alive)
            {
                for (int x = 0; x < bulletsL.Length; x++)
                {
                    bulletsL[x].rec.X -= bulletsL[x].speed;
                    bulletsR[x].rec.X += bulletsR[x].speed;
                }
            }
            else
            {
                for (int x = 0; x < bulletsL.Length; x++)
                {
                    bulletsL[x].rec.Y = this.Height + 100;
                    bulletsR[x].rec.Y = this.Height + 100;
                }
            }
            //to move bullets
            for (int x = 0; x < bullets.Length; x++)
            {
                bullets[x].rec.Y -= bullets[x].speed;
            }
            grafx.Graphics.DrawImage(ship, rec);
            //to show level up words
            if (showingLevel)
            {
                if (levelingUp)
                {
                    levelingUp = false;
                    LevelUp();
                }
                if (levelcount < 50)
                {
                    levelcount++;
                }
                else
                {
                    showingLevel = false;
                    levelcount = 0;
                }
            }
            else
                levelingUp = true;
            //to show life
            life.rec.Y += life.speed;
            if (life.rec.IntersectsWith(rec) && !gameover)
            {
                life = new Life(r, this.Size);
                life.speed = 0;
                lifes++;
            }
            //to show ammo
            if (ammo.alive)
            {
                ammo.rec = rec;
                ammo.rec.X += 10;
                ammo.rec.Y += 10;
                ammo.rec.Width = 25;
                ammo.rec.Height = 25;
            }
            else
                ammo.rec.Y += ammo.speed;
            if (ammo.count < 100)
            {
                if (ammo.rec.IntersectsWith(rec) && !gameover)
                {
                    ammo.count++;
                    ammo.alive = true;
                }
            }
            else
                ammo = new Ammo(r, this.Size) { speed = 0 };
            //to show sphere
            if (sphere.alive)
            {
                sphere.rec = rec;
                sphere.rec.X -= 10;
                sphere.rec.Y -= 10;
                sphere.rec.Width += 20;
                sphere.rec.Height += 20;
            }
            else
                sphere.rec.Y += sphere.speed;
            if (sphere.count < 200)
            {
                if (sphere.rec.IntersectsWith(rec) && !gameover)
                {
                    sphere.count++;
                    sphere.alive = true;
                    if (sphere.count > 180)
                    {
                        sphere.flashingNum++;
                        if (sphere.flashingNum < 3)
                            sphere.rec.Y = -100;
                        if (sphere.flashingNum > 6)
                            sphere.flashingNum = 0;
                    }
                }
            }
            else
                sphere = new Sphere(r, this.Size) { speed = 0 };
            int current = Environment.TickCount;
            if (current - start < 1000)
            {
                framecount++;
                current = Environment.TickCount;
            }
            else
            {
                start = Environment.TickCount;
                fps = framecount;
                framecount = 0;
            }
            draw();
        }
        void draw()
        {
            grafx.Graphics.DrawImage(background, 0, 0, this.Size.Width, this.Size.Height);
            for (int x = 0; x < astroids.Length; x++)
                grafx.Graphics.DrawImage(astroids[x].bitImage, astroids[x].rec);
            for (int x = 0; x < bullets.Length; x++)
                grafx.Graphics.DrawImage(bullets[x].bitImage, bullets[x].rec);
            grafx.Graphics.DrawImage(ship, rec);
            if (ammo.alive)
            {
                for (int x = 0; x < bulletsL.Length; x++)
                {
                    grafx.Graphics.DrawImage(bulletsL[x].bitImage, bulletsL[x].rec);
                    grafx.Graphics.DrawImage(bulletsR[x].bitImage, bulletsR[x].rec);
                }
            }
            if (showingLevel && levelcount < 50)
                grafx.Graphics.DrawString("Level:" + Level, gameoverFont, brush3, this.Width / 2 - 50, this.Height / 2 - 80);
            grafx.Graphics.DrawImage(life.bitStar, life.rec);
            grafx.Graphics.DrawImage(ammo.bitAmmo, ammo.rec);
            grafx.Graphics.DrawImage(sphere.bitSphere, sphere.rec);
            if (lifes < 0)
            {
                grafx.Graphics.DrawString("\tYou Lost!\nYour Final Score Was: " +
                  score, gameoverFont, brush2, (this.Width / 2) - 250, (this.Height / 2) - 100);
            }

            grafx.Graphics.DrawString("Score: " + score + "\nLevel: " + Level + "\nLifes left: " + lifes
                + "\n" + mode, scoreFont, Brushes.LightGreen, 0, 5);
            grafx.Graphics.DrawString("\nPress \"P\" to pause", scoreFont, Brushes.LightGreen, 0, this.Height - 100);
            grafx.Graphics.DrawString("\nFPS: " + fps, scoreFont, Brushes.LightGreen, this.Width - 120, this.Height - 100);
            if (Paused)
                grafx.Graphics.DrawString("\nPress \"P\" to resume", gameoverFont, brush3,
                    this.Width / 2 - 200, this.Height / 2 - 80);
            if (lifes < 0)
            {
                gameover = true;
                ShowReset();
            }
            grafx.Render();
        }
        void MainFormKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                goingRight = true;
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                goingLeft = true;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
                gointUp = true;
            else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
                goingDown = true;
        }
        void MainFormKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
                goingRight = false;
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
                goingLeft = false;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
                gointUp = false;
            else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
                goingDown = false;
        }
        void MainFormKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
                Shoot();
            if (e.KeyChar == 'P' || e.KeyChar == 'p')
            {
                if (Paused)
                {
                    Paused = false;
                }
                else
                {
                    Paused = true;
                    grafx.Graphics.DrawString("\nPress \"P\" to resume", gameoverFont, brush3, this.Width / 2 - 200, this.Height / 2 - 80);
                    grafx.Render();
                }
            }
        }
        void MainFormResize(object sender, EventArgs e)
        {
            resetRec = new Rectangle(this.Size.Width - 200, this.ClientRectangle.Y + 20, 150, 50);
            grafx = BufferedGraphicsManager.Current.Allocate(pictureBox1.CreateGraphics(), this.ClientRectangle);
            if (!button1.IsDisposed)
                button1.Location = new Point((this.Width / 2) - 50, (this.Height / 2) - 40);
            else
                draw();
            if (paused || !button1.IsDisposed)
            {
                numericUpDown1.Location = new Point((this.Width / 2) + 25, (this.Height / 2) + 20);
                label1.Location = new Point((this.Width / 2) - 90, (this.Height / 2) + 22);
            }
        }
        void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (paused)
                return;
            rec.X = e.X - 25;
            rec.Y = e.Y - 25;
            //to check if spaship is over reset button
            if (rec.IntersectsWith(resetRec) && gameover)
                brush4.Color = Color.Yellow;
            else
                brush4.Color = Color.Green;
        }
        void MainFormMouseClick(object sender, MouseEventArgs e)
        {
            if (rec.IntersectsWith(resetRec) && gameover)
                clicked = true;
            else
                Shoot();
        }
        void button1_Click(object sender, EventArgs e)
        {
            this.button1.Dispose();
            this.numericUpDown1.Hide();
            this.label1.Hide();
            this.Focus();
            Cursor.Hide();
            Paused = false;
        }
        void MainForm_LostFocus(object obj, EventArgs e)
        {
            if ((numericUpDown1.Focused || label1.Focused))
                return;
            Paused = true;
            grafx.Graphics.DrawString("Paused", gameoverFont, Brushes.Yellow, this.Width / 2 - 100, (this.Height / 2) - 120);
            grafx.Render();
            SystemSounds.Exclamation.Play();
        }
        void MainForm_GotFocus(object o, EventArgs e)
        {
            for (int x = 3; x > 0; x--)
            {
                if (!Paused)
                    return;
                SystemSounds.Exclamation.Play();
                grafx.Graphics.FillRectangle(Brushes.Green, this.Width / 2 - 200, this.Height / 2 - 80, 350, 50);
                grafx.Graphics.DrawString("RESUMMING IN: " + x, gameoverFont, brush3, this.Width / 2 - 200, this.Height / 2 - 80);
                grafx.Render();
                //so that the program does not appear frozen givign it a chance to so something every 200 milseconds
                timer1.Enabled = false;
                for (int y = 0; y <= 5; y++)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                }
                if (!this.Focused)
                    return;
            }
            Paused = false;
        }
        string mode = "Very Easy";
        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            //to hide the curser from view
            if (button1.IsDisposed&&!Paused)
                Cursor.Hide();
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            Cursor.Show();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (lifes<0)
                return;
            diff = (int)numericUpDown1.Value;
            switch (diff)
            {
                case 0:
                    mode = "Very Easy";
                    break;
                case 1:
                    mode = "Easy";
                    break;
                case 2:
                    mode = "Medium";
                    break;
                case 3:
                    mode = "Hard";
                    break;
                case 4:
                    mode = "Insane!";
                    break;
            }
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (button1.IsDisposed)
            {
                MainFormKeyPress(new object(), e);
                this.Focus();
            }
        }
    }
    class Ammo
    {
        public Rectangle rec;
        public Bitmap bitAmmo { get; set; }
        public int speed { get; set; }
        public int count { get; set; }
        public bool alive { get; set; }
        public Ammo(Random r, Size s)
        {
            bitAmmo = ((Bitmap)(MainForm.resources.GetObject("Ammo")));
            count = 0;
            alive = false;
            int witdth = s.Width - 50;
            int size = 30;
            speed = 5;
            rec = new Rectangle(r.Next(0, witdth), -70, size, size);
        }
    }
    class Astroid
    {
        public Rectangle rec;
        public Bitmap bitImage { get; set; }
        public int speed { get; set; }
        public int count { get; set; }
        public bool alive { get; set; }

        public Astroid(Random r, Size s, int l, int diff)
        {
            count = 0;
            alive = true;
            int witdth = s.Width;
            int size = r.Next(10, 80-(diff*10));
            if (r.Next(1, (40) / l) == 2)//to make a fast asteroid
            {
                rec = new Rectangle(r.Next(0, witdth), -size, size, size * 2);
                bitImage = ((Bitmap)(MainForm.resources.GetObject("astriod2")));
                speed = 25 + l + (diff * 3);
            }
            else
            {
                rec = new Rectangle(r.Next(0, witdth), -size, size, size);
                bitImage = ((Bitmap)(MainForm.resources.GetObject("astriod")));
                speed = r.Next(2, 10 + (diff * 3)) + l;
            }
        }
    }
    class Bullet
    {
        public Rectangle rec;
        public Bitmap bitImage { get; set; }
        public int speed { get; set; }

        public Bullet()
        {
            speed = 10;
            rec = new Rectangle(0, -20, 6, 10);
            bitImage = ((Bitmap)(MainForm.resources.GetObject("bullet")));
        }
        public Bullet(string arg, Size s)
        {
            speed = 10;
            rec = new Rectangle(0, s.Height + 100, 10, 6);
            bitImage = ((Bitmap)(MainForm.resources.GetObject("bullet")));
            if (arg == "L")
                bitImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
            else
                bitImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
        }
    }
    class Life
    {
        public Rectangle rec;
        public Bitmap bitStar { get; set; }
        public int speed { get; set; }
        public int count { get; set; }
        public Life(Random r, Size s)
        {
            bitStar = ((Bitmap)(MainForm.resources.GetObject("Star")));
            count = 0;
            int witdth = s.Width - 50;
            int size = 30;
            speed = 5;
            rec = new Rectangle(r.Next(0, witdth), -70, size, size);
        }
    }
    class Sphere
    {
        public Rectangle rec;
        public Bitmap bitSphere { get; set; }
        public int speed { get; set; }
        public int count { get; set; }
        public bool alive { get; set; }
        public int flashingNum { get; set; }
        public Sphere(Random r, Size s)
        {
            bitSphere = ((Bitmap)(MainForm.resources.GetObject("sphere")));
            count = 0;
            alive = false;
            int witdth = s.Width - 50;
            int size = 30;
            speed = 5;
            rec = new Rectangle(r.Next(0, witdth), -70, size, size);
        }
    }
}