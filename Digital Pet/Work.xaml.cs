﻿    using System;
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
    using System.IO;
    using System.Windows.Shapes;
    using System.Timers;
    using System.Threading;


    namespace SpaceInvaders
    {

        public partial class Work : Window
        {
            public System.Windows.Threading.DispatcherTimer GameTimer = new System.Windows.Threading.DispatcherTimer();
            public int ship_margin_rate = 4;
            public Random rnd = new Random();
            public bool isUserFiring;
            public List<System.Windows.Controls.Image> current_alien_bullets = new List<System.Windows.Controls.Image>();
            public List<int> current_allien_bullet_margins = new List<int> { };
            public double OriginalPlaneMargin;
           
            public Work()
            {
                InitializeComponent();
                GameTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 100);
                GameTimer.Tick += GameTimer_Tick;
                GameTimer.Start();
            }
            public void GameOver()
            {
                MessageBox.Show("Game Over","Game over", MessageBoxButton.OK);
                GameTimer.Stop();
            }
            public bool isHitting(UIElement item1, UIElement item2)
            {
                Point item1_point = item1.TransformToAncestor(WholeBoard).Transform(new Point(0, 0));
                double item1_upper_x = item1_point.X + item1.RenderSize.Width;
                double item1_upper_y = item1_point.Y + item1.RenderSize.Height;

                Point item2_point = item2.TransformToAncestor(WholeBoard).Transform(new Point(0, 0));
                double item2_point_upper_x = item2_point.X + item2.RenderSize.Width;
                double item2_point_lower_y = item2_point.Y - item2.RenderSize.Height;

                if (item2_point.X < item1_upper_x & item2_point_upper_x > item1_point.X)
                {
                    if (item2_point_lower_y < item1_point.Y & item2_point.Y > item1_point.Y)
                    {
                        return true;
                    }
                }
                return false;
            }
            public void Shoot(System.Windows.Controls.Image bullet, bool GoingUp, double OrignalMargin)
            {
                var margin_difference = new Thickness(OrignalMargin, bullet.Margin.Top - 4, 0, 0);
                if (GoingUp == true) { margin_difference = new Thickness(OrignalMargin, 0, 0, bullet.Margin.Bottom + 4); }
                if ((int)bullet.Margin.Top == -16 | bullet.Margin.Bottom == 16) 
                {
                  
                    ChangeRow(bullet, GoingUp); margin_difference = new Thickness(OrignalMargin,0,0,0); 
                }
                bullet.Margin = margin_difference;
               
            }
            public void ResetBullet(System.Windows.Controls.Image bullet, int row)
            {
                bullet.Visibility = Visibility.Collapsed;
                bullet.Margin = new Thickness(0);
                bullet.SetValue(Grid.RowProperty, row);
            }
            public void AlienBullet()
            {

                List<System.Windows.Controls.Image> alien_armoury = new List<System.Windows.Controls.Image> { AlienBullet1, AlienBullet2, AlienBullet3, AlienBullet4, AlienBullet5 };
                foreach (System.Windows.Controls.Image bullet in alien_armoury)
                {
                    if (rnd.Next(0, 50) == 15)
                    {
                        if (bullet.Visibility == Visibility.Collapsed)
                        {
                            current_alien_bullets.Add(bullet);
                            int random_margin = rnd.Next(0, (int)Math.Abs((AlienGrid.Margin.Left/2)));
                            random_margin = (int)AlienGrid.Margin.Left - random_margin;
                            if (random_margin % 2 == 0) { random_margin = random_margin + (int)AlienGrid.Margin.Left;}
                            current_allien_bullet_margins.Add(random_margin);
                            bullet.Margin = AlienGrid.Margin;
                            bullet.SetValue(Grid.RowProperty, AlienGrid.GetValue(Grid.RowProperty));
                            bullet.Visibility = Visibility.Visible;

                        }
                    }
                   
            }


            }
            public void ChangeRow(UIElement item, bool GoingUp)
            {
                int tmp = 1;
            if (GoingUp == true) { tmp = -1; }
                item.SetValue(Grid.RowProperty, (int)item.GetValue(Grid.RowProperty) + tmp);


            }
            public void MoveAliens()
            {
                AlienGrid.Margin = new Thickness(AlienGrid.Margin.Left + ship_margin_rate, 0, 0, 0);
                if ((int)AlienGrid.GetValue(Grid.RowProperty) == 4) { GameOver(); }
                if ((int)AlienGrid.Margin.Left == 160 || (int)AlienGrid.Margin.Left == 0 & ship_margin_rate < 0) { ChangeRow(AlienGrid, false); ship_margin_rate = ship_margin_rate * -1; }
            }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveAliens();
            AlienBullet();
            int index = 0;
            foreach (System.Windows.Controls.Image bullet in current_alien_bullets)
            {
                double random_margin = (double)current_allien_bullet_margins[index];
                Shoot(bullet, false, random_margin);
                bool PlaneShot = false;
                if ((int)bullet.GetValue(Grid.RowProperty) == 4) { PlaneShot = isHitting(bullet, Plane); ResetBullet(bullet, 0);}
                if (PlaneShot){ GameOver();}
                bool AlienShot = false;
                foreach (UIElement alien in AlienGrid.Children)
                {
                    if (alien.Visibility != Visibility.Collapsed)
                    {
                        AlienShot = isHitting(alien, Bullet);
                        if (AlienShot == true) { alien.Visibility = Visibility.Collapsed; ResetBullet(Bullet, 4); isUserFiring = false; }
                    }
                }
                index = index + 1;
            }
            if (isUserFiring == true)
            {
                if ((int)Bullet.GetValue(Grid.RowProperty) == 0 & Bullet.Margin.Bottom >= 15) { ResetBullet(Bullet, 4); isUserFiring = false; }
                Shoot(Bullet, true, OriginalPlaneMargin);
            }
            
        }

            private void Window_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Key == Key.Right)//If right arrow is pressed then move to the right by 2
                {
                    Plane.Margin = new Thickness(Plane.Margin.Left + 6, 0, 0, 0);
                }
                if (e.Key == Key.Left)
                {
                    Plane.Margin = new Thickness(Plane.Margin.Left - 6, 0, 0, 0);//If lft arrow is pressed then move to the left by 2
                }
                if (e.Key == Key.Space)
                {
                    if (isUserFiring == false)
                    {
                        isUserFiring = true;
                        Bullet.Margin = Plane.Margin;
                        OriginalPlaneMargin = Plane.Margin.Left;
                        Bullet.Visibility = Visibility.Visible;
                    }
               
                }
            }
        }

    }

