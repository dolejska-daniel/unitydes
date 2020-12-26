using System;

namespace UnityDES.Utils
{
    /// <summary>
    /// Pseudo-random number generator for values with logarithmic distribution.
    /// </summary>
    /// Source: https://www.fit.vutbr.cz/~peringer/SIMLIB/
    public class RandomLogarithimc : RandomBase
    {
        /// <summary>
        /// Mean value of the logarithmic distribution.
        /// </summary>
        public readonly double Mean;

        /// <summary>
        /// Variance of the logarithmic distribution.
        /// </summary>
        public readonly double Variance;

        /// <summary>
        /// Initializes the pseudo-random number generator for logarithmic distribution with given <paramref name="mean"/> value and <paramref name="variance"/>.
        /// </summary>
        /// 
        /// <param name="mean">Mean value of the distribution</param>
        /// <param name="variance">Variance of the distribution</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        public RandomLogarithimc(double mean, double variance, Random generator = null)
        {
            if (variance <= 0)
                throw new ArgumentOutOfRangeException("Variance cannot be less or equal to 0.");

            Mean = mean;
            Variance = variance;
        }


        /// <inheritdoc cref="NextDouble()"/>
        public override int Next() => Next((int)Mean, (int)Variance, Generator);

        /// <inheritdoc cref="NextDouble(double, double, Random)"/>
        public static int Next(int mean, int variance, Random generator = null)
            => (int)NextDouble(mean, variance, generator);

        /// <summary>
        /// Generates pseudo-random value of logarithmic distribution.
        /// </summary>
        /// 
        /// <returns>Pseudo-random number of logarithmic distribution</returns>
        public override double NextDouble() => NextDouble(Mean, Variance, Generator);

        /// <summary>
        /// Generates pseudo-random value of logarithmic distribution with given <paramref name="mean"/> and <paramref name="variance"/>.
        /// </summary>
        /// 
        /// <param name="mean">Mean value of the distribution</param>
        /// <param name="variance">Variance of the distribution</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        /// <returns>Pseudo-random number of logarithmic distribution</returns>
        public static double NextDouble(double mean, double variance, Random generator = null)
            => Math.Exp(RandomNormal.NextDouble(mean, variance, generator));
    }
}
