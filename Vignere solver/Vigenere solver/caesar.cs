namespace Vigenere_solver
{
    internal class CeaserSolver
    {
        private string text;
        private int[] keys = new int[2];
        private string decryption;

        public int[] Keys { get { return keys; } }
        public string Decryption { get { return decryption; } }

        public CeaserSolver(string Text)
        {
            text = Text;
        }

        //function to create a dictionary of the count frequency of each character in the testText given
        public static Dictionary<string, int> TextFrequency(string testText)
        {
            //dictionary for counting letters
            var characterCount = new Dictionary<string, int>() { { "a", 0 }, { "b", 0 }, { "c", 0 }, { "d", 0 }, { "e", 0 }, { "f", 0 }, { "g", 0 }, { "h", 0 }, { "i", 0 }, { "j", 0 }, { "k", 0 }, { "l", 0 }, { "m", 0 }, { "n", 0 }, { "o", 0 }, { "p", 0 }, { "q", 0 }, { "r", 0 }, { "s", 0 }, { "t", 0 }, { "u", 0 }, { "v", 0 }, { "w", 0 }, { "x", 0 }, { "y", 0 }, { "z", 0 } };

            foreach (char c in testText) { characterCount[c.ToString()]++; } //increse the count of that character by 1
            return characterCount;
        }

        //return a value that shows the testText's distribution compared to normal english  link to formula => https://g.co/kgs/si6Yta
        public static double ChiSquareTest(string testText)
        {
            //a dictionary with the excpected frequency of each letter
            var exspectedFrequencies = new Dictionary<string, double>() { { "e", 11.1607 }, { "a", 8.4966 }, { "r", 7.5809 }, { "i", 7.5448 }, { "o", 7.1635 }, { "t", 6.9509 }, { "n", 6.6544 }, { "s", 5.7351 }, { "l", 5.4893 }, { "c", 4.5388 }, { "u", 3.6308 }, { "d", 3.3844 }, { "p", 3.1671 }, { "m", 3.0129 }, { "h", 3.0034 }, { "g", 2.4705 }, { "b", 2.0720 }, { "f", 1.8121 }, { "y", 1.7779 }, { "w", 1.2899 }, { "k", 1.1016 }, { "v", 1.0074 }, { "x", 0.2902 }, { "z", 0.2722 }, { "j", 0.1965 }, { "q", 0.1962 } };

            //getting a dictionary of the observed counts of each letter
            Dictionary<string, int> textFrequencies = TextFrequency(testText);

            //chisquare test
            double score = 0;

            foreach (string d in textFrequencies.Keys)
            {
                string s = d.ToLower();
                double exspectedCount = exspectedFrequencies[s] / 100 * testText.Length;
                score += Math.Pow(textFrequencies[s] - exspectedCount, 2) / exspectedCount;
            }

            //return the score given by the chi square test
            return score;
        }

        //solve the caesar by bruteforcing it and doing the chisquare test for each to check which is english
        public void Solve()
        {
            //create an array for the chi square score for each possible caeser key 
            double[] scores = new double[26];

            //for each caeser key, 'decrypt' the text with the key and then get the chi square score for that key
            for (int i = 0; i < 26; i++)

            {
                string[] ceaser = new string[text.Length];

                //loop over each character in the text and take away the offset and then add it to the caeser array
                for (int p = 0; p < text.Length; p++)
                {
                    //convert the character to ascii then - 97 so a = 0 and z = 25 then take off the caeser offset
                    //then find the remainder of that divided by 26 so like a round robin to keep it in the range of 0 - 25
                    int x = (Convert.ToInt32(text[p]) - i - 97) % 26;
                    //add 26 to x if x is less than 0 to keep it within the range of 0 - 25
                    if (x < 0) { x += 26; }
                    //add 97 to x to make it the right number for ascii conversion then convert it back to a character and then add it to the caeser array
                    ceaser[p] = Convert.ToChar(x + 97).ToString();
                }
                //getting score for that key
                scores[i] = ChiSquareTest(string.Join("", ceaser));
            }
            //sets keys to the possible ceaser keys that got the best scores
            double[] secondLowest = new double[26];
            scores.CopyTo(secondLowest, 0);
            secondLowest[Array.IndexOf(secondLowest, secondLowest.Min())] = 9999;

            keys = new int[] { Array.IndexOf(scores, (secondLowest.Min() - scores.Min() < 15 && scores.Min() > 50)? secondLowest.Min() : scores.Min()), Array.IndexOf(scores, (secondLowest.Min() - scores.Min() < 15 && scores.Min() > 50)? scores.Min() : secondLowest.Min()) };// english language is weird - replacement of if else

            Decrypt();
        }

        //decrypting the ceaser with the right key
        private void Decrypt()
        {
            //create the decryted text list
            string[] decryptionArray = new string[text.Length];

            //cycling through each character in the text to decrypt the character each time
            for (int i = 0; i < text.Length; i++)
            {
                int x = (Convert.ToInt32(text[i]) - keys[0] - 97) % 26; //converting ascii of char to int and decrypting the number
                if (x < 0) { x += 26; } //% will keep negative numbers negative but we need the numbers to be positive to convert back to a char
                decryptionArray[i] = Convert.ToChar(x + 97).ToString(); //converting number to char and adding it to the decryption text
            }
            //returning the decryption text as a string
            decryption = string.Join("", decryptionArray);
        }
    }
}
