namespace Analyser
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtList1 = new System.Windows.Forms.TextBox();
            this.txtList2 = new System.Windows.Forms.TextBox();
            this.txtList3 = new System.Windows.Forms.TextBox();
            this.btnTry1 = new System.Windows.Forms.Button();
            this.btnTry2 = new System.Windows.Forms.Button();
            this.btnTry3 = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCheck = new System.Windows.Forms.TabPage();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnTry4 = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.chkCurlyParse = new System.Windows.Forms.CheckBox();
            this.chkActions = new System.Windows.Forms.CheckBox();
            this.txtSystemAeroVars = new System.Windows.Forms.TextBox();
            this.chkAero = new System.Windows.Forms.CheckBox();
            this.lblSystemVariables = new System.Windows.Forms.Label();
            this.lblSortHeader = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.txtSystemVariables = new System.Windows.Forms.TextBox();
            this.chkSortObjects = new System.Windows.Forms.CheckBox();
            this.chkSortVariables = new System.Windows.Forms.CheckBox();
            this.chkSortLocations = new System.Windows.Forms.CheckBox();
            this.txtVariableNames = new System.Windows.Forms.TextBox();
            this.chkVariableCalls = new System.Windows.Forms.CheckBox();
            this.tabTranslate = new System.Windows.Forms.TabPage();
            this.btnBeautify = new System.Windows.Forms.Button();
            this.grpTranslateCsv = new System.Windows.Forms.GroupBox();
            this.grpCsvDelimiter = new System.Windows.Forms.GroupBox();
            this.radComma = new System.Windows.Forms.RadioButton();
            this.radSemicolon = new System.Windows.Forms.RadioButton();
            this.btnTranslateFromCsv = new System.Windows.Forms.Button();
            this.chkIgnoreEmptyTranslationsCsv = new System.Windows.Forms.CheckBox();
            this.lblSuffixCsv = new System.Windows.Forms.Label();
            this.btnExportCsv = new System.Windows.Forms.Button();
            this.edtSuffixCsv = new System.Windows.Forms.TextBox();
            this.edtFile = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.tabControl.SuspendLayout();
            this.tabCheck.SuspendLayout();
            this.tabOptions.SuspendLayout();
            this.tabTranslate.SuspendLayout();
            this.grpTranslateCsv.SuspendLayout();
            this.grpCsvDelimiter.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This utility analyzes the QSP game code in form TXT2GAM.";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(591, 8);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(29, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtList1
            // 
            this.txtList1.BackColor = System.Drawing.SystemColors.Window;
            this.txtList1.Location = new System.Drawing.Point(3, 59);
            this.txtList1.Multiline = true;
            this.txtList1.Name = "txtList1";
            this.txtList1.ReadOnly = true;
            this.txtList1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtList1.Size = new System.Drawing.Size(205, 348);
            this.txtList1.TabIndex = 3;
            // 
            // txtList2
            // 
            this.txtList2.BackColor = System.Drawing.SystemColors.Window;
            this.txtList2.Location = new System.Drawing.Point(214, 59);
            this.txtList2.Multiline = true;
            this.txtList2.Name = "txtList2";
            this.txtList2.ReadOnly = true;
            this.txtList2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtList2.Size = new System.Drawing.Size(210, 348);
            this.txtList2.TabIndex = 4;
            // 
            // txtList3
            // 
            this.txtList3.BackColor = System.Drawing.SystemColors.Window;
            this.txtList3.Location = new System.Drawing.Point(430, 59);
            this.txtList3.Multiline = true;
            this.txtList3.Name = "txtList3";
            this.txtList3.ReadOnly = true;
            this.txtList3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtList3.Size = new System.Drawing.Size(229, 348);
            this.txtList3.TabIndex = 5;
            // 
            // btnTry1
            // 
            this.btnTry1.Location = new System.Drawing.Point(12, 6);
            this.btnTry1.Name = "btnTry1";
            this.btnTry1.Size = new System.Drawing.Size(77, 28);
            this.btnTry1.TabIndex = 6;
            this.btnTry1.Text = "Locations";
            this.btnTry1.UseVisualStyleBackColor = true;
            this.btnTry1.Click += new System.EventHandler(this.btnTry1_Click);
            // 
            // btnTry2
            // 
            this.btnTry2.Location = new System.Drawing.Point(95, 6);
            this.btnTry2.Name = "btnTry2";
            this.btnTry2.Size = new System.Drawing.Size(91, 28);
            this.btnTry2.TabIndex = 7;
            this.btnTry2.Text = "Variables";
            this.btnTry2.UseVisualStyleBackColor = true;
            this.btnTry2.Click += new System.EventHandler(this.btnTry2_Click);
            // 
            // btnTry3
            // 
            this.btnTry3.Location = new System.Drawing.Point(192, 6);
            this.btnTry3.Name = "btnTry3";
            this.btnTry3.Size = new System.Drawing.Size(80, 28);
            this.btnTry3.TabIndex = 8;
            this.btnTry3.Text = "Items";
            this.btnTry3.UseVisualStyleBackColor = true;
            this.btnTry3.Click += new System.EventHandler(this.btnTry3_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.BackColor = System.Drawing.SystemColors.Window;
            this.txtStatus.Location = new System.Drawing.Point(3, 413);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(656, 148);
            this.txtStatus.TabIndex = 9;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabCheck);
            this.tabControl.Controls.Add(this.tabOptions);
            this.tabControl.Controls.Add(this.tabTranslate);
            this.tabControl.Location = new System.Drawing.Point(1, 37);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(670, 590);
            this.tabControl.TabIndex = 12;
            // 
            // tabCheck
            // 
            this.tabCheck.Controls.Add(this.btnCancel);
            this.tabCheck.Controls.Add(this.btnTry4);
            this.tabCheck.Controls.Add(this.btnReload);
            this.tabCheck.Controls.Add(this.btnTry1);
            this.tabCheck.Controls.Add(this.txtStatus);
            this.tabCheck.Controls.Add(this.txtList3);
            this.tabCheck.Controls.Add(this.btnTry2);
            this.tabCheck.Controls.Add(this.txtList2);
            this.tabCheck.Controls.Add(this.btnTry3);
            this.tabCheck.Controls.Add(this.txtList1);
            this.tabCheck.Location = new System.Drawing.Point(4, 22);
            this.tabCheck.Name = "tabCheck";
            this.tabCheck.Padding = new System.Windows.Forms.Padding(3);
            this.tabCheck.Size = new System.Drawing.Size(662, 564);
            this.tabCheck.TabIndex = 0;
            this.tabCheck.Text = "Analysis";
            this.tabCheck.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(455, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 28);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnTry4
            // 
            this.btnTry4.Location = new System.Drawing.Point(278, 6);
            this.btnTry4.Name = "btnTry4";
            this.btnTry4.Size = new System.Drawing.Size(75, 28);
            this.btnTry4.TabIndex = 13;
            this.btnTry4.Text = "Events";
            this.btnTry4.UseVisualStyleBackColor = true;
            this.btnTry4.Click += new System.EventHandler(this.btnTry4_Click);
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(538, 6);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(77, 28);
            this.btnReload.TabIndex = 12;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // tabOptions
            // 
            this.tabOptions.Controls.Add(this.chkCurlyParse);
            this.tabOptions.Controls.Add(this.chkActions);
            this.tabOptions.Controls.Add(this.txtSystemAeroVars);
            this.tabOptions.Controls.Add(this.chkAero);
            this.tabOptions.Controls.Add(this.lblSystemVariables);
            this.tabOptions.Controls.Add(this.lblSortHeader);
            this.tabOptions.Controls.Add(this.btnReset);
            this.tabOptions.Controls.Add(this.txtSystemVariables);
            this.tabOptions.Controls.Add(this.chkSortObjects);
            this.tabOptions.Controls.Add(this.chkSortVariables);
            this.tabOptions.Controls.Add(this.chkSortLocations);
            this.tabOptions.Controls.Add(this.txtVariableNames);
            this.tabOptions.Controls.Add(this.chkVariableCalls);
            this.tabOptions.Location = new System.Drawing.Point(4, 22);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tabOptions.Size = new System.Drawing.Size(662, 564);
            this.tabOptions.TabIndex = 1;
            this.tabOptions.Text = "Settings";
            this.tabOptions.UseVisualStyleBackColor = true;
            // 
            // chkCurlyParse
            // 
            this.chkCurlyParse.AutoSize = true;
            this.chkCurlyParse.Checked = global::Analyser.Properties.Settings.Default.EnableCurlyParsing;
            this.chkCurlyParse.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCurlyParse.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "EnableCurlyParsing", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkCurlyParse.Location = new System.Drawing.Point(250, 290);
            this.chkCurlyParse.Name = "chkCurlyParse";
            this.chkCurlyParse.Size = new System.Drawing.Size(250, 17);
            this.chkCurlyParse.TabIndex = 23;
            this.chkCurlyParse.Text = "Content between curly brackets take as a code";
            this.chkCurlyParse.UseVisualStyleBackColor = true;
            // 
            // chkActions
            // 
            this.chkActions.AutoSize = true;
            this.chkActions.Checked = global::Analyser.Properties.Settings.Default.EnableSortingActions;
            this.chkActions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActions.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "EnableSortingActions", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkActions.Location = new System.Drawing.Point(260, 112);
            this.chkActions.Name = "chkActions";
            this.chkActions.Size = new System.Drawing.Size(61, 17);
            this.chkActions.TabIndex = 22;
            this.chkActions.Text = "Actions";
            this.chkActions.UseVisualStyleBackColor = true;
            // 
            // txtSystemAeroVars
            // 
            this.txtSystemAeroVars.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::Analyser.Properties.Settings.Default, "EnableAero", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtSystemAeroVars.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Analyser.Properties.Settings.Default, "AeroSystemVars", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtSystemAeroVars.Enabled = global::Analyser.Properties.Settings.Default.EnableAero;
            this.txtSystemAeroVars.Location = new System.Drawing.Point(423, 42);
            this.txtSystemAeroVars.Multiline = true;
            this.txtSystemAeroVars.Name = "txtSystemAeroVars";
            this.txtSystemAeroVars.ReadOnly = true;
            this.txtSystemAeroVars.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSystemAeroVars.Size = new System.Drawing.Size(157, 190);
            this.txtSystemAeroVars.TabIndex = 21;
            this.txtSystemAeroVars.Text = global::Analyser.Properties.Settings.Default.AeroSystemVars;
            // 
            // chkAero
            // 
            this.chkAero.AutoSize = true;
            this.chkAero.Checked = global::Analyser.Properties.Settings.Default.EnableAero;
            this.chkAero.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAero.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "EnableAero", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkAero.Location = new System.Drawing.Point(423, 13);
            this.chkAero.Name = "chkAero";
            this.chkAero.Size = new System.Drawing.Size(70, 17);
            this.chkAero.TabIndex = 20;
            this.chkAero.Text = "AeroQSP";
            this.chkAero.UseVisualStyleBackColor = true;
            // 
            // lblSystemVariables
            // 
            this.lblSystemVariables.AutoSize = true;
            this.lblSystemVariables.Location = new System.Drawing.Point(0, 255);
            this.lblSystemVariables.Name = "lblSystemVariables";
            this.lblSystemVariables.Size = new System.Drawing.Size(292, 13);
            this.lblSystemVariables.TabIndex = 19;
            this.lblSystemVariables.Text = "System variables (exception from the list of unused variables)";
            // 
            // lblSortHeader
            // 
            this.lblSortHeader.AutoSize = true;
            this.lblSortHeader.Location = new System.Drawing.Point(245, 14);
            this.lblSortHeader.Name = "lblSortHeader";
            this.lblSortHeader.Size = new System.Drawing.Size(76, 13);
            this.lblSortHeader.TabIndex = 15;
            this.lblSortHeader.Text = "Sort by output:";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(13, 519);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(147, 23);
            this.btnReset.TabIndex = 13;
            this.btnReset.Text = "Reset settings";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // txtSystemVariables
            // 
            this.txtSystemVariables.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Analyser.Properties.Settings.Default, "SystemVariables", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtSystemVariables.Location = new System.Drawing.Point(0, 274);
            this.txtSystemVariables.Multiline = true;
            this.txtSystemVariables.Name = "txtSystemVariables";
            this.txtSystemVariables.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSystemVariables.Size = new System.Drawing.Size(213, 220);
            this.txtSystemVariables.TabIndex = 18;
            this.txtSystemVariables.Text = global::Analyser.Properties.Settings.Default.SystemVariables;
            // 
            // chkSortObjects
            // 
            this.chkSortObjects.AutoSize = true;
            this.chkSortObjects.Checked = global::Analyser.Properties.Settings.Default.EnableSortingObjects;
            this.chkSortObjects.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortObjects.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "EnableSortingObjects", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkSortObjects.Location = new System.Drawing.Point(260, 88);
            this.chkSortObjects.Name = "chkSortObjects";
            this.chkSortObjects.Size = new System.Drawing.Size(51, 17);
            this.chkSortObjects.TabIndex = 17;
            this.chkSortObjects.Text = "Items";
            this.chkSortObjects.UseVisualStyleBackColor = true;
            // 
            // chkSortVariables
            // 
            this.chkSortVariables.AutoSize = true;
            this.chkSortVariables.Checked = global::Analyser.Properties.Settings.Default.EnableSortingVariables;
            this.chkSortVariables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortVariables.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "EnableSortingVariables", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkSortVariables.Location = new System.Drawing.Point(260, 65);
            this.chkSortVariables.Name = "chkSortVariables";
            this.chkSortVariables.Size = new System.Drawing.Size(69, 17);
            this.chkSortVariables.TabIndex = 16;
            this.chkSortVariables.Text = "Variables";
            this.chkSortVariables.UseVisualStyleBackColor = true;
            // 
            // chkSortLocations
            // 
            this.chkSortLocations.AutoSize = true;
            this.chkSortLocations.Checked = global::Analyser.Properties.Settings.Default.EnableSortingLocations;
            this.chkSortLocations.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortLocations.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "EnableSortingLocations", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkSortLocations.Location = new System.Drawing.Point(260, 42);
            this.chkSortLocations.Name = "chkSortLocations";
            this.chkSortLocations.Size = new System.Drawing.Size(72, 17);
            this.chkSortLocations.TabIndex = 14;
            this.chkSortLocations.Text = "Locations";
            this.chkSortLocations.UseVisualStyleBackColor = true;
            // 
            // txtVariableNames
            // 
            this.txtVariableNames.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Analyser.Properties.Settings.Default, "VariableNames", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtVariableNames.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::Analyser.Properties.Settings.Default, "EnableVariableCalls", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtVariableNames.Enabled = global::Analyser.Properties.Settings.Default.EnableVariableCalls;
            this.txtVariableNames.Location = new System.Drawing.Point(0, 42);
            this.txtVariableNames.Multiline = true;
            this.txtVariableNames.Name = "txtVariableNames";
            this.txtVariableNames.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtVariableNames.Size = new System.Drawing.Size(213, 190);
            this.txtVariableNames.TabIndex = 12;
            this.txtVariableNames.Text = global::Analyser.Properties.Settings.Default.VariableNames;
            // 
            // chkVariableCalls
            // 
            this.chkVariableCalls.AutoSize = true;
            this.chkVariableCalls.Checked = global::Analyser.Properties.Settings.Default.EnableVariableCalls;
            this.chkVariableCalls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkVariableCalls.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "EnableVariableCalls", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkVariableCalls.Location = new System.Drawing.Point(3, 6);
            this.chkVariableCalls.Name = "chkVariableCalls";
            this.chkVariableCalls.Size = new System.Drawing.Size(166, 30);
            this.chkVariableCalls.TabIndex = 11;
            this.chkVariableCalls.Text = "Consider variable assignment \r\nby link to location:";
            this.chkVariableCalls.UseVisualStyleBackColor = true;
            // 
            // tabTranslate
            // 
            this.tabTranslate.Controls.Add(this.btnBeautify);
            this.tabTranslate.Controls.Add(this.grpTranslateCsv);
            this.tabTranslate.Location = new System.Drawing.Point(4, 22);
            this.tabTranslate.Name = "tabTranslate";
            this.tabTranslate.Size = new System.Drawing.Size(662, 564);
            this.tabTranslate.TabIndex = 2;
            this.tabTranslate.Text = "Conversion";
            this.tabTranslate.UseVisualStyleBackColor = true;
            // 
            // btnBeautify
            // 
            this.btnBeautify.Location = new System.Drawing.Point(56, 279);
            this.btnBeautify.Name = "btnBeautify";
            this.btnBeautify.Size = new System.Drawing.Size(129, 23);
            this.btnBeautify.TabIndex = 9;
            this.btnBeautify.Text = "Format the code";
            this.btnBeautify.UseVisualStyleBackColor = true;
            this.btnBeautify.Click += new System.EventHandler(this.btnBeautify_Click);
            // 
            // grpTranslateCsv
            // 
            this.grpTranslateCsv.Controls.Add(this.grpCsvDelimiter);
            this.grpTranslateCsv.Controls.Add(this.btnTranslateFromCsv);
            this.grpTranslateCsv.Controls.Add(this.chkIgnoreEmptyTranslationsCsv);
            this.grpTranslateCsv.Controls.Add(this.lblSuffixCsv);
            this.grpTranslateCsv.Controls.Add(this.btnExportCsv);
            this.grpTranslateCsv.Controls.Add(this.edtSuffixCsv);
            this.grpTranslateCsv.Location = new System.Drawing.Point(7, 3);
            this.grpTranslateCsv.Name = "grpTranslateCsv";
            this.grpTranslateCsv.Size = new System.Drawing.Size(642, 168);
            this.grpTranslateCsv.TabIndex = 8;
            this.grpTranslateCsv.TabStop = false;
            this.grpTranslateCsv.Text = "Conversion";
            // 
            // grpCsvDelimiter
            // 
            this.grpCsvDelimiter.Controls.Add(this.radComma);
            this.grpCsvDelimiter.Controls.Add(this.radSemicolon);
            this.grpCsvDelimiter.Location = new System.Drawing.Point(24, 29);
            this.grpCsvDelimiter.Name = "grpCsvDelimiter";
            this.grpCsvDelimiter.Size = new System.Drawing.Size(138, 72);
            this.grpCsvDelimiter.TabIndex = 3;
            this.grpCsvDelimiter.TabStop = false;
            this.grpCsvDelimiter.Text = "Split CSV";
            // 
            // radComma
            // 
            this.radComma.AutoSize = true;
            this.radComma.Checked = true;
            this.radComma.Location = new System.Drawing.Point(6, 19);
            this.radComma.Name = "radComma";
            this.radComma.Size = new System.Drawing.Size(72, 17);
            this.radComma.TabIndex = 1;
            this.radComma.TabStop = true;
            this.radComma.Text = "Comma - ,";
            this.radComma.UseVisualStyleBackColor = true;
            // 
            // radSemicolon
            // 
            this.radSemicolon.AutoSize = true;
            this.radSemicolon.Location = new System.Drawing.Point(6, 42);
            this.radSemicolon.Name = "radSemicolon";
            this.radSemicolon.Size = new System.Drawing.Size(86, 17);
            this.radSemicolon.TabIndex = 2;
            this.radSemicolon.Text = "Semicolon - ;";
            this.radSemicolon.UseVisualStyleBackColor = true;
            // 
            // btnTranslateFromCsv
            // 
            this.btnTranslateFromCsv.Location = new System.Drawing.Point(303, 126);
            this.btnTranslateFromCsv.Name = "btnTranslateFromCsv";
            this.btnTranslateFromCsv.Size = new System.Drawing.Size(198, 23);
            this.btnTranslateFromCsv.TabIndex = 4;
            this.btnTranslateFromCsv.Text = "Make conversion, use CSV";
            this.btnTranslateFromCsv.UseVisualStyleBackColor = true;
            this.btnTranslateFromCsv.Click += new System.EventHandler(this.btnTranslateFromCsv_Click);
            // 
            // chkIgnoreEmptyTranslationsCsv
            // 
            this.chkIgnoreEmptyTranslationsCsv.AutoSize = true;
            this.chkIgnoreEmptyTranslationsCsv.Checked = global::Analyser.Properties.Settings.Default.IgnoreEmptyTranslationsCsv;
            this.chkIgnoreEmptyTranslationsCsv.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkIgnoreEmptyTranslationsCsv.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Analyser.Properties.Settings.Default, "IgnoreEmptyTranslationsCsv", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chkIgnoreEmptyTranslationsCsv.Location = new System.Drawing.Point(235, 55);
            this.chkIgnoreEmptyTranslationsCsv.Name = "chkIgnoreEmptyTranslationsCsv";
            this.chkIgnoreEmptyTranslationsCsv.Size = new System.Drawing.Size(295, 30);
            this.chkIgnoreEmptyTranslationsCsv.TabIndex = 7;
            this.chkIgnoreEmptyTranslationsCsv.Text = "Ignore unconverted strings\r\n(for \"empty\" value of conversion will be used origina" +
    "l text)";
            this.chkIgnoreEmptyTranslationsCsv.UseVisualStyleBackColor = true;
            // 
            // lblSuffixCsv
            // 
            this.lblSuffixCsv.AutoSize = true;
            this.lblSuffixCsv.Location = new System.Drawing.Point(232, 32);
            this.lblSuffixCsv.Name = "lblSuffixCsv";
            this.lblSuffixCsv.Size = new System.Drawing.Size(142, 13);
            this.lblSuffixCsv.TabIndex = 6;
            this.lblSuffixCsv.Text = "Extension for conversion file:";
            // 
            // btnExportCsv
            // 
            this.btnExportCsv.Location = new System.Drawing.Point(41, 126);
            this.btnExportCsv.Name = "btnExportCsv";
            this.btnExportCsv.Size = new System.Drawing.Size(97, 23);
            this.btnExportCsv.TabIndex = 0;
            this.btnExportCsv.Text = "Export to CSV";
            this.btnExportCsv.UseVisualStyleBackColor = true;
            this.btnExportCsv.Click += new System.EventHandler(this.btnExportCsv_Click);
            // 
            // edtSuffixCsv
            // 
            this.edtSuffixCsv.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Analyser.Properties.Settings.Default, "CsvTranslationSuffix", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.edtSuffixCsv.Location = new System.Drawing.Point(401, 29);
            this.edtSuffixCsv.Name = "edtSuffixCsv";
            this.edtSuffixCsv.Size = new System.Drawing.Size(100, 20);
            this.edtSuffixCsv.TabIndex = 5;
            this.edtSuffixCsv.Text = global::Analyser.Properties.Settings.Default.CsvTranslationSuffix;
            // 
            // edtFile
            // 
            this.edtFile.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Analyser.Properties.Settings.Default, "filepath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.edtFile.Location = new System.Drawing.Point(409, 10);
            this.edtFile.Name = "edtFile";
            this.edtFile.Size = new System.Drawing.Size(176, 20);
            this.edtFile.TabIndex = 1;
            this.edtFile.Text = global::Analyser.Properties.Settings.Default.filepath;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(5, 626);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(659, 23);
            this.progressBar1.TabIndex = 13;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 653);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.edtFile);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "QSP Code Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.tabControl.ResumeLayout(false);
            this.tabCheck.ResumeLayout(false);
            this.tabCheck.PerformLayout();
            this.tabOptions.ResumeLayout(false);
            this.tabOptions.PerformLayout();
            this.tabTranslate.ResumeLayout(false);
            this.grpTranslateCsv.ResumeLayout(false);
            this.grpTranslateCsv.PerformLayout();
            this.grpCsvDelimiter.ResumeLayout(false);
            this.grpCsvDelimiter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox edtFile;
        private System.Windows.Forms.OpenFileDialog dlgOpen;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtList1;
        private System.Windows.Forms.TextBox txtList2;
        private System.Windows.Forms.TextBox txtList3;
        private System.Windows.Forms.Button btnTry1;
        private System.Windows.Forms.Button btnTry2;
        private System.Windows.Forms.Button btnTry3;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabCheck;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.CheckBox chkVariableCalls;
        private System.Windows.Forms.TextBox txtVariableNames;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblSortHeader;
        private System.Windows.Forms.CheckBox chkSortLocations;
        private System.Windows.Forms.CheckBox chkSortObjects;
        private System.Windows.Forms.CheckBox chkSortVariables;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Label lblSystemVariables;
        private System.Windows.Forms.TextBox txtSystemVariables;
        private System.Windows.Forms.CheckBox chkAero;
        private System.Windows.Forms.TextBox txtSystemAeroVars;
        private System.Windows.Forms.Button btnTry4;
        private System.Windows.Forms.CheckBox chkActions;
        private System.Windows.Forms.TabPage tabTranslate;
        private System.Windows.Forms.GroupBox grpCsvDelimiter;
        private System.Windows.Forms.RadioButton radComma;
        private System.Windows.Forms.RadioButton radSemicolon;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.TextBox edtSuffixCsv;
        private System.Windows.Forms.Button btnTranslateFromCsv;
        private System.Windows.Forms.Label lblSuffixCsv;
        private System.Windows.Forms.CheckBox chkIgnoreEmptyTranslationsCsv;
        private System.Windows.Forms.CheckBox chkCurlyParse;
        private System.Windows.Forms.GroupBox grpTranslateCsv;
        private System.Windows.Forms.Button btnBeautify;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnCancel;
    }
}

