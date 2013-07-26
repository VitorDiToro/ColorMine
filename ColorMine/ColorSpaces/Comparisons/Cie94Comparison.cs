﻿using System;

namespace ColorMine.ColorSpaces.Comparisons
{
    /// <summary>
    /// Implements the Cie94 method of delta-e: http://en.wikipedia.org/wiki/Color_difference#CIE94
    /// </summary>
    public class Cie94Comparison : IColorSpaceComparison
    {
        /// <summary>
        /// Application type defines constants used in the Cie94 comparison
        /// </summary>
        public enum Application
        {
            GraphicArts,
            Textiles
        }

        /// <summary>
        /// Create new Cie94Comparison. Defaults to GraphicArts application type.
        /// </summary>
        public Cie94Comparison()
        {
            Constants = new ApplicationConstants(Application.GraphicArts);
        }

        private ApplicationConstants Constants { get; set; }
        /// <summary>
        /// Create new Cie94Comparison for specific application type.
        /// </summary>
        /// <param name="application"></param>
        public Cie94Comparison(Application application)
        {
            Constants = new ApplicationConstants(application);
        }

        /// <summary>
        /// Compare colors using the Cie94 algorithm. The first color (a) will be used as the reference color.
        /// </summary>
        /// <param name="a">Reference color</param>
        /// <param name="b">Comparison color</param>
        /// <returns></returns>
        public double Compare(IColorSpace a, IColorSpace b)
        {
            var labA = a.To<Lab>();
            var labB = b.To<Lab>();

            var deltaL = labA.L - labB.L;
            var deltaA = labA.A - labB.A;
            var deltaB = labA.B - labB.B;

            var c1 = Math.Sqrt(Math.Pow(labA.A, 2) + Math.Pow(labA.B, 2));
            var c2 = Math.Sqrt(Math.Pow(labB.A, 2) + Math.Pow(labB.B, 2));
            var deltaC = c1 - c2;

            var deltaH = Math.Pow(deltaA,2) + Math.Pow(deltaB,2) - Math.Pow(deltaC,2);
            deltaH = deltaH < 0 ? 0 : Math.Sqrt(deltaH);

            const double sl = 1.0;
            const double kc = 1.0;
            const double kh = 1.0;

            var sc = 1.0 + Constants.K1*c1;
            var sh = 1.0 + Constants.K2*c1;

            var i = Math.Pow(deltaL/(Constants.Kl*sl), 2) +
                    Math.Pow(deltaC/(kc*sc), 2) +
                    Math.Pow(deltaH/(kh*sh), 2);
            return i < 0 ? 0 : Math.Sqrt(i);
        }

        private class ApplicationConstants
        {
            internal double Kl { get; private set; }
            internal double K1 { get; private set; }
            internal double K2 { get; private set; }

            public ApplicationConstants(Application application)
            {
                switch (application)
                {
                    case Application.GraphicArts:
                        Kl = 1.0;
                        K1 = .045;
                        K2 = .015;
                        break;
                    case Application.Textiles:
                        Kl = 2.0;
                        K1 = .048;
                        K2 = .014;
                        break;
                    default:
                        throw new ArgumentException("Application type not supported");
                }
            }
        }
    }
}
