using System;
using System.Collections.Generic;
using System.Text;

namespace RNA_Folding
{
    class Loop
    {
        public List<Slice> slices = new List<Slice>();
        public Loop parent;
        public List<Loop> children = new List<Loop>();
        public Stem parentStem;
        public List<Stem> childStems = new List<Stem>();
        public double parentDirection;
        public List<double> childDirections = new List<double>();
        public KeyValuePair<double, double> centre;
        public double radius;
        public int numBases;
        public double perimLength;
        public double angle;
    }
}