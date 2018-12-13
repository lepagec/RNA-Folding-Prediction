using System;
using System.Collections.Generic;
using System.Text;

namespace RNA_Folding
{
    class SecondaryStructure
    {
        public Loop root = new Loop();
        //set by user, default = 0.8
        public double hLength = 0.8;
        //set by user, default = 1.0
        public double minPDLength = 1.0;
        public List<Base> bases;
        /**
         * Give this structure locations for each of its values in the graph
         */
        public void DrawStructure(String str)
        {
            root.parentDirection = 0.0;
            root.centre = new KeyValuePair<double, double>(0.0, 0.0);
            MakeBases(str);
            Iterate(root, bases[0], bases[bases.Count - 1]);
            AddDirections(root);
            AddLoopCentres(root);
            AddBases(root);
        }
        private void MakeBases(string str)
        {
            bases = new List<Base>();
            str = str.ToUpper();
            char[] arr = str.ToCharArray();
            int baseID = 1;
            foreach (char c in arr)
                if ((c == 'A') || (c == 'C') || (c == 'G') || (c == 'U'))
                {
                    bases.Add(new Base(c, baseID));
                    baseID += 1;
                }
            int leftBaseIndex = 0;
            for (int leftCharIndex = 0; leftCharIndex < arr.Length; ++leftCharIndex)
            {
                if (arr[leftCharIndex] == '(')
                {
                    int rightBaseIndex = leftBaseIndex;
                    int depth = 0;
                    for (int rightCharIndex = leftCharIndex + 1; rightCharIndex < arr.Length; ++rightCharIndex)
                    {
                        if (arr[rightCharIndex] == '(')
                            depth++;
                        else if (arr[rightCharIndex] == ')' && depth == 0)
                        {
                            bases[leftBaseIndex].SetBasePair(bases[rightBaseIndex - 1]);
                            bases[rightBaseIndex - 1].SetBasePair(bases[leftBaseIndex]);
                            break;
                        }
                        else if (arr[rightCharIndex] == ')' && depth != 0)
                            depth--;
                        else
                            rightBaseIndex++;
                    }
                }
                else if (arr[leftCharIndex] == ')')
                    continue;
                else
                    leftBaseIndex++;
            }
            for (int i = 0; i < bases.Count - 1; ++i)
            {
                bases[i].SetNextBase(bases[i + 1]);
                bases[i + 1].SetPreviousBase(bases[i]);
            }
            for (int i = 0; i < bases.Count; ++i)
            {
                if (bases[i].GetBasePair() != null && bases[i].GetBasePair().GetBasePair() != bases[i])
                {
                    Console.WriteLine("Inconsistent Hydrogen bonds.");
                    Environment.Exit(0);
                }
            }
        }
        private void Iterate(Loop loop, Base lBase, Base rBase)
        {
            Base current = lBase;
            Base prev = null;
            loop.slices.Add(new Slice());
            while (true)
            {
                int numSlices = loop.slices.Count;
                if (current.GetBasePair() == null)//we're in a loop slice
                {
                    loop.slices[numSlices - 1].bases.Add(current);//add the curent base into the slice
                    if (current == rBase)
                        return;
                    prev = current;
                    current = current.GetNext();
                }
                else if (current == rBase || current.GetBasePair() == prev || loop.parent != null && prev == null)
                {
                    if (current == rBase)
                        return;
                    prev = current;
                    current = current.GetNext();
                }
                else//we found a new stem (and a new loop)
                {
                    //Initialize child loop & stem; add them to loop and link everything together
                    Loop child = new Loop()
                    {
                        parent = loop
                    };
                    Stem stem = new Stem()
                    {
                        parentLoop = loop
                    };
                    stem.childLoop = child;
                    child.parentStem = stem;
                    loop.children.Add(child);
                    loop.childStems.Add(stem);
                    //initialize bases needed to process the stem
                    Base lBaseInner = current;
                    Base lBaseBond = lBaseInner.GetBasePair();
                    Base rBaseInner = lBaseBond;
                    //process the left side of the stem
                    while (lBaseInner.GetBasePair() == lBaseBond)
                    {
                        stem.bases.Add(lBaseInner);
                        lBaseInner = lBaseInner.GetNext();
                        lBaseBond = lBaseBond.GetPrevious();
                    }
                    lBaseInner = lBaseInner.GetPrevious();//hit the next loop, need to go back a step to process other stem
                    lBaseBond = lBaseBond.GetNext();
                    Base lBaseLoop = lBaseInner;//need for recursing
                    Base rBaseLoop = lBaseBond;
                    //process right side of the stem
                    while (true)
                    {
                        stem.bases.Add(lBaseBond);
                        if (lBaseBond == rBaseInner)
                            break;
                        lBaseInner = lBaseInner.GetPrevious();
                        lBaseBond = lBaseBond.GetNext();
                    }
                    Iterate(child, lBaseLoop, rBaseLoop);//recurse
                    loop.slices.Add(new Slice());
                    if (current == rBase)
                        return;
                    prev = current;
                    current = current.GetBasePair();//go from current loop slice to next
                }
            }
        }
        /**
         * Give each loop a sense of direction towards their parent and children. This method also calculates the perimiter of each
         * loop.
         */ 
        private void AddDirections(Loop loop)
        {
            loop.numBases = 0;
            foreach (Slice slice in loop.slices)
                loop.numBases += slice.bases.Count;
            /*
             * The following line sometimes adds more bases than necessary. This is intentional; in certain cases like the 3'
             * and 5',we will need to add some invisible bases so we can properly treat them as their own loop.
             */ 
            loop.numBases += loop.children.Count * 2 + 2;
            /*
             * find the length of the perimiter, including the invisible bases and PD bonds of the root
             */
            loop.perimLength = (loop.numBases - (loop.children.Count + 1)) * minPDLength + (loop.children.Count + 1) * hLength;
            foreach (Slice slice in loop.slices)
            {
                slice.pdLength = minPDLength;
                slice.pdAngle = 2.0 * Math.PI * slice.pdLength / loop.perimLength;
            }
            foreach (Stem stem in loop.childStems)
                stem.pdLength = minPDLength;
            loop.angle = 2.0*Math.PI * hLength / loop.perimLength;
            double direction = loop.parentDirection;
            for (int i=0; i<loop.children.Count; ++i)
            {
                direction = direction + ((loop.slices[i].bases.Count + 1) * loop.slices[i].pdAngle + loop.angle) % (2.0 * Math.PI);
                loop.childDirections.Add(direction);
                loop.children[i].parentDirection = (loop.childDirections[i] + Math.PI) % (2.0 * Math.PI);
            }
            loop.radius = minPDLength / Math.Sin(loop.slices[0].pdAngle);
            foreach (Loop child in loop.children)
                AddDirections(child);
        }
        /**
         * Find the centre coordinates of each loop to pinpoint where exactly to draw them
         */ 
        private void AddLoopCentres(Loop loop)
        {
            for (int i=0; i<loop.children.Count; ++i)
            {
                double lineLength = loop.radius * Math.Cos(loop.angle / 2.0) + loop.children[i].radius * Math.Cos(loop.children[i].angle / 2.0) + (loop.childStems[i].bases.Count / 2 - 1) * loop.childStems[i].pdLength;
                loop.children[i].centre = new KeyValuePair<double, double>((Math.Cos(loop.childDirections[i]) * lineLength + loop.centre.Key), (Math.Sin(loop.childDirections[i]) * lineLength + loop.centre.Value));
            }
            foreach (Loop child in loop.children)
                AddLoopCentres(child);
        }
        /**
         * Finally, draw each base in the correct location using the loop locations, minPDLength and hLength for reference
         */ 
        private void AddBases(Loop loop)
        {
            double direction = loop.parentDirection;
            direction += loop.angle / 2.0;
            for (int i=0; i<loop.slices.Count; ++i)
            {
                for (int j=0; j<loop.slices[i].bases.Count; ++j)
                {
                    direction += loop.slices[i].pdAngle;
                    loop.slices[i].bases[j].location = new KeyValuePair<double, double>(Math.Cos(direction) * loop.radius + loop.centre.Key, Math.Sin(direction) * loop.radius + loop.centre.Value);
                }
                direction += loop.slices[i].pdAngle;
                direction += loop.angle % (Math.PI * 2.0);
            }
            for (int i=0; i<loop.childStems.Count; ++i)
            {
                KeyValuePair<double, double> firstBaseCentre = new KeyValuePair<double, double>(Math.Cos(loop.childDirections[i] - loop.angle / 2.0) * loop.radius + loop.centre.Key, Math.Sin(loop.childDirections[i] - loop.angle / 2.0) * loop.radius + loop.centre.Value);
                KeyValuePair<double, double> altBaseCentre = new KeyValuePair<double, double>(Math.Cos(loop.childDirections[i] + loop.angle / 2.0) * loop.radius + loop.centre.Key, Math.Sin(loop.childDirections[i] + loop.angle / 2.0) * loop.radius + loop.centre.Value);
                for (int j=0; j<loop.childStems[i].bases.Count/2; ++j)
                {
                    int k = loop.childStems[i].bases.Count - j - 1;
                    loop.childStems[i].bases[j].location = new KeyValuePair<double, double>(Math.Cos(loop.childDirections[i]) * loop.childStems[i].pdLength * j + firstBaseCentre.Key, Math.Sin(loop.childDirections[i]) * loop.childStems[i].pdLength * j + firstBaseCentre.Value);
                    loop.childStems[i].bases[k].location = new KeyValuePair<double, double>(Math.Cos(loop.childDirections[i]) * loop.childStems[i].pdLength * j + altBaseCentre.Key, Math.Sin(loop.childDirections[i]) * loop.childStems[i].pdLength * j + altBaseCentre.Value);
                }
            }
            foreach (Loop child in loop.children)
                AddBases(child);
        }
        public void FitToWindow(double windowWidthX, double windowWidthY, double borderWidthX, double borderWidthY)
        {
            double maxX = bases[0].location.Key;
            double maxY = bases[0].location.Value;
            double minX = bases[0].location.Key;
            double minY = bases[0].location.Value;
            foreach (Base b in bases) {
                double baseX = b.location.Key;
                double baseY = b.location.Value;
                if (baseX < minX)
                    minX = baseX;
                if (baseX > maxX)
                    maxX = baseX;
                if (baseY < minY)
                    minY = baseY;
                if (baseY > maxY)
                    maxY = baseY;
            }
            double rangeMinMaxX = maxX - minX;
            double rangeMinMaxY = maxY - minY;
            double biggestRange = Math.Max(rangeMinMaxX, rangeMinMaxY);
            foreach (Base b in bases) {
                double baseX = b.location.Key;
                double baseY = b.location.Value;
                b.location = new KeyValuePair<double, double>((baseX - minX) / biggestRange, (baseY - minY) / biggestRange);
            }
            if (windowWidthX/windowWidthY>rangeMinMaxX/rangeMinMaxY)
            {
                foreach (Base b in bases)
                {
                    double baseX = b.location.Key;
                    double baseY = b.location.Value;
                    b.location = new KeyValuePair<double, double>(baseX*windowWidthY+borderWidthX, baseY*windowWidthY+borderWidthY);
                }
            }
            else
            {
                foreach (Base b in bases)
                {
                    double baseX = b.location.Key;
                    double baseY = b.location.Value;
                    b.location = new KeyValuePair<double, double>(baseX * windowWidthX + borderWidthX, baseY * windowWidthX + borderWidthY);
                }
            }
        }
    }
}