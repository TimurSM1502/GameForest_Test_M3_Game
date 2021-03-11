using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameForest_Test_M3_Game
{
    public class Position
    {
        int x_cord, y_cord;

        public Position()
        {
            x_cord = 0;
            y_cord = 0;
        }

        public Position(int x, int y)
        {
            x_cord = x;
            y_cord = y;
        }

        public Position(Position z)
        {
            x_cord = z.x_cord;
            y_cord = z.y_cord;
        }

        public int X
        {
            get { return x_cord; }
            set { x_cord = value; }
        }
        public int Y
        {
            get { return y_cord; }
            set { y_cord = value; }
        }
    }

    public class Line
    {
        Position start_pos, finish_pos;

        public Line()
        {
            start_pos = new Position();
            finish_pos = new Position();
        }

        public Line(Position start, Position finish)
        {
            start_pos = new Position(start);
            finish_pos = new Position(finish);
        }

        public Position Start
        {
            get { return start_pos; }
            set { start_pos = new Position(value); }
        }

        public Position Finish
        {
            get { return finish_pos; }
            set { finish_pos = new Position(value); }
        }
    }

    class Game
    {
        public readonly int Columns;
        public readonly int Rows;
        public readonly int Types;
        int Scores;
        sbyte[,] matrix;

        public delegate void ElementRemoveHandler(int x, int y);
        public event ElementRemoveHandler ElementRemoved;

        public delegate void MatchesRemoveHandler();
        public event MatchesRemoveHandler MatchesRemoved;

        public delegate void ElementsFallHandler(List<Position> elements);
        public event ElementsFallHandler ElementsFalled;

        public Game(int columns, int rows, int types)
        {
            Scores = 0;
            Columns = columns;
            Rows = rows;
            Types = types;
            matrix = new sbyte[Rows, Columns];
        }

        //Заполнение матрицы элементами (записывает коды графических элементов, по которым будут расставлены картинки)
        public void Fill()
        {
            Random rand = new Random();

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    sbyte Value = (sbyte)rand.Next(0, 5);
                    matrix[y, x] = Value;

                    if ((y >= 2) && (matrix[y, x] == matrix[y - 1, x] && matrix[y - 1, x] == matrix[y - 2, x]))
                    {
                        while (matrix[y, x] == matrix[y - 1, x])
                        {
                            Value = (sbyte)rand.Next(0, 5);
                            matrix[y, x] = Value;
                        }
                    }

                    if ((x >= 2) && (matrix[y, x] == matrix[y, x - 1] && matrix[y, x - 1] == matrix[y, x - 2]))
                    {
                        while (matrix[y, x] == matrix[y, x - 1])
                        {
                            Value = (sbyte)rand.Next(0, 5);
                            matrix[y, x] = Value;
                        }
                    }
                }
            }
        }

        //Поиск комбинаций и их удаление с поля
        public bool Remove()
        {
            List<Line> lines = new List<Line>();
            sbyte[,] matrix_clone = (sbyte[,])matrix.Clone();
            sbyte[,] empty_matrix = new sbyte[Rows, Columns]; //Нужна для добавления элементов-бонусов
            sbyte values;

            int hor_count, ver_count;

            //Поиск горизонтальных линий
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (matrix_clone[y, x] == -1) { continue; }

                    hor_count = 1;

                    for (int i = x + 1; i < Columns; i++)
                    {
                        if (matrix_clone[y, i] >= 0 && matrix_clone[y, i] <= 4)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[y, i] == matrix_clone[y, x]) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[y, i] == matrix_clone[y, x] - 5) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[y, i] == matrix_clone[y, x] - 10) { hor_count++; }
                                else { break; }
                            }
                            if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[y, i] == matrix_clone[y, x] - 15) { hor_count++; }
                                else { break; }
                            }
                        }
                        else if (matrix_clone[y, i] >= 5 && matrix_clone[y, i] <= 9)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[y, i] - 5 == matrix_clone[y, x]) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[y, i] - 5 == matrix_clone[y, x] - 5) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[y, i] - 5 == matrix_clone[y, x] - 10) { hor_count++; }
                                else { break; }
                            }
                            if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[y, i] - 5 == matrix_clone[y, x] - 15) { hor_count++; }
                                else { break; }
                            }
                        }
                        else if (matrix_clone[y, i] >= 10 && matrix_clone[y, i] <= 14)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[y, i] - 10 == matrix_clone[y, x]) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[y, i] - 10 == matrix_clone[y, x] - 5) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[y, i] - 10 == matrix_clone[y, x] - 10) { hor_count++; }
                                else { break; }
                            }
                            if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[y, i] - 10 == matrix_clone[y, x] - 15) { hor_count++; }
                                else { break; }
                            }
                        }
                        else if (matrix_clone[y, i] >= 15 && matrix_clone[y, i] <= 19)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[y, i] - 15 == matrix_clone[y, x]) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[y, i] - 15 == matrix_clone[y, x] - 5) { hor_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[y, i] - 15 == matrix_clone[y, x] - 10) { hor_count++; }
                                else { break; }
                            }
                            if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[y, i] - 15 == matrix_clone[y, x] - 15) { hor_count++; }
                                else { break; }
                            }
                        }
                    }

                    if (hor_count >= 3)
                    {
                        //Запоминаем, где нужно добавить бонус Line (горизонтальный)
                        if (hor_count == 4)
                        {
                            values = matrix_clone[y, x];

                            if (values == 0) { empty_matrix[y, x] = 5; }
                            else if (values == 1) { empty_matrix[y, x] = 6; }
                            else if (values == 2) { empty_matrix[y, x] = 7; }
                            else if (values == 3) { empty_matrix[y, x] = 8; }
                            else if (values == 4) { empty_matrix[y, x] = 9; }
                        }
                        //Запоминаем, где нужно добавить бонус Bomb
                        if (hor_count == 5)
                        {
                            values = matrix_clone[y, x];

                            if (values == 0) { empty_matrix[y, x] = 15; }
                            else if (values == 1) { empty_matrix[y, x] = 16; }
                            else if (values == 2) { empty_matrix[y, x] = 17; }
                            else if (values == 3) { empty_matrix[y, x] = 18; }
                            else if (values == 4) { empty_matrix[y, x] = 19; }
                        }
                        for (int i = x; i < x + hor_count; ++i) { matrix_clone[y, i] = -1; }    
                        
                        lines.Add(new Line(new Position(x, y), new Position(x + hor_count - 1, y)));
                    }
                }
            }

            matrix_clone = (sbyte[,])matrix.Clone();

            //Поиск вертикальных линий
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (matrix_clone[y, x] == -1) { continue; }

                    ver_count = 1;

                    for (int i = y + 1; i < Rows; i++)
                    {
                        if (matrix_clone[i, x] >= 0 && matrix_clone[i, x] <= 4)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[i, x] == matrix_clone[y, x]) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[i, x] == matrix_clone[y, x] - 5) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[i, x] == matrix_clone[y, x] - 10) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[i, x] == matrix_clone[y, x] - 15) { ver_count++; }
                                else { break; }
                            }
                        }
                        else if (matrix_clone[i, x] >= 5 && matrix_clone[i, x] <= 9)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[i, x] - 5 == matrix_clone[y, x]) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[i, x] - 5 == matrix_clone[y, x] - 5) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[i, x] - 5 == matrix_clone[y, x] - 10) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[i, x] - 5 == matrix_clone[y, x] - 15) { ver_count++; }
                                else { break; }
                            }
                        }
                        else if (matrix_clone[i, x] >= 10 && matrix_clone[i, x] <= 14)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[i, x] - 10 == matrix_clone[y, x]) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[i, x] - 10 == matrix_clone[y, x] - 5) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[i, x] - 10 == matrix_clone[y, x] - 10) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[i, x] - 10 == matrix_clone[y, x] - 15) { ver_count++; }
                                else { break; }
                            }
                        }
                        else if (matrix_clone[i, x] >= 15 && matrix_clone[i, x] <= 19)
                        {
                            if (matrix_clone[y, x] >= 0 && matrix_clone[y, x] <= 4)
                            {
                                if (matrix_clone[i, x] - 15 == matrix_clone[y, x]) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 5 && matrix_clone[y, x] <= 9)
                            {
                                if (matrix_clone[i, x] - 15 == matrix_clone[y, x] - 5) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 10 && matrix_clone[y, x] <= 14)
                            {
                                if (matrix_clone[i, x] - 15 == matrix_clone[y, x] - 10) { ver_count++; }
                                else { break; }
                            }
                            else if (matrix_clone[y, x] >= 15 && matrix_clone[y, x] <= 19)
                            {
                                if (matrix_clone[i, x] - 15 == matrix_clone[y, x] - 15) { ver_count++; }
                                else { break; }
                            }
                        }
                    }

                    if (ver_count >= 3)
                    {
                        //Запоминаем, где нужно добавить бонус Line (вертикальный)
                        if (ver_count == 4)
                        {
                            values = matrix_clone[y, x];

                            if (values == 0) { empty_matrix[y, x] = 10; }
                            else if (values == 1) { empty_matrix[y, x] = 11; }
                            else if (values == 2) { empty_matrix[y, x] = 12; }
                            else if (values == 3) { empty_matrix[y, x] = 13; }
                            else if (values == 4) { empty_matrix[y, x] = 14; }
                        }
                        //Запоминаем, где нужно добавить бонус Bomb
                        if (ver_count == 5)
                        {
                            values = matrix_clone[y, x];

                            if (values == 0) { empty_matrix[y, x] = 15; }
                            else if (values == 1) { empty_matrix[y, x] = 16; }
                            else if (values == 2) { empty_matrix[y, x] = 17; }
                            else if (values == 3) { empty_matrix[y, x] = 18; }
                            else if (values == 4) { empty_matrix[y, x] = 19; }
                        }
                        for (int i = y; i < y + ver_count; i++) { matrix_clone[i, x] = -1; }

                        lines.Add(new Line(new Position(x, y), new Position(x, y + ver_count - 1)));
                    }
                }
            }

            if (lines.Count == 0) { return false; }

            int baseValue = 10;

            //Удаление линий с игрового поля
            foreach (Line line in lines)
            {
                int count = 0;

                //Горизонтальные линии
                if (line.Start.Y == line.Finish.Y)
                {
                    for (int i = line.Start.X; i <= line.Finish.X; i++)
                    {
                        if (matrix[line.Start.Y, i] >= 0 && matrix[line.Start.Y, i] <= 4)
                        {
                            matrix[line.Start.Y, i] = -1;

                            if (ElementRemoved != null) { ElementRemoved(i, line.Start.Y); }

                            count++;
                        }
                        else if (matrix[line.Start.Y, i] >= 5 && matrix[line.Start.Y, i] <= 9)
                        {
                            for (int j = 0; j < Columns; j++)
                            {
                                matrix[line.Start.Y, j] = -1;

                                if (ElementRemoved != null) { ElementRemoved(j, line.Start.Y); }

                                count++;
                            }
                        }
                        else if (matrix[line.Start.Y, i] >= 10 && matrix[line.Start.Y, i] <= 14)
                        {
                            for (int j = 0; j < Rows; j++)
                            {
                                matrix[j, i] = -1;

                                if (ElementRemoved != null) { ElementRemoved(i, j); }

                                count++;
                            }
                        }
                        else if (matrix[line.Start.Y, i] >= 15 && matrix[line.Start.Y, i] <= 19)
                        {
                            if (line.Start.Y == 0 && i == 0)
                            {
                                for (int j = line.Start.Y; j < line.Start.Y + 1; j++)
                                {
                                    for (int m = i; m < i + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.Y == 0 && i == Columns)
                            {
                                for (int j = line.Start.Y; j < line.Start.Y + 1; j++)
                                {
                                    for (int m = i - 1; m < i; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.Y == Rows && i == Columns)
                            {
                                for (int j = line.Start.Y - 1; j < line.Start.Y; j++)
                                {
                                    for (int m = i - 1; m < i; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.Y == Rows && i == 0)
                            {
                                for (int j = line.Start.Y - 1; j < line.Start.Y; j++)
                                {
                                    for (int m = i; m < i + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.Y == 0 && (i > 0 && i < Columns))
                            {
                                for (int j = line.Start.Y; j < line.Start.Y + 1; j++)
                                {
                                    for (int m = i - 1; m < i + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.Y == Rows && (i > 0 && i < Columns))
                            {
                                for (int j = line.Start.Y - 1; j < line.Start.Y; j++)
                                {
                                    for (int m = i - 1; m < i + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if ((line.Start.Y > 0 && line.Start.Y < Rows) && i == 0)
                            {
                                for (int j = line.Start.Y - 1; j < line.Start.Y + 1; j++)
                                {
                                    for (int m = i; m < i + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if ((line.Start.Y > 0 && line.Start.Y < Rows) && i == Columns)
                            {
                                for (int j = line.Start.Y - 1; j < line.Start.Y + 1; j++)
                                {
                                    for (int m = i - 1; m < i; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if ((line.Start.Y > 0 && line.Start.Y < Rows) && (i > 0 && i < Columns))
                            {
                                for (int j = line.Start.Y - 1; j < line.Start.Y + 1; j++)
                                {
                                    for (int m = i - 1; m < i + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }
                //Вертикальные линии
                else
                {
                    for (int i = line.Start.Y; i <= line.Finish.Y; i++)
                    {
                        if (matrix[i, line.Start.X] >= 0 && matrix[i, line.Start.X] <= 4)
                        {
                            matrix[i, line.Start.X] = -1;

                            if (ElementRemoved != null) { ElementRemoved(line.Start.X, i); }

                            count++;
                        }
                        else if (matrix[i, line.Start.X] >= 5 && matrix[i, line.Start.X] <= 9)
                        {
                            for (int j = 0; j < Columns; j++)
                            {
                                matrix[i, j] = -1;

                                if (ElementRemoved != null) { ElementRemoved(j, line.Start.Y); }

                                count++;
                            }
                        }
                        else if (matrix[i, line.Start.X] >= 10 && matrix[i, line.Start.X] <= 14)
                        {
                            for (int j = 0; j < Rows; j++)
                            {
                                matrix[j, line.Start.X] = -1;

                                if (ElementRemoved != null) { ElementRemoved(i, j); }

                                count++;
                            }
                        }
                        else if (matrix[i, line.Start.X] >= 15 && matrix[i, line.Start.X] <= 19)
                        {
                            if (line.Start.X == 0 && i == 0)
                            {
                                for (int j = i; j < i + 1; j++)
                                {
                                    for (int m = line.Start.X; m < line.Start.X + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.X == 0 && i == Rows)
                            {
                                for (int j = i - 1; j < i; j++)
                                {
                                    for (int m = line.Start.X; m < line.Start.X + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.X == Columns && i == Rows)
                            {
                                for (int j = i - 1; j < i; j++)
                                {
                                    for (int m = line.Start.X - 1; m < line.Start.X; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.X == Columns && i == 0)
                            {
                                for (int j = i; j < i + 1; j++)
                                {
                                    for (int m = line.Start.X - 1; m < line.Start.X; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.X == 0 && (i > 0 && i < Rows))
                            {
                                for (int j = i - 1; j < i + 1; j++)
                                {
                                    for (int m = line.Start.X; m < line.Start.X + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if (line.Start.X == Rows && (i > 0 && i < Rows))
                            {
                                for (int j = i - 1; j < i + 1; j++)
                                {
                                    for (int m = line.Start.X - 1; m < line.Start.X; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if ((line.Start.X > 0 && line.Start.X < Rows) && i == 0)
                            {
                                for (int j = i; j < i + 1; j++)
                                {
                                    for (int m = line.Start.X - 1; m < line.Start.X + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if ((line.Start.X > 0 && line.Start.X < Rows) && i == Columns)
                            {
                                for (int j = i - 1; j < i; j++)
                                {
                                    for (int m = line.Start.X - 1; m < line.Start.X + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                            else if ((line.Start.X > 0 && line.Start.X < Rows) && (i > 0 && i < Columns))
                            {
                                for (int j = i - 1; j < i + 1; j++)
                                {
                                    for (int m = line.Start.X - 1; m < line.Start.X + 1; m++)
                                    {
                                        matrix[j, m] = -1;

                                        if (ElementRemoved != null) { ElementRemoved(m, j); }

                                        count++;
                                    }
                                }
                            }
                        }
                    }
                }

                //Набор очков
                Scores += count * baseValue;
            }

            //Расстановка элементов-бонусов
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    values = empty_matrix[y, x];

                    if (values >= 5) { matrix[y, x] = empty_matrix[y, x]; }
                }
            }

            if (MatchesRemoved != null) { MatchesRemoved(); }

            return true;
        }

        //Падение элементов (если под элементом есть пустая клетка)
        public bool Fall()
        {
            List<Position> elements = new List<Position>();

            for (int y = Rows - 2; y >= 0; y--)
            {
                for (int x = Columns - 1; x >= 0; x--)
                {
                    if (matrix[y + 1, x] == -1)
                    {
                        matrix[y + 1, x] = matrix[y, x];
                        matrix[y, x] = -1;
                        elements.Add(new Position(x, y + 1));
                    }
                }
            }

            Random random = new Random();
            for (int x = 0; x < Columns; x++)
            {
                if (matrix[0, x] == -1)
                {
                    matrix[0, x] = (sbyte)random.Next(0, Types);
                    elements.Add(new Position(x, 0));
                }
            }

            if (ElementsFalled != null && elements.Count != 0) { ElementsFalled(elements); }
            if (elements.Count == 0) { return false; }
            else { return true; }
        }

        public sbyte GetValue(int x, int y) { return matrix[y, x]; }

        //Ручное перемещение двух смежных элементов
        public void Swap(Position a, Position b)
        {
            sbyte Safe = matrix[a.Y, a.X];
            matrix[a.Y, a.X] = matrix[b.Y, b.X];
            matrix[b.Y, b.X] = Safe;
        }

        public int GetScore() { return Scores; }
    }
}