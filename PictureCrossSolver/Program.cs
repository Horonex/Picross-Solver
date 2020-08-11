using System;
using System.Collections.Generic;

namespace PictureCrossSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            int[][] rows2 = new int[][]
{
                new int[]{7},
                new int[]{8},
                new int[]{2,2},
                new int[]{2,2},
                new int[]{2,2},
                new int[]{2,2},
                new int[]{2,2},
                new int[]{2,2}
};
            int[][] cols2 = new int[][]
            {
                new int[]{1},
                new int[]{7},
                new int[]{8},
                new int[]{2,1},
                new int[]{2},
                new int[]{6},
                new int[]{8},
                new int[]{2,2}
            };

            int[][] rows3 = new int[][]
{
                new int[]{1},
                new int[]{1,1},
                new int[]{4,1},
                new int[]{4},
                new int[]{10},
                new int[]{2,3},
                new int[]{1},
                new int[]{1,2,1},
                new int[]{2},
                new int[]{1,2}
};
            int[][] cols3 = new int[][]
            {
                new int[]{1,1},
                new int[]{6,1,1},
                new int[]{4,1},
                new int[]{4,1},
                new int[]{2},
                new int[]{1,1,1},
                new int[]{2,1},
                new int[]{2,1},
                new int[]{2,1,1},
                new int[]{1,1}
            };

            int[][] rows4 = new int[][]
{
                new int[]{3},
                new int[]{4},
                new int[]{4},
                new int[]{7},
                new int[]{3,5},
                new int[]{6},
                new int[]{3},
                new int[]{4},
                new int[]{8},
                new int[]{3},
                new int[]{4},
                new int[]{1,2},
                new int[]{1,1},
                new int[]{1,1},
                new int[]{1}
};
            int[][] cols4 = new int[][]
            {
                new int[]{5},
                new int[]{2},
                new int[]{5},
                new int[]{1,4},
                new int[]{1,1},
                new int[]{1,1},
                new int[]{1},
                new int[]{1},
                new int[]{1,1,2},
                new int[]{1,1,2},
                new int[]{7},
                new int[]{7},
                new int[]{7},
                new int[]{6},
                new int[]{2,2}
            };

            int[][] rows5 = new int[][]
{
                new int[]{4},
                new int[]{6},
                new int[]{1,6},
                new int[]{1,4},
                new int[]{1,5},
                new int[]{3,4,1,2},
                new int[]{10,1},
                new int[]{2,10},
                new int[]{1,7},
                new int[]{4,2,2,1},
                new int[]{1,4,1,2},
                new int[]{3,1,2},
                new int[]{2,1},
                new int[]{2,1},
                new int[]{2}
};
            int[][] cols5 = new int[][]
            {
                new int[]{1},
                new int[]{2,1},
                new int[]{6,1},
                new int[]{2,3},
                new int[]{2,3,2,1},
                new int[]{4,5,2},
                new int[]{14},
                new int[]{9,1},
                new int[]{10},
                new int[]{2,8},
                new int[]{1,3,2},
                new int[]{5,1},
                new int[]{1,2},
                new int[]{1,1,1},
                new int[]{2,1}
            };

            int[][] rows6 = new int[][]
{
                new int[]{5},
                new int[]{4,1},
                new int[]{2,1,1,2},
                new int[]{1,1,1,1},
                new int[]{2,3,1,2},
                new int[]{2,2,1,1},
                new int[]{2,3,1,1},
                new int[]{1,2,1,3},
                new int[]{2,8},
                new int[]{1,1,1,9},
                new int[]{2,6},
                new int[]{4,1},
                new int[]{3,2},
                new int[]{2},
                new int[]{1}
};
            int[][] cols6 = new int[][]
            {
                new int[]{3,1},
                new int[]{7},
                new int[]{2,3},
                new int[]{1,1,2,4},
                new int[]{1,1,2,1,4},
                new int[]{1,1,1,3},
                new int[]{1,2,3},
                new int[]{1,2,3},
                new int[]{1,2,2,3},
                new int[]{1,4},
                new int[]{3,1,5},
                new int[]{3,3,1},
                new int[]{2,3},
                new int[]{4},
                new int[]{2}
            };

            var puzzle2 = new Puzzle(rows2, cols2);
            var puzzle3 = new Puzzle(rows3, cols3,empty:' ');
            var puzzle4 = new Puzzle(rows4, cols4,empty:' ');
            var puzzle5 = new Puzzle(rows5, cols5,empty:'.');
            var puzzle6 = new Puzzle(rows6, cols6,empty:'.');

            //puzzle2.Solve();
            puzzle3.Solve();
            puzzle4.Solve();
            puzzle5.Solve();
            puzzle6.Solve();

            puzzle5.Print();
            Console.WriteLine();
            puzzle6.Print();


        }
    }
}
