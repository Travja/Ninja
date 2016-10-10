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
    public class Object
    {
        private Texture2D tex;
        private List<Rectangle> recs = new List<Rectangle>();
        private ObjectType type;
        private Rectangle baseR = new Rectangle(0, 0, 0, 0);
        private Rectangle prevBack = new Rectangle(0, 0, 0, 0), source;
        private int width = 800;
        public Object(Texture2D tex, Rectangle rec, ObjectType type)
        {
            this.tex = tex;
            this.type = type;
            baseR = rec;
            Rectangle backRec = Game1.getBackRec();
            if (rec.X >= backRec.Width - width)
            {
                Rectangle tempRec = new Rectangle(-1 * backRec.Width + rec.X + 645 + backRec.X, rec.Y, rec.Width, rec.Height);
                recs.Add(tempRec);
            }
            else if (rec.X <= width)
            {
                Rectangle tempRec = new Rectangle(backRec.Width + rec.X - 645 + backRec.X, rec.Y, rec.Width, rec.Height);
                recs.Add(tempRec);
            }
            recs.Add(new Rectangle(rec.X + backRec.X, rec.Y, rec.Width, rec.Height));
            this.source = new Rectangle(0, 0, tex.Width, tex.Height);
        }

        public Object(Texture2D tex, Rectangle rec, Rectangle source, ObjectType type)
        {
            this.tex = tex;
            this.type = type;
            baseR = rec;
            Rectangle backRec = Game1.getBackRec();
            if (rec.X >= backRec.Width - width)
            {
                Rectangle tempRec = new Rectangle(-1 * backRec.Width + rec.X + 645 + backRec.X, rec.Y, rec.Width, rec.Height);
                recs.Add(tempRec);
            }
            else if (rec.X <= width)
            {
                Rectangle tempRec = new Rectangle(backRec.Width + rec.X - 645 + backRec.X, rec.Y, rec.Width, rec.Height);
                recs.Add(tempRec);
            }
            recs.Add(new Rectangle(rec.X + backRec.X, rec.Y, rec.Width, rec.Height));
            this.source = source;
        }

        public void setLocation(int x, int y)
        {
            Rectangle rec = recs.First<Rectangle>();
            List<Rectangle> temp = new List<Rectangle>();
            Rectangle backRec = Game1.getBackRec();
            if (x >= backRec.Width - width)
            {
                Rectangle tempRec = new Rectangle(-1 * backRec.Width + x + 645 + backRec.X, y, baseR.Width, baseR.Height);
                temp.Add(tempRec);
            }
            else if (x <= width)
            {
                Rectangle tempRec = new Rectangle(backRec.Width + x - 645 + backRec.X, y, baseR.Width, baseR.Height);
                temp.Add(tempRec);
            }
            baseR = new Rectangle(x + backRec.X, y, baseR.Width, baseR.Height);
            temp.Add(baseR);
            recs = temp;
            this.move();
        }

        public Texture2D getImage() { return tex; }
        public Rectangle[] getRectangles() { return recs.ToArray(); }

        public Rectangle getRectangleAt(int x, int y)
        {
            foreach (Rectangle rec in getRectangles())
            {
                if (rec.Contains(x, y))
                    return rec;
            }
            return new Rectangle(0, 0, 0, 0);
        }

        public void move()
        {
            Rectangle backRec = Game1.getBackRec();
            if (prevBack != null)
            {
                int diff = backRec.X - prevBack.X;
                move(diff);
                prevBack = backRec;
            }
            else
                prevBack = backRec;
        }

        int[] prev = new int[2] { -10000, -10000 };
        private void move(int x)
        {
            List<Rectangle> ret = new List<Rectangle>();
            int i = 0;
            foreach (Rectangle rec in recs)
            {
                Rectangle temp = rec;
                temp.X += x;

                int diff = (310 - baseR.Y) / 5;

                double xScale = ((double)baseR.Width / (baseR.Width + diff));
                double yScale = ((double)baseR.Height / (baseR.Height + diff));

                int prevHeight = temp.Height;
                int prevWidth = temp.Width;

                temp.Width = (int)Math.Round(baseR.Width * xScale);
                temp.Height = (int)Math.Round(baseR.Height * yScale);

                if (prev[i] != temp.Y)
                {
                    temp.X += prevWidth - temp.Width;
                    temp.Y += prevHeight - temp.Height;
                    prev[i] = temp.Y;
                }

                ret.Add(temp);
                i++;
            }
            recs = ret;
        }

        public ObjectType getType() { return type; }

    }
    public enum ObjectType
    {
        FLAN,
        SLIME,
        PLAYER,
        DEFAULT
    }
}
