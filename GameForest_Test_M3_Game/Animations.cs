using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameForest_Test_M3_Game
{
    public abstract class Animations
    {
        protected Timer timer;
        protected Level level;
        protected int time;
        protected int steps;
        protected int count;

        public Animations(Level lvl)
        {
            level = lvl;
            time = 250;
            steps = 8;
            count = 0;
        }

        public delegate void AnimationFinishHandler();
    }

    //Анимация перемещения двух смежных элементов
    public class SwapAnimation : Animations 
    {
        public SwapAnimation(Level lvl) : base(lvl) { steps = 8; }

        public void Start(Level.VisualElement a, Level.VisualElement b)
        {
            Point distance = new Point(b.Location.X - a.Location.X, b.Location.Y - a.Location.Y);
            Point speed = new Point(distance.X / steps, distance.Y / steps);
            int last = steps - 1;
            count = 0;

            timer = new Timer();
            timer.Interval = time / steps;
            timer.Tick += delegate
            {
                if (count > last)
                {
                    timer.Dispose();
                    return;
                }

                if (count < last)
                {
                    a.Location = new Point(a.Location.X + speed.X, a.Location.Y + speed.Y);
                    b.Location = new Point(b.Location.X - speed.X, b.Location.Y - speed.Y);
                }
                else
                {
                    a.Location = new Point(a.Location.X + (distance.X - speed.X * last), a.Location.Y + (distance.Y - speed.Y * last));
                    b.Location = new Point(b.Location.X - (distance.X - speed.X * last), b.Location.Y - (distance.Y - speed.Y * last));
                }

                count++;
                level.Refresh();
            };

            timer.Disposed += delegate { if (AnimationFinish != null) { AnimationFinish(); } };

            timer.Start();
        }

        public event AnimationFinishHandler AnimationFinish = null;
    }

    //Анимация падения элементов
    public class FallAnimation : Animations
    {
        public FallAnimation(Level lvl) : base(lvl) { time = 100; }

        public void Start(List<Level.VisualElement> elements)
        {
            int distance = elements.First().Size.Height;
            int speed = distance / steps;

            for (int j = 0; j < elements.Count; j++)
            {
                Level.VisualElement element = elements[j];
                element.Location = new Point(element.Location.X, element.Location.Y - distance);
            }

            int last = steps - 1;
            count = 0;
            timer = new Timer();
            timer.Interval = time / steps;
            timer.Tick += delegate
            {
                if (count > last)
                {
                    timer.Dispose();
                    return;
                }

                if (count < last)
                {
                    for (int j = 0; j < elements.Count; j++)
                    {
                        Level.VisualElement element = elements[j];
                        element.Location = new Point(element.Location.X, element.Location.Y + speed);
                    }
                }
                else
                {
                    int delta = distance - speed * last;

                    for (int j = 0; j < elements.Count; j++)
                    {
                        Level.VisualElement element = elements[j];
                        element.Location = new Point(element.Location.X, element.Location.Y + delta);
                    }
                }

                count++;
                level.Refresh();
            };

            timer.Disposed += delegate { if (AnimationFinish != null) { AnimationFinish(); } };

            level.Refresh();
            timer.Start();
        }

        public event AnimationFinishHandler AnimationFinish = null;
    }
}
