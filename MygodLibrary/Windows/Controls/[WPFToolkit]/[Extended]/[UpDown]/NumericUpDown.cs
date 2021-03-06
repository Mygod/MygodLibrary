﻿/************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2010-2012 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   This program can be provided to you by Xceed Software Inc. under a
   proprietary commercial license agreement for use in non-Open Source
   projects. The commercial version of Extended WPF Toolkit also includes
   priority technical support, commercial updates, and many additional 
   useful WPF controls if you license Xceed Business Suite for WPF.

   Visit http://xceed.com and follow @datagrid on Twitter.

  **********************************************************************/

using System;
using System.Globalization;
using System.Windows;
using Mygod.Windows.Controls.Primitives;

namespace Mygod.Windows.Controls
{
    public abstract class NumericUpDown<T> : UpDownBase<T>
    {
        #region Properties

        #region DefaultValue

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(T),
            typeof(NumericUpDown<T>), new UIPropertyMetadata(default(T)));

        public T DefaultValue { get { return (T) GetValue(DefaultValueProperty); } set { SetValue(DefaultValueProperty, value); } }

        #endregion //DefaultValue

        #region FormatString

        public static readonly DependencyProperty FormatStringProperty = DependencyProperty.Register("FormatString", typeof(string),
            typeof(NumericUpDown<T>), new UIPropertyMetadata(string.Empty, OnFormatStringChanged));

        public string FormatString
        {
            get { return (string) GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        private static void OnFormatStringChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = o as NumericUpDown<T>;
            if (numericUpDown != null)
                numericUpDown.OnFormatStringChanged((string) e.OldValue, (string) e.NewValue);
        }

        protected virtual void OnFormatStringChanged(string oldValue, string newValue)
        {
            if (IsInitialized)
                Text = ConvertValueToText();
        }

        #endregion //FormatString

        #region Increment

        public static readonly DependencyProperty IncrementProperty = DependencyProperty.Register("Increment", typeof(T),
            typeof(NumericUpDown<T>), new PropertyMetadata(default(T)));

        public T Increment { get { return (T) GetValue(IncrementProperty); } set { SetValue(IncrementProperty, value); } }

        #endregion

        #region Maximum

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(T),
            typeof(NumericUpDown<T>), new UIPropertyMetadata(default(T), OnMaximumChanged));

        public T Maximum { get { return (T) GetValue(MaximumProperty); } set { SetValue(MaximumProperty, value); } }

        private static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = o as NumericUpDown<T>;
            if (numericUpDown != null)
                numericUpDown.OnMaximumChanged((T) e.OldValue, (T) e.NewValue);
        }

        protected virtual void OnMaximumChanged(T oldValue, T newValue)
        {
            SetValidSpinDirection();
        }

        #endregion //Maximum

        #region Minimum

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(T),
            typeof(NumericUpDown<T>), new UIPropertyMetadata(default(T), OnMinimumChanged));

        public T Minimum { get { return (T) GetValue(MinimumProperty); } set { SetValue(MinimumProperty, value); } }

        private static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var numericUpDown = o as NumericUpDown<T>;
            if (numericUpDown != null)
                numericUpDown.OnMinimumChanged((T) e.OldValue, (T) e.NewValue);
        }

        protected virtual void OnMinimumChanged(T oldValue, T newValue)
        {
            SetValidSpinDirection();
        }

        #endregion //Minimum

        #endregion //Properties

        #region Methods

        protected static decimal ParseDecimal(string text, IFormatProvider cultureInfo)
        {
            return decimal.Parse(text, NumberStyles.Any, cultureInfo);
        }

        protected static double ParseDouble(string text, IFormatProvider cultureInfo)
        {
            return double.Parse(text, NumberStyles.Any, cultureInfo);
        }

        protected static int ParseInt(string text, IFormatProvider cultureInfo)
        {
            return int.Parse(text, NumberStyles.Any, cultureInfo);
        }

        protected static long ParseLong(string text, IFormatProvider cultureInfo)
        {
            return long.Parse(text, NumberStyles.Any, cultureInfo);
        }

        protected static decimal ParsePercent(string text, IFormatProvider cultureInfo)
        {
            NumberFormatInfo info = NumberFormatInfo.GetInstance(cultureInfo);

            text = text.Replace(info.PercentSymbol, null);

            decimal result = decimal.Parse(text, NumberStyles.Any, info);
            result = result / 100;

            return result;
        }

        #endregion //Methods
    }
}