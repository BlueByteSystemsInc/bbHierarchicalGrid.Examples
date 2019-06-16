using bbHierarchicalGrid.Attributes;
using bbHierarchicalGrid.Models;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.Models
{

    /// <summary>
    /// Item class.
    /// </summary>
    public class Item : bbItem
    {
        private string name;
        /// <summary>
        /// Name of this item.
        /// </summary>
        [bbSortable]
        public override string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        /// <summary>
        /// Creates a new instance of the Item. 
        /// </summary>
        /// <param name="parent">Parent of the Item.</param>
        /// <param name="localPath">Local path of the Item.</param>
        public Item(Item parent, string localPath) : base(parent)
        {
            LocalPath = localPath;
            Name = Path.GetFileName(LocalPath);
            if (string.IsNullOrWhiteSpace(Path.GetExtension(Name)))
            {
                ItemTypeName = "Directory";
                var directoryInfo = new DirectoryInfo(LocalPath);
                this.ModifiedDateTime = directoryInfo.LastWriteTime;
                
            }
            else
            {
                NativeMethods.SHFILEINFO info = new NativeMethods.SHFILEINFO();
                uint dwFileAttributes = NativeMethods.FILE_ATTRIBUTE.FILE_ATTRIBUTE_NORMAL;
                uint uFlags = (uint)(NativeMethods.SHGFI.SHGFI_TYPENAME | NativeMethods.SHGFI.SHGFI_USEFILEATTRIBUTES);
                NativeMethods.SHGetFileInfo(LocalPath, dwFileAttributes, ref info, (uint)Marshal.SizeOf(info), uFlags);
                ItemTypeName = info.szTypeName;
                var fileInfo = new FileInfo(LocalPath);
                this.ModifiedDateTime = fileInfo.LastWriteTime;
            }

            Bitmap shellThumb = ShellIcon.GetSmallIcon(LocalPath).ToBitmap();
            var thumbnail = NativeMethods.ToBitmapImage(shellThumb);
            Thumbnail = thumbnail;
            UseThumbnail = true;


        }
        private DateTime modifiedDateTime;
        [bbSortable]
        [ReadOnly(true)]
        [bbColumnOrderAttribute(2)]
        [Description("Modified")]
        public DateTime ModifiedDateTime
        {
            get { return modifiedDateTime; }
            set { SetProperty(ref modifiedDateTime, value); }
        }

        private string itemTypeName = "Unknown";
        /// <summary>
        /// Gets ors sets the type name of this item.
        /// </summary>
        [bbSortable]
        [Description("Type")]
        [bbColumnOrderAttribute(2)]
        public string ItemTypeName
        {
            get { return itemTypeName; }
            set { SetProperty(ref itemTypeName, value); }
        }


        private string localPath;
        /// <summary>
        /// Gets or sets the local of path this item.
        /// </summary>
        [bbSortable]
        [Description("Full Path")]
        [bbColumnOrderAttribute(3)]
        public string LocalPath
        {
            get { return localPath; }
            set { SetProperty(ref localPath, value); }
        }

        


    }
    #region helper classes
    // -----------------------------------------------------------------------
    // <copyright file="ShellIcon.cs" company="Mauricio DIAZ ORLICH (madd0@madd0.com)">
    //   Distributed under Microsoft Public License (MS-PL).
    //   http://www.opensource.org/licenses/MS-PL
    // </copyright>
    // -----------------------------------------------------------------------

    /// <summary>
    /// Get a small or large Icon with an easy C# function call
    /// that returns a 32x32 or 16x16 System.Drawing.Icon depending on which function you call
    /// either GetSmallIcon(string fileName) or GetLargeIcon(string fileName)
    /// </summary>
    public static class ShellIcon
    {
        #region Interop constants

        private const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        #endregion

        #region Interop data types

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [Flags]
        private enum SHGFI : int
        {
            /// <summary>get icon</summary>
            Icon = 0x000000100,
            /// <summary>get display name</summary>
            DisplayName = 0x000000200,
            /// <summary>get type name</summary>
            TypeName = 0x000000400,
            /// <summary>get attributes</summary>
            Attributes = 0x000000800,
            /// <summary>get icon location</summary>
            IconLocation = 0x000001000,
            /// <summary>return exe type</summary>
            ExeType = 0x000002000,
            /// <summary>get system icon index</summary>
            SysIconIndex = 0x000004000,
            /// <summary>put a link overlay on icon</summary>
            LinkOverlay = 0x000008000,
            /// <summary>show icon in selected state</summary>
            Selected = 0x000010000,
            /// <summary>get only specified attributes</summary>
            Attr_Specified = 0x000020000,
            /// <summary>get large icon</summary>
            LargeIcon = 0x000000000,
            /// <summary>get small icon</summary>
            SmallIcon = 0x000000001,
            /// <summary>get open icon</summary>
            OpenIcon = 0x000000002,
            /// <summary>get shell size icon</summary>
            ShellIconSize = 0x000000004,
            /// <summary>pszPath is a pidl</summary>
            PIDL = 0x000000008,
            /// <summary>use passed dwFileAttribute</summary>
            UseFileAttributes = 0x000000010,
            /// <summary>apply the appropriate overlays</summary>
            AddOverlays = 0x000000020,
            /// <summary>Get the index of the overlay in the upper 8 bits of the iIcon</summary>
            OverlayIndex = 0x000000040,
        }

        #endregion

        private class Win32
        {
            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);

        }

        public static Icon GetSmallFolderIcon()
        {
            return GetIcon("folder", SHGFI.SmallIcon | SHGFI.UseFileAttributes, true);
        }

        public static Icon GetLargeFolderIcon()
        {
            return GetIcon("folder", SHGFI.LargeIcon | SHGFI.UseFileAttributes, true);
        }

        public static Icon GetSmallIcon(string fileName)
        {
            return GetIcon(fileName, SHGFI.SmallIcon);
        }

        public static Icon GetLargeIcon(string fileName)
        {
            return GetIcon(fileName, SHGFI.LargeIcon);
        }

        public static Icon GetSmallIconFromExtension(string extension)
        {
            return GetIcon(extension, SHGFI.SmallIcon | SHGFI.UseFileAttributes);
        }

        public static Icon GetLargeIconFromExtension(string extension)
        {
            return GetIcon(extension, SHGFI.LargeIcon | SHGFI.UseFileAttributes);
        }

        private static Icon GetIcon(string fileName, SHGFI flags, bool isFolder = false)
        {
            SHFILEINFO shinfo = new SHFILEINFO();

            IntPtr hImgSmall = Win32.SHGetFileInfo(fileName, isFolder ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL, ref shinfo, (uint)Marshal.SizeOf(shinfo), (uint)(SHGFI.Icon | flags));

            Icon icon = (Icon)System.Drawing.Icon.FromHandle(shinfo.hIcon).Clone();
            Win32.DestroyIcon(shinfo.hIcon);
            return icon;
        }
    }
    static class NativeMethods
    {
        
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        public static class FILE_ATTRIBUTE
        {
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        }

        public static class SHGFI
        {
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
    }
    #endregion
}
