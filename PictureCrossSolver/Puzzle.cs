using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PictureCrossSolver
{
    class Puzzle
    {
        int[][] rows;
        int[][] colums;
        int[,] grid;
        bool[] rowValidity;
        bool[] columValidity;

        int[] rowFail;
        int[] columFail;
        int stallThresholdToExit = 2;
        int loops = 0;

        char none;
        char full;
        char empty;

        public enum types { row, colum };

        public Puzzle(int[][] rows, int[][] colums, char none = ' ', char full = 'H', char empty = 'x')
        {
            this.rows = rows;
            this.colums = colums;
            grid = new int[colums.Length, rows.Length];
            rowValidity = new bool[rows.Length];
            columValidity = new bool[colums.Length];
            rowFail = new int[rows.Length];
            columFail = new int[colums.Length];

            this.none = none;
            this.full = full;
            this.empty = empty;
        }

        public void Solve()
        {
            //start timer
            Stopwatch timer = new Stopwatch();
            timer.Start();

            ///step one is more efficient than step two but there was no noticeable gains for 15x15 puzzles
            //StepOne();

            while (!CheckDone())
            {
                StepTwo();
                loops++;
            }

            //stop timer
            timer.Stop();

            //format and print time taken to complet puzzle
            TimeSpan ts = timer.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);


        }


        /// <summary>
        ///fill spaces that are there in every permutations
        /// </summary>
        void StepOne()
        {
            //do all rows
            for (int i = 0; i < rows.Length; i++)
            {
                //number of space left at the end of the firt permutation
                //remove interspaces from the line size
                int spaceLeft = rows.Length - rows[i].Length + 1;
                for (int j = 0; j < rows[i].Length; j++)
                {
                    //remove the size of every block in the row
                    spaceLeft -= rows[i][j];
                }

                //index to start placing the block
                int currentIndex = 0;
                //place all block
                for (int j = 0; j < rows[i].Length; j++)
                {
                    //number of space taken by the block that are there in all permutations
                    int numberToPlace = rows[i][j] - spaceLeft;
                    //if the block has spaces that are there in all permutaions
                    if (numberToPlace > 0)
                    {
                        int endIndex = currentIndex + rows[i][j];
                        int startIndex = endIndex - numberToPlace;
                        //fill the spaces
                        Populate(startIndex, endIndex, i, types.row);

                    }
                    //move index the size of the block place and skip the interspace
                    currentIndex += 1 + rows[i][j];
                }
            }
            //do all colums
            //repeat but for colums
            for (int i = 0; i < colums.Length; i++)
            {
                int spaceLeft = colums.Length - colums[i].Length + 1;
                for (int j = 0; j < colums[i].Length; j++)
                {
                    spaceLeft -= colums[i][j];
                }
                int currentIndex = 0;
                for (int j = 0; j < colums[i].Length; j++)
                {
                    int numberToPlace = colums[i][j] - spaceLeft;
                    if (numberToPlace > 0)
                    {
                        int endIndex = currentIndex + colums[i][j];
                        int startIndex = endIndex - numberToPlace;
                        Populate(startIndex, endIndex, i, types.colum);

                    }
                    currentIndex += 1 + colums[i][j];
                }
            }
            ValidateAll();
        }

        /// <summary>
        /// step two in solving the puzzle
        /// </summary>
        void StepTwo()
        {
            //do all rows
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                //get the data for the row
                int[] line = new int[grid.GetLength(1)];
                for (int j = 0; j < line.Length; j++)
                {
                    line[j] = grid[j, i];
                }

                //generate the permutations of the blocks in the line
                var perms = GenerateAllPermutations(rows[i], grid.GetLength(1));
                //remove the permutations that dont match the data that is already there
                var matches = Matches(perms, line);
                //get the data that is common in all those permutations
                var coms = GetCommun(matches);
                //place the data that is there in all permutaions in the grid
                if(!Populate(coms, i, types.row))
                {                   
                            rowFail[i]++;
                }
            }
            //do all colums
            //repeat but for colums
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                int[] line = new int[grid.GetLength(0)];
                for (int j = 0; j < line.Length; j++)
                {
                    line[j] = grid[i, j];
                }

                var perms = GenerateAllPermutations(colums[i], grid.GetLength(0));
                var matches = Matches(perms, line);
                var coms = GetCommun(matches);
                if (!Populate(coms, i, types.colum))
                {
                    columFail[i]++;
                }
            }
            //check all line to see if they are completed
            ValidateAll();
        }

        /// <summary>
        /// fill spaces in the grid
        /// </summary>
        /// <param name="startIndex">start index on the line</param>
        /// <param name="endIndex">end index on the line</param>
        /// <param name="lineIndex">index of the line or colum</param>
        /// <param name="type">whether the line is a colum or a row</param>
        public void Populate(int startIndex, int endIndex, int lineIndex, types type)
        {
            //set parameters to use on a row
            int iterator = startIndex;
            ref int xCoord = ref iterator;
            ref int yCoord = ref lineIndex;

            //if colum
            if (type == types.colum)
            {
                //set params to use on a colum
                xCoord = ref lineIndex;
                yCoord = ref iterator;
            }

            //use params to fill spaces
            for (; iterator < endIndex; iterator++)
            {
                grid[xCoord, yCoord] = 1;
            }
        }

        /// <summary>
        /// set the data of an array in the grid
        /// </summary>
        /// <param name="toPopulate">data to set</param>
        /// <param name="lineIndex">index of the line or colum</param>
        /// <param name="type">whether the line is a colum or a row</param>
        public bool Populate(int[] toPopulate, int lineIndex, types type)
        {
            //set parameters to use on a row
            int iterator = 0;
            ref int xCoord = ref iterator;
            ref int yCoord = ref lineIndex;

            //if colum
            if (type == types.colum)
            {
                //set params to use on a colum
                xCoord = ref lineIndex;
                yCoord = ref iterator;
            }
            //set this bool to true if the data is updated
            bool change = false;
            //use params to place the data in the grid
            for (; iterator < toPopulate.Length; iterator++)
            {
                // if there is data to place at index && the data is different from what is there
                if (toPopulate[iterator] != 0 && grid[xCoord, yCoord] != toPopulate[iterator])
                {
                    //place data at the spot
                    grid[xCoord, yCoord] = toPopulate[iterator];
                    //some data has change
                    change = true;
                }
            }
            return change;
            ////if the data did not change 
            //if (!change)
            //{
            //    //add a fail point to the corespoding line
            //    if (type == types.row)
            //    {
            //        rowFail[lineIndex]++;
            //    }
            //    else
            //    {
            //        columFail[lineIndex]++;
            //    }
            //}
        }

        /// <summary>
        /// Print the whole puzzle formated 
        /// </summary>
        public void Print()
        {
            //for each row
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                //for each colum
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    //look at data in the grid and print the coresponding char
                    switch (grid[j, i])
                    {
                        default:
                            Console.Write(none);
                            break;
                        case 1:
                            Console.Write(full);
                            break;
                        case -1:
                            Console.Write(empty);
                            break;
                    }
                    //space the chars to have a uniformly scaled grid print (otherwise the output will be squiched on the side)
                    Console.Write(" ");

                }
                //go to next line
                Console.WriteLine();
            }
        }

        /// <summary>
        /// print the data of an array (not formated)
        /// </summary>
        /// <param name="arr">array to print</param>
        public void Print(int[] arr)
        {
            foreach (var item in arr)
            {
                Console.Write(item + " ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// fill completed lines with empty spaces
        /// </summary>
        void ValidateAll()
        {
            //validate all rows
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                ValidateLine(i, types.row);
            }
            //validate all colums
            for (int i = 0; i < grid.GetLength(1); i++)
            {
                ValidateLine(i, types.colum);
            }
        }

        /// <summary>
        /// fill the line with empty spaces if the line is completed 
        /// </summary>
        /// <param name="lineIndex">index of the line or colum</param>
        /// <param name="type">whether the line is a colum or a row</param>
        public void ValidateLine(int lineIndex, types type)
        {
            //set parameters to use on a row
            ref int[][] lines = ref rows;
            int size = grid.GetLength(0);
            int iterator = 0;
            ref int xCoord = ref iterator;
            ref int yCoord = ref lineIndex;

            //if colum
            if (type == types.colum)
            {
                //set params to use on a colum
                lines = ref colums;
                size = grid.GetLength(1);
                xCoord = ref lineIndex;
                yCoord = ref iterator;
            }

            //count the number of block that hae to be in the grid
            Dictionary<int, int> counts = new Dictionary<int, int>();
            foreach (var item in lines[lineIndex])
            {
                AddToDic(counts, item);
            }

            //lenght of the block in the grid
            int lenght = 0;
            //iterate through the line
            for (; iterator < size; iterator++)
            {
                //if a space is filled 
                if (grid[xCoord, yCoord] == 1)
                {
                    //add lenght to the block
                    lenght++;
                }
                //space is empty
                else
                {
                    // if a block was seen
                    if (lenght > 0)
                    {
                        //remove the block from the count
                        AddToDic(counts, lenght, -1);
                    }
                    //reset the size of the block for the next one
                    lenght = 0;
                }
            }
            //if the lenght was not reset remove the last block
            if (lenght > 0)
            {
                AddToDic(counts, lenght, -1);
            }
            //set this bool to false if there is a missmatch between what was added and what was remove
            bool complete = true;
            //check all count of block in the dictionary
            foreach (var item in counts)
            {
                //if the value is not 0 the line is not completed
                if (!(item.Value == 0))
                {
                    complete = false;
                }
            }
            //if there where no missmatch
            if (complete)
            {
                //set the line as completed
                CompleteLine(lineIndex, type);
                if (type == types.row)
                {
                    rowValidity[lineIndex] = true;
                }
                else
                {
                    columValidity[lineIndex] = true;
                }
            }
        }

        /// <summary>
        /// fill unused spaces with empty spaces
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="type"></param>
        void CompleteLine(int lineIndex, types type)
        {
            //if row 
            if (type == types.row)
            {
                //iterate through the line
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    //if the space is not filled
                    if (grid[i, lineIndex] != 1)
                    {
                        //set the space as empty
                        grid[i, lineIndex] = -1;
                    }

                }
            }
            //if colum
            //do the same but for colums
            else
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[lineIndex, j] != 1)
                    {
                        grid[lineIndex, j] = -1;
                    }

                }
            }
        }

        /// <summary>
        /// get the array where unused spaces are replaces by empty spaces
        /// </summary>
        /// <param name="line">array to complete</param>
        /// <returns>return the array filled with empty spaces</returns>
        int[] CompleteLine(int[] line)
        {
            //set all unset spaces to empty
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == 0)
                {
                    line[i] = -1;
                }
            }
            return line;
        }

        /// <summary>
        /// add a key to a dictionary, if the key is already present add to the value
        /// </summary>
        /// <param name="dic">target dictionary</param>
        /// <param name="key">value to add</param>
        /// <param name="value">use 1 to add to the dictionary and -1 to remove from it</param>
        void AddToDic(Dictionary<int, int> dic, int key, int value = 1)
        {
            //if the key is present in the dictionary add the value
            if (dic.ContainsKey(key))
            {
                dic[key] += value;
            }
            //if not present add the key and value to the dictionary
            else
            {
                dic.Add(key, value);
            }
        }

        /// <summary>
        /// generate all the permutations of a block combination
        /// </summary>
        /// <param name="nums">arr of block</param>
        /// <param name="spaces"></param>
        /// <returns></returns>
        public List<int[]> GenerateAllPermutations(int[] nums, int spaces)
        {
            //generate all permutations for a given set of block
            var output = new List<int[]>();
            foreach (var item in GeneratePermutation(nums, spaces))
            {
                output.Add(CompleteLine(item));
            }
            return output;
        }

        /// <summary>
        /// generate permutation for each block recursively 
        /// </summary>
        /// <param name="nums">blocks to permutates</param>
        /// <param name="spaces">spaces to fit blocks</param>
        /// <returns>return permutations</returns>
        public List<int[]> GeneratePermutation(int[] nums, int spaces)
        {
            var output = new List<int[]>();
            //if theire is only one block to place 
            if (nums.Length == 1)
            {
                //generate all permutations for one block
                for (int i = 0; i < spaces + 1 - nums[0]; i++)
                {
                    int[] perm = new int[spaces];
                    for (int j = 0; j < nums[0]; j++)
                    {
                        perm[i + j] = 1;
                    }
                    output.Add(perm);
                }
            }
            //if theire is more than one block to place 
            else
            {
                //calculate the minimum space that the rest of the blocks can fit in
                int minSizeForRest = nums.Length - 1;
                for (int i = 1; i < nums.Length; i++)
                {
                    minSizeForRest += nums[i];
                }
                //generate all permutation of a block in the space left
                for (int i = 0; i < spaces + 1 - minSizeForRest - nums[0]; i++)
                {
                    //generate the partial permutation for this block (size is number of spaces befor the block + size of the block + interspace)
                    int[] partialPerm = new int[i + nums[0] + 1];
                    for (int j = 0; j < nums[0]; j++)
                    {
                        partialPerm[i + j] = 1;
                    }

                    //calculate the permutation of the rest of the blocks
                    //make an array of the rest of the blocks
                    int[] restNums = new int[nums.Length - 1];
                    for (int j = 0; j < restNums.Length; j++)
                    {
                        restNums[j] = nums[j + 1];
                    }

                    //generate the permutations
                    foreach (var item in GeneratePermutation(restNums, spaces - partialPerm.Length))
                    {
                        //append the permutation to the partial permutation
                        int[] perm = new int[partialPerm.Length + item.Length];
                        partialPerm.CopyTo(perm, 0);
                        item.CopyTo(perm, partialPerm.Length);
                        output.Add(perm);
                    }

                }

            }
            //return the permutations
            return output;
        }

        /// <summary>
        /// remove the permutations that dont fit the data
        /// </summary>
        /// <param name="permutations">all permutations to test</param>
        /// <param name="present">data that has to be present in the permutation</param>
        /// <returns>return the permutations that fit the data</returns>
        public List<int[]> Matches(List<int[]> permutations, int[] present)
        {
            var output = new List<int[]>();
            //check all permutations
            foreach (var item in permutations)
            {
                //set this bool to fals if the permutation does not match the data present
                bool isValid = true;
                //check all data
                for (int i = 0; i < present.Length; i++)
                {
                    //invalide criteria:
                    //you know there are none but perm has one
                    //you know there are one but perm has none
                    if (present[i] == -1 && item[i] == 1)
                    {
                        //invalide
                        isValid = false;
                        break;
                    }
                    if (present[i] == 1 && item[i] != 1)
                    {
                        //invalide
                        isValid = false;
                        break;
                    }
                }
                //if all data matched
                if (isValid)
                {
                    //add to valid permutations
                    output.Add(item);
                }
            }
            //return permutations
            return output;
        }

        /// <summary>
        /// get the data that is common to multiples arrays
        /// </summary>
        /// <param name="candidats">arrays to consider</param>
        /// <returns>return the data that is common to all arrays</returns>
        public int[] GetCommun(List<int[]> candidats)
        {
            //if there is nothing in the list
            if (candidats.Count == 0)
            {
                return new int[1];
            }
            //if their is only one this is done
            else if (candidats.Count == 1)
            {
                return candidats[0];
            }
            //set the output to the first candidat
            int[] output = new int[candidats[0].Length];
            candidats[0].CopyTo(output, 0);

            //check all other candidat
            foreach (var item in candidats)
            {
                //check all data
                for (int i = 0; i < output.Length; i++)
                {
                    //if the data does not match the output set the value as unset
                    if (item[i] != output[i])
                    {
                        output[i] = 0;
                    }
                }
            }
            //return the common data
            return output;
        }

        /// <summary>
        /// check if the puzzle is solved or the algorithm is stuck
        /// </summary>
        /// <returns>return true if the algorithm should continue and false if it should stop</returns>
        bool CheckDone()
        {
            //check if any line is not completed
            foreach (var item in rowValidity)
            {
                if (!item)
                {
                    return false;
                }
            }
            foreach (var item in columValidity)
            {
                if (!item)
                {
                    return false;
                }
            }
            return true;
            //
            bool notFailed = false;
            foreach (var item in rowFail)
            {
                if (item < stallThresholdToExit)
                {
                    notFailed = true;
                }
            }
            foreach (var item in columFail)
            {
                if (item < stallThresholdToExit)
                {
                    notFailed = true;
                }
            }
            if(notFailed)
            {
                for (int i = 0; i < rowFail.Length; i++)
                {
                    rowFail[i] = 0;
                }
                for (int i = 0; i < columFail.Length; i++)
                {
                    rowFail[i] = 0;
                }
                return false;
            }
            return true;
        }
    }
}
