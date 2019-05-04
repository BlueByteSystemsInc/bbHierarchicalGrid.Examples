using bbHierarchicalGrid.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Models;

namespace WPF.ViewModels
{
    public class MainWindowViewModel : BindableBase 
    {
        #region properties 
        internal static bbViewModel<Item> ViewModel { get; set; } = new bbViewModel<Item>(Properties.Settings.Default.LicenseKey);
        private string currentDirectory;

        public string CurrentDirectory
        {
            get { return currentDirectory; }
            set { SetProperty(ref currentDirectory, value); }
        }

        #endregion

        #region commands 
        public DelegateCommand AddFolderCommand { get; set; } 
        #endregion 

        #region constructors 
        public MainWindowViewModel()
        {
            AddFolderCommand = new DelegateCommand(() => {
              var folders =  BrowseToFolder(System.Environment.CurrentDirectory, false);
                var folder = folders.First();

              

             if (string.IsNullOrWhiteSpace(folder) == false)
                {
                    var folderItem = new Item(null, folder);
                    Action<Item> traverse = default(Action<Item>);
                    traverse = (Item item) => {

                        var filesLst = new List<Item>();
                        var files = System.IO.Directory.GetFiles(item.LocalPath);
                        if( files != null)
                        foreach (var file in files)
                        {
                            if (Path.GetFileName(file).StartsWith("~$"))
                                continue;

                            var fileItem = new Item(item, file);
                            item.Children.Add(fileItem);

                        }
                       
                        var directories = System.IO.Directory.GetDirectories(item.LocalPath);
                        if (directories != null)
                        foreach (var directory in directories)
                        {

                            var subDirectoryItem = new Item(item, directory);
                            item.Children.Add(subDirectoryItem);
                            traverse(subDirectoryItem);

                        }
                    };

                    
                    traverse(folderItem);
                    MainWindowViewModel.ViewModel.Add(folderItem);
                    MainWindowViewModel.ViewModel.Invalidate();
                    CurrentDirectory = folder;
                }
            });
        }
        #endregion

        /// <summary>
        /// Opens the CommonOpenFileDialog window to browse to a folder.
        /// </summary>
        /// <param name="startupDirectory">startup directory.</param>
        /// <param name="multiSelect">Allow multiple selection.</param>
        /// <returns>Array of selected directory.</returns>
        public static string[] BrowseToFolder(string startupDirectory, bool multiSelect = false)
        {

            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Multiselect = multiSelect;


            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (multiSelect == false)
                {
                    string Directory = dialog.FileName;
                    if (System.IO.Directory.Exists(Directory))
                    {
                        return new string[] { Directory };
                    }

                }
                else
                {
                    var FileNames = dialog.FileNames;
                    if (FileNames != null || FileNames.Count() > 0)
                    {
                        return FileNames.ToArray();
                    }
                }

            }
            return new string[] { string.Empty };
        }
    }

}
