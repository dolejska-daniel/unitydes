using System;

namespace UnityDES.Utils
{
    /// <summary>
    /// Pseudo-random number generator for values with exponential distribution.
    /// </summary>
    /// Source: https://www.fit.vutbr.cz/~peringer/SIMLIB/
    public class RandomExponential : RandomBase
    {
        /// <summary>
        /// Mean value of the exponential distribution.
        /// </summary>
        public readonly double Mean;

        /// <summary>
        /// Initializes the pseudo-random number generator for exponential distribution with given <paramref name="mean"/> value.
        /// </summary>
        /// 
        /// <param name="mean">Mean value of the distribution</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        public RandomExponential(double mean, Random generator = null) : base(generator)
        {
            Mean = mean;
        }

        /// <inheritdoc cref="NextDouble()"/>
        public override int Next() => Next((int)Mean, Generator);

        /// <inheritdoc cref="NextDouble(double, Random)"/>
        public static int Next(int mean, Random generator = null)
            => (int)NextDouble(mean, generator);

        /// <summary>
        /// Generates pseudo-random value of exponential distribution.
        /// </summary>
        /// 
        /// <returns>Pseudo-random number of exponential distribution</returns>
        public override double NextDouble() => NextDouble(Mean, Generator);

        /// <summary>
        /// Generates pseudo-random value of exponential distribution with given <paramref name="mean"/> value.
        /// </summary>
        /// 
        /// <param name="mean">Mean value of the distribution</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        /// <returns>Pseudo-random number of exponential distribution</returns>
        public static double NextDouble(double mean, Random generator = null)
            => -mean * Math.Log((generator ?? new Random()).NextDouble());
    }
}
