﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FftSharp.Windows
{
    public class Tukey : Window, IWindow
    {
        private readonly double Alpha;

        public override string Name => "Tukey";
        public override string Description =>
            "A Tukey window has a flat center and tapers at the edges according to a cosine function. " +
            "The amount of taper is defined by alpha (with low values being less taper). " +
            "Tukey windows are ideal for analyzing transient data since the amplitude of transient signal " +
            "in the time domain is less likely to be altered compared to using Hanning or flat top.";

        public Tukey()
        {
            Alpha = .5;
        }

        public Tukey(double alpha = .5)
        {
            Alpha = alpha;
        }

        public override double[] Create(int size, bool normalize = false)
        {
            double[] window = new double[size];

            double m = 2 * Math.PI / (Alpha * (size - 1));

            int edgeSizePoints = (int)(size * Alpha / 2);

            if (size % 2 == 0)
                edgeSizePoints += 1;

            for (int i = 0; i < size; i++)
            {
                if (i < edgeSizePoints)
                {
                    // left edge
                    window[i] = (1 - Math.Cos(i * m)) / 2;
                }
                else if (i >= size - edgeSizePoints)
                {
                    // right edge
                    window[i] = (1 - Math.Cos(i * m)) / 2;
                }
                else
                {
                    window[i] = 1;
                }
            }

            if (normalize)
                NormalizeInPlace(window);

            return window;
        }
    }
}
