/*

The MIT License (MIT)

Copyright (c) 2015-2017 Secret Lab Pty. Ltd. and Yarn Spinner contributors.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

namespace Yarn.Unity.Example {
    /// Displays dialogue lines to the player, and sends
    /// user choices back to the dialogue system.

    /** Note that this is just one way of presenting the
     * dialogue to the user. The only hard requirement
     * is that you provide the RunLine, RunOptions, RunCommand
     * and DialogueComplete coroutines; what they do is up to you.
     */
    public class ClassicDialogueUI : Yarn.Unity.DialogueUIBehaviour
    {
        // Ropework specific stuff
        public Ropework.RopeworkManager ropework;
        public Text nameText;

        /// The object that contains the dialogue and the options.
        /** This object will be enabled when conversation starts, and 
         * disabled when it ends.
         */
        public GameObject dialogueContainer;

        /// The UI element that displays lines
        public Text lineText;

        /// A UI element that appears after lines have finished appearing
        public GameObject continuePrompt;
        ContinueBump bump;

        /// A delegate (ie a function-stored-in-a-variable) that
        /// we call to tell the dialogue system about what option
        /// the user selected
        private Yarn.OptionChooser SetSelectedOption;

        /// How quickly to show the text, in seconds per character
        [Tooltip("How quickly to show the text, in seconds per character")]
        public float textSpeed = 0.025f;

        /// The buttons that let the user choose an option
        public List<Button> optionButtons;

        public List<Text> buttonText;

        //public Image highlight;

        int highlightOption = 0;

        float accidentPrevention = 0; //Stops options from being selected accidentally

        /// Make it possible to temporarily disable the controls when
        /// dialogue is active and to restore them when dialogue ends
        public RectTransform gameControlsContainer;

        bool openToOptions = false; //Whether or not the program is waiting on an option to be selected.
        int optionCount = 0;

        void Awake ()
        {
            bump = continuePrompt.GetComponent<ContinueBump>();

            // if Ropework manager is null, then find it
            if ( ropework == null ) { ropework = FindObjectOfType<Ropework.RopeworkManager>(); }

            // Start by hiding the container, line and option buttons
            if (dialogueContainer != null)
                dialogueContainer.SetActive(false);

            lineText.gameObject.SetActive (false);
            //highlight.gameObject.SetActive(false);
            foreach (var button in optionButtons) {
                button.gameObject.SetActive (false);
            }

            // Hide the continue prompt if it exists
            if (continuePrompt != null)
                continuePrompt.SetActive (false);
        }

        /// Show a line of dialogue, gradually
        public override IEnumerator RunLine (Yarn.Line line)
        {
            // Show the text
            lineText.gameObject.SetActive (true);

            // ROPEWORK SPECIFIC STUFF:
            // Extract speaker's name, if possible
            string speakerName = "";
            string lineTextDisplay = line.text;
            if ( line.text.Contains(":") ) { // if there's a ":" separator, then identify the first part as a speaker
                var splitLine = line.text.Split( new char[] {':'}, 2); // but only split once
                speakerName = splitLine[0].Trim();
                lineTextDisplay = splitLine[1].Trim();
            }
            
            if ( speakerName.Length > 0 ) {
                // change dialog nameplate text and, if applicable the BG color
                nameText.transform.parent.gameObject.SetActive(true);
                nameText.text = speakerName;
                //if ( ropework.actorColors.ContainsKey(speakerName) ) {
                    //nameText.transform.parent.GetComponent<Image>().color = ropework.actorColors[speakerName];
               // }
                // Highlight actor's sprite (if on-screen) using RopeworkManager
                if ( ropework.actors.ContainsKey(speakerName) ) {
                    ropework.HighlightSprite( ropework.actors[speakerName] );
                }
            } else { // no speaker name found, so hide the nameplate
                nameText.transform.parent.gameObject.SetActive(false);
            }

            // display dialog
            if (textSpeed > 0.0f) {
                // Display the line one character at a time
                var stringBuilder = new StringBuilder ();

                bool earlyOut = false;
                yield return 0; // give time for previous Input.anyKeyDown event to become false

                bool informat = false; //If the text being parsed is formatting text
                bool formatPrimed = false; //If the text being parsed has passed the first > in the formatted block

                foreach (char c in lineTextDisplay) {

                    if (c.Equals('<') && !informat) //Checks for open format marks, if so, display all the formatted text at once to avoid showing format marks.
                    {
                        informat = true;
                        formatPrimed = false;
                    }

                    stringBuilder.Append(c);

                    if (!informat)
                    {
                        lineText.text = stringBuilder.ToString();
                        yield return new WaitForSeconds(textSpeed);
                    }

                    if (c.Equals('>'))
                    {
                        if (formatPrimed)
                            informat = false;
                        else
                            formatPrimed = true;
                    }

                    float timeWaited = 0f;
                    while ( timeWaited < textSpeed ) {
                        timeWaited += Time.deltaTime;
                        // early out / skip ahead
                        if ( Input.anyKeyDown ) {
                            lineText.text = lineTextDisplay;
                            earlyOut = true;
                        }
                        yield return 0;
                    }
                    if ( earlyOut ) { break; }

                }
            } else {
                // Display the line immediately if textSpeed == 0
                lineText.text = lineTextDisplay;
            }

            // Show the 'press any key' prompt when done, if we have one
            if (continuePrompt != null)
            {
                continuePrompt.SetActive(true);
                bump.bump();
            }

            // Wait for any user input
            while (Input.anyKeyDown == false) {
                yield return null;
            }

            // Hide the text and prompt
            // lineText.gameObject.SetActive (false); // commented this out, so that the last message still displays while making choices

            if (continuePrompt != null)
                continuePrompt.SetActive (false);

        }

        /// Show a list of options, and wait for the player to make a selection.
        public override IEnumerator RunOptions (Yarn.Options optionsCollection, 
                                                Yarn.OptionChooser optionChooser)
        {
            // Do a little bit of safety checking
            if (optionsCollection.options.Count > optionButtons.Count) {
                Debug.LogWarning("There are more options to present than there are" +
                                 "buttons to present them in. This will cause problems.");
            }

            // Display each option in a button, and make it visible
            openToOptions = true;
            //highlight.gameObject.SetActive(true);
            highlightOption = 0;
            accidentPrevention = 0;
            int i = 0;
            foreach (var optionString in optionsCollection.options) {
                optionButtons [i].gameObject.SetActive (true);
                optionButtons [i].GetComponentInChildren<Text> ().text = optionString;
                i++;
            }
            optionCount = i;

            // Record that we're using it
            SetSelectedOption = optionChooser;

            //highlight.transform.position = optionButtons[highlightOption].transform.position;

            // Wait until the chooser has been used and then removed (see SetOption below)
            while (SetSelectedOption != null) {
                if (accidentPrevention < 0.5f)
                {
                    accidentPrevention += Time.deltaTime;
                }
                //highlight.transform.position = optionButtons[highlightOption].transform.position;
                for(int x = 0; x < buttonText.Count; x++)
                {
                    if(x == highlightOption)
                    {
                        //optionButtons[x].image.color = new Color(0.56078f, 0.50196f, 0.92549f);
                        ColorBlock cb = optionButtons[x].colors;
                        cb.normalColor = new Color(0.1553724f, 0.137104f, 0.745283f);
                        //cb.normalColor = Color.red;
                        optionButtons[x].colors = cb;
                        buttonText[x].color = Color.white;
                    }
                    else
                    {
                        //optionButtons[x].image.color = Color.white;
                        ColorBlock cb = optionButtons[x].colors;
                        cb.normalColor = Color.white;
                        optionButtons[x].colors = cb;
                        buttonText[x].color = Color.black;
                    }
                }
                //highlight.gameObject.SetActive(true);
                yield return null;
            }

            // Hide all the buttons
            openToOptions = false;
            //highlight.gameObject.SetActive(false);
            foreach (var button in optionButtons) {
                button.gameObject.SetActive (false);
            }
        }

        /// Called by buttons to make a selection.
        public void SetOption (int selectedOption)
        {
            if (openToOptions)
            {
                // Call the delegate to tell the dialogue system that we've
                // selected an option.
                SetSelectedOption(selectedOption);

                // Now remove the delegate so that the loop in RunOptions will exit
                SetSelectedOption = null;
            }
        }

        public void ScrollOption (int scroll)
        {
            if (openToOptions)
            {
                if (accidentPrevention > 0.4f)
                {
                    if (scroll == 0)
                    {
                        // Call the delegate to tell the dialogue system that we've
                        // selected an option.
                        SetSelectedOption(highlightOption);

                        // Now remove the delegate so that the loop in RunOptions will exit
                        SetSelectedOption = null;
                    }
                    else
                    {
                        highlightOption += scroll;
                        if (highlightOption >= optionCount)
                        {
                            highlightOption = 0;
                        }
                        else if (highlightOption < 0)
                        {
                            highlightOption = optionCount - 1;
                        }
                        //highlight.transform.position = optionButtons[highlightOption].transform.position;
                    }
                }
            }
        }

        /// Run an internal command.
        public override IEnumerator RunCommand (Yarn.Command command)
        {
            // "Perform" the command
            Debug.Log ("Command: " + command.text);

            yield break;
        }

        /// Called when the dialogue system has started running.
        public override IEnumerator DialogueStarted ()
        {
            Debug.Log ("Dialogue starting!");

            // Enable the dialogue controls.
            if (dialogueContainer != null)
                dialogueContainer.SetActive(true);

            // Hide the game controls.
            if (gameControlsContainer != null) {
                gameControlsContainer.gameObject.SetActive(false);
            }

            yield break;
        }

        /// Called when the dialogue system has finished running.
        public override IEnumerator DialogueComplete ()
        {
            Debug.Log ("Complete!");

            // Hide the dialogue interface.
            if (dialogueContainer != null)
                dialogueContainer.SetActive(false);

            // Show the game controls.
            if (gameControlsContainer != null) {
                gameControlsContainer.gameObject.SetActive(true);
            }

            yield break;
        }

        public void closePortraits()
        {

        }

    }

}
