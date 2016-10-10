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
    public class Door
    {

        private Texture2D image;
        private Rectangle rec, source;
        private bool isopen = false, opening = false, closing = false;
        private Object obj;

        public Door(Texture2D image, Rectangle rec, Rectangle source)
        {
            this.image = image;
            this.rec = rec;
            this.source = source;
            obj = new Object(image, rec, ObjectType.DEFAULT);
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

        public bool Open
        {
            get { return isopen; }
        }

        public Object Object
        {
            get { return obj; }
        }

        public void open()
        {
            if (!opening && !isopen)
            {
                opening = true;
                new Worker(150, delegate
                {
                    source.X += 12;
                    new Worker(150, delegate
                    {
                        source.X += 12;
                        isopen = true;
                        opening = false;
                        new Worker(7000, close).run();
                    }).run();
                }).run();
            }
        }

        public void close()
        {
            if (!closing && isopen)
            {
                closing = true;
                new Worker(300, delegate
                {
                    source.X -= 12;
                    new Worker(300, delegate
                    {
                        source.X -= 12;
                        isopen = false;
                        closing = false;
                    }).run();
                }).run();
            }
        }

    }
}
