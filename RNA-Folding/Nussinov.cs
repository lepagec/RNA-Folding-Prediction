using System;
using System.Collections.Generic;
using System.Text;

namespace RNA_Folding
{
    class Nussinov
    {
        private String sequence;
        private char[] sequenceArray;
        private int[,] matrix;
        private int sequenceLength { get; set; }
        public Nussinov()
        {
            this.sequence = null;
            this.sequenceArray = null;
            
        }
        private void initializeMatrix()
        {
            matrix = new int[sequenceLength,sequenceLength];

            //initialize matrix
            for (int i = 0; i < sequenceLength; i++)
            {
                for (int j = 0; j < sequenceLength; j++)
                {
                    matrix[i,j] = -1;
                }
            }
        }
        public int nussinovAlgorithm(String sequence)
        {

            this.sequence = sequence;
            sequenceArray = sequence.ToCharArray();
            sequenceLength = sequence.ToCharArray().Length;
            initializeMatrix();
            return nussinovAlgorithm(0,sequenceLength-1);

        }

        public int nussinovAlgorithm(int i, int j)
        {

            if (matrix[i, j] != -1)
            {
                return matrix[i, j];
            }

            if (i >= j)
            {
                matrix[i, j] = 0;
                return 0;
            }

            int max = 0;

            max = Math.Max(max, Math.Max(nussinovAlgorithm(i + 1, j),
                nussinovAlgorithm(i,j-1)));
            if (isPair(sequenceArray[i], sequenceArray[j]) && i + 3 < j)
            {
                max = Math.Max(max,nussinovAlgorithm(i + 1,j - 1) + 1);
            }

            for (int k = i; k < j;k++)
            {
                max = Math.Max(max,nussinovAlgorithm(i,k) + nussinovAlgorithm(k + 1,j));
            }
            matrix[i, j] = max;

            return max;
        }

        public string traceBack(int i, int j)
        {
            if (i == j)
            {
                return char.ToString(sequenceArray[i]);
            }

            if (i > j)
            {
                return "";
            }

            if (nussinovAlgorithm(i,j) == nussinovAlgorithm(i + 1,j))
            {
                return sequenceArray[i] + traceBack(i + 1,j);
            }

            if (nussinovAlgorithm(i, j) == nussinovAlgorithm(i, j - 1))
            {
                return traceBack(i, j - 1) + sequenceArray[j];
               
            }
            if (isPair(sequenceArray[i], sequenceArray[j]) && nussinovAlgorithm(i, j) == nussinovAlgorithm(i + 1, j - 1) + 1)
            {
                return "(" + sequenceArray[i] + traceBack(i + 1, j - 1) + sequenceArray[j] + ")";
            }
            for(int k = i + 1;k< j; k++)
            {
                if (nussinovAlgorithm(i, j) == nussinovAlgorithm(i, k) + nussinovAlgorithm(k + 1, j))
                {
                    return traceBack(i, k) + traceBack(k+1,j);
                }
            }
            return "Trace Back failed";
        }

        private bool isPair(char base1 , char base2)
        {
            return ((base1 == 'A' && base2 == 'U') || (base1 == 'U' && base2 == 'A') || (base1 == 'C' && base2 == 'G') || (base1 == 'G' && base2 == 'C')
                ||(base1 =='G' && base2 =='U') || (base1 == 'U' && base2 == 'G'));
        }

        private bool checkIfInvalidChars(String input)
        {
            foreach (char c in input.ToUpper().ToCharArray())
            {
                switch (c)
                {
                    case 'A':
                    case 'C':
                    case 'G':
                    case 'U':
                        break;

                    default:
                        return true;
                }
            }

            return false;
        }

        public void printMatrix()
        {
            int rowLength = matrix.GetLength(0);
            int colLength = matrix.GetLength(1);

            for (int i = 0; i < rowLength; i++)
            {
                for (int j = 0; j < colLength; j++)
                {
                    Console.Write(string.Format("{0} ", matrix[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.ReadLine();
        }
    }
}
