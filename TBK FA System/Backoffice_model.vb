Imports System.Data.SqlClient
Imports System.Data.SQLite
Imports System.Globalization
Imports System.Data
Public Class Backoffice_model
	Public Shared total_nc As Integer = 0
	Public Shared flg_cat_layout_line As Integer = 0
	'Public Shared myConnection As New SqlConnection  'ตัวแปรสำหรับติดต่อฐานข้อมูล
	'Public Shared sqlConnect As String = "Server=192.168.161.101\PCSDBSV;Initial Catalog=tbkkfa01_dev;User ID=sa;Password=Te@m1nw;"
	Public Shared temp2Str As String
	Public Shared arr_backet_camp As List(Of String) = New List(Of String)
	Public Shared sqlConnect As String
	Public Shared qty_int As String
	Public Shared sSql As String 'ตัวแปรคำสั่ง sql
	Public Shared sqliteConnect As String = "Data Source=c:\sqlite3\FA_local_db.db3;Version=3"
	Public Shared img_user1 As Bitmap
	Public Shared img_user2 As Bitmap
	Public Shared img_user3 As Bitmap
	Public Shared img_user4 As Bitmap
	Public Shared img_user5 As Bitmap
	Public Shared img_user6 As Bitmap
	Public Shared MIN_PK_LOSS_ID As Integer = 0
	Public Shared LINE_PRODUCTION As String
	Public Shared SCANNER_PORT As String = ""
	Public Shared coles_lot_start_shift As String = ""
	Public Shared coles_lot_end_shift As String = ""
	Public Shared NEXT_PROCESS As String = ""
	Public Shared check_user As Integer = 0
	Public Shared arr_list_user As List(Of String) = New List(Of String)
	Public Shared S_chk_spec_line As String = "0"
	Public Shared start_check_date_paralell_line As String = ""
	Public Shared end_check_date_paralell_line As String = ""
    Public Shared start_master_shift As String = ""
    Public Shared date_time_start_master_shift As Date
    Public Shared date_time_end_check_date_paralell_linet As Date
    Public Shared date_time_click_start As Date
    'MsgBox(temp2Str)
    'SQLite.SQLiteDataReader

    Public Shared Function B_master_device()
        Dim sqliteConn As New SQLiteConnection(sqliteConnect)
        Try
            sqliteConn.Open()
        Catch ex As Exception
            sqliteConn.Close()
            sqliteConn.Open()
        End Try
        Try
            Dim cmd As New SQLiteCommand
            cmd.Connection = sqliteConn
            cmd.CommandText = "select * from catagory_counter where count_flg='1'"
            Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
            sqliteConn.Close()
            Return LoadSQL
        Catch ex As Exception
            MsgBox("SQLite Database connect failed. Please contact PC System [Function Clear_sqlite]" & ex.Message)
            sqliteConn.Dispose()
            'sqliteConn.Close()
            sqliteConn = Nothing
        End Try
    End Function
    Public Shared Function F_NEXT_PROCESS(ITEM_CD As String)
        Dim api = New api()
        Dim check_tag_type = api.Load_data("http://192.168.161.207/API_NEW_FA/GET_DATA_NEW_FA/GET_LINE_TYPE?line_cd=" & MainFrm.Label4.Text)
        If check_tag_type = "1" Then
            Dim result_update_count_pro1 = api.Load_data("http://192.168.161.207/API_NEW_FA/Api_next_process?line_cd=" & GET_LINE_PRODUCTION() & "&item_cd=" & ITEM_CD)
            Return result_update_count_pro1
        Else
            Return "ISUZU"
        End If
    End Function
    Public Shared Sub Clear_sqlite()
        '  Dim currdated As Date = DateTime.Now.ToString("yyyy/MM/dd")
        ' st_time = currdated.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)

        Dim currdated As String = DateTime.Now.ToString("yyyy-MM-dd")
        Dim today As Date = Date.Today
        Dim date_start As DateTime = today.AddDays(-200)
        Dim format_tommorow = "yyyy-MM-dd"
        Dim convert_date_start = date_start.ToString(format_tommorow)

        Dim currdated1 As String = DateTime.Now.ToString("yyyy/MM/dd")
        Dim today1 As Date = Date.Today
        Dim date_start1 As DateTime = today1.AddDays(-200)
        Dim format_tommorow1 = "yyyy/MM/dd"
        Dim convert_date_start1 = date_start1.ToString(format_tommorow1)

        Dim command_data() As String = {
                "DELETE FROM act_ins where st_time BETWEEN '" & convert_date_start & "' AND '" & currdated & "' and tr_status = '1' ",
                "DELETE FROM close_lot_act where prd_st_date BETWEEN '" & convert_date_start & "' AND '" & currdated & "' and transfer_flg = '1'",
                "DELETE FROM loss_actual where start_loss BETWEEN '" & convert_date_start1 & "' AND '" & currdated1 & "' and transfer_flg = '1'"
            }
        For i = 0 To command_data.Length - 1
            Check_connect_sqlite()
            Dim sqliteConn As New SQLiteConnection(sqliteConnect)
            Try
                sqliteConn.Open()
            Catch ex As Exception
                sqliteConn.Close()
                sqliteConn.Open()
            End Try
            Try
                Dim cmd As New SQLiteCommand
                cmd.Connection = sqliteConn
                cmd.CommandText = command_data(i)
                Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
                sqliteConn.Close()

            Catch ex As Exception
                MsgBox("SQLite Database connect failed. Please contact PC System [Function Clear_sqlite]" & ex.Message)
                sqliteConn.Dispose()
                'sqliteConn.Close()
                sqliteConn = Nothing
            End Try
        Next
    End Sub
    Public Shared Function check_version_result(NAME_VERSION As String)
		Dim api = New api()
		Dim result_update_count_pro = api.Load_data("http://192.168.161.207/API_NEW_FA/UPDATE_PATCH/F_UPDATE_PATCH?VERSION_NAME=" & NAME_VERSION)
		Return result_update_count_pro
	End Function
	Public Shared Function CHECK_VERSION(NAME_VERSION As String)
		Dim api = New api()
		Dim result_update_count_pro = api.Load_data("http://192.168.161.207/API_NEW_FA/UPDATE_PATCH/F_UPDATE_PATCH?VERSION_NAME=" & NAME_VERSION)
		Return result_update_count_pro
	End Function
	Public Shared Function SET_LINE_PRODUCTION(line As String)
		LINE_PRODUCTION = line
	End Function
	Public Shared Function GET_LINE_PRODUCTION()
		Return LINE_PRODUCTION
	End Function
	Public Shared Function Get_close_lot_time(SHIFT As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
		Try
			SQLConn.Open()
		Catch ex As Exception
			SQLConn.Close()
			SQLConn.Open()
		End Try
		Try
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_TIME_CLOSE_LOT_SHIFT] @SHIFT = '" & SHIFT & "'"
			reader = SQLCmd.ExecuteReader()
			While reader.Read()
				'temp2Str = "test"
				'If reader.Read() Then
				coles_lot_start_shift = reader("coles_lot_start_time").ToString()
				coles_lot_end_shift = reader("coles_lot_end_time").ToString()
                start_master_shift = reader("master_start_shift").ToString()

                date_time_start_master_shift = DateTime.Now.ToString("yyyy-MM-dd") & " " & start_master_shift
                If Trim(Prd_detail.Label12.Text.Substring(0, 1)) = "B" Or Trim(Prd_detail.Label12.Text.Substring(0, 1)) = "Q" Or Trim(Prd_detail.Label12.Text.Substring(0, 1)) = "S" Then
                    date_time_end_check_date_paralell_linet = DateTime.Now.ToString("yyyy-MM-dd") & " " & coles_lot_end_shift
                    date_time_end_check_date_paralell_linet = date_time_end_check_date_paralell_linet.AddDays(1)
                Else
                    date_time_end_check_date_paralell_linet = DateTime.Now.ToString("yyyy-MM-dd") & " " & coles_lot_end_shift
                End If
                'Else
                'MsgBox("ไม่มีข้อมูลกะการผลิต")
                'End If
            End While
			reader.Close()
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Get_close_lot_time]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function check_time(SEQ_NO, WI_PLAN, ST_TIME, END_TIME)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
		Try
			SQLConn.Open()
		Catch ex As Exception
			SQLConn.Close()
			SQLConn.Open()
		End Try
		Try
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[CHECK_SPECIAL_TIME] @seq_no = '" & SEQ_NO & "' , @wi_plan = '" & WI_PLAN & "', @st_time = '" & ST_TIME & "', @end_time = '" & END_TIME & "'"
			reader = SQLCmd.ExecuteReader()
			Dim result = 0
			While reader.Read()
				result = reader("c_id").ToString()
			End While
			reader.Close()
			Return result
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function check_time]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function sqlite_conn_dbsv()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
        Check_connect_sqlite()
        Clear_sqlite()

        Try
			sqliteConn.Open()
			Dim temp_stre As String
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from db_svr_info"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			While LoadSQL.Read()
				'temp2Str = "test"
				temp_stre = "Server=" & LoadSQL("ipaddress").ToString() & ";Initial Catalog=" & LoadSQL("db_name").ToString() & ";User ID=" & LoadSQL("username").ToString() & ";Password=" & LoadSQL("passwd").ToString() & ";"
			End While
			sqlConnect = temp_stre
			Return temp2Str
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function sqlite_conn_dbsv]")
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function get_new_information()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
		Try
			SQLConn.Open()
		Catch ex As Exception
			SQLConn.Close()
			'  SQLConn.Open()
		End Try
		Try
			SQLCmd.Connection = SQLConn
			Dim start_date = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
			SQLCmd.CommandText = "EXEC [dbo].[GET_INFORMATION] @date_now = '" & start_date & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_new_information]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function insert_lot_print_defact(wi_plan, item_cd, tag_defact_lot_no, tag_defact_seq, tag_defact_created_date, tag_defact_created_by, tag_defact_qr, tag_defact_status, tag_defact_line_cd)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
		Try
			SQLConn.Open()
		Catch ex As Exception
			SQLConn.Close()
			'  SQLConn.Open()
		End Try
		Try
			SQLCmd.Connection = SQLConn
			Dim time_tomorrow As DateTime = tag_defact_created_date
			Dim format_tommorow = "yyyy-MM-dd"
			Dim date_now_cerrnet = time_tomorrow.ToString(format_tommorow)
			Dim start_date = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
			SQLCmd.CommandText = "EXEC [dbo].[INSERT_LOG_PRINT_DEFACT_NC]  @tag_defact_wi = '" & wi_plan & "' , @tag_defact_item_cd = '" & item_cd & "' , @tag_defact_lot_no = '" & tag_defact_lot_no & "' , @tag_defact_seq = '" & tag_defact_seq & "' , @tag_defact_created_date = '" & date_now_cerrnet & "' , @tag_defact_qr = '" & tag_defact_qr & " ' ,  @tag_defact_status = '" & tag_defact_status & " ' , @tag_defact_line_cd = '" & tag_defact_line_cd & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function INSERT_LOG_PRINT_DEFACT_NC]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function load_qty_defact(item_cd As String, lot_no As String, seq As String, date_now As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
		Try
			SQLConn.Open()
		Catch ex As Exception
			SQLConn.Close()
			'  SQLConn.Open()
		End Try
		Try
			SQLCmd.Connection = SQLConn
			Dim start_date = DateTime.Now.ToString("yyyy-MM-dd H:m:s")
			SQLCmd.CommandText = "EXEC [dbo].[LOAD_QTY_DEFACT] @item_cd = '" & item_cd & "' , @lot_no = '" & lot_no & "' , @seq = '" & seq & "' , @date_now = '" & date_now & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function LOAD_QTY_DEFACT]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function GET_QTY_DEFACT_NC(WI As String, line_cd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
		Try
			SQLConn.Open()
		Catch ex As Exception
			SQLConn.Close()
			'  SQLConn.Open()
		End Try
		Try
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_QTY_DEFACT_NC] @WI = '" & WI & "' , @line_cd = '" & line_cd & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_QTY_DEFACT_NC]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function


	Public Shared Function get_data_wi_reprint(start_date, end_date, line_cd)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			start_date = start_date & " 00:00:00"
			end_date = end_date & " 23:59:59"
			SQLCmd.CommandText = "EXEC [dbo].[GET_WI_OF_DAY] @Date_work_production_start = '" & start_date & "' , @Date_work_production_end = '" & end_date & "',
 @line_cd = '" & line_cd & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_data_wi_reprint]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function get_data_item(WI)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_DATA_ABOUT_ITEM] @WI = '" & WI & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_data_wi_reprint]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function get_list_rm_scan(WI)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_RM_SCAN] @WI = '" & WI & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_list_rm_scan]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function Insert_Rm_Scan(WI As String, ITEM_CD As String, LOT_PO As String, SEQ As String, SHIFT As String, Rm_created_date As String, Rm_created_by As String, Rm_Updated_date As String, Rm_updated_by As String, Rm_line_cd As String, Rm_QR_code As String , ref_id As String )
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[INSERT_RM_SCAN] 
                                    @WI='" & WI & "' , 
                                    @ITEM_CD='" & ITEM_CD & "' , 
                                    @LOT_PO='" & LOT_PO & "' ,
                                    @SEQ='" & SEQ & "' ,
                                    @SHIFT='" & SHIFT.Substring(0, 1) & "' ,
                                    @Rm_created_date='" & Rm_created_date & "' ,
                                    @Rm_created_by='" & Rm_created_by & "' , 
                                    @Rm_Updated_date='" & Rm_Updated_date & "' , 
                                    @Rm_updated_by='" & Rm_updated_by & "' ,
                                    @Rm_line_cd='" & Rm_line_cd & "',
                                    @Rm_QR_code='" & Rm_QR_code & "',
									@Rm_ref_id='" & ref_id & "'"
			reader = SQLCmd.ExecuteReader()
			reader.Close()
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_Rm_Scan]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function GET_QTY_SEQ(WI, SEQ_NO)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_QTY_SEQ] @WI = '" & WI & "' , @SEQ = '" & SEQ_NO & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_QTY_SEQ]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function INSERT_DATA_RM_SCAN()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_QTY_SEQ] @WI = '" & WI & "' , @SEQ = '" & SEQ_NO & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function INSERT_DATA_RM_SCAN]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function GET_CHECK_LOSS(start_loss As String, end_loss As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			Try
				SQLConn.Open()
			Catch ex As Exception
				SQLConn.Close()
				SQLConn.Open()
			End Try
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[CHECK_LOSS_DOUBLE] @date_start = '" & start_loss.ToString() & "' , @date_end = '" & end_loss.ToString() & "', @line_cd = '" & GET_LINE_PRODUCTION() & "'"
            reader = SQLCmd.ExecuteReader()
            Dim tmp_result As Integer = 0
			While reader.Read()
				tmp_result = reader("c_id").ToString()
			End While
			reader.Close()
			Return tmp_result
			'Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_CHECK_LOSS]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function INSERT_REWORK_ACTUAL(RAW_PART_NO, RAW_QTY, RAW_SHIFT, RWA_CREATED_DATE_TIME, RWA_WI, RWA_PART_NAME, RWA_MODEL)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[INSERT_DATA_REWORK_ACTUAL] @RAW_PART_NO = '" & RAW_PART_NO & "' , @RAW_QTY = '" & RAW_QTY & "', @RAW_SHIFT = '" & RAW_SHIFT & "', @RWA_CREATED_DATE_TIME = '" & RWA_CREATED_DATE_TIME & "', @RWA_WI = '" & RWA_WI & "', @RWA_PART_NAME = '" & RWA_PART_NAME & "', @RWA_MODEL = '" & RWA_MODEL & "'"
			reader = SQLCmd.ExecuteReader()
			'Return reader
			reader.Close()
			SQLConn.Close()
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function INSERT_REWORK_ACTUAL]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function



	Public Shared Function INSERT_REWORK_ACTUAL_SQLITE(RAW_PART_NO, RAW_QTY, RAW_SHIFT, RWA_CREATED_DATE_TIME, RWA_WI, RWA_PART_NAME, RWA_MODEL, tr_status)
re_insert_rework_act:
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "INSERT INTO rework_actual (rwa_part_no,rwa_qty,rwa_ship,rwa_created_date_time,ref_wi ,rwa_part_name ,rwa_model,tr_status  )
		VALUES(
				'" & RAW_PART_NO & "', 
				'" & RAW_QTY & "',
				'" & RAW_SHIFT & "',
				'" & RWA_CREATED_DATE_TIME & "',
				'" & RWA_WI & "',
				'" & RWA_PART_NAME & "',
				'" & RWA_MODEL & "',
                '" & tr_status & "'
		)"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			sqliteConn.Close()
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function ConnectDBSQLite]")
			sqliteConn.Close()
			GoTo re_insert_rework_act
		End Try
	End Function

	Public Shared Function chk_spec_line()
		Dim api = New api()
		Dim result_update_count_pro = api.Load_data("http://192.168.161.207/API_NEW_FA/Api_check_data/chk_spec_line?line_cd=" & GET_LINE_PRODUCTION())
		Return result_update_count_pro
	End Function
	Public Shared Function INSERT_tmp_planseq(wi, line_cd, date_start, date_end)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT max(seq_no) as seq_no from production_actual where wi = '" & wi & "'"
			SQLCmd.CommandText = "insert into tmp_planseq (tmp_line_cd , tmp_production_date , tmp_last_sequence , tmp_created_date , tmp_created_by , tmp_updated_date , tmp_updated_by) 
			values(
					'" & line_cd & "',
					'" & date_start & "',
					'1',
					'" & date_start & "',
					'SYSTEM',
					'" & date_start & "',
					'SYSTEM'
			)"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function INSERT_tmp_planseq]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function GET_SEQ_PLAN_current(wi, line_cd, date_start, date_end)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Dim time As String = Trim(date_start.Substring(11))
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim today As Date = Date.Today
		Dim time_tomorrow As DateTime = today.AddDays(1)
		Dim format_tommorow = "yyyy/MM/dd"
		Dim date_tommorow = time_tomorrow.ToString(format_tommorow)
		date_end_covert = date_tommorow & " 07:59:59"
		Try
			Dim time_now As DateTime
			time_now = DateTime.Now.ToString("hh:mm:ss tt")
			If time_now >= "08:00:00 AM" And time_now <= "07:59:59 PM" Then
				date_start = currdated & " 08:00:00"
				' date_start = date_start & " 08:00:00"
			Else
				date_start = currdated & " 08:00:00"
			End If
			If time_now >= "12:00:00 AM" And time_now <= "08:00:00 AM" Then
				Dim format_tommorow_re = "yyyy/MM/dd"
				Dim del_date1 As DateTime = today.AddDays(-1)
				date_start = del_date1.ToString(format_tommorow_re)
				Dim sub_date_end1 = Trim(date_end.ToString.Substring(0, 10))
				date_start = date_start & " 08:00:00"
				date_end_covert = sub_date_end1 & " 07:59:59"
			End If
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT max(seq_no) as seq_no from production_actual where wi = '" & wi & "'"
			SQLCmd.CommandText = "SELECT max(tmp_last_sequence) as seq_no from tmp_planseq where tmp_created_date BETWEEN  '" & date_start & "' and '" & date_end_covert & "' and tmp_line_cd = '" & line_cd & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_SEQ_PLAN_current]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function Update_seqplan(wi, line_cd, date_start, date_end, Update_seq)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Dim tmp_id As String = ""
		'		If time >= "08:00:00" And time >= "08:00:00" Then
		'		date_start = currdated & " 08:00:00"
		'		Else
		'		date_start = currdated & " 20:00:00"
		'		End If
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim today As Date = Date.Today
		Dim time_tomorrow As DateTime = today.AddDays(1)
		Dim format_tommorow = "yyyy/MM/dd"
		Dim date_tommorow = time_tomorrow.ToString(format_tommorow)
		date_end_covert = date_tommorow & " 07:59:59"
		Try
			Dim time_now As DateTime
			time_now = DateTime.Now.ToString("hh:mm:ss tt")
			If time_now >= "08:00:00 AM" And time_now <= "07:59:59 PM" Then
				date_start = currdated & " 08:00:00"
				' date_start = date_start & " 08:00:00"
			Else
				date_start = currdated & " 08:00:00"
			End If
			If time_now >= "12:00:00 AM" And time_now <= "08:00:00 AM" Then
				Dim format_tommorow_re = "yyyy/MM/dd"
				Dim del_date1 As DateTime = today.AddDays(-1)
				date_start = del_date1.ToString(format_tommorow_re)
				Dim sub_date_end1 = Trim(date_end.ToString.Substring(0, 10))
				date_start = date_start & " 08:00:00"
				date_end_covert = sub_date_end1 & " 07:59:59"
			End If
		Catch ex As Exception

		End Try
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "SELECT tmp_id from tmp_planseq where tmp_created_date BETWEEN  '" & date_start & "' and '" & date_end_covert & "' and tmp_line_cd = '" & line_cd & "'"
			reader = SQLCmd.ExecuteReader()
			While reader.Read()
				tmp_id = reader("tmp_id").ToString()
			End While
			reader.Close()
			'SQLCmd.CommandText = "update tmp_planseq set tmp_last_sequence = '" & Update_seq & "' , tmp_updated_date = '" & date_end & "' where tmp_line_cd = '" & line_cd & "' and tmp_created_date BETWEEN  '" & date_start & "' and '" & date_end & "'"
			SQLCmd.CommandText = "update tmp_planseq set tmp_last_sequence = '" & Update_seq & "' , tmp_updated_date = '" & date_end & "' where tmp_id = '" & tmp_id & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_SEQ_PLAN_current]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function GET_QTY_SHIFT(LINE_CD, WI, SHIFT, DATE_NOW, date_end, time_st, time_end, lot_no)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_QTY_SHIFT_NO_WI_LOT_NEW]  @line_cd = '" & LINE_CD & "' , @WI = '" & WI & "' , @ship  = '" & SHIFT & "', @date_now  = '" & DATE_NOW & "' , @date_end  = '" & date_end & "', @time_st  = '" & time_st & "', @time_end  = '" & time_end & "' , @lot_no='" & lot_no & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_QTY_SHIFT_NO_WI]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function Insert_production_emp_detail_realtime(wi_plan, staff_cd, prd_seq_no)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "Insert into production_emp_detail_realtime (wi_plan , staff_cd , prd_seq_no , updated_date) values('" & wi_plan & "','" & staff_cd & "','" & prd_seq_no & "','" & currdated & "')"
			reader = SQLCmd.ExecuteReader()
			reader.Close()
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_production_emp_detail_realtime]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Sub Check_detail_actual_insert_act()
		updated_data_to_dbsvr()
		Dim api = New api()
        Dim result_update_count_pro = api.Load_data("http://192.168.161.207/API_NEW_FA/Check_detail_actual_insert_act/Get_detail_act?line_cd=" & GET_LINE_PRODUCTION())
    End Sub
	Public Shared Sub Check_detail_actual_insert_act_no_api()
		updated_data_to_dbsvr()
	End Sub
	Public Shared Function update_qty_seq(WI, SEQ, result_qty)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
recheck:
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[UPDATE_QTY_DESC_ACTUAL] @WI = '" & WI & "' , @SEQ = '" & SEQ & "' , @QTY_UPDATE='" & result_qty & "'"
			reader = SQLCmd.ExecuteReader()
			'Return reader
			reader.Close()
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function update_qty_seq]")
			SQLConn.Close()
			GoTo recheck
			'Application.Exit()
		End Try
	End Function
	Public Shared Function GET_NEXT_PROCESS()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_NEXT_PROCESS] @PD = '" & MainFrm.Label6.Text & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_NEXT_PROCESS]")
			SQLConn.Close()
			'Application.Exit()
		End Try
	End Function
	Public Shared Function update_qty_seq_sqlite(WI, SEQ, result_qty, tr_status)
re_up_date_data:
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
		Catch ex As Exception
			sqliteConn.Close()
			sqliteConn.Open()
		End Try
		Try
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "UPDATE close_lot_act set act_qty = '" & result_qty & "' ,  transfer_flg = '" & tr_status & "' where  wi = '" & WI & "' and seq_no = '" & SEQ & "'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			'Return LoadSQL
			'sqliteConn.Dispose()
			sqliteConn.Close()
			'sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function update_qty_seq_sqlite]" & ex.Message)
			sqliteConn.Dispose()
			'sqliteConn.Close()
			sqliteConn = Nothing
			GoTo re_up_date_data
		End Try
	End Function

	Public Shared Function GET_QTY_SEQ_ACTUAL_DESC(WI, ship, SEQ)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
            SQLCmd.CommandText = "EXEC [dbo].[GET_QTY_DESC_ACTUAl] @WI = '" & WI & "' , @ship = '" & ship & "' , @SEQ='" & SEQ & "'"
            reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GET_QTY_SEQ_ACTUAL_DESC]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function GET_QTY_SEQ_ACTUAL_DESC_SQLITE(WI, ship, SEQ)
re_update_data:
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
		Catch ex As Exception
			sqliteConn.Open()
			sqliteConn.Close()
		End Try
		Try
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "SELECT SUM(act_qty) as QTY_ACTUAL
 from close_lot_act 
where 
	wi = '" & WI & "' and 
	shift_prd = '" & ship & "' and 
  seq_no = '" & SEQ & "'
"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function GET_QTY_SEQ_ACTUAL_DESC_SQLITE]" & ex.Message)
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
			GoTo re_update_data
		End Try
	End Function
	Public Shared Function ConnectDB() ' ฟังชั่นก์ไว้สำหรับติดต่อฐานข้อมูลเมื่อต้องการใช้งาน
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try

			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			'MsgBox("Database connect successfully")
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function ConnectDB]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	Public Shared Function GetLine_mst()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "SELECT * FROM sys_line_mst WHERE enable='1'"

			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GetLine_mst]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function update_print_count(wi, seq_plan, seq_box)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Dim seq_box1 As Integer = CDbl(Val(seq_box))
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "SELECT * FROM tag_print_detail WHERE wi = '" & wi & "' and seq_no = '" & seq_plan & "' and box_no = '" & seq_box1 & "'"
			reader = SQLCmd.ExecuteReader()
			Dim print_count As Integer = 0
			Dim id As Integer = 0
			While reader.Read()
				id = reader("id").ToString()
				print_count = CDbl(Val(reader("print_count").ToString())) + 1
			End While
			reader.Close()
			Dim api = New api()
            Dim result_update_count_pro = api.Load_data("http://192.168.161.207/API_NEW_FA/UPDATE_DATA/UPDATE_PRINT?ID=" & id & "&PRINT_COUNT=" & print_count)
            Dim table_created As Integer = 2
			If flg_cat_layout_line = "1" Then
				table_created = 2
			ElseIf flg_cat_layout_line = "2" Then
				table_created = 3
			End If
			ins_log_print(MainFrm.Label4.Text, table_created, id)
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function GetLine_mst]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function get_information()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection()
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "SELECT * FROM sys_information WHERE enable = '1'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_information]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function inf_update(inf_text As String, staff_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "UPDATE sys_information SET inf_txt = '" & inf_text & "', created_date = '" & currdated & "', created_by = '" & staff_cd & "'  WHERE id = 1 "
			reader = SQLCmd.ExecuteReader()
			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function inf_update]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	'Public Shared Function Get_User_Line_detail(usernm As String, passwd As String)
	Public Shared Function Get_User_Line_detail()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"
			Dim line_cd As String = MainFrm.Label4.Text
			SQLCmd.CommandText = "SELECT * FROM sys_line_mst WHERE line_cd = '" & line_cd & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Get_User_Line_detail]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function Get_prd_plan_new(line_cd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String  
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			Dim api = New api()
            Dim result_api_checkper = api.Load_data("http://192.168.161.207/API_NEW_FA/Api_Get_plan_production?line_cd=" & GET_LINE_PRODUCTION())
            'SQLCmd.CommandText = "SELECT DISTINCT
            '                       cp.WI,
            '                       cp.PD,
            '                       cp.LINE_CD,
            '                       cp.ITEM_CD,
            '                       cp.ITEM_NAME,
            '                       cp.MODEL,
            '                       cp.PLAN_QTY AS PLAN_QTY,
            '                       ISNULL(pa.prd_qty, 0) AS prd_qty_sum,
            '                       cp.PLAN_DATE AS WORK_ODR_DLV_DATE,
            '                       sup.WORK_ODR_DLV_DATE AS ORIGINAL_PLAN,
            '                       sup.PS_UNIT_NUMERATOR,
            '                       sup.CT,
            '                       sup.WORK_ODR_DLV_DATE,
            '                       sup.LOCATION_PART,
            '                       sup.MODEL,
            '                       sup.PRODUCT_TYP,
            '                       ISNULL(ppap.seq_no, 0) AS seq_no,
            '                       cp.PLAN_ORDER,
            '                  (select SUM(abs(qty)) from production_defect_detail where flg_defact = '1' and wi_plan = sup.WI and line_cd = '" & line_cd & "') as QTY_NC,
            '                 (select SUM(abs(qty)) from production_defect_detail where flg_defact = '2' and wi_plan = sup.WI and line_cd = '" & line_cd & "') as QTY_NG
            '                 FROM
            '                     confirm_production_plan AS cp
            '                 LEFT JOIN (
            'Select Case Case
            '                           wi_plan,
            '                          SUM (qty) AS prd_qty
            '                      FROM
            '                          production_actual_detail
            '                      WHERE
            '                          line_cd = '" & line_cd & "'
            '                      AND updated_date > DATEADD(MONTH, - 1, GETDATE())
            '                      GROUP BY
            '                          wi_plan
            '                  ) AS pa ON cp.WI = pa.wi_plan
            '                 LEFT JOIN production_actual AS ppap ON cp.WI = ppap.WI
            '                 LEFT JOIN sup_work_plan_supply_dev AS sup ON cp.WI = sup.WI
            '                  LEFT JOIN production_defect_detail AS cd ON cd.wi_plan = sup.WI
            '                  WHERE
            '                      cp.LINE_CD = '" & line_cd & "'
            '                 AND SUP.LVL = '1'
            '                 AND (
            '                     sup.PRD_COMP_FLG IS NULL
            '                     OR sup.PRD_COMP_FLG = '0'
            '                 )
            '                 AND (
            '                     ppap.del_flg IS NULL
            '                     OR ppap.del_flg = '0'
            '                 )
            '                AND (
            '                    ppap.comp_flg IS NULL
            '                    OR ppap.comp_flg = '0'
            '                ) -- AND CONVERT ( DATE, cp.PLAN_DATE ) = CONVERT ( DATE, GETDATE( ) )
            '                AND CONVERT (DATE, cp.PLAN_DATE) BETWEEN (CONVERT(DATE, GETDATE() - 30))
            '                AND (CONVERT(DATE, GETDATE()))
            '	ORDER BY
            '	cp.PLAN_ORDER ASC"
            'reader = SQLCmd.ExecuteReader()
            '			'MsgBox(reader)
            Return result_api_checkper
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Get_prd_plan_new]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function Tag_seq_rec_sqlite(wi_plan As String, seq_no As Integer, qty As Integer, ref_key As String)
		Check_connect_sqlite()
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "INSERT INTO sc_inc_tag(wi,seq_no,qty,created_date,ref_key) VALUES ('" & wi_plan & "','" & seq_no & "','" & qty & "','" & currdated & "','" & ref_key & "')"

			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL



		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function Tag_seq_rec_sqlite]")
			sqliteConn.Close()
		End Try

	End Function

	Public Shared Function Insert_prd_detail(pd As String, line_cd As String, wi_plan As String, item_cd As String, item_name As String, staff_no As Integer, seq_no As Integer, qty As Integer, st_time As String, end_time As String, use_time As Double, number_qty As Integer)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		'MsgBox(st_time.ToString(currdated"dd'/'MM'/'yyyy H':'m':'ss"))

		Dim result_date_start As Date = st_time
		Dim st_time2 = result_date_start.ToString("yyyy/MM/dd H:m:s", CultureInfo.InvariantCulture)

		Dim result_date_end As Date = end_time
		Dim end_time2 = result_date_end.ToString("yyyy/MM/dd H:m:s", CultureInfo.InvariantCulture)

		'Dim st_time2 As String = st_time.ToString("yyyy/MM/dd H:m:s")
		'Dim end_time2 As String = end_time.ToString("yyyy/MM/dd H:m:s")

		'MsgBox("INSERT INTO production_actual_detail(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,st_time,end_time,use_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & st_time & "','" & end_time & "','" & use_time & "','" & currdated & "')")
		Try
recheck:
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"
			'SQLCmd.CommandText = "INSERT INTO production_actual_detail(pd,line_cd,st_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & st_time2 & "','" & currdated & "')"
			SQLCmd.CommandText = "INSERT INTO production_actual_detail(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,st_time,end_time,use_time,updated_date,number_qty) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & st_time2 & "','" & end_time2 & "','" & use_time & "','" & currdated & "','" & number_qty & "')"
			reader = SQLCmd.ExecuteReader()
			'MsgBox(reader)
			'Return reader
			reader.Close()
		Catch ex As Exception
			'MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_prd_detail]")
			SQLConn.Close()
			GoTo recheck
			'Application.Exit()
		End Try
	End Function

	Public Shared Function Insert_prd_detail_defact(pd As String, line_cd As String, wi_plan As String, item_cd As String, item_name As String, staff_no As Integer, seq_no As Integer, qty As Integer, st_time As DateTime, end_time As DateTime, use_time As Double, number_qty As Integer, tr_status As String, flg_defact As String, defact_id As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		'MsgBox(st_time.ToString("dd'/'MM'/'yyyy H':'m':'ss"))
		Dim st_time2 As String = st_time.ToString("yyyy/MM/dd H:m:s")
		Dim end_time2 As String = end_time.ToString("yyyy/MM/dd H:m:s")
		'MsgBox("INSERT INTO production_actual_detail(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,st_time,end_time,use_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & st_time & "','" & end_time & "','" & use_time & "','" & currdated & "')")
		Try
			Check_connect_sqlite()
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"
			'SQLCmd.CommandText = "INSERT INTO production_actual_detail(pd,line_cd,st_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & st_time2 & "','" & currdated & "')"
			SQLCmd.CommandText = "INSERT INTO production_defect_detail(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,st_time,end_time,use_time,updated_date,number_qty , flg_defact , defact_id ) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & st_time2 & "','" & end_time2 & "','" & use_time & "','" & currdated & "','" & number_qty & "' , '" & flg_defact & "' , '" & defact_id & "')"
			reader = SQLCmd.ExecuteReader()
			'MsgBox(reader)
			'Return reader
			reader.Close()
			Check_connect_sqlite()
		Catch ex As Exception
			'MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_prd_detail_defact]")
			SQLConn.Close()
			Check_connect_sqlite()
			'Application.Exit()
		End Try
	End Function
	Public Shared Function line_status_ins(line_id As String, st_time As DateTime, end_time As DateTime, st_type As String, comp_flg As String, loss_id As String, efficiancy As String, wi_plan As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Dim st_time2 As String = st_time.ToString("yyyy/MM/dd H:m:s")
		Dim end_time2 As String = end_time.ToString("yyyy/MM/dd H:m:s")
		'MsgBox("INSERT INTO production_actual_detail(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,st_time,end_time,use_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & st_time & "','" & end_time & "','" & use_time & "','" & currdated & "')")
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"
			'SQLCmd.CommandText = "INSERT INTO production_actual_detail(pd,line_cd,st_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & st_time2 & "','" & currdated & "')"
			SQLCmd.CommandText = "INSERT INTO line_status_detail(line_id,st_time,end_time,st_type,comp_flg,loss_id,updated_date,efficientcy,wi_plan) VALUES ('" & line_id & "','" & st_time2 & "','" & end_time2 & "','" & st_type & "','" & comp_flg & "','" & loss_id & "','" & currdated & "','" & efficiancy & "','" & wi_plan & "')"
			reader = SQLCmd.ExecuteReader()
			'MsgBox(reader)
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function line_status_ins]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function line_status_ins_sqlite(line_id As String, st_time As DateTime, end_time As DateTime, st_type As String, comp_flg As String, loss_id As String, efficiancy As String, wi_plan As String)
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "INSERT INTO line_status_detail(line_id,st_time,end_time,st_type,comp_flg,loss_id,updated_date,efficientcy,wi_plan) VALUES ('" & line_id & "','" & st_time2 & "','" & end_time2 & "','" & st_type & "','" & comp_flg & "','" & loss_id & "','" & currdated & "','" & efficiancy & "','" & wi_plan & "')"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function line_status_ins_sqlite]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function line_status_upd(line_id As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "UPDATE line_status_detail SET end_time = '" & currdated & "' ,comp_flg = '1', updated_date = '" & currdated & "'  WHERE id = (SELECT MAX(id) AS id FROM line_status_detail WHERE line_id = '" & line_id & "' AND comp_flg = '0') "
			reader = SQLCmd.ExecuteReader()
			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			'MsgBox("MSSQL Database connect failed. Please contact PC System [Function line_status_upd]")
			SQLConn.Close()

			'Application.Exit()
		End Try
	End Function

	Public Shared Function line_status_upd_sqlite(line_id As String)
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			Try
				sqliteConn.Open()
			Catch ex As Exception
				sqliteConn.Close()
				sqliteConn.Open()
			End Try
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "UPDATE line_status_detail SET end_time = '" & currdated & "' ,comp_flg = '1', updated_date = '" & currdated & "'  WHERE id = (SELECT MAX(id) AS id FROM line_status_detail WHERE line_id = '" & line_id & "' AND comp_flg = '0') "
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function ConnectDBSQLite]")
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function Get_Last_part(line_cd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"

			'Dim line_cd2 As String = "K1M057"

			SQLCmd.CommandText = "SELECT
	                                    *
                                    FROM
	                                    production_actual
                                    WHERE
	                                    id = (
		                                    SELECT
			                                    MAX (id) AS id
		                                    FROM
			                                    production_actual
		                                    WHERE
			                                    line_cd = '" & line_cd & "'
		                                    AND del_flg = '0'
	                                    )"

			reader = SQLCmd.ExecuteReader()

			'MsgBox(reader)

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Get_Last_part]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function Get_Line_id(line_cd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"

			'Dim line_cd As String = "K1A027"

			SQLCmd.CommandText = "SELECT * FROM sys_line_mst WHERE line_cd = '" & line_cd & "'"

			reader = SQLCmd.ExecuteReader()

			'MsgBox(reader)

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Get_Line_id]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function





	Public Shared Function Get_Line_skill_id(line_id As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"

			'Dim line_cd As String = "K1A027"

			SQLCmd.CommandText = "SELECT * FROM sys_skill_line_detail WHERE line_id = '" & line_id & "' AND enable = 1 "

			reader = SQLCmd.ExecuteReader()

			'MsgBox(reader)

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Get_Line_skill_id]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function chk_user_skill_line(emp_cd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"
			'Dim line_cd As String = "K1A027"
			SQLCmd.CommandText = "Select su.*, sk.sk_id From sys_user As su Left Join sys_user_skill_detail AS sk On su.su_id=sk.su_id WHERE su.emp_id = '" & emp_cd & "' And sk.enable = '1' AND su.enable = '1'"
			reader = SQLCmd.ExecuteReader()
			'MsgBox(reader)
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function chk_user_skill_line]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_all_skill()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "Select * From sys_skill_chart_mst WHERE enable = '1' ORDER BY sk_id ASC"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_all_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_department()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "Select * From sys_department WHERE enable = '1'"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_department]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
    Public Shared Function update_tagprint(wi As String)
        Dim reader As SqlDataReader
        Dim SQLConn As New SqlConnection() 'The SQL Connection
        Dim SQLCmd As New SqlCommand()

        Try
            SQLConn.ConnectionString = sqlConnect 'Set the Connection String
            SQLConn.Open()
            SQLCmd.Connection = SQLConn
            SQLCmd.CommandText = "update tag_print_detail set flg_control = '2' where flg_control = '0' and  wi = '" & wi & "'"
            reader = SQLCmd.ExecuteReader()
            reader.Close()
            'Return reader
        Catch ex As Exception
            SQLConn.Close()
        End Try
    End Function
    Public Shared Function update_tagprint_sub(wi As String)
        Dim reader As SqlDataReader
        Dim SQLConn As New SqlConnection() 'The SQL Connection
        Dim SQLCmd As New SqlCommand()

        Try
            SQLConn.ConnectionString = sqlConnect 'Set the Connection String
            SQLConn.Open()
            SQLCmd.Connection = SQLConn
            SQLCmd.CommandText = "update tag_print_detail_sub set flg_control = '2' where flg_control = '0' and  wi = '" & wi & "'"
            reader = SQLCmd.ExecuteReader()
            reader.Close()
            'Return reader
        Catch ex As Exception
            SQLConn.Close()
        End Try
    End Function
    Public Shared Function update_tagprint_main(wi As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "update tag_print_detail_main set flg_control = '2' where flg_control = '0' and  tag_wi_no = '" & wi & "'"
			reader = SQLCmd.ExecuteReader()
			reader.Close()
			'Return reader
		Catch ex As Exception
			SQLConn.Close()
		End Try
	End Function
	Public Shared Function Insert_tag_print(wi As String, qr_detail As String, box_no As Integer, print_count As Integer, seq_no As String, shift As String, flg_control As Integer, item_cd As String)
        Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
        update_tagprint(wi)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "INSERT INTO tag_print_detail(wi,qr_detail,box_no,print_count,created_date,updated_date,seq_no,shift , next_proc ,  flg_control) VALUES ('" & wi & "','" & qr_detail & "','" & box_no & "','" & print_count & "','" & currdated & "','" & currdated & "','" & seq_no & "','" & shift & "','" & F_NEXT_PROCESS(item_cd) & "' ,'" & flg_control & "')"
			reader = SQLCmd.ExecuteReader()
			'Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_tag_print]")
			SQLConn.Close()
			'Application.Exit()
		End Try
	End Function

	Public Shared Sub ins_log_print(created_by As String, table_created As String, log_ref_tag_id As String)
		Dim api = New api()
		Dim result = api.Load_data("http://192.168.161.207/API_NEW_FA/Api_insert_log_reprint/ins_los_reprint?created_by=" & created_by & "&table_created=" & table_created & "&log_ref_tag_id=" & log_ref_tag_id)
	End Sub

	Public Shared Function Insert_tag_print_main(wi As String, qr_detail As String, batch_no As Integer, print_count As Integer, seq_no As String, shift As String, flg_control As Integer, item_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		update_tagprint(wi)
		update_tagprint_main(wi)
		Dim start_id As String = Get_ref_start_id(wi, seq_no, Working_Pro.Label18.Text)
		Dim end_id As String = Get_ref_end_id(wi, seq_no, Working_Pro.Label18.Text)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "INSERT INTO tag_print_detail_main(tag_ref_str_id ,tag_ref_end_id , line_cd , tag_qr_detail , tag_batch_no , tag_next_proc , flg_control , created_date , updated_date , tag_wi_no) VALUES ('" & start_id & "','" & end_id & "','" & MainFrm.Label4.Text & "','" & qr_detail & "' ,'" & batch_no & "' ,'" & F_NEXT_PROCESS(item_cd) & "','" & flg_control & "','" & currdated & "','" & currdated & "','" & wi & "')"
			reader = SQLCmd.ExecuteReader()
			'Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_tag_print_main]")
			SQLConn.Close()
			'Application.Exit()
		End Try
	End Function
	Public Shared Function Get_ref_start_id(wi As String, seq_no As String, lot_no As String)
		Dim api = New api()
		Dim result = api.Load_data("http://192.168.161.207/API_NEW_FA/GET_DATA_NEW_FA/Get_ref_start_id?wi=" & wi & "&seq_no=" & seq_no & "&lot_no=" & lot_no)
		Return result
	End Function
	Public Shared Function Get_ref_end_id(wi As String, seq_no As String, lot_no As String)
		Dim api = New api()
		Dim result = api.Load_data("http://192.168.161.207/API_NEW_FA/GET_DATA_NEW_FA/Get_ref_end_id?wi=" & wi & "&seq_no=" & seq_no & "&lot_no=" & lot_no)
		Return result
	End Function
	Public Shared Function get_qr_detail_sub(ref_id)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[GET_DATA_SUB] @REF_ID = '" & ref_id & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_qr_detail_sub]")
			SQLConn.Close()
			'Application.Exit()
		End Try
	End Function
	Public Shared Function Insert_tag_print_sub(ref_id As String, line As String, qr_code As String, wi As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
        Try
            update_tagprint_sub(wi)
            SQLConn.ConnectionString = sqlConnect 'Set the Connection String
            SQLConn.Open()
            SQLCmd.Connection = SQLConn
            SQLCmd.CommandText = "INSERT INTO tag_print_detail_sub(tag_ref_id , line_cd , tag_qr_detail , flg_control , created_date , updated_date , tag_wi_no) VALUES ('" & ref_id & "','" & line & "','" & qr_code & "' ,'" & print_back.check_tagprint_main() & "' , '" & currdated & "' , '" & currdated & "' , '" & wi & "')"
            reader = SQLCmd.ExecuteReader()
            'Return reader
        Catch ex As Exception
            MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_tag_print_sub]")
			SQLConn.Close()
			'Application.Exit()
		End Try
	End Function

	Public Shared Function Insert_user(emp_cd As String, fname As String, lname As String, dep_id As Integer, created_by As String, group_id As Integer)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "INSERT INTO sys_user(emp_id,passwd,fname,lname,department_id,enable,created_date,created_by,updated_date,updated_by,sug_id) VALUES ('" & emp_cd & "','Sysadmin!','" & fname & "','" & lname & "','" & dep_id & "','1','" & currdated & "','" & created_by & "','" & currdated & "','" & created_by & "','" & group_id & "')"
			reader = SQLCmd.ExecuteReader()
			'Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_user]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function Insert_skill(sk_des As String, emp_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "INSERT INTO sys_skill_chart_mst(sk_name,sk_description,enable,created_date,created_by,updated_date,updated_by) VALUES ('" & sk_des & "','" & sk_des & "','1','" & currdated & "','" & emp_cd & "','" & currdated & "','" & emp_cd & "')"
			reader = SQLCmd.ExecuteReader()

			'Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function del_skill(sk_id As String, emp_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sys_skill_chart_mst SET enable = '0', updated_date = '" & currdated & "', updated_by = '" & emp_cd & "'  WHERE sk_id = '" & sk_id & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function del_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	Public Shared Function work_complete(wi As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sup_work_plan_supply_dev SET PRD_COMP_FLG = '9', PRD_COMP_DATE = '" & currdated & "' WHERE WI = '" & wi & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function edit_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function edit_skill(sk_id As Integer, sk_des As String, emp_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sys_skill_chart_mst SET sk_name = '" & sk_des & "',sk_description = '" & sk_des & "',  updated_date = '" & currdated & "', updated_by = '" & emp_cd & "'  WHERE sk_id = '" & sk_id & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function edit_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_user_last_id()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "SELECT IDENT_CURRENT('sys_user') AS last_id"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_user_last_id]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function Insert_user_skill(su_id As Integer, sk_id As Integer, created_by As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "INSERT INTO sys_user_skill_detail(su_id,sk_id,enable,created_date,created_by,updated_date,updated_by) VALUES ('" & su_id & "','" & sk_id & "','1','" & currdated & "','" & created_by & "','" & currdated & "','" & created_by & "')"
			reader = SQLCmd.ExecuteReader()

			'Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_user_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function Insert_line_skill(line_id As Integer, sk_id As Integer, process_no As String, created_by As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "INSERT INTO sys_skill_line_detail(line_id,sk_id,process_no,enable,created_date,created_by,updated_date,updated_by) VALUES ('" & line_id & "','" & sk_id & "','" & process_no & "','1','" & currdated & "','" & created_by & "','" & currdated & "','" & created_by & "')"
			reader = SQLCmd.ExecuteReader()

			'Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_line_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function get_all_line()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "Select * FROM sys_line_mst WHERE enable = '1' ORDER BY line_id ASC"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_all_line]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function del_line(line_id As String, emp_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sys_line_mst SET enable = '0', updated_date = '" & currdated & "', updated_by = '" & emp_cd & "'  WHERE line_id = '" & line_id & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function del_line]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function get_all_user()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "Select su.*, sd.sec_name From sys_user As su Left Join sys_department AS sd On su.department_id=sd.dep_id WHERE su.enable = '1' AND su.sug_id <> '1' ORDER BY su.emp_id ASC"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_all_user]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	Public Shared Function get_tag_reprint_spaceial(wi As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[REPRINT_SPACEIAL] @WI = '" & wi & "'"
			reader = SQLCmd.ExecuteReader()
			Dim result = reader
			'Dim result As Integer = 0
			'While reader.Read()
			'result = reader("c_id").ToString()
			'End While
			'reader.Close()
			Return result
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_tag_reprint_spaceial]")
			MsgBox(ex.Message)
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	Public Shared Function get_tag_reprint_sum_detail(wi As String , lot As string)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "EXEC [dbo].[REPRINT_NORAML_SPC] @WI = '" & wi & "' and @lot_no = '" & lot_no & "'"
			reader = SQLCmd.ExecuteReader()
			Dim result As Integer = 0
			While reader.Read()
				result = reader("c_id").ToString()
			End While
			reader.Close()
			Return result
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_tag_reprint_sum_detail]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	Public Shared Function check_line_reprint()
		Dim api = New api()
		Dim result_api_checkper = api.Load_data("http://192.168.161.207/API_NEW_FA/Api_check_data/check_line_reprint?line_cd=" & MainFrm.Label4.Text)
		Return result_api_checkper
	End Function
	Public Shared Function B_check_format_tag()
		Dim api = New api()
		Dim reusult_data = api.Load_data("http://192.168.161.207/API_NEW_FA/Api_check_data/check_format_tag?line_cd=" & MainFrm.Label4.Text)
		Return reusult_data
	End Function


	Public Shared Function get_tag_reprint_detail(wi As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			'Dim check_flg_reprint_line_mst = check_line_reprint()
			'If check_flg_reprint_line_mst = "0" Then
			SQLCmd.CommandText = "EXEC [dbo].[REPRINT_NORAML_BY_BOX] @WI = '" & wi & "'"
			reader = SQLCmd.ExecuteReader()
			'Return reader
			'Else
			'Dim result = get_tag_reprint_sum_detail(wi)
			Return reader
			'End If
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_tag_reprint_detail]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function get_tag_reprint_batch(wi As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			Try
				SQLConn.Open()
			Catch ex As Exception
				SQLConn.Close()
				SQLConn.Open()
			End Try
			SQLCmd.Connection = SQLConn
			'Dim check_flg_reprint_line_mst = check_line_reprint()
			'If check_flg_reprint_line_mst = "0" Then
			SQLCmd.CommandText = "EXEC [dbo].[REPRINT_BATCH] @WI = '" & wi & "'"
			reader = SQLCmd.ExecuteReader()
			Dim result = reader
			'Return reader
			'Else
			'Dim result = get_tag_reprint_sum_detail(wi)
			Return result
			'End If
		Catch ex As Exception
			MsgBox(ex.Message)
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_tag_reprint_batch]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function update_data_new_qr_detail(qr_code As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "select count(id) as c_id from tag_print_detail where qr_detail = '" & qr_code & "'"
			Dim LoadSQL As SqlDataReader = SQLCmd.ExecuteReader()
			Dim check_status As Integer = 0
			While LoadSQL.Read()
				check_status = LoadSQL("c_id").ToString()
			End While
			LoadSQL.Close()
			Dim wi2 As String = "NO_DATA"
			Dim qr_detailss2 As String = "NO_DATA"
			Dim box_no2 As String = "NO_DATA"
			Dim plan_seq2 As String = "NO_DATA"
			Dim shift2 As String = "NO_DATA"
			If check_status = 0 Then
				SQLCmd.CommandText = "select * from tag_print_detail_genarate where new_qr_detail = '" & qr_code & "'"
				Dim LoadSQL2 As SqlDataReader = SQLCmd.ExecuteReader()
				While LoadSQL2.Read()
					wi2 = LoadSQL2("wi").ToString()
					qr_detailss2 = LoadSQL2("new_qr_detail").ToString()
					box_no2 = LoadSQL2("box_no").ToString()
					plan_seq2 = qr_detailss2.Substring(95, 3)
					shift2 = LoadSQL2("shift").ToString()
				End While
				LoadSQL2.Close()
				Dim arr_item_cd = qr_detailss2.Substring(19).Split(" ")
				Dim item_cd As String = arr_item_cd(0)
				Insert_tag_print(wi2, qr_detailss2, box_no2, 1, plan_seq2, shift2, "", item_cd)
				SQLCmd.CommandText = "update tag_print_detail_genarate set flg_print = '0' where new_qr_detail = '" & qr_code & "'"
				reader = SQLCmd.ExecuteReader()
				reader.Close()
			Else
				Dim wi3 As String = "NO_DATA"
				Dim qr_detailss3 As String = "NO_DATA"
				Dim box_no3 As String = "NO_DATA"
				Dim seq_plan3 As String = "NO_DATA"
				Dim shift3 As String = "NO_DATA"
				Dim seq_box3 As String = "NO_DATA"
				SQLCmd.CommandText = "select * from tag_print_detail_genarate where new_qr_detail = '" & qr_code & "'"
				Dim LoadSQL3 As SqlDataReader = SQLCmd.ExecuteReader()
				While LoadSQL3.Read()
					wi3 = LoadSQL3("wi").ToString()
					seq_box3 = LoadSQL3("box_no").ToString()
					qr_detailss3 = LoadSQL3("new_qr_detail").ToString()
					seq_plan3 = qr_detailss3.Substring(95, 3)
				End While
				LoadSQL3.Close()
				If wi3 = "NO_DATA" Then
					SQLCmd.CommandText = "select * from tag_print_detail where qr_detail = '" & qr_code & "'"
					Dim LoadSQL1 As SqlDataReader = SQLCmd.ExecuteReader()
					Dim wi4 As String = "NODATA"
					Dim box_no4 As String = "NODATA"
					Dim qr_detail4 As String = "NODATA"
					Dim seq_plan4 As String = "NODATA"
					While LoadSQL1.Read()
						wi4 = LoadSQL1("wi").ToString()
						box_no4 = LoadSQL1("box_no").ToString()
						qr_detail4 = LoadSQL1("qr_detail").ToString()
						seq_plan4 = qr_detail4.Substring(95, 3)
					End While
					LoadSQL1.Close()
					update_print_count(wi4, seq_plan4, box_no4)
				Else
					update_print_count(wi3, seq_plan3, seq_box3)
				End If
			End If
		Catch
		End Try
	End Function

	Public Shared Function update_data_new_qr_detail_main(qr_code As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "select * from tag_print_detail_main where tag_qr_detail = '" & qr_code & "'"
			Dim LoadSQL1 As SqlDataReader = SQLCmd.ExecuteReader()
			Dim id As String = ""
			While LoadSQL1.Read()
				id = LoadSQL1("tag_id").ToString()
			End While
			LoadSQL1.Close()
			ins_log_print(MainFrm.Label4.Text, "3", id)
		Catch
			MsgBox("error function update_data_new_qr_detail_main == ")
		End Try
	End Function



	Public Shared Function get_tag_reprint_detail_genarate(wi As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try

			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "select * from tag_print_detail_genarate where wi = '" & wi & "'  and flg_print = '1'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_tag_reprint_detail_genarate]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function get_sec_user(sec_name As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "Select su.*, sd.sec_name From sys_user As su Left Join sys_department AS sd On su.department_id=sd.dep_id WHERE su.enable = '1' AND su.sug_id <> '1' AND sd.sec_name = '" & sec_name & "' ORDER BY su.emp_id ASC"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_sec_user]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function Get_data_picking(ref_id As String)
		Dim api = New api()
		Dim result_api_checkper = api.Load_data("http://192.168.161.207/API_NEW_FA/GET_DATA_NEW_FA/Get_data_picking?ref_id=" & ref_id)
		Return result_api_checkper
	End Function
	Public Shared Function del_user(su_id As String, emp_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sys_user SET enable = '0', updated_date = '" & currdated & "', updated_by = '" & emp_cd & "'  WHERE emp_id = '" & su_id & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function del_user]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function edit_user(emp_id As String, fname As String, lname As String, dept_id As Integer, emp_cd As String, sug_id As Integer, su_id As Integer)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sys_user SET emp_id = '" & emp_id & "',fname = '" & fname & "',lname = '" & lname & "',department_id = '" & dept_id & "',  updated_date = '" & currdated & "', updated_by = '" & emp_cd & "' , sug_id = '" & sug_id & "' WHERE su_id = '" & su_id & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function edit_user]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function del_user_skill_old(su_id As String, emp_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sys_user_skill_detail SET enable = '0', updated_date = '" & currdated & "', updated_by = '" & emp_cd & "'  WHERE su_id = '" & su_id & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function del_user_skill_old]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function del_line_skill_old(line_id As String, emp_cd As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "UPDATE sys_skill_line_detail SET enable = '0', updated_date = '" & currdated & "', updated_by = '" & emp_cd & "'  WHERE line_id = '" & line_id & "' "
			reader = SQLCmd.ExecuteReader()

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function del_line_skill_old]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function get_user_detail(emp_id As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "Select * From sys_user WHERE emp_id = '" & emp_id & "'"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_user_detail]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_line_skill(line_cd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "SELECT skl.* FROM sys_skill_line_detail AS skl LEFT JOIN sys_line_mst AS sl ON skl.line_id = sl.line_id WHERE sl.line_cd = '" & line_cd & "' AND skl.enable = '1' AND sl.enable = '1' ORDER BY skl.process_no ASC"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_line_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function get_user_skill(emp_id As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "Select sk_id From sys_user_skill_detail WHERE su_id = '" & emp_id & "' AND enable = '1' ORDER BY sk_id ASC"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_user_skill]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function chk_adm_login(emp_cd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"

			'Dim line_cd As String = "K1A027"

			SQLCmd.CommandText = "Select * From sys_user WHERE emp_id = '" & emp_cd & "' And enable = '1'"

			reader = SQLCmd.ExecuteReader()

			'MsgBox(reader)

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function chk_adm_login]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function get_prd_plan_reprint(line_cd As String)
		Dim reader As SqlDataReader
		Dim reader2 As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			'line_cd = "K1A027"
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "Select sw.WI,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,sw.qty - SUM (ISNULL(pa.act_qty, 0 )) as 'remain_qty',ISNULL(pa.prd_flg , 0 ) as 'prd_flg',sw.WORK_ODR_DLV_DATE AS 'DLV_DATE', sw.LOCATION_PART,sw.PS_UNIT_NUMERATOR,sw.CT,COUNT(pa.seq_no) AS seq_count,sw.MODEL , sw.PRODUCT_TYP  from sup_work_plan_supply_dev as sw full outer JOIN production_actual as pa on sw.WI = pa.wi WHERE sw.LINE_CD = '" & line_cd & "' and sw.LVL = '1' and (pa.comp_flg <> '1' or pa.comp_flg is NULL) GROUP BY sw.wi,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,pa.prd_flg,sw.WORK_ODR_DLV_DATE, sw.LOCATION_PART,sw.PS_UNIT_NUMERATOR,sw.CT,sw.MODEL,sw.PRODUCT_TYP"
			'SQLCmd.CommandText = "select * from sup_work_plan_supply_dev where LINE_CD = '" & line_cd & "' AND LVL = '1'"
			reader = SQLCmd.ExecuteReader()



			'SQLCmd.CommandText = "select * from production_actual where wi = '5100131123'"
			'reader = SQLCmd.ExecuteReader()


			'While reader.Read()
			'SQLCmd.CommandText = "select * from production_actual where wi = '" & reader("wi").ToString() & "'"
			'reader2 = SQLCmd.ExecuteReader()

			'If reader2.Read = False Then
			'End If

			'List_Emp.ListBox1.Items.Add(LoadSQLskill("sk_id").ToString())
			'End While

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_prd_plan]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_prd_plan(line_cd As String)
		Dim reader As SqlDataReader
		Dim reader2 As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			'line_cd = "K1A027"
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "Select sw.WI,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,sw.qty - SUM (ISNULL(pa.act_qty, 0 )) as 'remain_qty',ISNULL(pa.prd_flg , 0 ) as 'prd_flg',sw.WORK_ODR_DLV_DATE AS 'DLV_DATE', sw.LOCATION_PART,sw.PS_UNIT_NUMERATOR,sw.CT,COUNT(pa.seq_no) AS seq_count,sw.MODEL , sw.PRODUCT_TYP  from sup_work_plan_supply_dev as sw full outer JOIN production_actual as pa on sw.WI = pa.wi WHERE sw.LINE_CD = '" & line_cd & "' and sw.LVL = '1'  and (sw.PRD_COMP_FLG IS NULL OR sw.PRD_COMP_FLG <> '9') and (pa.comp_flg <> '1' or pa.comp_flg is NULL) GROUP BY sw.wi,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,pa.prd_flg,sw.WORK_ODR_DLV_DATE, sw.LOCATION_PART,sw.PS_UNIT_NUMERATOR,sw.CT,sw.MODEL,sw.PRODUCT_TYP"
			'SQLCmd.CommandText = "select * from sup_work_plan_supply_dev where LINE_CD = '" & line_cd & "' AND LVL = '1'"
			reader = SQLCmd.ExecuteReader()


			'SQLCmd.CommandText = "select * from production_actual where wi = '5100131123'"
			'reader = SQLCmd.ExecuteReader()

			'While reader.Read()
			'SQLCmd.CommandText = "select * from production_actual where wi = '" & reader("wi").ToString() & "'"
			'reader2 = SQLCmd.ExecuteReader()

			'If reader2.Read = False Then
			'End If

			'List_Emp.ListBox1.Items.Add(LoadSQLskill("sk_id").ToString())
			'End While

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_prd_plan]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_sum_loss(wi As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "SELECT wi,seq_no,SUM(loss_time) AS sum_loss FROM loss_actual WHERE wi = '" & wi & "' GROUP BY wi,seq_no"
			reader = SQLCmd.ExecuteReader()
			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_sum_loss]" & ex.Message)
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function

	Public Shared Function get_wi_plan_fromsc(wi_cd As String)
		Dim reader As SqlDataReader
		Dim reader2 As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			'line_cd = "K1A027"
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "SELECT
	                                sw.WI,
	                                sw.ITEM_CD,
	                                sw.ITEM_NAME,
	                                sw.LINE_CD,
	                                sw.QTY,
	                                sw.qty - SUM (ISNULL(pa.qty, 0)) AS 'remain_qty',
	                                sw.WORK_ODR_DLV_DATE AS 'DLV_DATE',
	                                sw.LOCATION_PART,
	                                sw.PS_UNIT_NUMERATOR,
	                                sw.CT,
	                                COUNT (pa.seq_no) AS seq_count,
	                                sw.MODEL,
	                                sw.PRODUCT_TYP,
									sw.PRD_COMP_FLG
                                FROM
	                                sup_work_plan_supply_dev AS sw
                                FULL OUTER JOIN production_actual_detail AS pa ON sw.WI = pa.wi_plan
                                WHERE
	                                sw.LVL = '1'
                                AND sw.wi = '" & wi_cd & "'
                                GROUP BY
	                                sw.wi,
	                                sw.ITEM_CD,
	                                sw.ITEM_NAME,
	                                sw.LINE_CD,
	                                sw.QTY,
	                                sw.WORK_ODR_DLV_DATE,
	                                sw.LOCATION_PART,
	                                sw.PS_UNIT_NUMERATOR,
	                                sw.CT,
	                                sw.MODEL,
	                                sw.PRODUCT_TYP,
									sw.PRD_COMP_FLG"
			'SQLCmd.CommandText = "select * from sup_work_plan_supply_dev where LINE_CD = '" & line_cd & "' AND LVL = '1'"
			reader = SQLCmd.ExecuteReader()


			Return reader

			SQLConn.Close()

		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_prd_plan]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_prd_plan_fromsc(line_cd As String, wi_cd As String)
		Dim reader As SqlDataReader
		Dim reader2 As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			'line_cd = "K1A027"
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "Select sw.WI,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,sw.qty - SUM (ISNULL(pa.act_qty, 0 )) as 'remain_qty',ISNULL(pa.prd_flg , 0 ) as 'prd_flg',sw.WORK_ODR_DLV_DATE AS 'DLV_DATE', sw.LOCATION_PART,sw.PS_UNIT_NUMERATOR,sw.CT,COUNT(pa.seq_no) AS seq_count,sw.MODEL , sw.PRODUCT_TYP  from sup_work_plan_supply_dev as sw full outer JOIN production_actual as pa on sw.WI = pa.wi WHERE sw.LINE_CD = '" & line_cd & "' and sw.LVL = '1' and (pa.comp_flg <> '1' or pa.comp_flg is NULL) AND sw.wi = '" & wi_cd & "' GROUP BY sw.wi,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,pa.prd_flg,sw.WORK_ODR_DLV_DATE, sw.LOCATION_PART,sw.PS_UNIT_NUMERATOR,sw.CT,sw.MODEL,sw.PRODUCT_TYP"
			'SQLCmd.CommandText = "select * from sup_work_plan_supply_dev where LINE_CD = '" & line_cd & "' AND LVL = '1'"
			reader = SQLCmd.ExecuteReader()

			'MsgBox("tet efsdf")
			'MsgBox(reader.Read)

			'SQLCmd.CommandText = "select * from production_actual where wi = '5100131123'"
			'reader = SQLCmd.ExecuteReader()
			'MsgBox(reader.Read)
			'MsgBox(reader("wi").ToString())

			'While reader.Read()
			'SQLCmd.CommandText = "select * from production_actual where wi = '" & reader("wi").ToString() & "'"
			'reader2 = SQLCmd.ExecuteReader()
			'MsgBox(reader2.Read)

			'If reader2.Read = False Then
			'MsgBox("dai naa")
			'End If

			'MsgBox(reader("wi").ToString())
			'List_Emp.ListBox1.Items.Add(LoadSQLskill("sk_id").ToString())
			'End While

			'MsgBox(reader)
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_prd_plan]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function get_loss_mst()
		Dim reader As SqlDataReader
		Dim reader2 As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "Select sw.WI,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,sw.qty - SUM (ISNULL(pa.act_qty, 0 )) as 'remain_qty',ISNULL(pa.prd_flg , 0 ) as 'prd_flg'  from sup_work_plan_supply_dev as sw full outer JOIN production_actual as pa on sw.WI = pa.wi WHERE sw.LINE_CD = '" & line_cd & "' and sw.LVL = '1' and (pa.comp_flg <> '1' or pa.comp_flg is NULL) GROUP BY sw.wi,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,pa.prd_flg"
			'SQLCmd.CommandText = "select * from sys_loss_mst where enable = '1'"
			' SQLCmd.CommandText = "select * from sys_loss_mst where enable = '1'"
			SQLCmd.CommandText = "EXEC [dbo].[GET_LOSS_GROUP] @PD = '" & MainFrm.Label6.Text & "'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_loss_mst]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function

	Public Shared Function get_defect_mst()
		Dim reader As SqlDataReader
		Dim reader2 As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
            'SQLCmd.CommandText = "Select sw.WI,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,sw.qty - SUM (ISNULL(pa.act_qty, 0 )) as 'remain_qty',ISNULL(pa.prd_flg , 0 ) as 'prd_flg'  from sup_work_plan_supply_dev as sw full outer JOIN production_actual as pa on sw.WI = pa.wi WHERE sw.LINE_CD = '" & line_cd & "' and sw.LVL = '1' and (pa.comp_flg <> '1' or pa.comp_flg is NULL) GROUP BY sw.wi,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,pa.prd_flg"
            SQLCmd.CommandText = "select * from sys_defect_mst where enable = '1'"
			reader = SQLCmd.ExecuteReader()
			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_defect_mst]")
            SQLConn.Close()
            Application.Exit()
		End Try
	End Function

	Public Function Get_default_line_detail()
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from line_detail"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Return LoadSQL
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function ConnectDBSQLite]")
			sqliteConn.Close()
		End Try
	End Function
	Public Function Get_default_pd_detail()
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from line_detail"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Return LoadSQL
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function ConnectDBSQLite]")
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function ConnectDBSQLite()
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from line_detail"

			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function ConnectDBSQLite]")
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function Check_sc_inc_dup(ref_key As String)
		Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			ref_key = RTrim(ref_key)
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from sc_inc_tag where ref_key = '" & ref_key & "' "
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function Check_sc_inc_dup]")
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function saveLineConfig(pd As String, line_cd As String, count_type As String, cavity As Integer, scanner_port As String, printer_port As String, dio_port As String)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
        Dim sqliteConn As New SQLiteConnection(sqliteConnect)
        Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "UPDATE line_detail SET pd = '" & pd & "', line_cd = '" & line_cd & "', updated_date = '" & currdated & "', count_type = '" & count_type & "', cavity = '" & cavity & "', scanner_port = '" & scanner_port & "', printer_port = '" & printer_port & "', dio_port = '" & dio_port & "'  WHERE id = 1 "

			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL


        Catch ex As Exception
            MsgBox("SQLite Database connect failed. Please contact PC System [Function saveLineConfig] = " & ex.Message)
            sqliteConn.Close()
		End Try



	End Function

	Public Shared Function insPrdDetail_sqlite(pd As String, line_cd As String, wi_plan As String, item_cd As String, item_name As String, staff_no As Integer, seq_no As Integer, qty As Integer, number_qty As Integer, st_time As String, end_time As String, use_time As Double, tr_status As String)
re_insert_data:
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		'st_time = Date.st_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		'MsgBox(st_time)
		Try
			sqliteConn.Open()
		Catch ex As Exception
			sqliteConn.Close()
			sqliteConn.Open()
		End Try
		Try
			Dim cmd1 As New SQLiteCommand
			cmd1.Connection = sqliteConn
			'cmd.CommandText = "UPDATE line_detail SET pd = '" & pd & "', line_cd = '" & line_cd & "', updated_date = '" & currdated & "' WHERE id = 1 "
			'cmd.CommandText = "INSERT INTO act_ins (pd,line_cd,wi_plan,item_cd,item_name,tr_status) VALUES ('pd123','line123','wi123','item123','nm123','1');"
			cmd1.CommandText = "INSERT INTO act_ins(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,number_qty,st_time,end_time,use_time,tr_status,updated_date ) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & number_qty & "','" & st_time & "','" & end_time & "','" & use_time & "','" & tr_status & "','" & currdated & "')"
			'cmd1.CommandText = "select * from act_ins"
			'MsgBox(cmd1.CommandText)
			Dim LoadSQL As SQLiteDataReader = cmd1.ExecuteReader()
			'MsgBox(LoadSQL)
			'Return LoadSQL
			'sqliteConn.Dispose()
			sqliteConn.Close()
			' sqliteConn = Nothing
		Catch ex As Exception
			'MsgBox("SQLite Insert Reccord failed. Please contact PC System [Function insPrdDetail_sqlite]" & ex.Message)
			sqliteConn.Close()
			GoTo re_insert_data
		End Try
		Return 0
	End Function

	Public Shared Function insPrdDetail_sqlite_defact(pd As String, line_cd As String, wi_plan As String, item_cd As String, item_name As String, staff_no As Integer, seq_no As Integer, qty As Integer, number_qty As Integer, st_time As String, end_time As String, use_time As Double, tr_status As String, flg_defact As String, NC As String)
re_insert_data:
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		'st_time = Date.st_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		'MsgBox(st_time)
		Try
			sqliteConn.Open()
		Catch ex As Exception
			sqliteConn.Close()
			sqliteConn.Open()
		End Try
		Try
			Dim cmd1 As New SQLiteCommand
			cmd1.Connection = sqliteConn
			'cmd.CommandText = "UPDATE line_detail SET pd = '" & pd & "', line_cd = '" & line_cd & "', updated_date = '" & currdated & "' WHERE id = 1 "
			'cmd.CommandText = "INSERT INTO act_ins (pd,line_cd,wi_plan,item_cd,item_name,tr_status) VALUES ('pd123','line123','wi123','item123','nm123','1');"
			cmd1.CommandText = "INSERT INTO production_defect_detail(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,number_qty,st_time,end_time,use_time,tr_status,updated_date , flg_defact , defact_id) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & number_qty & "','" & st_time & "','" & end_time & "','" & use_time & "','" & tr_status & "','" & currdated & "' , '" & flg_defact & "', '" & NC & "')"
			'cmd1.CommandText = "select * from act_ins"
			Dim LoadSQL As SQLiteDataReader = cmd1.ExecuteReader()
			'MsgBox(LoadSQL)
			'Return LoadSQL
			'sqliteConn.Dispose()
			sqliteConn.Close()
			' sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Insert Reccord failed. Please contact PC System [Function insPrdDetail_sqlite_defact]" & ex.Message)
			sqliteConn.Close()
			'GoTo re_insert_data
		End Try
		Return 0
	End Function

	Public Shared Function chkLogin(usernm As String, passwd As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
            SQLConn.ConnectionString = sqlConnect 'Set the Connection String
            SQLConn.Open()
            SQLCmd.Connection = SQLConn
            SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"
            reader = SQLCmd.ExecuteReader()
            Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function chkLogin]")
			SQLConn.Close()

			Application.Exit()
		End Try

	End Function
	Public Shared Function check_production_actual_detail_server(wi_plan As String, number_qty As String, seq_no As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "SELECT count(id) as c_id from  production_actual_detail where wi_plan = '" & wi_plan & "' and  number_qty = '" & number_qty & "' and seq_no = '" & seq_no & "' "
			reader = SQLCmd.ExecuteReader()
			Dim data As String = "0"
			While reader.Read()
				If reader("c_id").ToString() = "0" Then
					data = "0"
				Else
					data = "1"
				End If
			End While
			reader.Close()
			Return data
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function chkLogin]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	Public Shared Function updated_data_to_dbsvr()
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim LoadSQL = Backoffice_model.get_trdata_sqlite()
		Dim num_arr As Integer = 0
		Dim arr_list_id As ArrayList = New ArrayList()
		Dim tmp_wi As String = ""
		Check_connect_sqlite()
		'If LoadSQL.read Then
		While LoadSQL.Read()
				Dim id As String = LoadSQL("id").ToString()
				Dim pd As String = LoadSQL("pd").ToString()
				Dim line_cd As String = LoadSQL("line_cd").ToString()
				Dim wi_plan As String = LoadSQL("wi_plan").ToString()
				tmp_wi = wi_plan
				Dim item_cd As String = LoadSQL("item_cd").ToString()
				Dim item_name As String = LoadSQL("item_name").ToString()
				Dim staff_no As Integer = LoadSQL("staff_no").ToString()
				Dim seq_no As Integer = LoadSQL("seq_no").ToString()
				Dim qty As Integer = LoadSQL("qty").ToString()
				Dim number_qty As Integer = LoadSQL("number_qty").ToString()
				Dim st_time As Date = LoadSQL("st_time").ToString()
				Dim end_time As Date = LoadSQL("end_time").ToString()
				Dim use_time As Integer = LoadSQL("use_time").ToString()
				Check_connect_sqlite()
			Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, qty, st_time, end_time, use_time, number_qty)
			arr_list_id.Add(id)
		End While
		'End If
		Check_connect_sqlite()
		Dim array_id() As Object = arr_list_id.ToArray()
		Check_connect_sqlite()
		For Each element_id In array_id
			Dim value As String = element_id
			Check_connect_sqlite()
			Dim LoadSQLUpdate = Backoffice_model.update_tr_status(value)
			num_arr = num_arr + 1
		Next
		Check_connect_sqlite()
		Dim LoadSQLcl = Backoffice_model.get_tr_closelot_flg_sqlite()
		Dim num_arr2 As Integer = 0
		Dim arr_list_id2 As ArrayList = New ArrayList()
		If LoadSQLcl.HasRows Then
			While LoadSQLcl.Read()
				Dim wi_plan As String = LoadSQLcl("wi").ToString()
				Dim line_cd As String = LoadSQLcl("line_cd").ToString()
				Dim item_cd As String = LoadSQLcl("item_cd").ToString()
				Dim plan_qty As Integer = LoadSQLcl("plan_qty").ToString()
				Dim act_qty As Integer = LoadSQLcl("act_qty").ToString()
				Dim seq_no As Integer = LoadSQLcl("seq_no").ToString()
				Dim shift_prd As String = LoadSQLcl("shift_prd").ToString()
				Dim staff_no As Integer = LoadSQLcl("manpower_no").ToString()
				Dim prd_st_date As Date = LoadSQLcl("prd_st_date").ToString()
				'Dim prd_st_time As Date = LoadSQLcl("prd_st_time").ToString()
				Dim prd_end_date As Date = LoadSQLcl("prd_end_date").ToString()
				'Dim prd_end_time As Date = LoadSQLcl("prd_end_time").ToString()
				Dim lot_no As String = LoadSQLcl("lot_no").ToString()
				Dim comp_flg As String = check_completed_plan(wi_plan, plan_qty)
				Dim transfer_flg As String = "1"
				Dim del_flg As String = "0"
				Dim prd_flg As String = "1"
				Dim close_lot_flg As String = "1"
				Dim avarage_eff As Double = LoadSQLcl("avarage_eff").ToString()
				Dim avarage_act_prd_time As Double = LoadSQLcl("avarage_act_prd_time").ToString()
				If check_data(wi_plan, seq_no) = 0 Then
					Check_connect_sqlite()
					Backoffice_model.Insert_prd_close_lot(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_date, prd_end_date, lot_no, comp_flg, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
					If comp_flg = "1" Then
						Backoffice_model.work_complete(wi_plan)
					End If
				Else
					Check_connect_sqlite()
					update_qty_seq(wi_plan, seq_no, act_qty)
				End If
				arr_list_id2.Add(LoadSQLcl("id").ToString())
			End While
		End If
		Dim Load_check_act_rework = check_rework_actual()
		If Load_check_act_rework > 0 Then
			Check_connect_sqlite()
			Get_data_rework_actual()
		End If
		Dim Load_check_loss_actual = check_loss_actual()
		If Load_check_loss_actual > 0 Then
			Check_connect_sqlite()
			Get_data_loss_actual()
		End If
		Dim check_defact_detail = check_data_defact_detail()
		If check_defact_detail > 0 Then
			Check_connect_sqlite()
			Get_data_defact_actual()
		End If

		Dim array_id2() As Object = arr_list_id2.ToArray()
		For Each element_id2 In array_id2
			Dim value2 As String = element_id2
			Check_connect_sqlite()
			Dim LoadSQLUpdate2 = Backoffice_model.update_tr_close_lot_status(value2)
			num_arr2 = num_arr2 + 1
		Next
		'Console.WriteLine(array_id(2))
		'Dim array() As Object = arr_list.ToArray()
		'For Each element In array
		'    ' Cast object to string.
		'    Dim value As String = element
		'    Console.WriteLine(value)
		'Next
		'MsgBox(arr_id(0))
	End Function
	Public Shared Function check_rework_actual()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select count(rwa_id) as check_data from rework_actual where tr_status ='0'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Dim get_qty_sqlite As Integer = 0
			Try
				While LoadSQL.Read()
					get_qty_sqlite = LoadSQL("check_data").ToString()
				End While
				LoadSQL.Close()
				'sqliteConn.Dispose()
				sqliteConn.Close()
				'sqliteConn = Nothing
				If get_qty_sqlite = 0 Then
					Return 0
				Else
					Return 1
				End If
			Catch ex As Exception
				Return 0
			End Try
			'MsgBox(LoadSQL)
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function check_rework_actual]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function Get_data_rework_actual()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
recheck:
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from rework_actual where tr_status ='0'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Dim get_qty_sqlite As Integer = 0
			Try
				While LoadSQL.Read()
					Dim rwa_part_no As String = LoadSQL("rwa_part_no").ToString()
					Dim rwa_qty As String = LoadSQL("rwa_qty").ToString()
					Dim rwa_shift As String = LoadSQL("rwa_qty").ToString()
					Dim date_now_rework As String = LoadSQL("rwa_created_date_time").ToString()
					Dim ref_wi As String = LoadSQL("ref_wi").ToString()
					Dim ref_part_name As String = LoadSQL("rwa_part_name").ToString()
					Dim rwa_model As String = LoadSQL("rwa_model").ToString()
					Dim tr_status As String = "1"
					INSERT_REWORK_ACTUAL(rwa_part_no, rwa_qty, rwa_shift, date_now_rework, ref_wi, ref_part_name, rwa_model)
				End While
				LoadSQL.Close()
				update_flg_rework_act_sqlite()
			Catch ex As Exception
				MsgBox("error function Get_data_rework_actual == > " & ex.Message)
				Check_connect_sqlite()
				GoTo recheck
			End Try
			'MsgBox(LoadSQL)
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function Get_data_rework_actual]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function update_flg_rework_act_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "update rework_actual set tr_status = '1'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function update_tr_status]")
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function update_flg_loss_atc_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "update loss_actual set transfer_flg = '1'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function update_tr_status]")
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function update_flg_defact_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "update production_defect_detail  set tr_status = '1'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function update_flg_defact_sqlite]")
			sqliteConn.Close()
		End Try
	End Function


	Public Shared Function check_data_sqlite(wi As String, seq_no As String)
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select count(id) as c_id from close_lot_act where wi = '" & wi & "' and seq_no = '" & seq_no & "'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function update_tr_status]")
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function check_data(wi As String, seq_no As String)
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String
		Try
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			SQLCmd.CommandText = "select count(id) as c_id from production_actual where wi = '" & wi & "' and seq_no = '" & seq_no & "'"
			reader = SQLCmd.ExecuteReader()
			Try
				If reader.Read() Then
					If reader("c_id").ToString() = "0" Then
						Return 0
					Else
						get_qty_sqlite = reader("c_id").ToString()
						Return 1
					End If
				Else
					Return 0
				End If
				reader.Close()
			Catch ex As Exception
				reader.Close()
				Return 0
			End Try
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function chkLogin]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function check_completed_plan(wi, plan_qty)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select sum(act_qty) as total_qty from close_lot_act where wi='" & wi & "'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Dim get_qty_sqlite As Integer = 0
			Try
				While LoadSQL.Read()
					get_qty_sqlite = LoadSQL("total_qty").ToString()
				End While
				sqliteConn.Dispose()
				'sqliteConn.Close()
				'sqliteConn = Nothing
				If CDbl(Val(plan_qty)) > get_qty_sqlite Then
					'MsgBox("0" & "plan_qty = " & plan_qty & "-->total_act_qty = " & get_qty_sqlite)
					Return 0
				Else
					Return 1
				End If
			Catch ex As Exception
				Return 0
			End Try
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function check_completed_plan]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function Check_connect_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
		Catch ex As Exception
			sqliteConn.Dispose()
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function update_tr_status_load(id As Integer)
		Check_connect_sqlite()
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			'recheck_update:
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "UPDATE act_ins SET tr_status = '1', updated_date = '" & currdated & "' WHERE id = '" & id & "' "
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			'Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
			'Check_connect_sqlite()
		Catch ex As Exception
			'MsgBox("SQLite Database connect failed. Please contact PC System [Function update_tr_status]")
			'Check_connect_sqlite()
			'GoTo recheck_update
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function update_tr_status(id As Integer)
		Check_connect_sqlite()
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			'recheck_update:
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "UPDATE act_ins SET tr_status = '1', updated_date = '" & currdated & "' WHERE id = '" & id & "' "
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
			'Check_connect_sqlite()
		Catch ex As Exception
			'MsgBox("SQLite Database connect failed. Please contact PC System [Function update_tr_status]")
			'Check_connect_sqlite()
			'GoTo recheck_update
			sqliteConn.Close()
		End Try
	End Function



	Public Shared Function update_tr_close_lot_status(id As Integer)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
recheck:
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "UPDATE close_lot_act SET transfer_flg = '1', updated_date = '" & currdated & "' WHERE id = '" & id & "' "
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function update_tr_close_lot_status]")
			sqliteConn.Close()
			Check_connect_sqlite()
			GoTo recheck
		End Try
	End Function



	Public Shared Function get_trdata_sqlite()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "SELECT * FROM act_ins WHERE tr_status = 0 "
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function get_trdata_sqlite]")
			sqliteConn.Close()
		End Try
	End Function


	Public Shared Function get_tr_closelot_flg_sqlite()

		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			Check_connect_sqlite()
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "SELECT * FROM close_lot_act where transfer_flg = '0'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function get_tr_closelot_flg_sqlite]")
			sqliteConn.Close()
		End Try
	End Function


	Public Shared Function get_trdata_sqlite_act()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "SELECT * FROM close_lot_act WHERE transfer_flg = 0 "

			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function get_trdata_sqlite]")
            sqliteConn.Close()
        End Try
	End Function


	Public Shared Function Insert_prd_close_lot(wi_plan As String, line_cd As String, item_cd As String, plan_qty As Integer, act_qty As Integer, seq_no As Integer, shift_prd As String, manpower_no As Integer, st_time As DateTime, end_time As DateTime, lot_no As String, comp_flg As String, transfer_flg As String, del_flg As String, prd_flg As String, close_lot_flg As String, avarage_eff As Double, avarage_act_prd_time As Double)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.Open()
		Catch ex As Exception
			SQLConn.Close()
		End Try
		'MsgBox(st_time.ToString("dd'/'MM'/'yyyy H':'m':'ss"))
		Dim st_time2 As String = st_time.ToString("H:m:s")
		Dim st_datetime2 As String = st_time.ToString("yyyy/MM/dd H:m:s")
		Dim end_time2 As String = end_time.ToString("H:m:s")
		Dim end_datetime2 As String = end_time.ToString("yyyy/MM/dd H:m:s")
		'MsgBox("INSERT INTO production_actual_detail(pd,line_cd,wi_plan,item_cd,item_name,staff_no,seq_no,qty,st_time,end_time,use_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & wi_plan & "','" & item_cd & "','" & item_name & "','" & staff_no & "','" & seq_no & "','" & qty & "','" & st_time & "','" & end_time & "','" & use_time & "','" & currdated & "')")
		Try
recheck:
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "SELECT * FROM sys_user WHERE emp_id = '" & usernm & "' AND passwd = '" & passwd & "'"
			'SQLCmd.CommandText = "INSERT INTO production_actual_detail(pd,line_cd,st_time,updated_date) VALUES ('" & pd & "','" & line_cd & "','" & st_time2 & "','" & currdated & "')"
			SQLCmd.CommandText = "INSERT INTO production_actual (wi,line_cd,item_cd,plan_qty,act_qty,seq_no,shift_prd,manpower_no,prd_st_date,prd_st_time,prd_end_date,prd_end_time,lot_no,comp_flg,transfer_flg,del_flg,updated_date,prd_flg,close_lot_flg,avarage_eff,avarage_act_prd_time) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & plan_qty & "','" & act_qty & "','" & seq_no & "','" & shift_prd & "','" & manpower_no & "','" & st_datetime2 & "','" & st_time2 & "','" & end_datetime2 & "','" & end_time2 & "','" & lot_no & "','" & comp_flg & "','" & transfer_flg & "','" & del_flg & "','" & currdated & "','" & prd_flg & "','" & close_lot_flg & "','" & avarage_eff & "','" & avarage_act_prd_time & "')"
			reader = SQLCmd.ExecuteReader()
			'MsgBox(reader)
			'Return reader
			SQLConn.Close()
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_prd_close_lot]", ex.Message)
			SQLConn.Close()
			GoTo recheck
		End Try
	End Function



	Public Shared Function Insert_prd_close_lot_sqlite(wi_plan As String, line_cd As String, item_cd As String, plan_qty As Integer, act_qty As Integer, seq_no As Integer, shift_prd As String, manpower_no As Integer, st_time As DateTime, end_time As DateTime, lot_no As String, comp_flg As String, transfer_flg As String, del_flg As String, prd_flg As String, close_lot_flg As String, avarage_eff As Double, avarage_act_prd_time As Double)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd")
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)

		Dim st_time2 As String = st_time.ToString("H:m:s")
		'Dim st_datetime2 As String = st_time.ToString("yyyy/MM/dd H:m:s")
		Dim st_datetime2 As String = st_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
		Dim end_time2 As String = end_time.ToString("H:m:s")
		'Dim end_datetime2 As String = end_time.ToString("yyyy/MM/dd H:m:s")
		Dim end_datetime2 As String = end_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)

		Try
			sqliteConn.Open()

			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			'cmd.CommandText = "UPDATE line_detail SET pd = '" & pd & "', line_cd = '" & line_cd & "', updated_date = '" & currdated & "' WHERE id = 1 "
			'cmd.CommandText = "INSERT INTO act_ins (pd,line_cd,wi_plan,item_cd,item_name,tr_status) VALUES ('pd123','line123','wi123','item123','nm123','1');"

			cmd.CommandText = "INSERT INTO close_lot_act (wi,line_cd,item_cd,plan_qty,act_qty,seq_no,shift_prd,manpower_no,prd_st_date,prd_st_time,prd_end_date,prd_end_time,lot_no,comp_flg,transfer_flg,del_flg,updated_date,prd_flg,close_lot_flg,avarage_eff,avarage_act_prd_time) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & plan_qty & "','" & act_qty & "','" & seq_no & "','" & shift_prd & "','" & manpower_no & "','" & st_datetime2 & "','" & st_time2 & "','" & end_datetime2 & "','" & end_time2 & "','" & lot_no & "','" & comp_flg & "','" & transfer_flg & "','" & del_flg & "','" & currdated & "','" & prd_flg & "','" & close_lot_flg & "','" & avarage_eff & "','" & avarage_act_prd_time & "')"

			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			'MsgBox(LoadSQL)
			Return LoadSQL
			sqliteConn.Dispose()
			sqliteConn.Close()
			sqliteConn = Nothing


		Catch ex As Exception
			MsgBox("SQLite Insert Reccord failed. Please contact PC System [Function insPrdDetail_sqlite]")
			sqliteConn.Close()
		End Try
	End Function



	Public Shared Function Insert_emp_cd(wi_plan As String, staff_cd As String, prd_seq As Integer)
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn

			SQLCmd.CommandText = "INSERT INTO production_emp_detail (wi_plan,staff_cd,prd_seq_no,updated_date) VALUES ('" & wi_plan & "','" & staff_cd & "','" & prd_seq & "','" & currdated & "')"

			reader = SQLCmd.ExecuteReader()
			'Console.WriteLine("INSERT INTO production_emp_detail (wi_plan,staff_cd,prd_seq_no,updated_date) VALUES ('" & wi_plan & "','" & staff_cd & "','" & prd_seq & "','" & currdated & "')")
			'MsgBox("INSERT INTO production_emp_detail (wi_plan,staff_cd,prd_seq_no,updated_date) VALUES ('" & wi_plan & "','" & staff_cd & "','" & prd_seq & "','" & currdated & "')")

			Return reader
			SQLConn.Dispose()
			SQLConn.Close()
			SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function Insert_emp_cd]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function


	Public Shared Function get_loss_op_mst(lind_cd As String)
		Dim reader As SqlDataReader
		Dim reader2 As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect 'Set the Connection String
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'SQLCmd.CommandText = "Select sw.WI,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,sw.qty - SUM (ISNULL(pa.act_qty, 0 )) as 'remain_qty',ISNULL(pa.prd_flg , 0 ) as 'prd_flg'  from sup_work_plan_supply_dev as sw full outer JOIN production_actual as pa on sw.WI = pa.wi WHERE sw.LINE_CD = '" & line_cd & "' and sw.LVL = '1' and (pa.comp_flg <> '1' or pa.comp_flg is NULL) GROUP BY sw.wi,sw.ITEM_CD,sw.ITEM_NAME,sw.QTY,pa.prd_flg"
			SQLCmd.CommandText = "SELECT DISTINCT skld.sk_id,skld.process_no,sscm.sk_name FROM sys_line_mst slm LEFT JOIN sys_skill_line_detail skld ON slm.line_id = skld.line_id LEFT JOIN sys_skill_chart_mst sscm ON skld.sk_id = sscm.sk_id WHERE slm.line_cd = '" & lind_cd & "'"
			reader = SQLCmd.ExecuteReader()

			Return reader
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function get_loss_op_mst]")
			SQLConn.Close()

			Application.Exit()
		End Try
	End Function
	Public Shared Function get_loss_op_mst_sqlite(lind_cd As String)
re_insert_rework_act:
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:mm:ss")
			Dim reader As SqlDataReader
			Dim SQLConn As New SqlConnection() 'The SQL Connection
			Dim SQLCmd As New SqlCommand()
			cmd.CommandText = "SELECT DISTINCT skld.sk_id,skld.process_no,sscm.sk_name FROM sys_line_mst slm LEFT JOIN sys_skill_line_detail skld ON slm.line_id = skld.line_id LEFT JOIN sys_skill_chart_mst sscm ON skld.sk_id = sscm.sk_id WHERE slm.line_cd = '" & lind_cd & "'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Return LoadSQL
			'  MsgBox(LoadSQL)
			'Return LoadSQL
			LoadSQL.Close()
			sqliteConn.Close()
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function get_loss_op_mst_sqlite]" & ex.Message)
			sqliteConn.Close()
			GoTo re_insert_rework_act
		End Try
	End Function

	Public Shared Function ins_loss_act(pd As String, line_cd As String, wi_plan As String, item_cd As String, seq_no As String, shift_prd As String, st_time As DateTime, end_time As DateTime, loss_time As Integer, loss_type As String, loss_id As String, op_id As String, transfer_flg As String, flg_control As String)
		Check_loss_and_update_flg_loss()
		Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()

		Dim st_datetime2 As String = st_time.ToString("yyyy/MM/dd H:m:s")
		Dim end_datetime2 As String = end_time.ToString("yyyy/MM/dd H:m:s")
		'Console.WriteLine("INSERT INTO loss_actual (wi,line_cd,item_cd,seq_no,shift_prd,start_loss,end_loss,loss_time,updated_date,loss_type,loss_cd_id,line_op_id,pd,transfer_flg) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & seq_no & "','" & shift_prd & "','" & st_datetime2 & "','" & end_datetime2 & "','" & loss_time & "','" & currdated & "','" & loss_type & "','" & loss_id & "','" & op_id & "','" & pd & "','" & transfer_flg & "')")
		'MsgBox("INSERT INTO loss_actual (wi,line_cd,item_cd,seq_no,shift_prd,start_loss,end_loss,loss_time,updated_date,loss_type,loss_cd_id,line_op_id,pd,transfer_flg) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & seq_no & "','" & shift_prd & "','" & st_datetime2 & "','" & end_datetime2 & "','" & loss_time & "','" & currdated & "','" & loss_type & "','" & loss_id & "','" & op_id & "','" & pd & "','" & transfer_flg & "')")
		Try
			SQLConn.ConnectionString = sqlConnect
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'    MsgBox("INSERT INTO loss_actual (wi,line_cd,item_cd,seq_no,shift_prd,start_loss,end_loss,loss_time,updated_date,loss_type,loss_cd_id,line_op_id,pd,transfer_flg) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & seq_no & "','" & shift_prd & "','" & st_datetime2 & "','" & end_datetime2 & "','" & loss_time & "','" & currdated & "','" & loss_type & "','" & loss_id & "','" & op_id & "','" & pd & "','" & transfer_flg & "')")
			SQLCmd.CommandText = "INSERT INTO loss_actual (wi,line_cd,item_cd,seq_no,shift_prd,start_loss,end_loss,loss_time,updated_date,loss_type,loss_cd_id,line_op_id,pd,transfer_flg , flg_control) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & seq_no & "','" & shift_prd & "','" & st_datetime2 & "','" & end_datetime2 & "','" & loss_time & "','" & currdated & "','" & loss_type & "','" & loss_id & "','" & op_id & "','" & pd & "','" & transfer_flg & "','" & flg_control & "')"
			reader = SQLCmd.ExecuteReader()
			'SQLConn.Dispose()
			SQLConn.Close()
			Dim api = New api()
			Dim result_api_checkper = api.Load_data("http://192.168.161.207/linenotify_fa/Alert_loss_fa/notify_fa?wi=" & wi_plan & "&flg_control=" & flg_control & "&pd=" & pd & "&loss_id=" & loss_id)
			Loss_reg.date_time_commit_data.Text = st_datetime2
			'SQLConn = Nothing
		Catch ex As Exception
			MsgBox("MSSQL Database connect failed. Please contact PC System [Function ins_loss_act]")
			SQLConn.Close()
			Application.Exit()
		End Try
	End Function
	Public Shared Function Check_loss_and_update_flg_loss()
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		Try
			SQLConn.ConnectionString = sqlConnect
			SQLConn.Open()
			SQLCmd.Connection = SQLConn
			'    MsgBox("INSERT INTO loss_actual (wi,line_cd,item_cd,seq_no,shift_prd,start_loss,end_loss,loss_time,updated_date,loss_type,loss_cd_id,line_op_id,pd,transfer_flg) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & seq_no & "','" & shift_prd & "','" & st_datetime2 & "','" & end_datetime2 & "','" & loss_time & "','" & currdated & "','" & loss_type & "','" & loss_id & "','" & op_id & "','" & pd & "','" & transfer_flg & "')")
			SQLCmd.CommandText = "Update loss_actual set flg_control = '1' where flg_control = '0' and line_cd = '" & GET_LINE_PRODUCTION() & "'"
			reader = SQLCmd.ExecuteReader()
			'SQLConn.Dispose()
			SQLConn.Close()
		Catch

		End Try
	End Function
	Public Shared Function ins_loss_act_sqlite(pd As String, line_cd As String, wi_plan As String, item_cd As String, seq_no As String, shift_prd As String, st_time As DateTime, end_time As DateTime, loss_time As Integer, loss_type As String, loss_id As String, op_id As String, transfer_flg As String, flg_control As String)
re_insert_rework_act:
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:mm:ss")
			Dim reader As SqlDataReader
			Dim SQLConn As New SqlConnection() 'The SQL Connection
			Dim SQLCmd As New SqlCommand()
			Dim st_datetime2 As String = st_time.ToString("yyyy/MM/dd H:mm:ss")
			Dim end_datetime2 As String = end_time.ToString("yyyy/MM/dd H:mm:ss")
			cmd.CommandText = "INSERT INTO loss_actual (wi,line_cd,item_cd,seq_no,shift_prd,start_loss,end_loss,loss_time,updated_date,loss_type,loss_cd_id,line_op_id,pd,transfer_flg , flg_control) VALUES ('" & wi_plan & "','" & line_cd & "','" & item_cd & "','" & seq_no & "','" & shift_prd & "','" & st_datetime2 & "','" & end_datetime2 & "','" & loss_time & "','" & currdated & "','" & loss_type & "','" & loss_id & "','" & op_id & "','" & pd & "','" & transfer_flg & "','" & flg_control & "')"
			Loss_reg.date_time_commit_data.Text = st_datetime2
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			sqliteConn.Close()
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function ins_loss_act_sqlite]" & ex.Message)
			sqliteConn.Close()
			GoTo re_insert_rework_act
		End Try
	End Function

	Public Shared Function check_loss_actual()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select count(id) as check_data from loss_actual where transfer_flg ='0'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Dim get_qty_sqlite As Integer = 0
			Try
				While LoadSQL.Read()
					get_qty_sqlite = LoadSQL("check_data").ToString()
				End While
				LoadSQL.Close()
				'sqliteConn.Dispose()
				sqliteConn.Close()
				'sqliteConn = Nothing
				If get_qty_sqlite = 0 Then
					Return 0
				Else
					Return 1
				End If
			Catch ex As Exception
				Return 0
			End Try
			'MsgBox(LoadSQL)
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function check_loss_actual]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function check_data_defact_detail()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select count(id) as check_data from production_defect_detail where tr_status ='0'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Dim get_qty_sqlite As Integer = 0
			Try
				While LoadSQL.Read()
					get_qty_sqlite = LoadSQL("check_data").ToString()
				End While
				LoadSQL.Close()
				'sqliteConn.Dispose()
				sqliteConn.Close()
				'sqliteConn = Nothing
				If get_qty_sqlite = 0 Then
					Return 0
				Else
					Return 1
				End If
			Catch ex As Exception
				Return 0
			End Try
			'MsgBox(LoadSQL)
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function check_data_defact_detail]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function
	Public Shared Function Get_data_loss_actual()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
recheck:
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from loss_actual where transfer_flg ='0'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Dim get_qty_sqlite As Integer = 0
			Try
				While LoadSQL.Read()
					Dim wi As String = LoadSQL("wi").ToString()
					Dim line_cd As String = LoadSQL("line_cd").ToString()
					Dim item_cd As String = LoadSQL("item_cd").ToString()
					Dim seq_no As String = LoadSQL("seq_no").ToString()
					Dim shift_prd As String = LoadSQL("shift_prd").ToString()
					Dim start_loss As String = LoadSQL("start_loss").ToString()
					Dim end_loss As String = LoadSQL("end_loss").ToString()
					Dim loss_time As String = LoadSQL("loss_time").ToString()
					Dim updated_date As String = LoadSQL("updated_date").ToString()
					Dim loss_type As String = LoadSQL("loss_type").ToString()
					Dim loss_cd_id As String = LoadSQL("loss_cd_id").ToString()
					Dim op_id As String = LoadSQL("line_op_id").ToString()
					Dim pd As String = LoadSQL("pd").ToString()
					Dim transfer_flg As String = LoadSQL("transfer_flg").ToString()
					Dim flg_control As String = LoadSQL("flg_control").ToString()
					Backoffice_model.ins_loss_act(pd, line_cd, wi, item_cd, seq_no, shift_prd, start_loss, end_loss, loss_time, loss_type, loss_cd_id, op_id, "1", flg_control)
				End While
				LoadSQL.Close()
				update_flg_loss_atc_sqlite()
			Catch ex As Exception
				MsgBox("error function Get_data_rework_actual == > " & ex.Message)
				Check_connect_sqlite()
				GoTo recheck
			End Try
			'MsgBox(LoadSQL)
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function Get_data_rework_actual]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function

	Public Shared Function Get_data_defact_actual()
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
recheck:
			sqliteConn.Open()
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "select * from production_defect_detail where tr_status ='0'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			Dim get_qty_sqlite As Integer = 0
			Try
				While LoadSQL.Read()
					Dim id As String = LoadSQL("id").ToString()
					Dim pd As String = LoadSQL("pd").ToString()
					Dim line_cd As String = LoadSQL("line_cd").ToString()
					Dim wi_plan As String = LoadSQL("wi_plan").ToString()
					tmp_wi = wi_plan
					Dim item_cd As String = LoadSQL("item_cd").ToString()
					Dim item_name As String = LoadSQL("item_name").ToString()
					Dim staff_no As Integer = LoadSQL("staff_no").ToString()
					Dim seq_no As Integer = LoadSQL("seq_no").ToString()
					Dim qty As Integer = LoadSQL("qty").ToString()
					Dim number_qty As String = LoadSQL("number_qty").ToString()
					Dim st_time As String = LoadSQL("st_time").ToString()
					Dim end_time As String = LoadSQL("end_time").ToString()
					Dim use_time As Integer = LoadSQL("use_time").ToString()
					Dim tr_status As Integer = LoadSQL("tr_status").ToString()
					Dim flg_defact As String = LoadSQL("flg_defact").ToString()
					Dim defact_id As String = LoadSQL("defact_id").ToString()
					Insert_prd_detail_defact(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, qty, st_time, end_time, use_time, number_qty, tr_status, "1", defact_id)
				End While
				LoadSQL.Close()
				update_flg_defact_sqlite()
			Catch ex As Exception
				MsgBox("error function Get_data_defact_actual == > " & ex.Message)
			End Try
			'MsgBox(LoadSQL)
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function Get_data_defact_actual]" & ex.Message)
			sqliteConn.Close()
			Check_connect_sqlite()
			GoTo recheck
		End Try
	End Function

	Public Shared Function alert_loss(wi_plan, flg_control, pd, loss_id)
		Dim api = New api()
		Dim result_api_checkper = api.Load_data("http://192.168.161.207/linenotify_fa/Alert_loss_fa/notify_fa?wi=" & wi_plan & "&flg_control=" & flg_control & "&pd=" & pd & "&loss_id=" & loss_id)
	End Function
	Public Shared Function Update_flg_loss(pd As String, line_cd As String, wi_plan As String, item_cd As String, seq_no As String, shift_prd As String, st_time As DateTime, end_time As DateTime, loss_time As Integer, loss_type As String, loss_id As String, op_id As String, transfer_flg As String, flg_control As String)
re_insert_rework_act:
		Dim reader As SqlDataReader
		Dim SQLConn As New SqlConnection() 'The SQL Connection
		Dim SQLCmd As New SqlCommand()
		SQLConn.ConnectionString = sqlConnect 'Set the Connection String
		SQLConn.Open()
		SQLCmd.Connection = SQLConn
		Try
			If flg_control = "2" Then
				loss_time = "0"
			End If
			Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:mm:ss")
			Dim st_datetime2 As String = st_time.ToString("yyyy/MM/dd H:mm:ss")
			Dim end_datetime2 As String = end_time.ToString("yyyy/MM/dd H:mm:ss")
			Dim date_now As String = end_time.ToString("dd/MM/yyyy")
			SQLCmd.CommandText = "Update loss_actual 
			set end_loss = '" & end_datetime2 & "',
			loss_time = '" & loss_time & "' , 
			updated_date = '" & currdated & "' , 
			line_op_id = '" & op_id & "'  , 
			transfer_flg = '" & transfer_flg & "' , 
			flg_control ='" & flg_control & "'
			where wi='" & wi_plan & "' and 
			line_cd = '" & line_cd & "' and 
			item_cd = '" & item_cd & "' and 
			seq_no = '" & seq_no & "' and 
			shift_prd = '" & shift_prd & "' and 
			start_loss = '" & st_datetime2 & "'"
			reader = SQLCmd.ExecuteReader()
			SQLConn.Close()
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function Update_flg_loss]" & ex.Message)
			SQLConn.Close()
			GoTo re_insert_rework_act
		End Try
	End Function
	Public Shared Function Update_flg_loss_sqlite(pd As String, line_cd As String, wi_plan As String, item_cd As String, seq_no As String, shift_prd As String, st_time As DateTime, end_time As DateTime, loss_time As Integer, loss_type As String, loss_id As String, op_id As String, transfer_flg As String, flg_control As String)
		Dim sqliteConn As New SQLiteConnection(sqliteConnect)
		Try
			sqliteConn.Open()
			Dim currdated As String = DateTime.Now.ToString("yyyy/MM/dd H:mm:ss")
			Dim st_datetime2 As String = st_time.ToString("yyyy/MM/dd H:mm:ss")
			Dim end_datetime2 As String = end_time.ToString("yyyy/MM/dd H:mm:ss")
			Dim date_now As String = end_time.ToString("dd/MM/yyyy")
			Dim cmd As New SQLiteCommand
			cmd.Connection = sqliteConn
			cmd.CommandText = "Update loss_actual set end_loss = '" & end_datetime2 & "',loss_time = '" & loss_time & "' , updated_date = '" & currdated & "' , line_op_id = '" & op_id & "'  , transfer_flg = '" & transfer_flg & "' , flg_control ='" & flg_control & "' where wi='" & wi_plan & "' and line_cd = '" & line_cd & "' and item_cd = '" & item_cd & "' and seq_no = '" & seq_no & "' and shift_prd = '" & shift_prd & "' and start_loss = '" & st_datetime2 & "'"
			Dim LoadSQL As SQLiteDataReader = cmd.ExecuteReader()
			LoadSQL.Close()
			sqliteConn.Close()
			Dim api = New api()
			'MsgBox(LoadSQL)
		Catch ex As Exception
			MsgBox("SQLite Database connect failed. Please contact PC System [Function Update_flg_loss_sqlite]" & ex.Message)
			sqliteConn.Close()
		End Try
	End Function
End Class
