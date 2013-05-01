// Copyright (c) Sven Groot (Ookii.org) 2006
// See license.txt for details

using System.ComponentModel;
using System.IO;
using Mygod.Windows.Dialogs.Interop;

namespace Mygod.Windows.Dialogs
{
    /// <summary>
    ///     Prompts the user to open a file.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Windows Vista provides a new style of common file dialog, with several new features (both from
    ///         the user's and the programmers perspective).
    ///     </para>
    ///     <para>
    ///         This class will use the Vista-style file dialogs if possible, and automatically fall back to the old-style
    ///         dialog on versions of Windows older than Vista. This class is aimed at applications that
    ///         target both Windows Vista and older versions of Windows, and therefore does not provide any
    ///         of the new APIs provided by Vista's file dialogs.
    ///     </para>
    ///     <para>
    ///         This class precisely duplicates the public interface of <see cref="Microsoft.Win32.OpenFileDialog" /> so you can just replace
    ///         any instances of <see cref="Microsoft.Win32.OpenFileDialog" /> with the <see cref="OpenFileDialog" /> without any further changes
    ///         to your code.
    ///     </para>
    /// </remarks>
    /// <threadsafety instance="false" static="true" />
    [Description("Prompts the user to open a file.")]
    public sealed class OpenFileDialog : FileDialog
    {
        private const int _openDropDownId = 0x4002;
        private const int _openItemId = 0x4003;
        private const int _readOnlyItemId = 0x4004;
        private bool _readOnlyChecked;
        private bool _showReadOnly;

        /// <summary>
        ///     Creates a new instance of <see cref="OpenFileDialog" /> class.
        /// </summary>
        public OpenFileDialog()
        {
            if (!IsVistaFileDialogSupported)
                DownlevelDialog = new Microsoft.Win32.OpenFileDialog();
        }

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether the dialog box displays a warning if the user specifies a file name that does not exist.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the dialog box displays a warning if the user specifies a file name that does not exist; otherwise,
        ///     <see
        ///         langword="false" />
        ///     . The default value is <see langword="true" />.
        /// </value>
        [DefaultValue(true),
         Description("A value indicating whether the dialog box displays a warning if the user specifies a file name that does not exist.")]
        public override bool CheckFileExists { get { return base.CheckFileExists; } set { base.CheckFileExists = value; } }

        /// <summary>
        ///     Gets or sets a value indicating whether the dialog box allows multiple files to be selected.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the dialog box allows multiple files to be selected together or concurrently; otherwise,
        ///     <see
        ///         langword="false" />
        ///     .
        ///     The default value is <see langword="false" />.
        /// </value>
        [Description("A value indicating whether the dialog box allows multiple files to be selected."), DefaultValue(false),
         Category("Behavior")]
        public bool Multiselect
        {
            get
            {
                if (DownlevelDialog != null)
                    return ((Microsoft.Win32.OpenFileDialog) DownlevelDialog).Multiselect;
                return GetOption(NativeMethods.FOS.FOS_ALLOWMULTISELECT);
            }
            set
            {
                if (DownlevelDialog != null)
                    ((Microsoft.Win32.OpenFileDialog) DownlevelDialog).Multiselect = value;

                SetOption(NativeMethods.FOS.FOS_ALLOWMULTISELECT, value);
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the dialog box contains a read-only check box.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the dialog box contains a read-only check box; otherwise, <see langword="false" />. The default value is
        ///     <see
        ///         langword="false" />
        ///     .
        /// </value>
        /// <remarks>
        ///     If the Vista style dialog is used, this property can only be used to determine whether the user chose
        ///     Open as read-only on the dialog; setting it in code will have no effect.
        /// </remarks>
        [Description("A value indicating whether the dialog box contains a read-only check box."), Category("Behavior"), DefaultValue(false)
        ]
        public bool ShowReadOnly
        {
            get
            {
                if (DownlevelDialog != null)
                    return ((Microsoft.Win32.OpenFileDialog) DownlevelDialog).ShowReadOnly;
                return _showReadOnly;
            }
            set
            {
                if (DownlevelDialog != null)
                    ((Microsoft.Win32.OpenFileDialog) DownlevelDialog).ShowReadOnly = value;
                else
                    _showReadOnly = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the read-only check box is selected.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the read-only check box is selected; otherwise, <see langword="false" />. The default value is
        ///     <see
        ///         langword="false" />
        ///     .
        /// </value>
        [DefaultValue(false), Description("A value indicating whether the read-only check box is selected."), Category("Behavior")]
        public bool ReadOnlyChecked
        {
            get
            {
                if (DownlevelDialog != null)
                    return ((Microsoft.Win32.OpenFileDialog) DownlevelDialog).ReadOnlyChecked;
                return _readOnlyChecked;
            }
            set
            {
                if (DownlevelDialog != null)
                    ((Microsoft.Win32.OpenFileDialog) DownlevelDialog).ReadOnlyChecked = value;
                else
                    _readOnlyChecked = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Resets all properties to their default values.
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            if (DownlevelDialog == null)
            {
                CheckFileExists = true;
                _showReadOnly = false;
                _readOnlyChecked = false;
            }
        }

        /// <summary>
        ///     Opens the file selected by the user, with read-only permission. The file is specified by the FileName property.
        /// </summary>
        /// <returns>A Stream that specifies the read-only file selected by the user.</returns>
        /// <exception cref="System.ArgumentNullException">
        ///     The file name is <see langword="null" />.
        /// </exception>
        public Stream OpenFile()
        {
            if (DownlevelDialog != null)
                return ((Microsoft.Win32.OpenFileDialog) DownlevelDialog).OpenFile();
            else
            {
                string fileName = FileName;
                return new FileStream(fileName, FileMode.Open, FileAccess.Read);
            }
        }

        #endregion

        #region Internal Methods

        internal override IFileDialog CreateFileDialog()
        {
            return new NativeFileOpenDialog();
        }

        internal override void SetDialogProperties(IFileDialog dialog)
        {
            base.SetDialogProperties(dialog);
            if (_showReadOnly)
            {
                var customize = (IFileDialogCustomize) dialog;
                customize.EnableOpenDropDown(_openDropDownId);
                customize.AddControlItem(_openDropDownId, _openItemId,
                                         ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.OpenButton));
                customize.AddControlItem(_openDropDownId, _readOnlyItemId,
                                         ComDlgResources.LoadString(ComDlgResources.ComDlgResourceId.ReadOnly));
            }
        }

        internal override void GetResult(IFileDialog dialog)
        {
            if (Multiselect)
            {
                IShellItemArray results;
                ((IFileOpenDialog) dialog).GetResults(out results);
                uint count;
                results.GetCount(out count);
                var fileNames = new string[count];
                for (uint x = 0; x < count; ++x)
                {
                    IShellItem item;
                    results.GetItemAt(x, out item);
                    string name;
                    item.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out name);
                    fileNames[x] = name;
                }
                FileNamesInternal = fileNames;
            }
            else
                FileNamesInternal = null;

            if (ShowReadOnly)
            {
                var customize = (IFileDialogCustomize) dialog;
                int selected;
                customize.GetSelectedControlItem(_openDropDownId, out selected);
                _readOnlyChecked = (selected == _readOnlyItemId);
            }

            base.GetResult(dialog);
        }

        #endregion
    }
}