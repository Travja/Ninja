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
    public class Room
    {
        private int number;
        private Door entryDoor, door;
        private List<RoomObject> objects = new List<RoomObject>();
        private bool scroll;
        private Texture2D image;

        public Room(Texture2D image, int number, Door entryDoor, Door door, RoomObject[] objects, bool scroll)
        {
            this.image = image;
            this.number = number;
            this.entryDoor = entryDoor;
            this.door = door;
            this.objects = new List<RoomObject>(objects);
            this.scroll = scroll;
        }

        public bool Scroll
        {
            get { return scroll; }
        }

        public Texture2D Image
        {
            get { return image; }
        }

        public List<RoomObject> Objects
        {
            get { return objects; }
        }

        public Door EntryDoor
        {
            get { return entryDoor; }
        }

        public Door Door
        {
            get { return door; }
        }

        public int Number
        {
            get { return number; }
        }

        public void addObject(RoomObject rmobj)
        {
            objects.Add(rmobj);
        }
    }

    public class RoomObject
    {
        private Texture2D image;
        private Rectangle rec, source;
        private Position pos;

        public RoomObject(Texture2D image, Rectangle rec, Position pos = Position.DEFAULT)
        {
            this.image = image;
            this.rec = rec;
            this.source = new Rectangle(0, 0, image.Width, image.Height);
            this.pos = pos;
        }

        public RoomObject(Texture2D image, Rectangle rec, Rectangle source, Position pos = Position.DEFAULT)
        {
            this.image = image;
            this.rec = rec;
            this.source = source;
            this.pos = pos;
        }

        public Texture2D Image
        {
            get { return image; }
        }

        public Rectangle Rectangle
        {
            set { rec = value; }
            get { return rec; }
        }

        public Rectangle Source
        {
            set { source = value; }
            get { return source; }
        }

        public Position Position
        {
            get { return pos; }
        }
    }
}
