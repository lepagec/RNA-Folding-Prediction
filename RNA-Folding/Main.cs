using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RNA_Folding
{
    //The Main class that does most of everthing, such as the Base production, and the Pairing of Bases
    public class Main 
    {
        //MainWindow so we can deal with graphics and change what's on the screen
        private MainWindow mainWindow;
        //String storage
        private String RNAString, searchString;
        //Stores the bases 
        private Base[] bases;
        //Size of the display, cannot be changed (i.e FINAL)
        private readonly int WIDTH, HEIGHT;
        //Radius of the circle, will be set to depend on RNA Length
        private readonly int RADIUS;

        public Main(String s, MainWindow mw, int r)
        {
            RNAString = s;
            mainWindow = mw;
            HEIGHT = (int) mw.Stack_Panel.Height;
            WIDTH = (int) mw.Stack_Panel.Width;
            bases = new Base[RNAString.Length];
            RADIUS = r;
            Process();
        }

        //Process the RNAString and draws the structure, will also generate the Base pairs
        public void Process()
        {
            //Create bases
            CreateBases(RNAString);

            //Display bases
            DisplayBases();
        }

        public Base[] GetBases()
        {
            return bases;
        }

        public void SearchForString(String s)
        {
            //Remove all currently highlighted bases
            searchString = s;
            foreach (Base b in bases)
            {
                b.SetIsHightlighted(false);
            }
            List<int> basesToHighlight = new List<int>();
            char[] chars = s.ToCharArray();
            //Search for the subsequence
            for (int i = 0; i <= bases.Length - chars.Length; i++)
            {
                Base b = bases[i];
                for (int j = 0; j < chars.Length; j++)
                {
                    if (b.GetBaseType() == chars[j])
                    {
                        basesToHighlight.Add(b.GetID() - 1);
                    }
                    if(b.GetID() != bases.Length)
                    {
                        b = bases[b.GetID()];
                    }
                }
                if (basesToHighlight.Count == chars.Length)
                {
                    foreach(int t in basesToHighlight)
                    {
                        bases[t].SetIsHightlighted(true);
                    }
                }
                basesToHighlight.Clear();
            }
            //Redraw the bases
            DisplayBases();
        }

        //Returns a base if there is any
        public Base SearchForBase(int x1, int y1, int x2, int y2)
        {
            Base baseToReturn = null;
            foreach(Base b in bases)
            {
                //If X and Y are similar to b.X and b.Y 
                if ((b.GetX() / RADIUS) == (x1 / RADIUS) && (b.GetY() / RADIUS) == (y1 / RADIUS))
                {
                    baseToReturn = b;
                    break;
                }
                else if ((b.GetX() / RADIUS) == (x2 / RADIUS) && (b.GetY() / RADIUS) == (y2 / RADIUS))
                {
                    baseToReturn = b;
                    break;
                }
            }
            return baseToReturn;
        }

        public void ChangeBaseType(String s)
        {
            CreateBases(s);
            if (searchString != null)
            {
                SearchForString(searchString);
            }
            DisplayBases();
        }

        public void CreateBases(String rna)
        {
            //Nussinov will go here
            Nussinov n = new Nussinov();
            n.nussinovAlgorithm(rna);
            String st = n.traceBack(0, rna.Length - 1);
            SecondaryStructure s = new SecondaryStructure();
            s.DrawStructure(st);
            s.FitToWindow(WIDTH - 30, HEIGHT - 30, 15, 15);
            List<Base> listOfBases = s.bases;
            int i = 0;
            foreach (Base b in s.bases)
            {
                bases[i] = b;
                i++;
            }
        }

        //Method to draw the bases to the screen
        public void DisplayBases()
        {
            mainWindow.DrawBases(bases);
        }
    }
}
