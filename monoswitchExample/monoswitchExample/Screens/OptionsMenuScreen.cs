#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace monoswitchExample
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {

        #region members

            #region public

            #endregion

            #region protected

                protected MenuEntry m_ungulateMenuEntry;
                protected MenuEntry m_languageMenuEntry;
                protected MenuEntry m_frobnicateMenuEntry;
                protected MenuEntry m_elfMenuEntry;
                protected static Ungulate s_currentUngulate = Ungulate.Dromedary;
                protected static string[] s_languages = { "C#", "French", "Deoxyribonucleic acid" };
                protected static int s_currentLanguage = 0;
                protected static bool s_frobnicate = true;
                protected static int s_elf = 23;

                protected enum Ungulate
                {
                    BactrianCamel,
                    Dromedary,
                    Llama,
                }

            #endregion

            #region private

            #endregion

        #endregion

        #region properties

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region events

            #region public

            #endregion

            #region protected

            #endregion

            #region private

            #endregion

        #endregion

        #region functions

            #region public

                /// <summary>
                /// Constructor.
                /// </summary>
                public OptionsMenuScreen() : base("Options")
                {
                    // Create our menu entries.
                    this.m_ungulateMenuEntry = new MenuEntry(string.Empty);
                    this.m_languageMenuEntry = new MenuEntry(string.Empty);
                    this.m_frobnicateMenuEntry = new MenuEntry(string.Empty);
                    this.m_elfMenuEntry = new MenuEntry(string.Empty);
                    SetMenuEntryText();
                    MenuEntry back = new MenuEntry("Back");
                    // Hook up menu event handlers.
                    this.m_ungulateMenuEntry.Selected += UngulateMenuEntrySelected;
                    this.m_languageMenuEntry.Selected += LanguageMenuEntrySelected;
                    this.m_frobnicateMenuEntry.Selected += FrobnicateMenuEntrySelected;
                    this.m_elfMenuEntry.Selected += ElfMenuEntrySelected;
                    back.Selected += OnCancel;
                    // Add entries to the menu.
                    this.menuEntries.Add(this.m_ungulateMenuEntry);
                    this.menuEntries.Add(this.m_languageMenuEntry);
                    this.menuEntries.Add(this.m_frobnicateMenuEntry);
                    this.menuEntries.Add(this.m_elfMenuEntry);
                    menuEntries.Add(back);
                }



            #endregion

            #region protected

                /// <summary>
                /// Fills in the latest values for the options screen menu text.
                /// </summary>
                protected void SetMenuEntryText()
                {
                    this.m_ungulateMenuEntry.text = "Preferred ungulate: " + OptionsMenuScreen.s_currentUngulate;
                    this.m_languageMenuEntry.text = "Language: " + OptionsMenuScreen.s_languages[OptionsMenuScreen.s_currentLanguage];
                    this.m_frobnicateMenuEntry.text = "Frobnicate: " + (OptionsMenuScreen.s_frobnicate ? "on" : "off");
                    this.m_elfMenuEntry.text = "elf: " + OptionsMenuScreen.s_elf;
                }

                /// <summary>
                /// Event handler for when the Ungulate menu entry is selected.
                /// </summary>
                protected void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    OptionsMenuScreen.s_currentUngulate++;
                    if (OptionsMenuScreen.s_currentUngulate > Ungulate.Llama)
                    {
                        OptionsMenuScreen.s_currentUngulate = 0;
                    }
                    SetMenuEntryText();
                }

                /// <summary>
                /// Event handler for when the Language menu entry is selected.
                /// </summary>
                protected void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    OptionsMenuScreen.s_currentLanguage = (OptionsMenuScreen.s_currentLanguage + 1) % OptionsMenuScreen.s_languages.Length;
                    SetMenuEntryText();
                }

                /// <summary>
                /// Event handler for when the Frobnicate menu entry is selected.
                /// </summary>
                void FrobnicateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    OptionsMenuScreen.s_frobnicate = !OptionsMenuScreen.s_frobnicate;
                    SetMenuEntryText();
                }

                /// <summary>
                /// Event handler for when the Elf menu entry is selected.
                /// </summary>
                void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    OptionsMenuScreen.s_elf++;
                    SetMenuEntryText();
                }

            #endregion

            #region private

            #endregion

        #endregion

    }
}
