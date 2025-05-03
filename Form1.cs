using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Words
{
    public partial class Form1 : Form
    {

        const int labelRowCount = 7; // Fixed row length 
        public List<TextBox> topLabels = new List<TextBox>(); // Array of character areas    
        public List<Button> onScreenKeys = new List<Button>();  // On-screen keys      
        //Color startBgCol;

        //curent word variables       
        List<string> wordDict = new List<string>(); // Dictionary for current word length
        string curWord = ""; // Chosen word
        List<char> wordChars = new List<char>(); // Cache for current word characters
        int curWordLength = 0; // Cache for current word length        
        int curRowStartIndex = 0; // Row index count       





        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //initialise
            ObtainTextBoxReferences();

            //default values
            ResetCurrentWord();

            //onscreen keys
            OnScreenKeysSetup();
        }


        // Reference all Textboxes on the form into an array
        void ObtainTextBoxReferences()
        {
            //add each textbox control to variables
            foreach (Control item in Controls)
            {
                if (item.GetType() == typeof(TextBox)) // only interested in textbox
                {
                    //ignore the input textBox
                    if (item.Name != "txtInput")
                    {
                        //Debug.WriteLine("Found " + item.Name);
                        topLabels.Add((TextBox)item);
                    }
                }
            }
            //need to reverse for correct index
            topLabels.Reverse();
        }

        //Resets the form
        void ResetCurrentWord()
        {
            // cycle through all text boxes
            for (int i = 0; i < topLabels.Count; i++)
            {
                //default states
                topLabels[i].Text = "";
                topLabels[i].Enabled = true;
                topLabels[i].BackColor = Color.White;
            }

            // manager variables
            curRowStartIndex = 0;
            btnInput.Enabled = true;

            //reset onscreen keys
            ResetOnScreenKeys();
        }

        // Programatically layout an on-screen keyboard
        void OnScreenKeysSetup()
        {
            //starting co-ordinates on form
            int x = 40;
            int y = 400;

            //spacing between each button
            int xPadding = 20;
            int hSpacing = 25;

            //array of keyboard (QUERTY)
            char[] top = new char[] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P' };
            char[] mid = new char[] { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L' };
            char[] bot = new char[] { 'Z', 'X', 'C', 'V', 'B', 'N', 'M' };


            //TOP
            RowButtonLayout(top, x, y, xPadding);
            x = 50;
            y += hSpacing;

            //MIDDLE
            RowButtonLayout(mid, x, y, xPadding);
            x = 60;
            y += hSpacing;

            //BOTTOM
            RowButtonLayout(bot, x, y, xPadding);



            //starting color for comparison
            //startBgCol = onScreenKeys[0].BackColor;

        }

        // Row placement loop
        private void RowButtonLayout(char[] row, int x, int y, int xPad)
        {

            for (int i = 0; i < row.Length; i++)
            {
                //create a new button with click event handler
                Button temp = new Button
                {
                    Location = new Point(x, y),
                    Width = 20,
                    Text = (row[i]).ToString()
                };
                temp.Click += (sender, EventArgs) => { Temp_Click(sender, EventArgs, temp.Text[0]); };

                //add to form
                Controls.Add(temp);
                //add to public variable
                onScreenKeys.Add(temp);

                //pad the button
                x += xPad;
            }
        }



        // Pick random word
        void WordPicker(string fileName)
        {
            try
            {
                //pull list from text file
                wordDict = new List<string>(File.ReadAllLines(fileName));
            }
            catch (Exception)
            {

                MessageBox.Show("Error loading file: " + fileName);
                return;
            }

            //random index
            curWord = wordDict[new Random().Next(0, wordDict.Count)].ToUpper();
            Debug.WriteLine("New word: " + curWord);
            wordChars = new List<char>(curWord); // cache
            curWordLength = curWord.Length; // cache            


            //reset
            ResetCurrentWord();

            //disable disused slots
            HideNA();

            //set max char length for input
            txtInput.MaxLength = curWordLength;

        }








        //buttons for choosing word length
        private void btn4_Click(object sender, EventArgs e)
        {
            WordPicker("words4.txt");
        }

        private void btn5_Click(object sender, EventArgs e)
        {
            WordPicker("words5.txt");
        }

        private void btn6_Click(object sender, EventArgs e)
        {
            WordPicker("words6.txt");
        }

        private void btn7_Click(object sender, EventArgs e)
        {
            WordPicker("words7.txt");
        }


        // Disabled any boxes over word length
        void HideNA()
        {
            //individual index
            int curColIndex = 0;
            //row index
            int curRowIndex = 0;

            //for each guess label
            for (int i = 0; i < topLabels.Count; i++)
            {
                //if over word length
                if (curColIndex + 1 > curWordLength)
                {
                    topLabels[i].Enabled = false;
                }

                //if end of the row of boxes
                curColIndex++;
                if (curColIndex >= labelRowCount)
                {
                    curColIndex = 0;
                    curRowIndex++;
                }
            }
        }







        // Main guess input button
        private void btnInput_Click(object sender, EventArgs e)
        {

            //clear input if not correct length
            if (txtInput.Text.Length != curWordLength)
            {
                //txtInput.Text = "";
                MessageBox.Show("Incorrect input length");
                return;
            }

            //check if in dictionary
            if (!wordDict.Contains(txtInput.Text.ToLower()))
            {
                //txtInput.Text = "";
                MessageBox.Show("Word not in dictionary");
                return;
            }




            //check the input
            EvaluateGuessChars();
            //disable missing keys


            DisableOnScreenKeys();
            //clear input box
            txtInput.Text = "";



            //next row
            curRowStartIndex++;
            //if run out of rows
            if (curRowStartIndex > 5)
            {
                btnInput.Enabled = false;
                MessageBox.Show("Word was " + curWord);
            }
        }












        // Function for Button Event
        private void Temp_Click(object sender, EventArgs e, char c)
        {
            if (txtInput.Text.Length >= curWordLength) return;

            Debug.WriteLine(c.ToString());
            txtInput.Text += c.ToString().ToLower(); ;
        }


        // Show user state of each character guessed
        void EvaluateGuessChars()
        {
            //seperate the input into characters
            List<char> guessChars = new List<char>(txtInput.Text);


            //loop through all input
            for (int i = 0; i < guessChars.Count; i++)
            {
                //put each character in place
                topLabels[(curRowStartIndex * labelRowCount) + i].Text = guessChars[i].ToString();

                //loop through each current word Char
                for (int c = 0; c < wordChars.Count; c++)
                {
                    //if we match a char
                    if (guessChars[i] == wordChars[c])
                    {
                        //if we match same place
                        if (i == c)
                        {
                            Debug.WriteLine("Matched  CORRECT " + guessChars[i]);
                            topLabels[(curRowStartIndex * labelRowCount) + i].BackColor = Color.Green;
                            MarkOnScreenKeys(guessChars[i], Color.Green);
                        }
                        else
                        {
                            Debug.WriteLine("Matched INCLUDES " + guessChars[i]);
                            topLabels[(curRowStartIndex * labelRowCount) + i].BackColor = Color.Orange;
                            MarkOnScreenKeys(guessChars[i], Color.Orange);
                        }
                    }
                }
            }
        }


        //colour keys
        void MarkOnScreenKeys(char c, Color col)
        {
            //match
            for (int i = 0; i < onScreenKeys.Count; i++)
            {
                if (onScreenKeys[i].Text == c.ToString())
                {
                    Debug.WriteLine("Matched: " + c);
                    onScreenKeys[i].BackColor = col;
                }
            }
        }



        // Disable onscreen keyboard for bad guesses
        void DisableOnScreenKeys()
        {
            //seperate the input into characters
            List<char> guessChars = new List<char>(txtInput.Text);

            // Get all bad keys
            List<char> badChars = new List<char>();

            //foreach char in guess word
            foreach (var c in guessChars)
            {
                //flag for adding to bad chars
                bool includes = false;

                //for each character in actual word
                foreach (var ac in wordChars)
                {
                    //add bad characters to list
                    if (c == ac)
                    {
                        //ignore as char match
                        includes = true;
                        break;
                    }
                }

                //add as was not found
                if (!includes)
                {
                    badChars.Add(c);
                }

            }

            //loop all bad characters
            foreach (var bc in badChars)
            {
                // Loop through each button
                for (int i = 0; i < onScreenKeys.Count; i++)
                {
                    //if we match a char
                    if (bc == onScreenKeys[i].Text[0])
                    {
                        onScreenKeys[i].BackColor = Color.Black;
                        onScreenKeys[i].Enabled = false;
                    }
                }
            }



        }




        // Reset button colours
        void ResetOnScreenKeys()
        {
            for (int i = 0; i < onScreenKeys.Count; i++)
            {
                onScreenKeys[i].BackColor = Color.White;
                onScreenKeys[i].Enabled = true;
            }
        }







    }//class










}
