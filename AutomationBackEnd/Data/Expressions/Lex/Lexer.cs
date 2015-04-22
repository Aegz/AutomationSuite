﻿using AutomationSuite.Expressions.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutomationSuite.Expressions.Lex
{
    class Lexer
    {
        SlidingTextWindow oWindow;

        private LexingContext Context { get; set; }


        public Lexer(String xsGivenText)
        {
            oWindow = new SlidingTextWindow(xsGivenText);
        }


        /// <summary>
        /// Catches when a keyword starting char is located (eg. <)
        /// </summary>
        public ExpressionOrConst GenerateExpressionTree()
        {
            // Initialise the context now
            Context = new LexingContext();

            // Iterate through the window
            while (oWindow.HasCharactersLeftToProcess())
            {
                // Peek a character
                char cNextItem = oWindow.PeekCharacter();

                // Add to Context

                // Switch case for maintainability (if more keyword operators are added)
                switch (cNextItem)
                {
                    // Only catch < then determine if it is a keyword or a string
                    case '<':
                        // Call Keyword specific function to determine what we actually have
                        InterpretPossibleKeyword();
                        break;
                    default:
                        // Cache this character
                        oWindow.CacheCharacter();
                        break;
                }
            }

            // Finish off by flushing out the buffer
            GenerateConstantExpressionFromBuffer();

            return Context.RootNode;
        }

        private void InterpretPossibleKeyword()
        {
            // Move anything that is possibly a new expression into the 
            // Current Expression buffer

            // <
            Context.CurrentExpression.Enqueue(oWindow.PopCharacter());
            
            // Next Char
            char cNextItem = oWindow.PopCharacter();
            // 
            Context.CurrentExpression.Enqueue(cNextItem);

            // 1. Determine if this is a constant or a keyword
            switch (cNextItem)
            {
                // keyword, Try and determine the type of Expression
                case '$':
                    // Flush out the buffer now that we nave a new expression
                    GenerateConstantExpressionFromBuffer();

                    // Generate that new expression
                    GenerateExpression();
                    break;
                case '/':
                    // Check if we have a close to a keyword
                    char cNextNextItem = oWindow.PeekCharacter();

                    // We have a closing item
                    if (cNextNextItem.Equals('$'))
                    {
                        // Scan ahead and see if the tag matches the current element
                        String sScanAhead = oWindow.ScanAheadForCharacters(new char[] { '>' });

                        // If we have a closing tag for this parent
                        if (Context.CurrentNode.Parent != null && sScanAhead.Contains(Context.CurrentNode.Tag))
                        {
                            // Flush out the buffer now that we have a keyword
                            GenerateConstantExpressionFromBuffer();

                            // skip ahead to get rid of the closing tag
                            oWindow.SkipAhead(sScanAhead.Length);

                            // We need to move the context back up one
                            if (Context.CurrentNode.Parent != null)
                            {
                                Context.CurrentNode = Context.CurrentNode.Parent;
                            }
                        }
                        else
                        {
                            // Do nothing..
                        }
                    }
                    else
                    {
                        //
                        oWindow.EnqueueInCache(Context.CurrentExpression);

                        // Probably a HTML expression
                        break;
                    }

                    break;
                default:
                    // String, just return a ConstExpression
                    break;
            }

        }

        private void GenerateExpression()
        {
            // Default the value to false
            Boolean bExpressionFound = false;

            // Look ahead and see if we can find a /> or >
            while (oWindow.HasCharactersLeftToProcess() && !bExpressionFound)
            {
                // Get the character out into an intermediate var
                char cNextChar = oWindow.PopCharacter();
                // Add to our buffer
                Context.CurrentExpression.Enqueue(cNextChar);

                // Switch on the type of char
                switch (cNextChar)
                {
                    case '/':
                        // /> Something simpler
                        // Check if we have a close to a keyword
                        char cNextNextItem = oWindow.PeekCharacter();

                        // We have a closing item
                        if (cNextNextItem.Equals('>'))
                        {
                            // Flush out the > character
                            oWindow.PopCharacter();

                            // Generate the expression as a single child obj
                            Context.CurrentNode.AddChild(InterpretKeywordAndGenerateExpr());

                            // Say that we found an expression
                            bExpressionFound = true;
                        }
                        else
                        {
                            // Not the one we wanted, so we keep going
                            break;
                        }
                        break;
                    case '>':
                        // > BlockExpression

                        // 1. Generate a new expression depending on what text we got
                        ExpressionOrConst oTemp = InterpretKeywordAndGenerateExpr();
                        // 2. add it to the tree (change the current node to this since its a block)
                        Context.CurrentNode.AddChild(oTemp);
                        // 3. Change the context/current node
                        Context.CurrentNode = oTemp;

                        bExpressionFound = true;
                        break;
                }
            }

            // Nothing Found
            if (!bExpressionFound)
            {
                // Just cache it all because we clearly have no values
                oWindow.EnqueueInCache(Context.CurrentExpression);
            }

        }

        private ExpressionOrConst InterpretKeywordAndGenerateExpr()
        {
            // Join the current expression into a single string for processing
            String sKeyword = String.Join("", Context.CurrentExpression);
            // Clear the queue to be safe
            Context.CurrentExpression.Clear();

            // 1. Trim the text so we can get ride of the <$, </$ and />, >
            sKeyword = sKeyword.Trim(new char[] { '<', '/', '$', '>', ' ' });

            // 2. If it is a keyword we know
            switch (sKeyword)
            {
                // Block Expression
                case "FOREACH(COLUMN)":
                    return new ForEachExpression(sKeyword, ForEachType.feColumn);
                case "FOREACH(ROW)":
                    return new ForEachExpression(sKeyword, ForEachType.feRow);
                // Directories
                case "WORKINGDIR(ALL)":
                    // Get a path from the config
                    return new ConstExpression("WorkingDir ALL");
                case "WORKINGDIR(JOB)":
                case "WORKINGDIR(THIS)":
                    // Get a path from this job
                    return new ConstExpression("WorkingDir JOB");
                default:
                    // 3. Is it a coordinate?
                        // 3a. Is it a single number
                        // 3b. Is it a coordinate (X,X)
                    return new VariableUseExpression(sKeyword);
            }
        }

        private void GenerateConstantExpressionFromBuffer()
        {
            // Intermediate var
            String sCurrentBufferText = oWindow.GetCachedText();

            // If there is anything to add
            if (!String.IsNullOrWhiteSpace(sCurrentBufferText) && sCurrentBufferText.Length > 0)
            {
                Context.CurrentNode.AddChild(new ConstExpression(sCurrentBufferText));
            }
        }

    }
}