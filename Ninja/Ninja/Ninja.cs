using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Ninja
{
    class Ninja
    {
        private Rectangle baseRec, source;
        private Texture2D image;
        private int movement = 2;
        private int health = 3;
        private bool sprinting = false;
        private int stamina = 400;
        public int maxStamina = 400;
        public Ninja(Rectangle baseRec, Rectangle source, Texture2D image)
        {
            this.baseRec = baseRec;
            this.source = source;
            this.image = image;
        }

        public Rectangle Rectangle
        {
            get { return baseRec; }
        }

        public int Health
        {
            set
            {
                if (value <= 3)
                    health = value;
                else
                {
                    int t = 0;
                    new Worker(0, delegate
                    {
                        while (t < 100)
                        {
                            if (stamina >= maxStamina)
                            {
                                t = 100;
                                return;
                            }
                            stamina += 4;
                            System.Threading.Thread.Sleep(17);
                            t++;
                        }
                    }).run();
                }
            }
            get { return health; }
        }

        public bool Sprinting
        {
            set { sprinting = value; }
            get { return sprinting; }
        }

        public int Stamina
        {
            set { stamina = value; }
            get { return stamina; }
        }

        public void setSize(int x, int y)
        {
            baseRec.Width = x;
            baseRec.Height = y;
        }
        public void setSourceSize(int x, int y)
        {
            source.Width = x;
            source.Height = y;
        }
        public Rectangle getRectangle()
        {
            return this.baseRec;
        }
        int runs = 0;
        public void setPosition(int x, int y)
        {
            int prevX = baseRec.X;
            baseRec.X = x;
            bool dontSet = false;
            if (baseRec.Y == y)
                dontSet = true;

            if (Game1.ActiveRoom == null)
            {
                if ((baseRec.Right >= 640 && baseRec.X > prevX) || (baseRec.Left <= 0 && baseRec.X < prevX))
                    baseRec.X = prevX;
            }
            else
            {
                //if (baseRec.Right >= 640 - ((baseRec.Right - 640) * 5) && baseRec.X > prevX)
                //    baseRec.X = prevX;
            }

            if (Game1.ActiveRoom != null && y + baseRec.Height < 290 && y < baseRec.Y)
                dontSet = true;

            if (y + baseRec.Height < 260 && y < baseRec.Y)
                dontSet = true;
            if (y + baseRec.Height > 360 && y > baseRec.Y)
                dontSet = true;

            if (!dontSet)
            {
                bool change = Math.Abs(baseRec.Y - y) >= 2;
                if (runs % 5 == 0 || change)
                {
                    if (y > baseRec.Y)
                    {
                        baseRec.X -= y - baseRec.Y;
                        baseRec.Width += (y - baseRec.Y) * 2 / (change ? 5 : 1);
                        baseRec.Height += (y - baseRec.Y) * 2 / (change ? 5 : 1);
                    }
                    else
                    {
                        baseRec.X += baseRec.Y - y;
                        baseRec.Width -= (baseRec.Y - y) * 2 / (change ? 5 : 1);
                        baseRec.Height -= (baseRec.Y - y) * 2 / (change ? 5 : 1);
                    }
                }
                baseRec.Y = y;
                runs++;
            }
        }
        public void setSource(int x, int y)
        {
            source.X = x;
            source.Y = y;
        }
        public Rectangle getSource()
        {
            return this.source;
        }
        public void setImage(Texture2D image)
        {
            this.image = image;
        }
        public Texture2D getImage()
        {
            return this.image;
        }

        private int counter;
        private int i = 0;
        private Keys last;
        private bool active;
        private int blink;
        private Keys lockKey;
        private bool locked;
        private bool attacking;
        public void animate()
        {
            if (!attacking)
            {
                if (!active)
                {
                    if (blink == 0)
                        blink = new Random().Next(360, 720);
                    this.setSource(0, 0);
                    i = 0;
                }
                else
                    blink = 0;

                if (blink == 0)
                {
                    this.setSource(16 * ((int)(i / 15)), source.Y);
                    i++;
                    if (i > 59)
                        i = 0;
                }

                if (blink > 0)
                {
                    blink--;
                    if (blink <= 40 && blink > 25)
                        this.setSource(16, 0);
                    else if (blink <= 25 && blink > 15)
                        this.setSource(32, 0);
                    else if (blink <= 15)
                        this.setSource(48, 0);
                }
            }
        }
        public void move(KeyboardState ks, Room room)
        {
            if (!attacking)
            {
                Keys key = last;
                if (ks.IsKeyDown(Keys.D))
                {
                    this.setSource(source.X, 32);
                    key = Keys.D;
                    active = true;
                }
                else if (ks.IsKeyDown(Keys.A))
                {
                    this.setSource(source.X, 48);
                    key = Keys.A;
                    active = true;
                }
                else if (ks.IsKeyDown(Keys.S))
                {
                    this.setSource(source.X, 16);
                    key = Keys.S;
                    active = true;
                }
                else if (ks.IsKeyDown(Keys.W))
                {
                    this.setSource(source.X, 64);
                    key = Keys.W;
                    active = true;
                }
                else
                {
                    this.setSource(0, 0);
                    active = false;
                }
                counter++;

                if (active && sprinting && stamina >= 0)
                    stamina -= 4;
                else if (!active && stamina < maxStamina)
                    stamina++;
                else if (active && stamina < maxStamina && counter % 10 == 0)
                    stamina++;

                if (stamina == 0)
                    sprinting = false;


                setPosition(ks, room);

                if (!key.Equals(last))
                {
                    last = key;
                }
                this.animate();
            }
        }

        private void setPosition(KeyboardState ks, Room room)
        {
            if (ks.IsKeyDown(Keys.D))
            {
                if (room != null)
                    this.setPosition(baseRec.X + movement, baseRec.Y);
                else
                    Game1.addBackX(movement * (sprinting && !attacking ? 2 : 1));
            }
            else if (ks.IsKeyDown(Keys.A))
            {
                if (room != null)
                    this.setPosition(baseRec.X - movement, baseRec.Y);
                else
                    Game1.addBackX(-1 * movement * (sprinting && !attacking ? 2 : 1));
            }
            else if (ks.IsKeyDown(Keys.S))
            {
                this.setPosition(baseRec.X, baseRec.Y + movement/ 2 * (sprinting && !attacking ? 2 : 1));
            }
            else if (ks.IsKeyDown(Keys.W))
            {
                this.setPosition(baseRec.X, baseRec.Y - movement/ 2 * (sprinting && !attacking ? 2 : 1));
            }
        }

        private void setPosition(Keys key, Room room)
        {
            if (key.Equals(Keys.D))
            {
                if (room != null)
                    this.setPosition(baseRec.X + movement, baseRec.Y);
                else
                    Game1.addBackX(movement * (sprinting && !attacking ? 2 : 1));
                
            }
            else if (key.Equals(Keys.A))
            {
                if (room != null)
                    this.setPosition(baseRec.X - movement, baseRec.Y);
                else
                    Game1.addBackX(-1 * movement * (sprinting && !attacking ? 2 : 1));
            }
            else if (key.Equals(Keys.S))
            {
                this.setPosition(baseRec.X, baseRec.Y + movement * (sprinting && !attacking ? 2 : 1));
            }
            else if (key.Equals(Keys.W))
            {
                this.setPosition(baseRec.X, baseRec.Y - movement * (sprinting && !attacking ? 2 : 1));
            }
        }


        public void attack()
        {
            if (!attacking)
            {
                attacking = true;
                if (!last.Equals(Keys.D) && !last.Equals(Keys.A))
                    last = Keys.D;

                Game1.activateTrigger(Trigger.ATTACK);

                new Worker(0, delegate
                {
                    for (int v = 0; v < 39; v++)
                    {
                        this.setSource(16 * ((int)v / 10), (last.Equals(Keys.D) ? 80 : 96));
                        if ((int)(v / 10) < 2)
                            this.setPosition(last, Game1.ActiveRoom);
                        System.Threading.Thread.Sleep(17);
                    }
                    attacking = false;
                }).run();
            }
        }

        public void jump()
        {

        }

        public bool isAttacking() { return attacking; }

    }
}
