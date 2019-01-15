namespace OCRReader.Classes
{
    /// <summary>
    ///   Class to store Confidence Results
    /// </summary>
    internal class ConfidenceResult
    {
        /// <summary>
        ///   Result of Hammin Distance Calculation
        /// </summary>
        public int Hamming { get; set; }

        /// <summary>
        ///   Result of Dice Coefficient Calculation
        /// </summary>
        public int DiceCoeff { get; set; }
    }
}