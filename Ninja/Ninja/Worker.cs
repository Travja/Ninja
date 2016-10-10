using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ninja
{
    class Worker
    {

        int delay, cycles = 0;
        Action func;
        Thread thread;

        public Worker()
        {
            delay = 0;
            func = delegate { };
            this.thread = new Thread(new ThreadStart(delegate
            {
                for (int i = 0; i < delay; i++)
                {
                    if (Game1.exit)
                        break;
                    System.Threading.Thread.Sleep(1);
                    cycles++;
                }
                func.DynamicInvoke();
            }));
        }

        public Worker(int delay)
        {
            this.delay = delay;
            func = delegate { };
            this.thread = new Thread(new ThreadStart(delegate
            {
                for (int i = 0; i < delay; i++)
                {
                    if (Game1.exit)
                        break;
                    System.Threading.Thread.Sleep(1);
                }
                func.DynamicInvoke();
            }));
        }

        public Worker(int delay, Action func)
        {
            this.delay = delay;
            this.func = func;
            this.cycles = 0;
            this.thread = new Thread(new ThreadStart(delegate
            {
                for (int i = 0; i < delay; i++)
                {
                    if (Game1.exit)
                        break;
                    if (i % 2 == 0)
                        System.Threading.Thread.Sleep(1);
                    cycles++;
                }
                func.DynamicInvoke();
            }));
        }

        public Action Action
        {
            set { this.func = value; }
            get { return this.func; }
        }

        public int Delay
        {
            set { this.delay = value; }
            get { return this.delay; }
        }

        public Thread Thread
        {
            get { return thread; }
        }

        public void run()
        {
            thread.Start();
        }
    }
}
