using bbHierarchicalGrid.Attributes;
using bbHierarchicalGrid.Models;
using bbHierarchicalGrid.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms
{
    public partial class WinForms : Form
    {
        bbViewModel<Country> viewModel = default(bbViewModel<Country>);
        public WinForms()
        {
            viewModel = new bbViewModel<Country>(Properties.Settings.Default.LicenseKey, bbHierarchicalGrid.Enums.DisplayLanguage.Arabic);
            viewModel.Initialize();
            InitializeComponent();
        }

        private void WinForms_Load(object sender, EventArgs e)
        {
           
            var bbHierarchicalDataGridControl = new bbHierarchicalGrid.bbHierarchicalDataGrid();
            this.elementHost1.Child = bbHierarchicalDataGridControl;
            bbHierarchicalDataGridControl.DataContext = viewModel;



            // set flow direction from right to left
            viewModel.FlowDirection = System.Windows.FlowDirection.RightToLeft;
            viewModel.NameColumnHeader = "بلد";

            // read data from excel

            string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Data\\worldcountries-ar.xlsx";
            FileInfo existingFile = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(existingFile))
            {
                //get the first worksheet in the workbook
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int colCount = worksheet.Dimension.End.Column;   
                int rowCount = worksheet.Dimension.End.Row;     
                for (int row = 2; row <= rowCount; row++)
                {
                     


                    if (worksheet.Cells[row, 1].Value != null)
                    {
                        viewModel.Add(new Country() {
                            Name = worksheet.Cells[row, 1].Value.ToString().Trim(),
                            Region = worksheet.Cells[row, 2].Value.ToString().Trim(),
                            Population = int.Parse(worksheet.Cells[row, 3].Value.ToString())

                        });

                    }
                }
            }

        }
    }

    public class Country : bbItem
    {
        [Description("منطقة")]
        public string Region { get; set; }
        [bbSortable]
        [Description("تعداد السكان")]
        public int Population { get; set; }
        
        
        public Country(Country parent =  null) : base(parent)
        {

        }
    }
}
