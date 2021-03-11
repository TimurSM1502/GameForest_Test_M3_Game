using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameForest_Test_M3_Game
{
    public partial class Level : Form
    {
        const int FieldSize = 8;
        int TimerCount;
        int elementSize;
        bool active;

        Bitmap[] bitmaps;
        VisualElement[,] elements;
        Game game;
        Timer timer;

        public enum ElementTypes
        {
            Red = 0,
            Orange = 1,
            Yellow = 2,
            Green = 3,
            Blue = 4
        }

        public Level() { InitializeComponent(); }

        //Выход в главное меню по кнопке ESC
        private void Level_KeyDown(object sender, KeyEventArgs e) 
        { 
            if (e.KeyCode == Keys.Escape && active) 
            {
                timer.Dispose();
                Close(); 
            } 
        }

        private void Level_Load(object sender, EventArgs e)
        {
            Paint += delegate { Field.Refresh(); };

            //Инициализация таймера на 60 секунд
            TimerCount = 60;
            timer = new Timer();
            timer.Interval = 1000;
            TimerLabel.Text = (TimerCount / 60).ToString("00") + ":" + (TimerCount - 60 * (TimerCount / 60)).ToString("00");
            timer.Tick += delegate
            {
                if (TimerCount > 0)
                {
                    TimerCount--;
                    TimerLabel.Text = (TimerCount / 60).ToString("00") + ":" + (TimerCount - 60 * (TimerCount / 60)).ToString("00");
                }
                //Завершение игры происходит только если она активна (завершены все анимации, все возможные комбинации засчитаны)
                else if (active == true) { Endgame(); }
            };

            //Инициализация игрового уровня
            game = new Game(FieldSize, FieldSize, Enum.GetValues(typeof(ElementTypes)).Length);
            game.ElementRemoved += Game_ElementRemoved;
            game.MatchesRemoved += Game_MatchesRemoved;
            game.ElementsFalled += Game_ElementsFalled;
            game.Fill();

            Field.Paint += Field_Paint;

            elementSize = Math.Min(Field.Width, Field.Height) / FieldSize;
            bitmaps = new Bitmap[20];
            elements = new VisualElement[FieldSize, FieldSize];

            UpdateBitmaps();
            InitElements();
            UpdateElements();
            Scorelabel.Text = "Score: " + game.GetScore().ToString();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            active = true;

            timer.Start();
        }

        private void Field_Paint(object sender, PaintEventArgs e)
        {
            for (int y = 0; y < FieldSize; y++)
            {
                for (int x = 0; x < FieldSize; x++)
                {
                    VisualElement el = elements[y, x];

                    if (el.BackColor != Color.Transparent)
                    {
                        SolidBrush b = new SolidBrush(el.BackColor);
                        e.Graphics.FillRectangle(b, el.Rectangle);
                    }

                    if (el.Image != null) { e.Graphics.DrawImage(el.Image, el.Rectangle); }
                }
            }
        }

        //Обработка событий
        //Удаление картинки при уничтожении элемента
        private void Game_ElementRemoved(int x, int y) { elements[y, x].Image = null; }

        //Вывод очков на экран и запуск процесса падения элементов
        private void Game_MatchesRemoved()
        {
            Scorelabel.Text = "Score: " + game.GetScore().ToString();
            game.Fall();
        }

        //Проверка на оставшиеся возможности для падения элементов (после обработки комбинации, появившейся из-за ручного перемещения элементов)
        private void Game_ElementsFalled(List<Position> positions)
        {
            FallAnimation anim = new FallAnimation(this);
            List<VisualElement> Elements = new List<VisualElement>();

            foreach (Position position in positions) { Elements.Add(elements[position.Y, position.X]); }

            anim.AnimationFinish += delegate
            {
                if (game.Fall()) { }
                else if (game.Remove() == false) { active = true; } 
            };

            UpdateElements();
            anim.Start(Elements);
        }

        //Завершение игры
        public void Endgame()
        {
            active = false;
            MessageBox.Show("GameOver. Your score is " + game.GetScore().ToString());
            Close();
        }

        //Обновление графических элементов (один раз при запуске)
        public void UpdateBitmaps()
        {
            if (elementSize == 0) { return; }

            Size s = new Size(elementSize, elementSize);
            bitmaps[0] = new Bitmap(Properties.Resources.Red_Circle, s);
            bitmaps[1] = new Bitmap(Properties.Resources.Orange_Circle, s);
            bitmaps[2] = new Bitmap(Properties.Resources.Yellow_Circle, s);
            bitmaps[3] = new Bitmap(Properties.Resources.Green_Circle, s);
            bitmaps[4] = new Bitmap(Properties.Resources.Blue_Circle, s);
            bitmaps[5] = new Bitmap(Properties.Resources.Red_Horizontal_Line_Bonus, s);
            bitmaps[6] = new Bitmap(Properties.Resources.Orange_Horizontal_Line_Bonus, s);
            bitmaps[7] = new Bitmap(Properties.Resources.Yellow_Horizontal_Line_Bonus, s);
            bitmaps[8] = new Bitmap(Properties.Resources.Green_Horizontal_Line_Bonus, s);
            bitmaps[9] = new Bitmap(Properties.Resources.Blue_Horizontal_Line_Bonus, s);
            bitmaps[10] = new Bitmap(Properties.Resources.Red_Vertical_Line_Bonus, s);
            bitmaps[11] = new Bitmap(Properties.Resources.Orange_Vertical_Line_Bonus, s);
            bitmaps[12] = new Bitmap(Properties.Resources.Yellow_Vertical_Line_Bonus, s);
            bitmaps[13] = new Bitmap(Properties.Resources.Green_Vertical_Line_Bonus, s);
            bitmaps[14] = new Bitmap(Properties.Resources.Blue_Vertical_Line_Bonus, s);
            bitmaps[15] = new Bitmap(Properties.Resources.Red_Bomb_Bonus, s);
            bitmaps[16] = new Bitmap(Properties.Resources.Orange_Bomb_Bonus, s);
            bitmaps[17] = new Bitmap(Properties.Resources.Yellow_Bomb_Bonus, s);
            bitmaps[18] = new Bitmap(Properties.Resources.Green_Bomb_Bonus, s);
            bitmaps[19] = new Bitmap(Properties.Resources.Blue_Bomb_Bonus, s);
        }

        //Инициализация клеток под элементы (один раз при запуске)
        public void InitElements()
        {
            for (int y = 0; y < FieldSize; ++y)
            {
                for (int x = 0; x < FieldSize; ++x)
                {
                    elements[y, x] = new VisualElement(this);
                    elements[y, x].Position = new Position(x, y);
                }
            }
        }

        //Обновление элементов на игровом поле
        public void UpdateElements()
        {
            Point start = new Point((Field.Width - elementSize * FieldSize) / 2, (Field.Height - elementSize * FieldSize) / 2);
            
            for (int y = 0; y < FieldSize; y++)
            {
                for (int x = 0; x < FieldSize; x++)
                {
                    elements[y, x].Location = new Point(start.X + x * elementSize, start.Y + y * elementSize);
                    elements[y, x].Size = new Size(elementSize, elementSize);
                    sbyte value = game.GetValue(x, y);
                    if (value >= 0) { elements[y, x].Image = bitmaps[value]; }
                    else { elements[y, x].Image = null; }
                }
            }
        }

        //Изменение размеров под полноэкранный режим
        private void Level_Resize(object sender, EventArgs e)
        {
            elementSize = Math.Min(Field.Width, Field.Height) / FieldSize;
            UpdateElements();
        }

        //Перемещение смежных элементов
        private void SwapElements(Position a, Position b)
        {
            VisualElement El_Clone = elements[a.Y, a.X];
            elements[a.Y, a.X] = elements[b.Y, b.X];
            elements[b.Y, b.X] = El_Clone;

            elements[a.Y, a.X].Position = new Position(a.X, a.Y);
            elements[b.Y, b.X].Position = new Position(b.X, b.Y);
        }

        //Анимация перемещения элементов
        public void MoveElements(VisualElement a, VisualElement b)
        {
            Position a_pos = new Position(a.Position);
            Position b_pos = new Position(b.Position);
            game.Swap(a_pos, b_pos);
            SwapElements(a_pos, b_pos);
            bool result = game.Remove();

            //Если в результате перемещения не возникает комбинация, то элементы возвращаются на исходные позиции
            if (result == false)
            {
                SwapAnimation Swap = new SwapAnimation(this);
                Swap.AnimationFinish += delegate
                {
                    a_pos = new Position(a.Position);
                    b_pos = new Position(b.Position);
                    game.Swap(a_pos, b_pos);
                    SwapElements(a_pos, b_pos);
                    active = true;
                };

                Swap.Start(b, a);
            }
        }

        //Выбор элемента
        private void Field_MouseDown(object sender, MouseEventArgs e)
        {
            Point start = new Point((Field.Width - elementSize * FieldSize) / 2, (Field.Height - elementSize * FieldSize) / 2);
            Point pos = new Point(e.Location.X - start.X, e.Location.Y - start.Y);

            int col = pos.X / elementSize;
            int row = pos.Y / elementSize;

            if (col < FieldSize && row < FieldSize) { elements[row, col].Click(); }
        }

        public class VisualElement
        {
            public VisualElement(Level lvl) : base() { level = lvl; }

            public void Click()
            {
                if (level.active == false) { return; }
                if (checkedElement != null)
                {
                    //Выделение первого выбранного элемента
                    if (checkedElement == this)
                    {
                        backColor = Color.Transparent;
                        checkedElement = null;
                    }
                    else
                    {
                        bool near = false;
                        
                        //Проверка второго выбранного элемента на соседство с первым
                        if ((checkedElement.Position.X == Position.X - 1 && checkedElement.Position.Y == Position.Y) ||
                           (checkedElement.Position.X == Position.X + 1 && checkedElement.Position.Y == Position.Y) ||
                           (checkedElement.Position.X == Position.X && checkedElement.Position.Y == Position.Y - 1) ||
                           (checkedElement.Position.X == Position.X && checkedElement.Position.Y == Position.Y + 1)) 
                           { near = true; }

                        if (near == false)
                        {
                            //Выбор другого элемента
                            checkedElement.backColor = Color.Transparent;
                            backColor = Color.DarkSlateBlue;
                            checkedElement = this;
                        }
                        else
                        {
                            //проверка двух соседних элементов
                            checkedElement.backColor = Color.Transparent;
                            level.active = false;
                            VisualElement el = checkedElement;
                            checkedElement = null;
                            SwapAnimation Swap = new SwapAnimation(level);
                            Swap.AnimationFinish += delegate { level.MoveElements(el, this); };
                            Swap.Start(el, this);
                        }
                    }
                }
                else
                {
                    //выбор элемента
                    backColor = Color.DarkSlateBlue;
                    checkedElement = this;
                }
                level.Refresh();
            }

            public Position Position { get { return position; } set { position = new Position(value); } }
            public ElementTypes Types { get { return types; } set { types = value; } }
            public Point Location { get { return rect.Location; } set { rect = new Rectangle(value, rect.Size); } }
            public Size Size { get { return rect.Size; } set { rect = new Rectangle(rect.Location, value); } }
            public Rectangle Rectangle { get { return rect; } set { rect = value; } }
            public Color BackColor { get { return backColor; } set { backColor = value; } }
            public Image Image { get { return image; } set { image = value; } }

            Position position;
            ElementTypes types;
            Level level;
            Rectangle rect;
            Color backColor;
            Image image;

            static VisualElement checkedElement = null;
        }
    }
}
