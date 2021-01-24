namespace Brickwork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Brickwork.Models;
    public class Program
    {
        static void Main(string[] args)
        {
            int[,] firstLayerBricks = ReadInputAndValidateIt();

            if (firstLayerBricks == null)
            {
                Console.WriteLine(ErrorMessages.ErrorMessages.InvalidInputException);
                return;
            }

            int[,] secondLayerBricks = CopyFirstLayer(firstLayerBricks);

            try
            {
                secondLayerBricks = CheckRowsForSameConsecuviteNumbersAndChangeTheirPosition(secondLayerBricks);
                secondLayerBricks = ChangePositionOfNumbersOnConsecutiveRowsOnTheSameColumn(secondLayerBricks);
                secondLayerBricks = CheckIfNumberHasTheSameNumberNextToItAndFindSameNumberAndBringThemBothTogetherIfItDoesNotHaveItNextToIt(secondLayerBricks);

                // find squares in which there are two bricks in first and second layer, compare them and swap elements in second layer for those squares that are the same in the first layer
                List<BaseSquare> firstLayerSquares = FindSquares(firstLayerBricks);
                List<BaseSquare> secondLayerSquares = FindSquares(secondLayerBricks);
                secondLayerBricks = ChekForSameSquaresAndSwapElements(firstLayerSquares, secondLayerSquares, secondLayerBricks);

                PrintSecondLayer(secondLayerBricks);
            }
            catch (Exception)
            {
                Console.WriteLine("-1");
                Console.WriteLine(ErrorMessages.ErrorMessages.NoSolutionExistsException);
            }
        }

        /// <summary>
        /// if two numbers are far away from each other this method gets the position of the second one since we are already working with the first one and know its position
        /// </summary>
        /// <param name="l">row of the element</param>
        /// <param name="k">column of the element</param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[] FindTheSameNumber(int l, int k, int[,] matrix)
        {
            // put the number of the row and column in this array so we can extract the position later from it
            int[] positionOfSameNumber;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    // check if the values are the same but they are not the same brick square
                    if (matrix[i, j] == matrix[l, k] && (i != l || k != j))
                    {
                        positionOfSameNumber = new int[] { i, j };
                        return positionOfSameNumber;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// we swap the position of two numbers of our choice with given coordinates
        /// </summary>
        /// <param name="i">row of first number</param>
        /// <param name="j">column of first number</param>
        /// <param name="k">row of second number</param>
        /// <param name="l">column of second number</param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[,] SwapTwoNumbers(int i, int j, int k, int l, int[,] matrix)
        {
            int temp = matrix[i, j];
            matrix[i, j] = matrix[k, l];
            matrix[k, l] = temp;
            return matrix;
        }

        /// <summary>
        /// checks if the current number has one more like it somewhere next to it; we check in different directions according to the position of the current element
        /// </summary>
        /// <param name="l">row of the current element</param>
        /// <param name="k">column of the current element</param>
        /// <param name="matrix">array where the elements are located</param>
        /// <returns></returns>
        private static bool CheckIfNumberHasABuddy(int l, int k, int[,] matrix)
        {
            // on first row and column check right and down
            if (k == 0 && l == 0)
            {
                if (matrix[l, k + 1] == matrix[l, k] || matrix[l + 1, k] == matrix[l, k]) return true;
            }

            // on last row and column check left and up
            else if (k == matrix.GetLength(1) - 1 && l == matrix.GetLength(0) - 1)
            {
                if (matrix[l, k - 1] == matrix[l, k] || matrix[l - 1, k] == matrix[l, k]) return true;
            }

            // on first column and last row check up and right
            else if (k == 0 && l == matrix.GetLength(0) - 1)
            {
                if (matrix[l - 1, k] == matrix[l, k] || matrix[l, k + 1] == matrix[l, k]) return true;
            }

            // on first row and last column check left and down
            else if (l == 0 && k == matrix.GetLength(1) - 1)
            {
                if (matrix[l, k - 1] == matrix[l, k] || matrix[l + 1, k] == matrix[l, k]) return true;
            }

            // on first column but not first or last row check up down right
            else if (k == 0 && l < matrix.GetLength(0) - 1 && l > 0)
            {
                if (matrix[l - 1, k] == matrix[l, k] || matrix[l + 1, k] == matrix[l, k] || matrix[l, k + 1] == matrix[l, k]) return true;
            }

            // on last column but not first or last row check up down left
            else if (k == matrix.GetLength(1) - 1 && l < matrix.GetLength(0) - 1 && l > 0)
            {
                if (matrix[l - 1, k] == matrix[l, k] || matrix[l + 1, k] == matrix[l, k] || matrix[l, k - 1] == matrix[l, k]) return true;
            }

            // on first row but not first or last column check left right down
            else if (l == 0 && k < matrix.GetLength(1) - 1)
            {
                if (matrix[l, k - 1] == matrix[l, k] || matrix[l, k + 1] == matrix[l, k] || matrix[l + 1, k] == matrix[l, k]) return true;
            }

            // on last row but not on last or first column check up right and left
            else if (l == matrix.GetLength(0) - 1 && k < matrix.GetLength(1) - 1 && k > 0)
            {
                if (matrix[l, k - 1] == matrix[l, k] || matrix[l, k + 1] == matrix[l, k] || matrix[l - 1, k] == matrix[l, k]) return true;
            }

            // if not on first or last row or column check up and down left and right
            else if (l != 0 && k != 0 && l != matrix.GetLength(0) - 1 && k != matrix.GetLength(1) - 1)
            {
                if (matrix[l, k - 1] == matrix[l, k] || matrix[l - 1, k] == matrix[l, k]
                || matrix[l, k + 1] == matrix[l, k] || matrix[l + 1, k] == matrix[l, k])
                    return true;
            }

            return false;
        }

        /// <summary>
        /// checks for a given element that has no element next to it with the same number whether elements around it are coupled with their likes; the first one that is also separated from its other part of the brick is chosen to be swapped for the element that is the same with the one we are currently working with but is far away from its twin; we essentially do not want to swap for elements that are already coupled with their twins; if there is no such element that is alone we proceed with checking the same for the far away number (its buddy);
        /// </summary>
        /// <param name="i">row of current element</param>
        /// <param name="j">column of current element</param>
        /// <param name="matrix">the second layer of bricks we are working with</param>
        /// <returns></returns>
        private static int[] FindPositionOfNumberToSwapForFirstNumber(int i, int j, int[,] matrix)
        {
            int[] positionOfNumberToSwap;

            // we are not on the first row and we check the number above
            if (i > 0)
            {
                bool hasBuddy = CheckIfNumberHasABuddy(i - 1, j, matrix);

                if (!hasBuddy)
                {
                    positionOfNumberToSwap = new int[] { i - 1, j };
                    return positionOfNumberToSwap;
                }
            }

            // we are not on the last row and we check the number below
            if (i < matrix.GetLength(0) - 1)
            {
                bool hasBuddy = CheckIfNumberHasABuddy(i + 1, j, matrix);

                if (!hasBuddy)
                {
                    positionOfNumberToSwap = new int[] { i + 1, j };
                    return positionOfNumberToSwap;
                }
            }

            // we are not on the first column and we check the number left
            if (j > 0)
            {
                bool hasBuddy = CheckIfNumberHasABuddy(i, j - 1, matrix);

                if (!hasBuddy)
                {
                    positionOfNumberToSwap = new int[] { i, j - 1 };
                    return positionOfNumberToSwap;
                }
            }

            // we are not on the last column and we check the number right
            if (j < matrix.GetLength(1) - 1)
            {
                bool hasBuddy = CheckIfNumberHasABuddy(i, j + 1, matrix);

                if (!hasBuddy)
                {
                    positionOfNumberToSwap = new int[] { i, j + 1 };
                    return positionOfNumberToSwap;
                }
            }

            return null;
        }

        /// <summary>
        /// the methods checks if there are two bricks that form a square together and puts them in a list of squares; squares can be vertical or horizontal depending on the position of the bricks in them; each square has a starting point which serves as a coordinate and can be used later to compare if squares of the same type have the same position on the first and second layer of bricks; if they do, we want to change the position of the bricks in the second layer;
        /// </summary>
        /// <param name="matrix">the layer of bricks</param>
        /// <returns></returns>
        private static List<BaseSquare> FindSquares(int[,] matrix)
        {
            List<BaseSquare> squares = new List<BaseSquare>();

            for (int i = 0; i < matrix.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < matrix.GetLength(1) - 1; j++)
                {
                    List<int> startingPoint = new List<int>();
                    BaseSquare square;

                    if (matrix[i, j] == matrix[i, j + 1] && matrix[i + 1, j] == matrix[i + 1, j + 1])
                    {
                        startingPoint.Add(i);
                        startingPoint.Add(j);
                        square = new VerticalSquare(startingPoint);
                        squares.Add(square);
                    }
                    else if (matrix[i, j] == matrix[i + 1, j] && matrix[i, j + 1] == matrix[i + 1, j + 1])
                    {
                        startingPoint.Add(i);
                        startingPoint.Add(j);
                        square = new HorizontalSquare(startingPoint);
                        squares.Add(square);
                    }
                }
            }

            return squares;
        }

        /// <summary>
        /// the method checks if there are identical squares in both layers and changes the position of the bricks in identical squares in the second layer
        /// </summary>
        /// <param name="firstLayerSquares"></param>
        /// <param name="secondLayerSquares"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[,] ChekForSameSquaresAndSwapElements(List<BaseSquare> firstLayerSquares, List<BaseSquare> secondLayerSquares, int[,] matrix)
        {
            foreach (var firstLayerSquare in firstLayerSquares)
            {
                foreach (var secondLayerSquare in secondLayerSquares)
                {
                    if (firstLayerSquare.StartingPoint[0] == secondLayerSquare.StartingPoint[0] && firstLayerSquare.StartingPoint[1] == secondLayerSquare.StartingPoint[1] && firstLayerSquare.GetType().Name == secondLayerSquare.GetType().Name)
                    {
                        int startingRow = secondLayerSquare.StartingPoint[0];
                        int startingCol = secondLayerSquare.StartingPoint[1];
                        matrix = SwapTwoNumbers(startingRow, startingCol, startingRow + 1, startingCol + 1, matrix);
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// as the name of the method says we check if the current number is coupled with its twin and if not we find it and check if there is a number around them that is also alone and can be swapped so they can form a brick
        /// </summary>
        /// <param name="matrix">the layer of bricks</param>
        /// <returns></returns>
        private static int[,] CheckIfNumberHasTheSameNumberNextToItAndFindSameNumberAndBringThemBothTogetherIfItDoesNotHaveItNextToIt(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    bool hasABuddy = CheckIfNumberHasABuddy(i, j, matrix);

                    if (!hasABuddy)
                    {
                        int[] positionOfBuddyNumber = FindTheSameNumber(i, j, matrix);
                        int[] positionOfNumberToSwapForFirstNumber = FindPositionOfNumberToSwapForFirstNumber(i, j, matrix);

                        if (positionOfNumberToSwapForFirstNumber != null)
                        {
                            matrix = SwapTwoNumbers(positionOfBuddyNumber[0], positionOfBuddyNumber[1], positionOfNumberToSwapForFirstNumber[0], positionOfNumberToSwapForFirstNumber[1], matrix);
                        }
                        else
                        {
                            int[] positionOfNumberToSwapForSecondNumber = FindPositionOfNumberToSwapForFirstNumber(positionOfBuddyNumber[0], positionOfBuddyNumber[1], matrix);

                            if (positionOfNumberToSwapForSecondNumber != null)
                            {
                                matrix = SwapTwoNumbers(i, j, positionOfNumberToSwapForSecondNumber[0], positionOfNumberToSwapForSecondNumber[1], matrix);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// this is the second check we do after having gotten and validated the input and it checks if there are vertical bricks and changes their position in the second layer so that they are not identical with the first layer
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[,] ChangePositionOfNumbersOnConsecutiveRowsOnTheSameColumn(int[,] matrix)
        {
            // check consecutive rows for same numbers with first layer in cols and move them
            for (int i = 0; i < matrix.GetLength(0) - 1; i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    int first = matrix[i, j];
                    int second = matrix[i + 1, j];
                    // first two numbers on consecutive rows are equal
                    // 1 2   1 1
                    // 1 2   2 2
                    if (first == second && j == 0)
                    {
                        matrix = SwapTwoNumbers(i + 1, j, i, j + 1, matrix);
                    }
                    // two numbers on consecutive rows are equal after the first column
                    // 1 1 3 5   1 1 4 5
                    // 2 4 3 6   2 3 3 6
                    else if (first == second)
                    {
                        matrix = SwapTwoNumbers(i, j, i + 1, j - 1, matrix);
                    }
                }
            }

            return matrix;
        }

        /// <summary>
        /// this is the first check after having gotten the input and validated it and checks for horizontal bricks and changes their position if there are such
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[,] CheckRowsForSameConsecuviteNumbersAndChangeTheirPosition(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1) - 1; j++)
                {
                    int first = matrix[i, j];
                    int second = matrix[i, j + 1];
                    
                    if (first == second && j == 0 && matrix.GetLength(1) > 2)
                    {
                        // 1 1 3 3 5 5 - first two are equal and columns are more than 2
                        matrix = SwapTwoNumbers(i, j, i, j + 2, matrix);
                        j++;
                    }
                    else if (first == second && matrix.GetLength(1) > 2)
                    {
                        // 1 2 2 3 3 - equal after the first two
                        matrix = SwapTwoNumbers(i, j + 1, i, j - 1, matrix);
                    }
                }
            }

            return matrix;
        }

        private static int[,] ReadInputAndValidateIt()
        {
            int[] dimensionsOfLayer = Console.ReadLine().Split(" ").Select(x => int.Parse(x)).ToArray();
            
            // rows
            int n = dimensionsOfLayer[0];

            // cols
            int m = dimensionsOfLayer[1];

            if (n < 100 && m < 100)
            {
                int[,] matrix = new int[n, m];

                for (int i = 0; i < n; i++)
                {
                    int[] row = Console.ReadLine().Split(" ").Select(x => int.Parse(x)).ToArray();

                    for (int j = 0; j < m; j++)
                    {
                        matrix[i, j] = row[j];
                    }
                }

                matrix = CheckForBricksWithThreeParts(matrix);

                if (matrix != null)
                {
                    return matrix;
                }
            }

            return null;
        }

        /// <summary>
        /// method gets the count of each number in the layer and if there are three numbers with the same value returns null 
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        private static int[,] CheckForBricksWithThreeParts(int[,] matrix)
        {
            // counts same parts
            int counter = 0;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    int current = matrix[i, j];

                    foreach (var element in matrix)
                    {
                        if (element == current)
                        {
                            counter++;

                            if (counter == 3)
                            {
                                return null;
                            }
                        }
                    }

                    counter = 0;
                }
            }

            return matrix;
        }

        /// <summary>
        /// we want to get a copy of the first layer as a second layer which we can start working on and changing the positions of the bricks
        /// </summary>
        /// <param name="matrixToCopyFrom"></param>
        /// <returns></returns>
        private static int[,] CopyFirstLayer(int[,] matrixToCopyFrom)
        {
            int[,] copiedMatrix = new int[matrixToCopyFrom.GetLength(0), matrixToCopyFrom.GetLength(1)];

            for (int i = 0; i < matrixToCopyFrom.GetLength(0); i++)
            {
                for (int j = 0; j < matrixToCopyFrom.GetLength(1); j++)
                {
                    copiedMatrix[i, j] = matrixToCopyFrom[i, j];
                }
            }

            return copiedMatrix;
        }

        private static void PrintSecondLayer(int[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i == 0)
                {
                    Console.Write("————————————————————————————");
                    Console.WriteLine();
                }

                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + (matrix[i, j] < 10 ? "  " : " "));

                    if (j < matrix.GetLength(1) - 1)
                    {
                        if (matrix[i, j + 1] != matrix[i, j])
                        {
                            Console.Write("|");
                        }
                    }

                    if (j == matrix.GetLength(1) - 1)
                    {
                        Console.Write("|");
                    }
                }

                Console.WriteLine();

                for (int m = 0; m < matrix.GetLength(1); m++)
                {
                    if (i < matrix.GetLength(0) - 1)
                    {
                        if (matrix[i, m] != matrix[i + 1, m])
                        {
                            Console.Write("————");
                        }
                        else
                        {
                            Console.Write("   ");
                        }
                    }
                }

                if (i == matrix.GetLength(0) - 1)
                {
                    Console.Write("————————————————————————————");
                }
                Console.WriteLine();
            }
        }
    }
}
