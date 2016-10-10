using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Ninja
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Ninja ninja;
        static Slime[] slime = new Slime[10];

        Texture2D flan, back, speech, box, sword,
            hearts, gameover, banner, stamina, doorImg;
        Rectangle flanRec, boxRec, swordRec, restartRec;
        int flans;
        int kills;

        Door[] doors = new Door[2];
        private Door activeDoor;
        int entering = 0;

        SpriteFont bubbleFont;
        SpriteFont flanFont;

        bool gameOver = false;
        public static bool exit = false;

        private static Trigger[] activeTriggers = new Trigger[0];
        private static Dictionary<Trigger, Action[]> triggerMap = new Dictionary<Trigger, Action[]>();
        private static List<Object> objects = new List<Object>();

        private static Random rand = null;
        Room[] rooms = new Room[1];
        private static Room room;

        public static Random r
        {
            get { return rand; }
        }

        public static Room ActiveRoom
        {
            set { room = value; }
            get { return room; }
        }

        public Door ActiveDoor
        {
            set { activeDoor = value; }
            get { return activeDoor; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 360;
            Content.RootDirectory = "Content";
            rand = new Random();
        }

        public static Rectangle getBackRec() { return backRec; }

        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            addEvent(delegate
            {
                setBubbleText(600, new String[] {
                    "Hello there!",
                    "I'm Travja!",
                    "It's nice to see a new face!",
                    "",
                    "The game we'll be playing",
                    "today is simple:",
                    "Solve the puzzles",
                    "",
                    "Oh, and find flan!",
                    "Good luck!"
                });
                new Worker(10000, delegate
                {
                    setBubbleText(600, new String[] {
                        "Oh, and use 'A' and 'D'",
                        "to move, space to jump,",
                        "'E' is to attack!",
                        "Have fun!"
                    });
                }).run();
            }, Trigger.START);

            addEvent(delegate
            {
                setBubbleText(600, new String[] {
                    "Ah, see! Attacking isn't",
                    "that hard!",
                    "Now go find some enemies!"
                });
            }, Trigger.ATTACK);

            addEvent(delegate
            {
                setBubbleText(600, new String[] {
                    "Good job, you found flan!",
                    "This will heal a heart",
                    "if you are missing one",
                    "or, it will fill your",
                    "stamina!"
                });
            }, Trigger.FLAN);

            base.Initialize();
            this.startTask();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ninja = new Ninja(new Rectangle(280, 240, 99, 99), new Rectangle(0, 0, 16, 16), Content.Load<Texture2D>("Ninja"));
            flan = Content.Load<Texture2D>("Flan");
            flanRec = new Rectangle(r.Next(0, 2874), r.Next(0, 100)+235, 80, 25);
            back = Content.Load<Texture2D>("back");
            speech = Content.Load<Texture2D>("speech");

            bubbleFont = Content.Load<SpriteFont>("text");
            flanFont = Content.Load<SpriteFont>("text_big");

            box = Content.Load<Texture2D>("Box");
            boxRec = new Rectangle(10, 10, 48, 48);
            sword = Content.Load<Texture2D>("sword");
            swordRec = new Rectangle(10, 68, 48, 48);

            hearts = Content.Load<Texture2D>("heart");
            gameover = Content.Load<Texture2D>("gameover");
            banner = Content.Load<Texture2D>("Banner");
            stamina = Content.Load<Texture2D>("stamina");
            doorImg = Content.Load<Texture2D>("door");
            restartRec = new Rectangle(this.Window.ClientBounds.Width / 2 - 150, this.Window.ClientBounds.Height / 2 - 50, 299, 99);

            for (int t = 0; t < slime.Length; t++)
            {
                slime[t] = new Slime(new Rectangle(r.Next(500, 2374), r.Next(0, 100) + 180, 80, 80), new Rectangle(0, 0, 16, 16), Content.Load<Texture2D>("blob"));
                slime[t].setObject(new Object(slime[t].getImage(), slime[t].getRectangle(), slime[t].getSource(), ObjectType.SLIME));
            }

            doors[0] = new Door(doorImg, new Rectangle(1310, 148, 91, 112), new Rectangle(0, 0, 12, 16));
            doors[1] = new Door(doorImg, new Rectangle(800, 148, 91, 112), new Rectangle(0, 0, 12, 16));

            for (int t = 0; t < rooms.Length; t++)
            {
                rooms[t] = new Room(Content.Load<Texture2D>("room1"), t, doors[0], new Door(doorImg, new Rectangle(360-96, 290-128, 96, 128), new Rectangle(0, 0, 12, 16)), new RoomObject[0], false);
                rooms[t].addObject(new RoomObject(Content.Load<Texture2D>("blob"), new Rectangle(100, 100, 50, 50), new Rectangle(0, 0, 16, 16)));
            }

            addObject(flan, flanRec, ObjectType.FLAN);
        }

        protected override void UnloadContent()
        {
        }

        int counter = 0;
        bool hit = false;
        protected override void Update(GameTime gameTime)
        {
            KeyboardState board = Keyboard.GetState();
            if (board.IsKeyDown(Keys.Escape))
                this.Exit();


            MouseState mouse = Mouse.GetState();
            if (gameOver && mouse.LeftButton == ButtonState.Pressed && restartRec.Contains(mouse.X, mouse.Y))
            {
                restart();
            }

            if (!gameOver)
            {
                if (entering == 0)
                {
                    if (mouse.LeftButton == ButtonState.Pressed)
                    {
                        CheckClick(mouse);
                    }

                    CheckCollision();

                    foreach (Door door in doors)
                    {
                        if (door.Open && entering == 0)
                        {
                            Point pos = ninja.getRectangle().Center;
                            pos.Y += (int)((ninja.getRectangle().Bottom - pos.Y) / 1.25);
                            foreach (Rectangle rec in door.Object.getRectangles())
                            {
                                if (rec.Contains(pos))
                                {
                                    activeDoor = door;
                                    entering = 1;
                                }
                            }
                        }
                    }

                    if(ActiveRoom != null)
                    {
                        if (ActiveRoom.Door.Open && entering == 0)
                        {
                            Point pos = ninja.getRectangle().Center;
                            pos.Y += (int)((ninja.getRectangle().Bottom - pos.Y) / 1.25);
                            if (ActiveRoom.Door.Rectangle.Contains(pos))
                            {
                                activeDoor = ActiveRoom.Door;
                                entering = 1;
                            }
                        }
                    }
                }
                MoveNinja(board, ActiveRoom);
            }


            //if (ninja.getRectangle().Intersects(flanRec))
            //{
            //    flanRec.X = new Random().Next(0, 560);
            //    flanRec.Y = new Random().Next(0, 335);
            //    if (ninja.getRectangle().Width < 220)
            //        ninja.setSize((int)(ninja.getRectangle().Width * 1.1), (int)(ninja.getRectangle().Height * 1.1));
            //}

            counter++;

            base.Update(gameTime);
        }

        private void MoveNinja(KeyboardState board, Room rm)
        {
            if (board.IsKeyDown(Keys.E))
                ninja.attack();
            else if (board.IsKeyDown(Keys.Space))
                ninja.jump();

            if (board.IsKeyDown(Keys.LeftShift) && ninja.Stamina > 0)
                ninja.Sprinting = true;
            else
                ninja.Sprinting = false;

            ninja.move(board, rm);
        }

        private void CheckClick(MouseState mouse)
        {
            if (ActiveRoom == null)
            {
                Object obj = getObjectAt(mouse.X, mouse.Y);
                if (obj != null)
                {
                    if (obj.getType() == ObjectType.FLAN)
                    {
                        Rectangle rec = obj.getRectangleAt(mouse.X, mouse.Y);
                        if (rec.Height > 0)
                        {
                            obj.setLocation(r.Next(0, 2874), r.Next(0, 100) + 235);
                            flans++;
                            activateTrigger(Trigger.FLAN);
                            ninja.Health++;
                        }
                    }
                }

                foreach (Door door in doors)
                {
                    foreach (Rectangle rec in door.Object.getRectangles())
                    {
                        if (rec.Contains(mouse.X, mouse.Y))
                        {
                            if (!door.Open)
                                door.open();
                            else
                                door.close();
                        }
                    }
                }

            }
            else
            {

                if (ActiveRoom != null)
                {
                    if (ActiveRoom.Door.Rectangle.Contains(mouse.X, mouse.Y))
                    {
                        if (!ActiveRoom.Door.Open)
                            ActiveRoom.Door.open();
                        else
                            ActiveRoom.Door.close();
                    }
                }
            }
        }

        private void CheckCollision()
        {
            if (ActiveRoom != null)
                return;
            if (ninja.isAttacking())
            {
                if (counter % 20 == 0)
                    for (int t = 0; t < slime.Length; t++)
                        foreach (Rectangle rec in slime[t].getObject().getRectangles())
                        {
                            if (ninja.getRectangle().Intersects(rec) && !slime[t].isHurt())
                            {
                                slime[t].hurt(ninja.getRectangle().Center.X - rec.Center.X);
                                if (slime[t].getHealth() == 0)
                                    kills++;
                                //slime.setPosition(ninja.getRectangle().X, 100);
                            }
                        }
            }
            else
            {
                if (!hit)
                {
                    for (int t = 0; t < slime.Length; t++)
                        foreach (Rectangle rec in slime[t].getObject().getRectangles())
                        {
                            if (/*ninja.getRectangle().Intersects(rec) &&*/
                                intersects(ninja.getImage(), ninja.getRectangle(), ninja.getSource(),
                                slime[t].getImage(), rec, slime[t].getSource()) && !slime[t].isHurt() && !hit)
                            {
                                ninja.Health--;
                                hit = true;
                                new Worker(1500, delegate
                                {
                                    hit = false;
                                }).run();
                                if (ninja.Health == 0)
                                    gameOver = true;
                            }
                        }
                }
            }
        }

        private static Rectangle backRec = new Rectangle(0, 0, 2874, 360);
        private bool bubble = false;
        private int bubbleLive = 0;
        private List<String> text = new List<String>();
        private List<SpriteData> spriteData = new List<SpriteData>();
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            spriteBatch.Draw(back, backRec, Color.White);
            /*Rectangle shadow = ninja.getRectangle();
            shadow.X += 10;
            shadow.Y -= 10;
            spriteData.Add(new SpriteData(ninja.getImage(), shadow, ninja.getSource(), Color.Black * 0.5f));*/
            spriteData.Add(new SpriteData(ninja.getImage(), ninja.getRectangle(), ninja.getSource(), Color.White));
            if (ActiveRoom == null)
            {
                foreach (Object obj in objects)
                {
                    obj.move();
                    foreach (Rectangle rec in obj.getRectangles())
                        spriteData.Add(new SpriteData(obj.getImage(), rec, Color.White));
                }
                for (int t = 0; t < slime.Length; t++)
                    foreach (Rectangle rec in slime[t].getObject().getRectangles())
                        spriteData.Add(new SpriteData(slime[t].getImage(), rec, slime[t].getSource(), Color.White));

                foreach (Door door in doors)
                {
                    door.Object.move();
                    foreach (Rectangle rec in door.Object.getRectangles())
                        spriteData.Add(new SpriteData(door.Image, rec, door.Source, Color.White, Position.BOTTOM));
                }

                if (!gameOver && entering == 0)
                    for (int t = 0; t < slime.Length; t++)
                        slime[t].move();
            }

            DrawActiveRoom();

            for (int t = 0; t < 3; t++)
                spriteData.Add(new SpriteData(hearts, new Rectangle(t * 23 + 3, 337, 20, 20), new Rectangle((ninja.Health > t ? 0 : 7), 0, 7, 7), Color.White, Position.TOP));

            int width = (int)(((double)ninja.Stamina / (double)ninja.maxStamina) * 100);
            spriteData.Add(new SpriteData(stamina, new Rectangle(this.Window.ClientBounds.Width - 110, this.Window.ClientBounds.Height - 21, 100, 16),
                new Rectangle(0, 32, 100, 16), Color.White, Position.TOP));
            spriteData.Add(new SpriteData(stamina, new Rectangle(this.Window.ClientBounds.Width - 110, this.Window.ClientBounds.Height - 21, width, 16),
                new Rectangle(0, 0, width, 16), Color.White, Position.TOP));
            spriteData.Add(new SpriteData(stamina, new Rectangle(this.Window.ClientBounds.Width - 110, this.Window.ClientBounds.Height - 21, 100, 16),
                new Rectangle(0, 16, 100, 16), Color.White, Position.TOP));


            int prevCount = 0;
            int run = 0;
            while (spriteData.Count > 0)
            {
                //               if (run >= 1000)
                //                   break;
                run++;
                SpriteData sprite = null;
                int furthestY = 4000;
                foreach (SpriteData sd in spriteData)
                {
                    if (sd.Position == Position.BOTTOM || (sd.Rectangle.Y + sd.Rectangle.Height < furthestY && (sd.Position == Position.DEFAULT || prevCount == spriteData.Count)))
                    {
                        furthestY = sd.Rectangle.Y + sd.Rectangle.Height;
                        sprite = sd;
                    }
                }

                prevCount = spriteData.Count;

                if (sprite != null)
                {
                    if (sprite.SourceRectangle.X >= 0)
                        spriteBatch.Draw(sprite.Texture, sprite.Rectangle, sprite.SourceRectangle, sprite.Color);
                    else
                        spriteBatch.Draw(sprite.Texture, sprite.Rectangle, sprite.Color);
                    spriteData.Remove(sprite);
                }
            }

            if (bubble)
                drawBubble();
            else
                bubble = false;

            spriteBatch.Draw(box, boxRec, Color.White);
            spriteBatch.Draw(flan, new Rectangle(13, 30, 41, 12), Color.White);
            spriteBatch.DrawString(flanFont, flans.ToString(), new Vector2(65, boxRec.Center.Y - flanFont.MeasureString("0").Y / 2), Color.Black);
            spriteBatch.Draw(sword, swordRec, Color.White);
            spriteBatch.DrawString(flanFont, kills.ToString(), new Vector2(65, swordRec.Center.Y - flanFont.MeasureString("0").Y / 2), Color.Black);

            if (gameOver)
            {
                Rectangle bannerRec = restartRec;
                bannerRec.Height += 50;
                bannerRec.Width = this.Window.ClientBounds.Width;
                bannerRec.X = 0;
                bannerRec.Y -= 25;
                spriteBatch.Draw(banner, bannerRec, Color.White);
                spriteBatch.Draw(gameover, restartRec, Color.White);
            }

            if (entering > 0)
            {
                if (entering <= 100)
                {
                    spriteBatch.Draw(back, backRec, Color.Black * (float)(0.01 * entering));
                }
                else
                {
                    spriteBatch.Draw(back, backRec, Color.Black * (float)(0.01 * (300 - entering)));
                }
                entering++;
                if (entering == 101)
                {
                    ActiveDoor.close();
                    ActiveRoom = getRoom(ActiveDoor);
                    ninja.setPosition(320-ninja.Rectangle.Width/2, 230);
                    ActiveDoor = null;
                }
                if (entering == 301)
                    entering = 0;
            }

            spriteBatch.End();
            spriteData.Clear();

            base.Draw(gameTime);
        }

        private void DrawActiveRoom()
        {
            if (ActiveRoom != null)
            {
                spriteBatch.Draw(ActiveRoom.Image, new Rectangle(0, 0, 640, 360), Color.White);
                foreach (RoomObject rmobj in ActiveRoom.Objects)
                {
                    spriteData.Add(new SpriteData(ActiveRoom.Door.Image, ActiveRoom.Door.Rectangle, ActiveRoom.Door.Source, Color.White, Position.BOTTOM));
                    spriteData.Add(new SpriteData(rmobj.Image, rmobj.Rectangle, rmobj.Source, Color.White, rmobj.Position));
                }
            }
        }

        private void drawBubble()
        {
            Rectangle rec = new Rectangle(350, 0, 300, 300);
            spriteBatch.Draw(speech, rec, Color.White);


            int y = rec.Y + 37;


            //Draw the text in the bubble. The max lines is 11.
            int cap = 11;
            for (int t = 0; t < (text.ToArray().Length >= cap ? cap : text.ToArray().Length); t++)
            {
                String str = text.ToArray()[t];
                Vector2 FontPos = new Vector2(rec.X + 25, y);
                spriteBatch.DrawString(bubbleFont, str, FontPos, Color.Black);
                y += 15;
            }

            bubbleLive--;
            if (bubbleLive <= 0)
                bubble = false;
        }

        private void drawObject(Object obj)
        {
            Rectangle[] recs = obj.getRectangles();
            Texture2D image = obj.getImage();
            foreach (Rectangle rec in recs)
            {
                spriteBatch.Draw(image, rec, Color.White);
            }
        }

        public static void addObject(Texture2D tex, Rectangle rec, ObjectType type)
        {
            objects.Add(new Object(tex, rec, type));
        }

        public static void addObject(Object obj)
        {
            objects.Add(obj);
        }

        public Object getObjectAt(int x, int y)
        {
            foreach (Object obj in objects)
            {
                foreach (Rectangle rec in obj.getRectangles())
                {
                    if (rec.Contains(x, y))
                        return obj;
                }
            }
            return null;
        }

        public static void addBackX(int add)
        {
            backRec.X -= add;
            if (backRec.X > 0)
                backRec.X = -1 * backRec.Width + 640 - 3 * add;
            if (backRec.X < -1 * backRec.Width + 640)
                backRec.X = -3 * add;

            foreach (Object obj in objects)
            {
                obj.move();
            }

            for (int t = 0; t < slime.Length; t++)
                slime[t].getObject().move();
        }

        //Max string length per line is about 27 characters
        public void setBubbleText(int time, String[] text)
        {
            this.text.Clear();
            for (int t = 0; t < text.Length; t++)
                this.text.Add(text[t]);
            bubble = true;
            bubbleLive = time;
        }



        private static System.Array ResizeArray(System.Array oldArray, int newSize)
        {
            int oldSize = oldArray.Length;
            System.Type elementType = oldArray.GetType().GetElementType();
            System.Array newArray = System.Array.CreateInstance(elementType, newSize);

            int preserveLength = System.Math.Min(oldSize, newSize);

            if (preserveLength > 0)
                System.Array.Copy(oldArray, newArray, preserveLength);

            return newArray;
        }




        //Run this ONLY ONCE
        int count = 0;
        private void startTask()
        {
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(
                delegate
                {
                    bool run = true;
                    while (run)
                    {
                        count++;
                        if (exit)
                        {
                            run = false;
                            break;
                        }

                        if (count % 400 == 0)
                            rand = new Random();

                        for (int t = 0; t < activeTriggers.Length; t++)
                        {
                            Trigger trig = activeTriggers[t];
                            switch (trig)
                            {
                                case Trigger.START:
                                    activateTrigger(trig);
                                    break;
                                default:
                                    break;
                            }
                        }
                        System.Threading.Thread.Sleep((count % 3 == 0 ? 17 : 16));
                    }
                }
            ));
            thread.Start();

        }

        public static void addEvent(Action func, Trigger trig)
        {
            addTrigger(trig);
            if (!triggerMap.ContainsKey(trig))
                triggerMap.Add(trig, new Action[1]);
            else
                triggerMap[trig] = (Action[]) ResizeArray(triggerMap[trig], triggerMap[trig].Length + 1);
            triggerMap[trig][triggerMap[trig].Length - 1] = func;
        }

        private static void addTrigger(Trigger trig)
        {
            activeTriggers = (Trigger[]) ResizeArray(activeTriggers, activeTriggers.Length + 1);
            activeTriggers[activeTriggers.Length - 1] = trig;
        }

        public static void activateTrigger(Trigger trig)
        {
            if (!triggerMap.ContainsKey(trig))
                return;
            for (int t = 0; t < triggerMap[trig].Length; t++)
            {
                triggerMap[trig][t].DynamicInvoke();
            }

            triggerMap[trig] = new Action[0];

            List<Trigger> tmp = new List<Trigger>(activeTriggers);
            tmp.Remove(trig);
            activeTriggers = tmp.ToArray();
        }
        



        private void restart()
        {
            flans = 0;
            kills = 0;
            Random r = new Random();
            foreach (Object obj in objects)
                if(obj.getType()== ObjectType.FLAN)
                    obj.setLocation(r.Next(0, 2874), r.Next(0, 100) + 235);

            for (int t = 0; t < slime.Length; t++)
            {
                slime[t].setPosition(r.Next(500, 2374), r.Next(0, 100) + 180);
                slime[t].Health = 3;
            }

            backRec.X = 0;

            ninja.Health = 3;
            ninja.Stamina = ninja.maxStamina;
            gameOver = false;
        }

        private bool intersects(Texture2D t1, Rectangle r1, Rectangle s1, Texture2D t2, Rectangle r2, Rectangle s2)
        {
            if (!r1.Intersects(r2))
                return false;

            Color[] c1 = new Color[(s1.Width) * (s1.Height)];
            if (s1.X >= t1.Width)
                return false;
            t1.GetData<Color>(0, s1, c1, 0, c1.Length);
            Color[] c2 = new Color[(s2.Width) * (s2.Height)];
            if (s2.X >= t2.Width)
                return false;
            t2.GetData<Color>(0, s2, c2, 0, c2.Length);

            // Find the bounds of the rectangle intersection
            int x1 = Math.Max(r1.Left, r2.Left);
            int x2 = Math.Min(r1.Right, r2.Right);
            int y1 = Math.Max(r1.Top, r2.Top);
            int y2 = Math.Min(r1.Bottom, r2.Bottom);

            // For each single pixel in the intersecting rectangle
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    int l1 = (int)(((x - r1.Left) * ((double)s1.Width / r1.Width)) +
                                 (int)((y - r1.Top) * ((double)s1.Width / r1.Width)) * s1.Width);
                    int l2 = (int)(((x - r2.Left) * ((double)s2.Width / r2.Width)) +
                                 (int)((y - r2.Top) * ((double)s2.Width / r2.Width)) * s2.Width);
                    if (l1 < 0 || l1 >= c1.Length || l2 < 0 || l2 >= c2.Length)
                        break;
                    Color a = c1[l1];
                    Color b = c2[l2];

                    if (a.A != 0 && b.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
                    {
                        return true;
                    }
                }
            }
            // If no collision occurred by now, we're clear.
            return false;
        }

        public Room getRoom(Door door)
        {
            foreach (Room rm in rooms)
            {
                if (rm.EntryDoor == door)
                    return rm;
                if (rm.Door == door)
                    return null;
            }
            return null;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            exit = true;
            base.OnExiting(sender, args);
        }

    }
}
