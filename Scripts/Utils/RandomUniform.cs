using System;

namespace UnityDES.Utils
{
    /// <summary>
    /// Pseudo-random number generator for values with uniform distribution.
    /// </summary>
    public class RandomUniform : RandomBase
    {
        /// <summary>
        /// Lower bound of the distribution values (inclusive).
        /// </summary>
        public readonly double ValueMin;

        /// <summary>
        /// Upper bound of the distribution values (inclusive).
        /// </summary>
        public readonly double ValueMax;

        /// <summary>
        /// Initializes the pseudo-random number generator for uniform distribution with given lower (<paramref name="valueMin"/>) and upper (<paramref name="valueMax"/>) bound.
        /// </summary>
        /// 
        /// <param name="valueMin">Lower bound of the distribution values (inclusive)</param>
        /// <param name="valueMax">Upper bound of the distribution values (inclusive)</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        public RandomUniform(double valueMin, double valueMax, Random generator = null) : base(generator)
        {
            if (ValueMin >= ValueMax)
                throw new ArgumentOutOfRangeException("Uniform distribution interval is invalid (From >= To).");

            ValueMin = valueMin;
            ValueMax = valueMax;
        }

        /// <summary>
        /// Generates pseudo-random value of uniform distribution.
        /// </summary>
        /// 
        /// <returns>Pseudo-random number of uniform distribution</returns>
        public override int Next() => Next((int)ValueMin, (int)ValueMax, Generator);

        /// <summary>
        /// Generates pseudo-random value of uniform distribution from [<paramref name="valueMin"/>, <paramref name="valueMax"/>] interval.
        /// </summary>
        /// 
        /// <param name="valueMin">Lower bound of the distribution values (inclusive)</param>
        /// <param name="valueMax">Upper bound of the distribution values (inclusive)</param>
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        /// <returns>Pseudo-random number of uniform distribution</returns>
        public static int Next(int valueMin, int valueMax, Random generator = null)
            => (generator ?? new Random()).Next(valueMin, valueMax + 1);

        /// <inheritdoc cref="Next()"/>
        public override double NextDouble() => NextDouble(ValueMin, ValueMax, Generator);

        /// <inheritdoc cref="Next(double, double, Random)"/>
        public static double NextDouble(double valueMin, double valueMax, Random generator = null)
            => valueMin + (valueMax - valueMin) * (generator ?? new Random()).NextDouble();
    }
}
