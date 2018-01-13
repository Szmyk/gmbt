using System.Text;

namespace GMBT
{
    /// <summary> 
    /// Represents game's call arguments.
    /// </summary>
    public class GothicArguments
    {
        private readonly StringBuilder content = new StringBuilder();

        /// <summary> 
        /// Adds an argument. 
        /// </summary>
        /// <param name="argument">Argument's name.</param>
        public void Add(string argument)
        {
            content.AppendFormat("-{0} ", argument);
        }

        /// <summary> 
        /// Adds an argument with a value. 
        /// </summary>
        /// <param name="argument">Argument's name.</param>
        /// <param name="value">Argument's value</param>
        public void Add (string argument, string value)
        {
            content.AppendFormat("-{0}:{1} ", argument, value);
        }

        /// <summary> 
        /// Returns ready to work string of arguments.
        /// </summary>
        public override string ToString ()
        {
            return content.ToString();
        }
    }  
}
