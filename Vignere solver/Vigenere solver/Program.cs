namespace Vigenere_solver
{

    //////////QUICK NOTE//////////
    /*  
        * cipher text must be at least 50 characters long
        * Works best with texts over 500 letters long 
        * With less than 500 letters and it may get 1 or 2 letters wrong
        * sometimes may have the key be doubled e.g. cryptiicryptii instead of cryptii
        * longer keys take slightly longer to run because it has to output every permutation - sometimes creating cool patterns ☺
    */

    //////////libarys//////////
    using System;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                //////////get the text input//////////
                string text = "";
                string lettersText = "";
                while (lettersText.Length < 200)
                {
                    Console.Write("Enter encrypted text (at least 200 characters long): ");
                    text = Console.ReadLine().ToLower();
                    lettersText = string.Join("", text.Where(char.IsLetter).ToArray()); //remove non letters
                }

                //////////get key length//////////
                float[] keyScores = new float[14];

                //try key lengths from 2 to 15
                for (int n = 2; n <= 15; n++)
                {
                    //index of coincidence calculation
                    string[] sequences = TextToArray(lettersText, n);
                    float[] scores = new float[sequences.Length];
                    foreach (var item in sequences)
                    {
                        Dictionary<string, int> frequencies = CeaserSolver.TextFrequency(item);
                        int[] occurances = new int[26];
                        for (int i = 0; i < occurances.Length; i++)
                        {
                            int f = frequencies.Values.ToArray()[i];
                            occurances[i] = f * (f - 1);
                        }
                        float occurancesSum = occurances.Sum();
                        float lengthCalc = item.Length * (item.Length - 1);
                        scores[Array.IndexOf(sequences, item)] = occurancesSum / lengthCalc;
                    }
                    //add to array
                    keyScores[n - 2] = scores.Sum() / scores.Length;
                }

                List<float> possibleKeys = new List<float>();
                foreach (var key in keyScores) { if (key >= 0.055) { possibleKeys.Add(key + Array.IndexOf(keyScores, key) + 2); } }
                int keyLength = Array.IndexOf(keyScores, keyScores.Max()) + 2;

                //////////list text//////////

                string[] solveArray = TextToArray(lettersText, keyLength);

                //create a list of non letters and indexes to add non letters back in at the end
                List<string[]> nonLetters = new();

                for (int i = 0; i < text.Length; i++)
                {
                    if (!char.IsLetter(text[i]))
                    {
                        string[] nonLetterIndex = { text[i].ToString(), i.ToString() };
                        nonLetters.Add(nonLetterIndex);
                    }
                }

                //////////solve cipthers//////////

                //create arrays for the possible keys and  decryption
                int[][] keysPos = new int[keyLength][]; //each part of array will contain 2 possible keys for that part of the cipher
                string[] decryption = new string[keyLength];

                //looping through the text array to solve each caesar
                for (int i = 0; i < keyLength; i++)
                {
                    CeaserSolver ceaserSolver = new CeaserSolver(solveArray[i]);
                    ceaserSolver.Solve();

                    //add possible keys and the decryprtion to the relavent arrays
                    keysPos[i] = ceaserSolver.Keys;
                    decryption[i] = ceaserSolver.Decryption;
                }

                //creating every permutation of each possible key for each part of the cipher
                int[][] keysPermutations = new int[Convert.ToInt32(Math.Pow(2, keyLength))][];
                for (int i = 0; i <= ~(-1 << keyLength); i++)
                {
                    //using binary indexes for permutations
                    string s = Convert.ToString(i, 2).PadLeft(7, '0');
                    int[] permutation = new int[s.Length];
                    for (int x = 0; x < s.Length; x++)
                    {
                        int index = int.Parse(s[x].ToString());
                        permutation[x] = keysPos[x][index];
                    }
                    keysPermutations[i] = permutation;
                }

                //setting the keys that are the most likely to be right
                int[] keys = new int[keyLength];
                for (int i = 0; i < keysPos.Length; i++) { keys[i] = keysPos[i][0]; }

                ////////combine the decryption arrays together e.g. 1,4,7  2,5,8  3,6,9  => 1,2,3,4,5,6,7,8,9////////
                List<string> output = new();

                for (int i = 0; i < decryption[0].Length; i++)
                {
                    foreach (var item in decryption)
                    {
                        //try needed because not all decryption parts are the same size
                        try { output.Add(item[i].ToString()); }
                        catch (IndexOutOfRangeException) { break; }
                    }
                }

                //adding back the non letters to the decryption
                foreach (var item in nonLetters) { output.Insert(Convert.ToInt32(item[1]), item[0].ToString()); } //use the index and character from the array item to insert the character back into the decryption

                //output the key and the decryption
                Console.WriteLine();
                Console.WriteLine(String.Join(",", keys)); //keys as numbers
                foreach (var item in keys) { Console.Write(Convert.ToChar(item + 97).ToString()); } //keys as letters
                Console.WriteLine();
                Console.WriteLine(string.Join("", output)); //the decryption
                Console.WriteLine();
                foreach (var item in keysPermutations) { foreach (var ascii in item) { Console.Write(Convert.ToChar(ascii + 97).ToString()); } if (Array.IndexOf(keysPermutations, item) < keysPermutations.Length - 1) Console.Write(", "); } // every key permutation incase key isn't quite right
                Console.WriteLine();

            }

            //////////end of program//////////
        }

        //turns the vigenere cipher into an array of caesar ciphers
        public static string[] TextToArray(string text, int keyLength)
        {
            string[] textArray = new string[keyLength];

            for (int x = 1; x < keyLength + 1; x++)
            {
                List<string> textList = new();

                for (int l = x; l < text.Length + 1; l += keyLength)
                {
                    textList.Add(Convert.ToString(text.Substring(l - 1, 1)));
                }
                textArray[x - 1] = string.Join("", textList);
            }
            //returning the array created
            return textArray;

        }
    }
}