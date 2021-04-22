using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace SQLServerDatabaseManagement
{
    public partial class frm_Main : XtraForm
    {
        private SQLServer _sqlServer = new SQLServer();
        private  string _connectionString = string.Empty;
        private string _database = string.Empty;
        private bool _isDisconnect = false;
        private const string SYSTEM_ERROR = "Error";

        public frm_Main()
        {
            InitializeComponent();
        }

        private void LoadDatabase()
        {
            try
            {
                var dt = _sqlServer.GetDatabase();
                gv_Database.BestFitColumns();
                grd_Database.DataSource = dt;
                grd_Database.RefreshDataSource();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LoadTables(string database)
        {
            try
            {
                var dt = _sqlServer.GetTables(database);
                grd_Tables.DataSource = dt;
                grd_Tables.RefreshDataSource();
                gv_Tables.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LoadTableColumns(string database, string tableName)
        {
            try
            {
                var dt = _sqlServer.GetTableColumns(database, tableName);
                grd_Columns.DataSource = dt;
                grd_Columns.RefreshDataSource();
                gv_Columns.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LoadViews(string database)
        {
            try
            {
                var dt = _sqlServer.GetViews(database);
                grd_Views.DataSource = dt;
                grd_Views.RefreshDataSource();
                gv_Views.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LoadFunctions(string database)
        {
            try
            {
                var dt = _sqlServer.GetFunctions(database);
                grd_Functions.DataSource = dt;
                grd_Functions.RefreshDataSource();
                gv_Functions.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LoadParamsFunctions(string database, int funcId)
        {
            try
            {
                var dt = _sqlServer.GetFunctionParams(database, funcId);
                grd_FunctionParams.DataSource = dt;
                grd_FunctionParams.RefreshDataSource();

                dt = _sqlServer.GetDataTypeReturnFunction(database, funcId);
                grd_DataReturn.DataSource = dt;
                grd_DataReturn.RefreshDataSource();

                gv_FunctionParams.BestFitColumns();
                gv_DataReturn.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }


        private void LoadStoredProc(string database)
        {
            try
            {
                var dt = _sqlServer.GetStoredProcedures(database);
                grd_StoredProc.DataSource = dt;
                grd_StoredProc.RefreshDataSource();
                gv_StoredProc.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LoadParamsStoredProc(string database, int storedId)
        {
            try
            {
                var dt = _sqlServer.GetStoredProcParams(database, storedId);
                grd_StoredProcParams.DataSource = dt;
                grd_StoredProcParams.RefreshDataSource();
                gv_StoredProcParams.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void LoadTriggers(string database)
        {
            try
            {
                var dt = _sqlServer.GetTriggers(database);
                grd_Triggers.DataSource = dt;
                grd_Triggers.RefreshDataSource();
                gv_Triggers.BestFitColumns();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ClearDataSource(bool clearDB)
        {
            _database = string.Empty;
            _connectionString = string.Empty;
            if (clearDB)
            {
                (grd_Database.DataSource as DataTable).Rows.Clear();
                grd_Database.DataSource = null;
                grd_Database.RefreshDataSource();
            }
            grd_Tables.DataSource = null;
            grd_Tables.RefreshDataSource();
            grd_Columns.DataSource = null;
            grd_Columns.RefreshDataSource();
            grd_Views.DataSource = null;
            grd_Views.RefreshDataSource();
            grd_Functions.DataSource = null;
            grd_Functions.RefreshDataSource();
            grd_FunctionParams.DataSource = null;
            grd_FunctionParams.RefreshDataSource();
            grd_DataReturn.DataSource = null;
            grd_DataReturn.RefreshDataSource();
            grd_StoredProc.DataSource = null;
            grd_StoredProc.RefreshDataSource();
            grd_StoredProcParams.DataSource = null;
            grd_StoredProcParams.RefreshDataSource();
            grd_Triggers.DataSource = null;
            grd_Triggers.RefreshDataSource();
        }

        private void SetReadOnlyControls(bool isReadOnly)
        {
            txt_ServerName.ReadOnly = isReadOnly;
            lk_Authentication.ReadOnly = isReadOnly;
            txt_UserLogin.ReadOnly = isReadOnly;
            txt_Password.ReadOnly = isReadOnly;
        }

        private void frm_Main_Load(object sender, EventArgs e)
        {
            try
            {
                lk_Authentication.Properties.DataSource = new Dictionary<string, string>()
                {
                    {"WinAuth", "Windows Authentication"},
                    {"SQLAuth", "SQL Server Authentication"}
                };
                lk_Authentication.ItemIndex = 0;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void btn_Connect_Click(object sender, EventArgs e)
        {
            try
            {
                _connectionString = string.Empty;
                if (string.IsNullOrEmpty(txt_ServerName.Text))
                {
                    XtraMessageBox.Show("Server name cannot empty value!", SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_ServerName.Focus();
                    return;
                }

                if (lk_Authentication.ItemIndex == 1)
                {
                    if (string.IsNullOrEmpty(txt_UserLogin.Text))
                    {
                        XtraMessageBox.Show("User login cannot empty value!", SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txt_UserLogin.Focus();
                        return;
                    }
                    if (string.IsNullOrEmpty(txt_Password.Text))
                    {
                        XtraMessageBox.Show("Password cannot empty value!", SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txt_Password.Focus();
                        return;
                    }
                    _connectionString = _sqlServer.GetConnectionString(txt_ServerName.Text, txt_UserLogin.Text, txt_Password.Text, "master");
                }
                else
                {
                    _connectionString = _sqlServer.GetConnectionString(txt_ServerName.Text, "master");
                }

                this.Cursor = Cursors.WaitCursor;
                var check = _sqlServer.CheckConnection(_connectionString);
                if (!check)
                {
                    _isDisconnect = true;
                    XtraMessageBox.Show("Connect to server failed!", SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Cursor = Cursors.Default;
                    return;
                }
                _isDisconnect = false;
                btn_Connect.Enabled = false;
                btn_Disconnect.Enabled = true;
                SetReadOnlyControls(true);
                _sqlServer.ConnectionString = _connectionString;
                _sqlServer.DbConnection = _sqlServer.OpenConnection();
                lbl_Server.Caption = @"Server Name: " + txt_ServerName.Text;
                lbl_User.Caption = @"User: " + txt_UserLogin.Text;
                lbl_Status.Caption = @"Status: Connected";
                LoadDatabase();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            this.Cursor = Cursors.Default;
        }

        private void gv_Database_FocusedRowObjectChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventArgs e)
        {
            try
            {
                if (e.FocusedRowHandle < 0)
                {
                    return;
                }
                if (_isDisconnect)
                {
                    return;
                }
                ClearDataSource(false);
                _database = gv_Database.GetFocusedRowCellValue("name").ToString();
                LoadTables(_database);
                LoadViews(_database);
                LoadFunctions(_database);
                LoadStoredProc(_database);
                LoadTriggers(_database);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void gv_Tables_FocusedRowObjectChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventArgs e)
        {
            try
            {
                if (e.FocusedRowHandle < 0)
                {
                    return;
                }
                var table = gv_Tables.GetRowCellValue(e.FocusedRowHandle, "TABLE_NAME").ToString();
                LoadTableColumns(_database, table);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void lk_Authentication_EditValueChanged(object sender, EventArgs e)
        {
            lbl_UserLogin.Enabled = lk_Authentication.ItemIndex == 1;
            lbl_Password.Enabled = lk_Authentication.ItemIndex == 1;
            txt_UserLogin.Enabled = lk_Authentication.ItemIndex == 1;
            txt_Password.Enabled = lk_Authentication.ItemIndex == 1;
            if (lk_Authentication.ItemIndex == 1)
            {
                txt_UserLogin.Focus();
            }
            else
            {
                txt_UserLogin.ResetText();
                txt_Password.ResetText();
            }
        }

        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                _isDisconnect = true;
                _sqlServer.CloseConnection();
                btn_Connect.Enabled = true;
                btn_Disconnect.Enabled = false;
                SetReadOnlyControls(false);
                ClearDataSource(true);
                lbl_Server.Caption = string.Empty;
                lbl_User.Caption = string.Empty;
                lbl_Status.Caption = string.Empty;
                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void gv_Stored_FocusedRowObjectChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventArgs e)
        {
            try
            {
                if (e.FocusedRowHandle < 0)
                {
                    return;
                }
                var id = int.Parse(gv_StoredProc.GetFocusedRowCellValue("STORE_ID").ToString());
                LoadParamsStoredProc(_database, id);
            }
            catch (Exception ex)
            {

                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void gv_Functions_FocusedRowObjectChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventArgs e)
        {
            try
            {
                if (e.FocusedRowHandle < 0)
                {
                    return;
                }
                var id = int.Parse(gv_Functions.GetFocusedRowCellValue("FUNC_ID").ToString());
                LoadParamsFunctions(_database, id);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.Message, SYSTEM_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
