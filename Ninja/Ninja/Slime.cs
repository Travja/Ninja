using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ninja
{
    class Slime
    {


        private Rectangle baseRec, source;
        private Texture2D image;
        private int movement = 1;
        private Object obj;
        private int health = 3;
        bool prevDir;
        bool dir;
        public Slime(Rectangle baseRec, Rectangle source, Texture2D image)
        {
            this.baseRec = baseRec;
            this.source = source;
            this.image = image;
            int rand = Game1.r.Next(0, 2);
            dir = rand == 1 ? true : false;
            prevDir = dir;
        }

        public int Health
        {
            set { health = value; }
            get { return health; }
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
        public void setPosition(int x, int y)
        {
            baseRec.X = x;
            baseRec.Y = y;
            obj.setLocation(x, y);
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

        int counter = 0;
        public void animate()
        {
            if (prevDir != dir || counter % 20 == 0)
            {
                if (counter % 20 == 0)
                {
                    if (source.X != 16 + (dir ? 0 : 32))
                        source.X = 16 + (dir ? 0 : 32);
                    else
                        source.X = 0 + (dir ? 0 : 32);
                }
                else
                {
                    source.X += (dir ? -1 : 1) * 32;
                }
            }
            prevDir = dir;
        }

        int steps = 0;
        bool ishurt = false, dead = false, alive = true;
        public void move()
        {
            if (!ishurt && alive)
            {
                int chance = Game1.r.Next(0, 100);
                if (steps >= 50)
                {
                    if (chance >= 75)
                        dir = !dir;
                    steps = 0;
                }

                if (counter % 2 == 0)
                    steps++;
                this.setPosition(baseRec.X + (counter % 2 == 0 ? (dir ? 1 : -1) : 0) * movement, baseRec.Y);

                animate();
                counter++;
            }
            obj.move();
        }

        public void hurt(int diff)
        {
            if (!ishurt)
            {
                ishurt = true;
                health--;
                int prevX = source.X;
                int x = 0;
                if (diff < 0)
                    x = 32;
                setSource(x, 16);
                new Worker(0, delegate
                {
                    for (int v = 0; v < 40; v++)
                    {
                        int change = (!(v >= 15 && v < 25) ? (v < 15 ? -1 : 1) : 0);
                        setPosition(baseRec.X + (diff >= 0 ? -3 : 3) * movement, baseRec.Y + change);
                        if (v % 15 == 0)
                            x += (x == 0 || x == 32) ? 16 : -16;
                        setSource(x, 16);
                        System.Threading.Thread.Sleep(17);
                    }
                    if (health == 0)
                    {
                        alive = false;
                        x = 0;
                        for (int v = 0; v < 60; v++)
                        {
                            if (v % 15 == 0)
                                x += 16;
                            setSource(x, 32);
                            System.Threading.Thread.Sleep(17);
                        }
                        dead = true;
                        for (int i = 0; i < Game1.r.Next(5000, 15000); i++)
                        {
                            if (Game1.exit)
                                return;
                            System.Threading.Thread.Sleep(1);
                        }
                        int xloc = Game1.r.Next(0, 2874);
                        int y = Game1.r.Next(0, 100) + 180;
                        setPosition(xloc, y);
                        dead = false;
                        for (int v = 0; v < 60; v++)
                        {
                            if (v % 15 == 0)
                                x -= 16;
                            setSource(x, 32);
                            System.Threading.Thread.Sleep(17);
                        }
                        alive = true;
                        health = 3;
                    }
                    ishurt = false;
                    setSource(prevX, 0);
                }).run();
            }
        }

        public void move(KeyboardState ks) { return; }

        public void setObject(Object obj)
        {
            this.obj = obj;
        }

        public Object getObject() { return obj; }

        public bool isDead() { return this.dead; }

        public int getHealth() { return health; }

        public bool isHurt() { return this.ishurt; }

    }
}
