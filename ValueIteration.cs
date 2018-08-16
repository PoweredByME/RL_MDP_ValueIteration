using System;
using System.Linq;
namespace RL_MDP
{
    public class GridSpace
    {
        double?[,] matrix;
        bool[,] r_mask;
        int R, C;

        void init()
        {
            for (int c = 0; c < R; c++)
            {
                for (int c1 = 0; c1 < C; c1++)
                {
                    matrix[c, c1] = 0;
                    r_mask[c, c1] = false;
                }
            }
        }

        public GridSpace(int rows, int cols)
        {
            matrix = new double?[rows, cols];
            r_mask = new bool[rows, cols];
            R = rows;
            C = cols;
            init();
        }

        public GridSpace(GridSpace g)
        {
            matrix = new double?[g.Rows(), g.Cols()];
            r_mask = new bool[g.Rows(), g.Cols()];
            R = g.R;
            C = g.C;
            for (int c = 0; c < R; c++)
            {
                for (int c1 = 0; c1 < C; c1++)
                {
                    this.matrix[c, c1] = g.Value(c, c1);
                    this.r_mask[c, c1] = g.r_mask[c, c1];
                }
            }
        }

        public int Rows() { return R; }
        public int Cols() { return C; }

        public void Value(int row, int col, double? val)
        {
            this.matrix[row, col] = val;
            if (val == null)
            {
                this.r_mask[row, col] = true;
            }
        }

        public double? Value(int row, int col)
        {
            if (this.isBoundary(row, col) || this.isNoGo(row, col)) { return double.NegativeInfinity; }
            return this.matrix[row, col];
        }

        public void restrictValue(int row, int col, double? val)
        {
            this.matrix[row, col] = val;
            this.r_mask[row, col] = true;
        }

        public void NoGo(int row, int col)
        {
            this.Value(row, col, null);
        }

        public void Print()
        {
            Console.WriteLine("\n--------------------------------------\nThe Grid");
            for (int c = 0; c < R; c++)
            {
                for (int c1 = 0; c1 < C; c1++)
                {
                    if (matrix[c, c1] == null)
                    {
                        Console.Write("\tx");
                    }
                    else
                    {
                        Console.Write("\t" + string.Format("{0:N3}", matrix[c, c1]));
                    }
                }
                Console.WriteLine();
            }
        }

        public bool isBoundary(int row, int col)
        {
            if (row >= R || col >= C)
            {
                return true;
            }
            else if (row < 0 || col < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Restrict(int row, int col)
        {
            r_mask[row, col] = true;
        }

        public bool isRestricted(int row, int col)
        {
            return r_mask[row, col];
        }

        public bool isNoGo(int row, int col)
        {
            if (this.isRestricted(row, col) && this.matrix[row, col] == null)
            {
                return true;
            }
            return false;
        }

        public static bool compareAtThreshold(GridSpace g1, GridSpace g2, double threshold)
        {
            if (g1.Rows() == g2.Rows() && g1.Cols() == g2.Cols())
            {
                for (int c = 0; c < g1.Rows(); c++)
                {
                    for (int c1 = 0; c1 < g2.Cols(); c1++)
                    {
                        if (!g1.isNoGo(c, c1) && !g2.isNoGo(c, c1))
                        {
                            if (Math.Abs((double)g1.Value(c, c1) - (double)g2.Value(c, c1)) > threshold)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Grid sizes do not match");
            }
            return true;
        }

    }

    public delegate double? CostFunction(GridSpace grid, string current_action, int current_row, int current_col, double living_reward, double discount_value);

    public class ValueIteration
    {
        GridSpace grid;
        double discount_factor;
        double living_reward;
        string[] actions;
        CostFunction CF;

        public ValueIteration(GridSpace g, double df, double lr, string[] a, CostFunction cf)
        {
            grid = g;
            discount_factor = df;
            living_reward = lr;
            actions = a;
            CF = cf;
        }

        public GridSpace getGridSpace() { return grid; }

        public void Process()
        {
            for (int c = 0; c < grid.Rows(); c++)
            {
                for (int c1 = 0; c1 < grid.Cols(); c1++)
                {
                    if (!grid.isRestricted(c, c1))
                    {
                        double?[] temp = new double?[actions.Length];
                        for (int c2 = 0; c2 < actions.Length; c2++)
                        {
                            temp[c2] = CF(grid, actions[c2], c, c1, living_reward, discount_factor);
                        }
                        grid.Value(c, c1, temp.Max());
                        //grid.Print();
                    }
                }
            }
        }
    }

}