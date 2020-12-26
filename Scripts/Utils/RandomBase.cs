using System;

namespace UnityDES.Utils
{
    /// <summary>
    /// Base class of all pseudo-random distribution generators.
    /// </summary>
    public abstract class RandomBase
    {
        /// <summary>
        /// Pseudo-random number generator instance.
        /// </summary>
        public Random Generator { get; set; }

        /// <summary>
        /// Initializes the base class.
        /// </summary>
        /// 
        /// <param name="generator">Custom instance of pseudo-random generator</param>
        public RandomBase(Random generator = null)
        {
            Generator = generator ?? new Random();
        }

        /// <summary>
        /// Generates pseudo-random value of given distribution.
        /// </summary>
        /// 
        /// <returns>Pseudo-random value of given distribution</returns>
        public abstract int Next();

        /// <inheritdoc cref="Next"/>
        public abstract double NextDouble();
    }
}
