﻿using Murder.Services;
using Murder.Utilities;
using System;

namespace Murder.Core.Input
{
    public struct MenuInfo
    {
        public int PreviousSelection;
        public int Selection;
        public float LastMoved;
        public float LastPressed;
        public bool Canceled;
        public bool Disabled = false;
        public int Overflow = 0;

        /// <summary>
        /// NUmber of options in this menu
        /// </summary>
        public int Length => Options.Length;

        public MenuOption[] Options = new MenuOption[0];
        public bool HasOptions => Options != null && Options.Length>0;

        public float LargestOptionText
        {
            get
            {
                float largestTextWidth = 0;
                foreach (var item in Options)
                {
                    largestTextWidth = Math.Max(MurderFonts.PixelFont.GetLineWidth(item.Text), largestTextWidth);
                }
                return largestTextWidth;
            }
        }

        public MenuInfo(params MenuOption[] options)
        {
            Options = options;
        }

        public MenuInfo(params string[] options)
        {
            Options = options.Select(s => new MenuOption(s, true)).ToArray();
        }
        public MenuInfo(IEnumerable<string> options)
        {
            Options = options.Select(s => new MenuOption(s, true)).ToArray();
        }

        public MenuInfo(int size)
        {
            Options = new MenuOption[size];
            for (int i = 0; i < size; i++)
            {
                size = new();
            }
        }

        public void Clamp(int max)
        {
            Selection = Math.Max(0, Math.Min(Selection, max));
        }

        public MenuInfo Disable(bool disabled)
        {
            Disabled = disabled;
            return this;
        }

        public void SnapRight(int width)
        {
            Selection = (Calculator.FloorToInt(Selection / width) + 1) * width-1;
        }

        public void SnapLeft(int width)
        {
            Selection = (Calculator.FloorToInt(Selection / width)) * width;
        }

        public string GetSelectedOptionText()
        {
            return Options[Selection].Text;
        }
        public string GetOptionText(int index)
        {
            return Options[index % Options.Length].Text;
        }

        internal bool IsEnabled(int index)
        {
            return Options[index % Options.Length].Enabled;
        }
        
        public bool IsOptionAvailable(int option)
        {
            if (Options is null)
            {
                return true;
            }

            if (Options.Length <= option)
            {
                return false;
            }

            return Options[option].Enabled;
        }

        /// <param name="option">The currently selected option. If -1, it means that is being initialized.</param>
        /// <param name="direction">A sign number (1 or -1) with the direction.</param>
        /// <returns>The next option that is available.</returns>
        public int NextAvailableOption(int option, int direction)
        {
            int totalOptionsTried = 0;
            while (totalOptionsTried < Length)
            {
                option += direction;
                option = Calculator.WrapAround(option, 0, Length - 1);

                if (IsOptionAvailable(option))
                {
                    break;
                }

                totalOptionsTried++;
            }

            return option;
        }


        /// <summary>
        /// Resets the menu info selector to the first available option.
        /// </summary>
        public void Reset()
        {
            Selection = 0;
            LastMoved = 0;
            PreviousSelection = 0;
        }
    }
}