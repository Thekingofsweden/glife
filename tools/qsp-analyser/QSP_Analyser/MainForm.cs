using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Analyser
{
    public partial class MainForm : Form
    {
        //Глобальные переменные II
        QSPGameCode game;


        //Глобальные переменные
        ArrayList ObjectsAdded;
        ArrayList ObjectsDeleted;

        bool inited = false;
        string logText = "";
        static BackgroundWorker _bw;

        public MainForm()
        {
            InitializeComponent();
            _bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            _bw.DoWork += bw_DoWork;
            _bw.ProgressChanged += bw_ProgressChanged;
            _bw.RunWorkerCompleted += bw_RunWorkerCompleted;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            //выбираем файл
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.filepath = dlgOpen.FileName;
                txtStatus.Clear();
                logText = "";
                ClearMains();
                ParseFile();
            }
        }

        private delegate void PrintErrorsDelegate();
        private void PrintErrors()
        {

            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                this.BeginInvoke(new PrintErrorsDelegate(PrintErrors));

                return;
            }
            //Выводим ошибки и ворнинги
            //Log("Строка #" + Convert.ToString(game.GetErrorLine()) + ": " + game.GetError());
            Log("No of errors: " + Common.GetErrorsCount() + ", warnings: " + Common.GetWarningsCount());
            List<Common.QspError> errors = Common.GetErrors();
            foreach (Common.QspError error in errors)
            {
                int err_line = game.GetLocation(error.locationName).GetLine() + error.line;
                string errorMessage = "Location \"" + error.locationName + "\", Line #" + Convert.ToString(err_line) + " : " + error.text;
                if (error.isError)
                {
                    Log("Error, " + errorMessage);
                }
                else
                {
                    Log("Warning, " + errorMessage);
                }
            }
            PrintLog();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            //Устанаваливаем настройки парсера
            List<string> systemVars = new List<string>(txtSystemVariables.Lines);
            if (chkAero.Checked)
                systemVars.AddRange(txtSystemAeroVars.Lines);
            Common.SetConfig(new List<string>(txtVariableNames.Lines), systemVars, chkCurlyParse.Checked);
            if (CheckForFile(edtFile.Text, true))
            {
                game.ParseGame(edtFile.Text, sender, e);
                if (!e.Cancel)
                { 
                    this.Log("The file was read successfully");
                    this.PrintLog();
                }
            }
            else
            {
                this.Log("File was not read.");
                PrintLog();
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            this.PrintErrors();
            Common.ClearNonGameErrors();
            this.PrintLog();
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }


        private void ParseFile()
        {
            Log("Trying to read the file ...");
            PrintLog();
            _bw.RunWorkerAsync();

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Analyser.Properties.Settings.Default.Reset();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Analyser.Properties.Settings.Default.Save();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text += "   v " + System.Windows.Forms.Application.ProductVersion;
        }

        public void Log(string line)
        {
            logText += line + Environment.NewLine;
        }

        private delegate void PrintLogDelegate();
        private void PrintLog()
        {
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                this.BeginInvoke(new PrintLogDelegate(PrintLog));

                return;
            }
            txtStatus.Text = logText;
        }

        private void ClearMains()
        {
            txtList1.Clear();
            txtList2.Clear();
            txtList3.Clear();
        }

        //Выводим список значений в textbox, перед списком выводим заголовок
        private void PrintArray(Control textbox, ArrayList val, string header)
        {
            if (textbox.Text.Length > 0)
                textbox.Text += Environment.NewLine;
            if (header.Length > 0)
                textbox.Text += "[" + header + "]" + Environment.NewLine;
            string s = String.Join(Environment.NewLine, (string[])val.ToArray(typeof(string)));
            textbox.Text += s;
        }

        private ArrayList SubstractArray(ArrayList search_in, ArrayList search_for)
        {
            ArrayList res = new ArrayList();
            string tosearch = Environment.NewLine + String.Join(Environment.NewLine, (string[])search_in.ToArray(typeof(string))) + Environment.NewLine;
            tosearch = tosearch.ToUpper();
            foreach (string titem in search_for)
            {
                if (!tosearch.Contains(Environment.NewLine + titem.ToUpper() + Environment.NewLine))
                    res.Add(titem);
            }
            return res;
        }

        private bool CheckForFile(string filename, bool silent)
        {
            if (filename != "")
            {
                if (File.Exists(filename))
                {
                    return true;
                }
                else
                {
                    if (!silent)
                        MessageBox.Show("Input file was not found!");
                }
            }
            else
            {
                if (!silent)
                    MessageBox.Show("Select input file!");
            }
            return false;
        }

        private void SortArray(ArrayList arr)
        {
            string[] tarr = (string[])arr.ToArray(typeof(string));
            Array.Sort(tarr);
            arr.Clear();
            arr.AddRange(tarr);
        }

        //************************************************************
        //************************************************************
        //************************************************************

//Локации
        private void btnTry1_Click(object sender, EventArgs e)
        {
            ClearMains();

            ArrayList LocationsDefined = new ArrayList();
            ArrayList LocationsReferenced = new ArrayList();
            ArrayList LocationsUnDefined = new ArrayList();
            ArrayList LocationsUnReferenced = new ArrayList();

            foreach (Common.QspLocationLink loc in Common.locationLinks)
            {
                if (loc.LocationExists)
                    LocationsDefined.Add(loc.LocationName);
                if (loc.LocationCalled)
                    LocationsReferenced.Add(loc.LocationName);
                if (!loc.LocationExists)
                    LocationsUnDefined.Add(loc.LocationName);
                if (!loc.LocationCalled)
                    LocationsUnReferenced.Add(loc.LocationName);
            }

            if (chkSortLocations.Checked)
            {
                SortArray(LocationsDefined);
                SortArray(LocationsReferenced);
                SortArray(LocationsUnDefined);
                SortArray(LocationsUnReferenced);
            }

            Log("We process the names of locations ...");
            PrintArray(txtList1, LocationsDefined, "Existing locations");
            Log("Added locations: " + LocationsDefined.Count.ToString());

            Log("We process the reference to locations...");
            PrintArray(txtList2, LocationsReferenced, "References to locations");
            Log("Added reference to location: " + LocationsReferenced.Count.ToString());

            Log("We begin to search for incorrect links ...");
            PrintArray(txtList3, LocationsUnDefined, "Incorrect links");
            Log("Found incorrect links: " + LocationsUnDefined.Count.ToString());

            Log("We begin to search for lost locations - those for which there are no references ...");
            PrintArray(txtList3, LocationsUnReferenced, "Lost locations");
            Log("Found lost locations: " + LocationsUnReferenced.Count.ToString());
            
            PrintLog();
        }

//Переменные
        private void btnTry2_Click(object sender, EventArgs e)
        {
            ClearMains();

            ArrayList TextVars = new ArrayList();
            ArrayList NumericVars = new ArrayList();
            ArrayList TextVars_unassigned = new ArrayList();
            ArrayList NumericVars_unassigned = new ArrayList();
            ArrayList TextVars_unused = new ArrayList();
            ArrayList NumericVars_unused = new ArrayList();

            Log("Search for variables ...");

            foreach (Common.QspVariable var in Common.vars)
            {
                if (var.IsString)
                {
                    if (!var.Assigned)
                        TextVars_unassigned.Add(var.Name);
                    if (!var.Used)
                        TextVars_unused.Add(var.Name);
                    TextVars.Add(var.Name);
                }
                else
                {
                    if (!var.Assigned)
                        NumericVars_unassigned.Add(var.Name);
                    if (!var.Used)
                        NumericVars_unused.Add(var.Name);
                    NumericVars.Add(var.Name);
                }
            }

            if (chkSortVariables.Checked)
            {
                SortArray(TextVars);
                SortArray(NumericVars);
            }
            PrintArray(txtList1, TextVars, "String variables");
            PrintArray(txtList2, NumericVars, "Numeric variables");

            Log("Added variables: " + Common.vars.Count.ToString());
            
            if (chkSortVariables.Checked)
                SortArray(TextVars_unassigned);
            PrintArray(txtList3, TextVars_unassigned, "no init variables:");
            Log("Found using of uninitialized text variables: " + TextVars_unassigned.Count.ToString());
            if (chkSortVariables.Checked)
                SortArray(NumericVars_unassigned);
            PrintArray(txtList3, NumericVars_unassigned, "");
            Log("Found using of uninitialized numeric variables: " + NumericVars_unassigned.Count.ToString());

            if (chkSortVariables.Checked)
                SortArray(TextVars_unused);
            PrintArray(txtList3, TextVars_unused, "Unused variables:");
            Log("Found unused string variables: " + TextVars_unused.Count.ToString());
            if (chkSortVariables.Checked)
                SortArray(NumericVars_unused);
            PrintArray(txtList3, NumericVars_unused, "");
            Log("Found unused numeric variables: " + NumericVars_unused.Count.ToString());

            PrintLog();
        }

//Предметы
        private void btnTry3_Click(object sender, EventArgs e)
        {
            ClearMains();

            ArrayList ObjectsAdded = new ArrayList();
            ArrayList ObjectsDeleted = new ArrayList();
            ArrayList ObjectsUnDefined = new ArrayList();
            ArrayList ObjectsLost = new ArrayList();

            foreach (Common.QspObj obj in Common.objects)
            {
                if (obj.Added)
                    ObjectsAdded.Add(obj.Name);
                if (obj.Removed)
                    ObjectsDeleted.Add(obj.Name);
                if (!obj.Added)
                    ObjectsUnDefined.Add(obj.Name);
                if (!obj.Removed)
                    ObjectsLost.Add(obj.Name);
            }

            if (chkSortObjects.Checked)
            {
                SortArray(ObjectsAdded);
                SortArray(ObjectsDeleted);
                SortArray(ObjectsUnDefined);
                SortArray(ObjectsLost);
            }

            Log("Search of items...");
            PrintArray(txtList1, ObjectsAdded, "Added items");
            PrintArray(txtList2, ObjectsDeleted, "Remove item");
            Log("Items adding: " + ObjectsAdded.Count.ToString());
            Log("Items removing: " + ObjectsDeleted.Count.ToString());

            Log("Search of nonexisting items...");
            PrintArray(txtList3, ObjectsUnDefined, "Invalid deletion");
            Log("Found invalid deletions: " + ObjectsUnDefined.Count.ToString());

            Log("Looking for lost items - those that are not deleted during the game ...");
            PrintArray(txtList3, ObjectsLost, "Lost items");
            Log("Lost items found: " + ObjectsLost.Count.ToString());

            PrintLog();
        }

//Действия
        private void btnTry4_Click(object sender, EventArgs e)
        {
            ClearMains();

            ArrayList ActsAdded = new ArrayList();
            ArrayList ActsDeleted = new ArrayList();
            ArrayList ActsUnDefined = new ArrayList();

            foreach (Common.QspAct act in Common.acts)
            {
                if (act.Added)
                    ActsAdded.Add(act.Name);
                if (act.Removed)
                    ActsDeleted.Add(act.Name);
                if (!act.Added)
                    ActsUnDefined.Add(act.Name);
            }

            if (chkSortObjects.Checked)
            {
                SortArray(ActsAdded);
                SortArray(ActsDeleted);
                SortArray(ActsUnDefined);
            }

            Log("Search for actions...");
            PrintArray(txtList1, ActsAdded, "Add an action");
            PrintArray(txtList2, ActsDeleted, "Remove an action");
            Log("Added action: " + ActsAdded.Count.ToString());
            Log("Removed action: " + ActsDeleted.Count.ToString());

            Log("We begin to search for deletion of nonexistent actions...");
            PrintArray(txtList3, ActsUnDefined, "Invalid deletion");
            Log("Invalid deletions found: " + ActsUnDefined.Count.ToString());

            PrintLog();
        }

        //************************************************************
        //************************************************************
        //************************************************************

        private void btnReload_Click(object sender, EventArgs e)
        {
            txtStatus.Clear();
            logText = "";
            ClearMains();
            ParseFile();
        }

        private void setBindingContext(Control ctrl)
        {
            ctrl.Visible = true;
            if (ctrl.HasChildren)
                foreach (Control child in ctrl.Controls)
                    setBindingContext(child);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!inited)
            {
                setBindingContext(this);
                btnBrowse.Select();


                game = new QSPGameCode();
                ParseFile();


                ObjectsDeleted = new ArrayList();
                ObjectsAdded = new ArrayList();
                inited = true;
            }
        }

        private void PrintNonGameErrors()
        {
            List<Common.QspError> errors = Common.GetErrors();
            foreach (Common.QspError error in errors)
            {
                if (error.isError && (error.line == Common.INVALID_INDEX))
                {
                    MessageBox.Show("Error: " + error.text);
                }
            }
            Common.ClearNonGameErrors();
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            //Делаем экспорт текста в csv
            string FileName = edtFile.Text;
            if (CheckForFile(FileName, false))
            {
                if (Common.GetErrorsCount() > 0)
                {
                    MessageBox.Show("There are syntax errors in the file, export is not possible!");
                    return;
                }

                edtFile.Enabled = false;
                btnBrowse.Enabled = false;

                bool success = false;
                if (radComma.Checked)
                {
                    success = Common.ExportToCsv(FileName, ",");
                }
                else
                {
                    success = Common.ExportToCsv(FileName, ";");
                }
                if (!success)
                {
                    PrintNonGameErrors();
                }

                edtFile.Enabled = true;
                btnBrowse.Enabled = true;
            }
        }

        private void btnTranslateFromCsv_Click(object sender, EventArgs e)
        {
            //Делаем перевод текста на основе csv
            string FileName = edtFile.Text;
            if (CheckForFile(FileName, false))
            {
                if (Common.GetErrorsCount() > 0)
                {
                    MessageBox.Show("There are syntax errors in the file, export is not possible!");
                    return;
                }

                string CsvFile = Common.GenerateCsvName(FileName);
                if (!CheckForFile(FileName, true))
                {
                    MessageBox.Show("CSV file was not found!");
                }

                if (edtSuffixCsv.Text.Trim(Common.WhiteSpace).Length == 0)
                {
                    MessageBox.Show("Enter an extension of the conversion file.");
                }

                edtFile.Enabled = false;
                btnBrowse.Enabled = false;

                bool IgnoreEmptyTranslations = chkIgnoreEmptyTranslationsCsv.Checked;
                bool success = false;
                if (radComma.Checked)
                {
                    success = Common.TranslateFromCsv(FileName, edtSuffixCsv.Text, IgnoreEmptyTranslations, ",");
                }
                else
                {
                    success = Common.TranslateFromCsv(FileName, edtSuffixCsv.Text, IgnoreEmptyTranslations, ";");
                }
                if (!success)
                {
                    PrintNonGameErrors();
                }

                edtFile.Enabled = true;
                btnBrowse.Enabled = true;
            }
        }

        private void btnBeautify_Click(object sender, EventArgs e)
        {
            string FileName = edtFile.Text;
            if (CheckForFile(FileName, false))
            {
                edtFile.Enabled = false;
                btnBrowse.Enabled = false;

                bool success = false;
                success = game.Beautify(FileName);
                if (!success)
                {
                    PrintNonGameErrors();
                }

                edtFile.Enabled = true;
                btnBrowse.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _bw.CancelAsync();
            Log("Cancelled loading of file.");
            PrintLog();
        }
    }
}