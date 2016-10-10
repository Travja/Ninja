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
    public class SpriteData
    {

        private Texture2D texture;
        private Rectangle rectangle, source = new Rectangle(-1, 0, 0, 0);
        private Color color;
        private Position pos = Position.DEFAULT;

        public SpriteData(Texture2D image, Rectangle rec, Color color)
        {
            Texture = image;
            Rectangle = rec;
            Color = color;
            Position = pos;
        }

        public SpriteData(Texture2D image, Rectangle rec, Rectangle source, Color color)
        {
            Texture = image;
            Rectangle = rec;
            SourceRectangle = source;
            Color = color;
        }

        public SpriteData(Texture2D image, Rectangle rec, Rectangle source, Color color, Position pos)
        {
            Texture = image;
            Rectangle = rec;
            SourceRectangle = source;
            Color = color;
            Position = pos;
        }

        public SpriteData(Texture2D image, Rectangle rec, Color color, Position pos)
        {
            Texture = image;
            Rectangle = rec;
            Color = color;
            Position = pos;
        }

        public Texture2D Texture
        {
            set { texture = value; }
            get { return texture; }
        }

        public Rectangle Rectangle
        {
            set { rectangle = value; }
            get { return rectangle; }
        }

        public Rectangle SourceRectangle
        {
            set { source = value; }
            get { return source; }
        }

        public Color Color
        {
            set { color = value; }
            get { return color; }
        }

        public Position Position
        {
            set { pos = value; }
            get { return pos; }
        }

    }

    public enum Position
    {
        BOTTOM,
        TOP,
        DEFAULT
    }
}
