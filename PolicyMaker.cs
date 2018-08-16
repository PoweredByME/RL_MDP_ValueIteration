using System;
namespace RL_MDP
{
    public delegate PolicySpace ImplementPolicy(GridSpace grid);

    public class PolicySpace
    {
        public string[,] matrix;
        int R, C;
        public PolicySpace(int rows, int cols)
        {
            matrix = new string[rows, cols];
            R = rows;
            C = cols;
        }

        public void Print()
        {
            Console.WriteLine("\n--------------------------------------\nThe Policy Space");
            for (int c = 0; c < R; c++)
            {
                for (int c1 = 0; c1 < C; c1++)
                {
                    Console.Write("\t" + matrix[c, c1]);
                }
                Console.WriteLine();
            }
        }
    }


    public class PolicyMaker
    {
        ValueIteration vi;
        double threshold;
        ImplementPolicy ip;
        int leastIterations = 0;

        public PolicyMaker(ValueIteration VI, double THRESHOLD, ImplementPolicy IP, int LEAST_ITR = 3)
        {
            vi = VI;
            threshold = THRESHOLD;
            ip = IP;
            leastIterations = LEAST_ITR;
        }

        public PolicySpace getBestPolicy(bool debug = false)
        {
            GridSpace og = new GridSpace(vi.getGridSpace().Rows(), vi.getGridSpace().Cols());
            int c = 0;
            while (true)
            {
                c++;
                vi.Process();
                if (debug)
                {
                    Console.Write("Original Grid");
                    vi.getGridSpace().Print();
                    Console.Write("Old Grid");
                    og.Print();
                }
                if (GridSpace.compareAtThreshold(og, vi.getGridSpace(), threshold) && c > leastIterations)
                {
                    break;
                }
                og = new GridSpace(vi.getGridSpace());
            }
            Console.WriteLine("Problem converged after " + c + " iterations");
            vi.getGridSpace().Print();

            return ip(vi.getGridSpace());
        }

    }

}