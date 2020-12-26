using System;

namespace UnityDES.Utils
{
    /// <summary>
    /// Pseudo-random number generator for values with Poisson distribution.
    /// </summary>
    /// Source: https://www.fit.vutbr.cz/~peringer/SIMLIB/
    public class RandomPoisson : RandomBase
    {
        /// <summary>
        /// Mean value and variance of the Poisson distribution.
        /// </summary>
        public readonly double Lambda;

        /// <summary>
        /// Initializes the pseudo-random number generator for Poisson distribution with given mean value and variance (<paramref name="lambda"/>).
        /// </summary>
        /// 
        /// <param name="lambda">Mean value and variance of the distribution</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        public RandomPoisson(double lambda, Random generator = null) : base(generator)
        {
            if (lambda <= 0d)
                throw new ArgumentOutOfRangeException("Lambda cannot be less or equal to 0.");

            Lambda = lambda;
        }

        /// <summary>
        /// Generates pseudo-random value of Poisson distribution.
        /// </summary>
        /// 
        /// <returns>Pseudo-random number of Poisson distribution</returns>
        public override int Next() => Next(Lambda, Generator);

        /// <summary>
        /// Generates pseudo-random value of Poisson distribution with given mean value and variance (<paramref name="lambda"/>).
        /// </summary>
        /// 
        /// <param name="lambda">Mean value and variance of the distribution</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        /// <returns>Pseudo-random number of Poisson distribution</returns>
        public static int Next(double lambda, Random generator = null)
        {
            generator ??= new Random();
            int result = 0;

            if (lambda <= 9.0)
            {
                var y = Math.Exp(-lambda);
                var x = 1.0;

                do
                {
                    x *= generator.NextDouble();
                    if (x < y) break;
                    result++;
                }
                while (true);
            }
            else
            {
                double sl = Math.Sqrt(lambda);

                do
                {
                    result = (int)(RandomNormal.NextDouble(lambda, sl, generator) + 0.5);
                }
                while (result < 0);
            }

            return result;
        }

        /// <inheritdoc cref="Next()"/>
        public override double NextDouble() => Next();

        /// <inheritdoc cref="Next(double, Random)"/>
        public static double NextDouble(double lambda, Random generator = null)
            => Next((int)lambda, generator);
    }
}
