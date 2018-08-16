using System;
namespace RL_MDP
{
    class MainClass
    {

        static string[] actions = new string[] { "n", "s", "e", "w" };
        public static double? costFunction(GridSpace grid, string current_action, int current_row, int current_col, double living_reward, double discount_value)
        {
            double r = living_reward;
            double df = discount_value;
            int[] forward = new int[2];
            int[] left = new int[2];
            int[] right = new int[2];
            double pf = 0.8;
            double pl = 0.1;
            double pr = 0.1;

            if (current_action == "n")
            {
                forward = new int[2] { current_col, current_row - 1 };
                left = new int[2] { current_col - 1, current_row };
                right = new int[2] { current_col + 1, current_row };
            }

            else if (current_action == "s")
            {
                forward = new int[2] { current_col, current_row + 1 };
                left = new int[2] { current_col + 1, current_row };
                right = new int[2] { current_col - 1, current_row };
            }

            else if (current_action == "e")
            {
                forward = new int[2] { current_col + 1, current_row };
                left = new int[2] { current_col, current_row - 1 };
                right = new int[2] { current_col, current_row + 1 };
            }


            else if (current_action == "s")
            {
                forward = new int[2] { current_col - 1, current_row };
                left = new int[2] { current_col, current_row + 1 };
                right = new int[2] { current_col, current_row - 1 };
            }

            double? cal_f = 0, cal_l = 0, cal_r = 0;
            cal_f = calculateTransition(grid, forward, current_row, current_col, pf);
            cal_l = calculateTransition(grid, left, current_row, current_col, pl);
            cal_r = calculateTransition(grid, right, current_row, current_col, pr);
            return living_reward + discount_value * (cal_f + cal_l + cal_r);

        }

        public static double? calculateTransition(GridSpace grid, int[] direction, int cr, int cc, double pt)
        {
            if (grid.isBoundary(direction[0], direction[1]) || grid.isNoGo(direction[0], direction[1]))
            {
                return pt * grid.Value(cr, cc);
            }
            else
            {
                return pt * grid.Value(direction[0], direction[1]);
            }
        }

        public static PolicySpace IP(GridSpace g)
        {
            PolicySpace ps = new PolicySpace(g.Rows(), g.Cols());
            for (int c = 0; c < g.Rows(); c++)
            {
                for (int c1 = 0; c1 < g.Cols(); c1++)
                {
                    if (g.isNoGo(c, c1))
                    {
                        ps.matrix[c, c1] = "X";
                    }
                    else if (g.isRestricted(c, c1))
                    {
                        ps.matrix[c, c1] = g.Value(c, c1).ToString();
                    }
                    else
                    {
                        int[] n = new int[] { c - 1, c1 };
                        int[] s = new int[] { c + 1, c1 };
                        int[] e = new int[] { c, c1 + 1 };
                        int[] w = new int[] { c, c1 - 1 };

                        double? val_n = 0, val_s = 0, val_e = 0, val_w = 0;

                        val_n = g.Value(n[0], n[1]);
                        val_s = g.Value(s[0], s[1]);
                        val_e = g.Value(e[0], e[1]);
                        val_w = g.Value(w[0], w[1]);

                        if (compareVal_for_PS(val_n, val_s, val_w, val_e))
                        {
                            ps.matrix[c, c1] = "N";
                        }
                        else if (compareVal_for_PS(val_s, val_n, val_w, val_e))
                        {
                            ps.matrix[c, c1] = "S";
                        }
                        else if (compareVal_for_PS(val_w, val_s, val_n, val_e))
                        {
                            ps.matrix[c, c1] = "W";
                        }
                        else if (compareVal_for_PS(val_e, val_s, val_w, val_n))
                        {
                            ps.matrix[c, c1] = "E";
                        }
                        else
                        {
                            ps.matrix[c, c1] = "C";
                        }

                    }
                }
            }
            return ps;
        }

        public static bool compareVal_for_PS(double? target, double? o1, double? o2, double? o3)
        {
            return (
                target > o1 &&
                target > o2 &&
                target > o3
            );
        }

        public static void Main(string[] args)
        {
            int R = 6;
            int C = 6;

            GridSpace g = new GridSpace(R, C);
            GridSpace og = new GridSpace(R, C);

            g.restrictValue(1, 3, -10);
            g.restrictValue(2, 3, 20);

            g.NoGo(1, 1);
            g.NoGo(2, 1);
            g.NoGo(5, 5);
            g.Print();

            ValueIteration vi = new ValueIteration(g,0.9,-10.05, actions, costFunction);
            PolicyMaker pm = new PolicyMaker(vi, 0.000002, IP);
            pm.getBestPolicy(true).Print();

        }

    }
}