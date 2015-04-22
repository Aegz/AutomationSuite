using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationService.Data.Expressions.Lex
{
    /// <summary>
    /// A simple encapsulated class designed to 
    /// </summary>
    class SlidingTextWindow
    {
        // Queue up all text
        List<Char> acAllText;

        // buffer used to enable rollbacks where necessary
        List<Char> acBuffer;

        int iCurrentPosition = 0;

        public SlidingTextWindow(String xsAllText)
        {
            acAllText = new List<char>(xsAllText);
            acBuffer = new List<char>();
        }

        public char PeekCharacter()
        {
            // If there is actually anything left to peek
            if (iCurrentPosition < acAllText.Count)
            {
                return acAllText[iCurrentPosition];
            }
            else
            {
                throw new Exception("Sliding Window out of bounds");
            }

        }

        public char PopCharacter()
        {
            // Get the character out
            char cTemp = PeekCharacter();
            // Move our index along
            iCurrentPosition++;
            // return the character
            return cTemp;
        }

        public void CacheCharacter()
        {
            acBuffer.Add(PopCharacter());
        }

        public String GetCachedText()
        {
            // Simply return the list as a string
            String sReturn = String.Join("", acBuffer);

            // Flush out the buffer
            acBuffer.Clear();

            return sReturn;
        }

        public Boolean HasCharactersLeftToProcess()
        {
            return iCurrentPosition < acAllText.Count;
        }

        public String ScanAheadForCharacters(char[] xacCharactersToLookOutFor)
        {
            // Initialise return var
            String sReturn = "";

            // Loop through the remaining text
            for (int iIndex = 0; iIndex < acAllText.Count - iCurrentPosition; iIndex++ )
            {
                // Intermediate var
                char cCurrentItem = acAllText[iCurrentPosition + iIndex];
                // Add the character to the return text
                sReturn += cCurrentItem;
                // If we find the item we want
                if (xacCharactersToLookOutFor.Contains(cCurrentItem))
                {
                    // return immediately
                    return sReturn;
                }
            }

            // Default to empty/invalid
            return String.Empty;
        }

        public void SkipAhead(int xiLength)
        {
            iCurrentPosition += xiLength;
        }

        public void EnqueueInCache(IEnumerable<char> xacGiven)
        {
            acBuffer.AddRange(xacGiven);
        }
    }
}
