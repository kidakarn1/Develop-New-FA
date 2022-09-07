Imports VB = Microsoft.VisualBasic
Imports System
Imports System.Management
Imports System.ComponentModel
Imports System.Threading
Imports System.IO
Imports System.IO.Ports
Imports System.Text
Imports System.Globalization
Imports System.Drawing.Imaging
Imports IDAutomation.Windows.Forms.LinearBarCode
Imports System.Drawing.Printing
Imports System.Configuration
Imports GenCode128
Imports BarcodeLib.Barcode
Public Class Working_Pro
	Dim QR_Generator As New MessagingToolkit.QRCode.Codec.QRCodeEncoder
	Dim start_flg As Integer = 0
	Dim comp_flg As Integer = 0
	Dim start_year As Integer = 0
	Dim start_month As Integer = 0
	Dim start_days As Integer = 0
	Dim Id As Short                             ' Device ID
	Dim Ret As Integer                          ' Return Code
	Dim szError As New StringBuilder("", 256)   ' Error String
	Dim szText As New String("", 100)
	Dim UpCount(2) As Short                     'zr Up Counter
	Dim DownCount(2) As Short                   ' Down Counter
	Dim Check(7) As CheckBox
	Dim Edit_Up(1) As TextBox
	Dim Edit_Down(7) As TextBox
	Dim delay_btn As Integer = 0
	Dim check_bull As Integer = 0
	Public check_in_up_seq As Integer = 0
	Dim value_next_process As String = ""
	Public check_format_tag As String = Backoffice_model.B_check_format_tag()
	Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
		Label44.Text = TimeOfDay.ToString("H:mm:ss")
		Label43.Text = DateTime.Now.ToString("yyyy/MM/dd")
		lb_loss_status.Text = lb_loss_status.Text
		If lb_loss_status.Right < 0 Then
			lb_loss_status.Left = Panel6.ClientSize.Width
		Else
			lb_loss_status.Left -= 10
		End If
	End Sub
	Private Sub Working_Pro_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Wait_data.Close()
		'Prd_detail.Timer3.Enabled = False
		Label47.Visible = False
		Timer1.Start()
		'sc_prd_plan.SerialPort1.Close()
		lb_loss_status.Parent = Panel6

		Dim next_process = Backoffice_model.GET_NEXT_PROCESS()
		Backoffice_model.S_chk_spec_line = Backoffice_model.chk_spec_line()
		While next_process.read()
			value_next_process = next_process("NEXT_PROCESS").ToString
		End While
		next_process.Close()
		lb_loss_status.Location = New Point(Panel6.ClientSize.Width,
		Panel6.ClientSize.Height / 2 - (lb_loss_status.Height / 2))
		Check(0) = _Check_0
		Check(1) = _Check_1
		Edit_Up(0) = _Edit_Up_0
		Edit_Up(1) = _Edit_Up_1
		'Edit_DeviceName.Text = "DIO000"
		Edit_DeviceName.Text = MainFrm.lb_dio_port.Text
		Label24.Text = Backoffice_model.GET_LINE_PRODUCTION()
		For ii = 0 To 1
			Edit_Up(ii).Text = ""
		Next ii
		For iii = 0 To 1
			Check(iii).Checked = True
		Next
		Dim szDeviceName As String
		' Get device name from screen
		szDeviceName = Edit_DeviceName.Text
		' Execute initialization function and get ID
		Ret = DioInit(szDeviceName, Id)
		' Error process
		Call DioGetErrorString(Ret, szError)
		If Ret = 10000 Then
			Edit_DeviceName.Text = "DIO001"
			' Get device name from screen
			szDeviceName = Edit_DeviceName.Text
			' Execute initialization function and get ID
			Ret = DioInit(szDeviceName, Id)
			' Error process
			Call DioGetErrorString(Ret, szError)
			If Ret = 10000 Then
				Edit_DeviceName.Text = "DIO002"
				' Get device name from screen
				szDeviceName = Edit_DeviceName.Text
				' Execute initialization function and get ID
				Ret = DioInit(szDeviceName, Id)
				' Error process
				Call DioGetErrorString(Ret, szError)
			End If
		End If
		'MsgBox(Ret)
		'Edit_ReturnCode.Text = "Ret = " & Ret & ":" & szError.ToString()
		Dim i As Short
		Dim BitNo As Short
		Dim Kind As Short
		Dim Tim As Integer
		For i = 0 To 1
			UpCount(i) = 0
			DownCount(i) = 0
		Next i
		Tim = 100
		Kind = DIO_TRG_RISE
		For BitNo = 0 To 1
			If Check(BitNo).CheckState = 1 Then
				Ret = DioNotifyTrg(Id, BitNo, Kind, Tim, Me.Handle.ToInt32)
				If (Ret <> DIO_ERR_SUCCESS) Then
					'MsgBox("Connect success")
					Exit For
				End If
			Else
				Ret = DioStopNotifyTrg(Id, BitNo)
				If (Ret <> DIO_ERR_SUCCESS) Then
					'MsgBox("Connect Failed")
					Exit For
				End If
			End If
		Next BitNo
		Call DioGetErrorString(Ret, szError)
		Dim date_now As String = DateTime.Now.ToString("dd-MM-yyyy")
		Dim date_now_date As Date = DateTime.Now.ToString("dd-MM-yyyy")
		Dim time As Date = DateTime.Now.ToString("H:m:s")
		Dim date_st = DateTime.Now.ToString("dd-MM-yyyy")
		Dim date_end = DateTime.Now.ToString("dd-MM-yyyy")
		Dim result_qty = DateTime.Now.ToString("dd-MM-yyyy")
		Dim time_st = " 00:00:00"
		Dim time_end = " 23:59:59"
		If Label14.Text = "A" Then
			time_st = " 08:00:00"
			time_end = " 17:00:00"
		ElseIf Label14.Text = "P" Then
			time_st = " 08:00:00"
			time_end = " 20:00:00"
		ElseIf Label14.Text = "B" Then
			time_st = " 20:00:00"
			time_end = " 05:00:00"
		ElseIf Label14.Text = "Q" Then
			time_st = " 20:00:00"
			time_end = " 08:00:00"
		End If

		If time > "23:59:59" Then
			date_st = date_now_date.AddDays(-1)
			date_st = Convert.ToDateTime(date_st).ToString("dd-MM-yyyy")
			date_end = date_now_date
			date_end = Convert.ToDateTime(date_end).ToString("dd-MM-yyyy")
			time_st = " 20:00:00"
			time_end = " 08:00:00"
		End If
		If time >= "00:00:00" And time <= "08:00:00" Then
			date_st = date_now_date.AddDays(-1)
			date_st = Convert.ToDateTime(date_st).ToString("dd-MM-yyyy")
			date_end = date_now_date
			date_end = Convert.ToDateTime(date_end).ToString("dd-MM-yyyy")
			time_st = " 20:00:00"
			time_end = " 08:00:00"
		End If
		Dim reader_shift = Backoffice_model.GET_QTY_SHIFT(MainFrm.Label4.Text, wi_no.Text, Label14.Text, date_st, date_end, time_st, time_end, Label18.Text) '
		Dim reader_defact_nc = Backoffice_model.GET_QTY_DEFACT_NC(wi_no.Text, MainFrm.Label4.Text)
		While reader_defact_nc.read()
			lb_nc_qty.Text = reader_defact_nc("QTY_DEFACT_NC").ToString()
			If lb_nc_qty.Text = "" Then
				lb_nc_qty.Text = 0
			End If
		End While
		reader_defact_nc.close()
		While reader_shift.read()
			LB_COUNTER_SHIP.Text = reader_shift("QTY_SHIFT").ToString()
		End While
		If LB_COUNTER_SHIP.Text = "" Then
			LB_COUNTER_SHIP.Text = 0
		End If
		Dim reader_seq = Backoffice_model.GET_QTY_SEQ(wi_no.Text, CDbl(Val(Label22.Text))) '
		While reader_seq.read()
			If reader_seq.read Then
				LB_COUNTER_SEQ.Text = reader_seq("QTY_SEQ").ToString()
			Else
				LB_COUNTER_SEQ.Text = 0
			End If
		End While
		Backoffice_model.Get_close_lot_time(Label14.Text)
		Timer2.Start()
	End Sub
	Private Sub Label8_Click(sender As Object, e As EventArgs)

	End Sub
	Private Sub btn_setup_Click(sender As Object, e As EventArgs) Handles btn_setup.Click
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Sel_prd_setup.Show()
				Me.Enabled = False
			Else
				load_show.Show()
			End If
		Catch ex As Exception
			load_show.Show()
		End Try
		'Dim i As Integer
		'ProgressBar1.Minimum = 0
		'ProgressBar1.Maximum = 200
		'For i = 0 To 200
		'ProgressBar1.Value = i
		'Next
	End Sub

	Private Sub SendMessageLong(hwnd As Object, prgBrClr As Object, p1 As Object, p2 As Object)
		Throw New NotImplementedException()
	End Sub

	Private Sub ProgressBar1_Click(sender As Object, e As EventArgs)

	End Sub
	Public Sub stop_working()
		Dim line_id As String = MainFrm.line_id.Text
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.line_status_upd(line_id)
			Else
				Backoffice_model.line_status_upd_sqlite(line_id)
			End If
		Catch ex As Exception
			Backoffice_model.line_status_upd_sqlite(line_id)
		End Try


		Dim date_st As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim date_end As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.line_status_ins(line_id, date_st, date_end, "1", "0", 24, "0", Prd_detail.lb_wi.Text)
			Else
				Backoffice_model.line_status_ins_sqlite(line_id, date_st, date_end, "1", "0", 24, "0", Prd_detail.lb_wi.Text)
			End If
		Catch ex As Exception
			Backoffice_model.line_status_ins_sqlite(line_id, date_st, date_end, "1", "0", 24, "0", Prd_detail.lb_wi.Text)
		End Try
		start_flg = 0
		'Chang_Loss.Show()
		Button1.Visible = False
		Panel1.BackColor = Color.Red
		Label30.Text = "STOPPED"
		'btn_back.Visible = True
		btn_setup.Visible = True
		btn_ins_act.Visible = True
		btn_desc_act.Visible = True
		btn_defect.Visible = True
		btn_closelot.Visible = True
		btn_stop.Visible = False
		btn_start.Visible = True
	End Sub
	Private Sub btn_stop_Click(sender As Object, e As EventArgs) Handles btn_stop.Click
		check_network_frist = 1
		stop_working()
	End Sub

	Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btn_defect.Click
		Defect_menu.Show()
		Me.Enabled = False
		'Close_lot.Show()
		'Me.Close()
	End Sub
	Private Sub Label10_Click(sender As Object, e As EventArgs)

	End Sub
	Private Sub Label7_Click(sender As Object, e As EventArgs) Handles Label7.Click

	End Sub

	Private Sub Label28_Click(sender As Object, e As EventArgs) Handles Label28.Click

	End Sub
	Private Sub Label29_Click(sender As Object, e As EventArgs) Handles Label29.Click

	End Sub
	Private Sub Label10_Click_1(sender As Object, e As EventArgs) Handles Label10.Click

	End Sub
	Private Sub CircularProgressBar1_Click(sender As Object, e As EventArgs) Handles CircularProgressBar1.Click

	End Sub
	Private Sub Button6_Click(sender As Object, e As EventArgs) Handles btn_back.Click
		Dim line_id As String = MainFrm.line_id.Text
		Backoffice_model.line_status_upd(line_id)
		Prd_detail.Enabled = True
		Prd_detail.Timer3.Enabled = True
		Insert_list.ListView1.View = View.Details
		Dim line_cd As String = MainFrm.Label4.Text
		Dim LoadSQL = Backoffice_model.get_prd_plan(line_cd)
		Dim numberOfindex As Integer = 0
		While LoadSQL.Read()
			'MsgBox(LoadSQL("prd_flg").ToString())
			If LoadSQL("prd_flg").ToString() = 1 Then
				'MsgBox("Red")
				Insert_list.ListView1.ForeColor = Color.Red
				Insert_list.ListView1.Items.Add(LoadSQL("WI").ToString()).SubItems.AddRange(New String() {LoadSQL("ITEM_CD").ToString(), LoadSQL("ITEM_NAME").ToString(), LoadSQL("QTY").ToString(), LoadSQL("remain_qty").ToString()})
				Insert_list.ListView1.Items(numberOfindex).ForeColor = Color.Red
			Else
				'MsgBox("Blue")
				Insert_list.ListView1.ForeColor = Color.Blue
				Insert_list.ListView1.Items.Add(LoadSQL("WI").ToString()).SubItems.AddRange(New String() {LoadSQL("ITEM_CD").ToString(), LoadSQL("ITEM_NAME").ToString(), LoadSQL("QTY").ToString(), LoadSQL("remain_qty").ToString()})
				Insert_list.ListView1.Items(numberOfindex).ForeColor = Color.Blue
			End If
			Insert_list.ListBox1.Items.Add(LoadSQL("PS_UNIT_NUMERATOR"))
			Insert_list.ListBox2.Items.Add(LoadSQL("CT"))
			Insert_list.ListBox3.Items.Add(LoadSQL("seq_count"))
			Insert_list.lbx_dlv_date.Items.Add(LoadSQL("DLV_DATE"))
			Insert_list.lbx_location.Items.Add(LoadSQL("LOCATION_PART"))
			Insert_list.lbx_model.Items.Add(LoadSQL("MODEL"))
			Insert_list.lbx_prd_type.Items.Add(LoadSQL("PRODUCT_TYP"))
			numberOfindex = numberOfindex + 1
			'Insert_list.ListView1.ForeColor = Color.Red
			'Insert_list.ListView1.Items.Add(LoadSQL("WI").ToString()).SubItems.AddRange(New String() {LoadSQL("ITEM_CD").ToString(), LoadSQL("ITEM_NAME").ToString(), LoadSQL("QTY").ToString(), LoadSQL("remain_qty").ToString()})
			'line_id.Text = LoadSQL("line_id").ToString()
		End While
		'Insert_list.Show()
		Prd_detail.Show()
		Me.Close()
	End Sub

	Private Sub btn_start_Click(sender As Object, e As EventArgs) Handles btn_start.Click
		If check_network_frist = 0 Then
			Try
				If My.Computer.Network.Ping("192.168.161.101") Then
					Prd_detail.Timer3.Enabled = False
				End If
			Catch ex As Exception

			End Try
		End If
		If check_in_up_seq = 0 Then
			Dim date_st1 As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
			Dim date_end1 As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
			Dim GET_SEQ = Backoffice_model.GET_SEQ_PLAN_current(Prd_detail.lb_wi.Text, Backoffice_model.GET_LINE_PRODUCTION(), date_st1, date_end1)
			Try
				If GET_SEQ.Read() Then
					Dim C_seq_no As Integer = CDbl(Val(GET_SEQ("seq_no")))
					If C_seq_no > 0 Then
						Dim seq_no_naja = GET_SEQ("seq_no")
						Dim update_data = Backoffice_model.Update_seqplan(Prd_detail.lb_wi.Text, Backoffice_model.GET_LINE_PRODUCTION(), date_st1, date_end1, CDbl(Val(Prd_detail.lb_seq.Text)) + 1)
					Else
						Dim insert_data = Backoffice_model.INSERT_tmp_planseq(Prd_detail.lb_wi.Text, Backoffice_model.GET_LINE_PRODUCTION(), date_st1, date_end1)
						Dim seq_no_naja = 0
					End If
				End If
			Catch ex As Exception
				Dim insert_data = Backoffice_model.INSERT_tmp_planseq(Prd_detail.lb_wi.Text, Backoffice_model.GET_LINE_PRODUCTION(), date_st1, date_end1)
				Dim seq_no_naja = 0
			End Try
			GET_SEQ.close()
			Dim temp_co_emp As Integer = List_Emp.ListView1.Items.Count
			For i = 0 To temp_co_emp - 1
				emp_cd = List_Emp.ListView1.Items(i).Text
				Backoffice_model.Insert_production_emp_detail_realtime(wi_no.Text, emp_cd, Label22.Text)
				'MsgBox(List_Emp.ListView1.Items(i).Text)
			Next
			check_in_up_seq += 1
		End If

		Dim line_id As String = MainFrm.line_id.Text
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.line_status_upd(line_id)
				Backoffice_model.line_status_upd_sqlite(line_id)
			Else
				Backoffice_model.line_status_upd_sqlite(line_id)
			End If
		Catch ex As Exception
			Backoffice_model.line_status_upd_sqlite(line_id)
		End Try

		Dim date_st As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")
		Dim date_end As String = DateTime.Now.ToString("yyyy/MM/dd H:m:s")

		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.line_status_ins(line_id, date_st, date_end, "2", "0", 0, "0", Prd_detail.lb_wi.Text)
				Backoffice_model.line_status_ins_sqlite(line_id, date_st, date_end, "2", "0", 0, "0", Prd_detail.lb_wi.Text)
			Else
				Backoffice_model.line_status_ins_sqlite(line_id, date_st, date_end, "2", "0", 0, "0", Prd_detail.lb_wi.Text)
			End If
		Catch ex As Exception
			Backoffice_model.line_status_ins_sqlite(line_id, date_st, date_end, "2", "0", 0, "0", Prd_detail.lb_wi.Text)
		End Try
		Dim c_type As String = MainFrm.count_type.Text
		If c_type = "TOUCH" Then
			Button1.Visible = True
		Else
			Button1.Visible = False
		End If

		start_flg = 1

		'Button1.Visible = True

		Dim st_counter As String = Label32.Text

		If st_counter = "0" Then
			Label16.Text = TimeOfDay.ToString("H : mm")
			Label32.Text = "1"
			btn_back.Enabled = False
			If lb_ch_man_flg.Text = "1" Then
				Dim value_temps1 As Double
				value_temps1 = Double.TryParse(Label34.Text, value_temps1)
				Dim testt1 As Integer = Label34.Text
				Dim newDate1 As Date = DateAdd("n", testt1, Now)
				Label20.Text = newDate1.ToString("H : mm")
			End If
			Dim pd As String = MainFrm.Label6.Text
			Dim line_cd As String = MainFrm.Label4.Text
			Dim wi_plan As String = wi_no.Text
			Dim item_cd As String = Label3.Text
			Dim item_name As String = Label12.Text
			Dim staff_no As String = Label29.Text
			Dim seq_no As String = Label22.Text
			Dim prd_qty As Integer = 0
			Dim start_time As Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			Dim end_time As Date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			Dim use_time As Double = 0.00
			Dim tr_status As String = "0"
			Dim number_qty As Integer = Label6.Text
			Dim start_time2 As String = start_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			Dim end_time2 As String = end_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			Try
				If My.Computer.Network.Ping("192.168.161.101") Then
					tr_status = "1"
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
					Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_time, number_qty)
					'MsgBox("Ping completed")
				Else
					tr_status = "0"
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
					'MsgBox("Ping incompleted")
				End If
			Catch ex As Exception
				tr_status = "0"
				Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
			End Try
			st_time.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
			st_count_ct.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
			btn_setup.Enabled = True
			btn_ins_act.Enabled = True
			btn_desc_act.Enabled = True
			btn_defect.Enabled = True
			btn_closelot.Enabled = True
			'Dim temppo As Double = Label34.Text
			CircularProgressBar2.Text = 0 & "%"
			CircularProgressBar2.Value = 0

			Dim value_temps As Double

			value_temps = Double.TryParse(Label34.Text, value_temps)

			Dim testt As Integer = Label34.Text

			Dim newDate As Date = DateAdd("n", testt, Now)
			Label20.Text = newDate.ToString("H : mm")
			'MsgBox(Label20.Text)
		Else
			'Label32.Text = "0"
		End If
		Panel1.BackColor = Color.Green
		Label30.Text = "NORMAL"
		btn_start.Visible = False
		btn_back.Visible = False
		btn_setup.Visible = False
		btn_ins_act.Visible = False
		btn_desc_act.Visible = False
		btn_defect.Visible = False
		btn_closelot.Visible = False
		btn_stop.Visible = True
		Prd_detail.Timer3.Enabled = False
	End Sub
	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Dim yearNow As Integer = DateTime.Now.ToString("yyyy")
		Dim monthNow As Integer = DateTime.Now.ToString("MM")
		Dim dayNow As Integer = DateTime.Now.ToString("dd")
		Dim hourNow As Integer = DateTime.Now.ToString("HH")
		Dim minNow As Integer = DateTime.Now.ToString("mm")
		Dim secNow As Integer = DateTime.Now.ToString("ss")
		'MsgBox(yearNow & monthNow & dayNow & hourNow & minNow & secNow)
		Dim yearSt As Integer = st_time.Text.Substring(0, 4)
		Dim monthSt As Integer = st_time.Text.Substring(5, 2)
		Dim daySt As Integer = st_time.Text.Substring(8, 2)
		Dim hourSt As Integer = st_time.Text.Substring(11, 2)
		Dim minSt As Integer = st_time.Text.Substring(14, 2)
		Dim secSt As Integer = st_time.Text.Substring(17, 2)
		'MsgBox(yearSt & minthSt & daySt & hourSt & minSt & secSt)
		Dim firstDate As New System.DateTime(yearSt, monthSt, daySt, hourSt, minSt, secSt)
		Dim secondDate As New System.DateTime(yearNow, monthNow, dayNow, hourNow, minNow, secNow)
		Dim diff As System.TimeSpan = secondDate.Subtract(firstDate)
		Dim diff1 As System.TimeSpan = secondDate - firstDate
		Dim diff2 As String = (secondDate - firstDate).TotalSeconds.ToString()
		'MsgBox(diff2)
		'MsgBox(diff2 / 60)
		Dim actCT As Double = Format(diff2 / 60, "0.00")
		'Format(ListBox2.Items(numOfindex), "0.00")
		'st_time.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
		Dim cnt_btn As Integer = Integer.Parse(MainFrm.cavity.Text)
		'Counter
		count = count + cnt_btn
		_Edit_Up_0.Text = count
		Dim Act As Integer = 0
		If Label6.Text <= 0 Then
			Act = 1
		Else
			Act = Label6.Text
		End If
		If comp_flg = 0 Then
			Dim result_mod As Double = Integer.Parse(Act + 1) Mod Integer.Parse(Label27.Text) 'Integer.Parse(_Edit_Up_0.Text) Mod Integer.Parse(Label27.Text)
			lb_qty_for_box.Text = lb_qty_for_box.Text + cnt_btn
			Dim textp_result As Integer = Label10.Text
			textp_result = Math.Abs(textp_result) - 1
			'MsgBox(textp_result)
			If result_mod = 0 And textp_result <> 0 Then
				If Backoffice_model.check_line_reprint() = "0" Then
					lb_box_count.Text = lb_box_count.Text + 1
					Label_bach.Text = Label_bach.Text + 1
					tag_print()
				Else
					If CDbl(Val(Label27.Text)) = 1 Or CDbl(Val(Label27.Text)) = 999999 Then
					Else
						lb_box_count.Text = lb_box_count.Text + 1
						Label_bach.Text = Label_bach.Text + 1
						tag_print()
					End If
				End If
			End If
			Dim sum_act_total As Integer = Label6.Text + cnt_btn
			Label6.Text = sum_act_total
			Dim sum_prg As Integer = (Label6.Text * 100) / Label8.Text
			'MsgBox(sum_prg)
			If sum_prg > 100 Then
				sum_prg = 100
			End If
			CircularProgressBar1.Text = sum_prg & "%"
			CircularProgressBar1.Value = sum_prg
			Dim use_time As Integer = Label34.Text
			'MsgBox(use_time)
			'Dim Starttime As New DateTime(Label16.Text)     ' 10:25:06 AM
			'Dim EndTime As New DateTime(TimeOfDay())     ' 1:25:06 PM
			'DateDiff(DateInterval.Day, st_time.Text, Now)
			'Dim Result As Long = DateDiff(DateInterval.Day, st_time.Text, Now)
			'MsgBox(st_time.Text)
			Dim dt1 As DateTime = DateTime.Now
			Dim dt2 As DateTime = st_count_ct.Text
			Dim dtspan As TimeSpan = dt1 - dt2
			'MsgBox(("Second: " & dtspan.Seconds))
			'use_time = 1
			'MsgBox(dtspan)
			'MsgBox(dtspan.Minutes)
			Dim actCT_jing As Double = Format((dtspan.Seconds / _Edit_Up_0.Text) + (dtspan.Minutes * 60), "0.00")
			'Label37.Text = actCT_jing
			ListBox1.Items.Add((dtspan.Seconds) + (dtspan.Minutes * 60) + (dtspan.Hours * 3600))
			Dim Total As Double = 0
			Dim Count As Double = 0
			Dim Average As Double = 0
			Dim I As Integer
			For I = 0 To ListBox1.Items.Count - 1
				Total += Val(ListBox1.Items(I))
				Count += 1
			Next
			Average = Total / Count
			'MsgBox(Count)
			If Count = 1 Then
				Try
					Backoffice_model.Tag_seq_rec_sqlite(lb_ref_scan.Text.Substring(0, 10), lb_ref_scan.Text.Substring(10, 3), lb_ref_scan.Text.Substring(13, (lb_ref_scan.Text.Length - 13)), RTrim(lb_ref_scan.Text))
				Catch ex As Exception

				End Try
			End If
			Label37.Text = Format(Average, "0.0")

			Dim start_time As Date = st_count_ct.Text

			st_count_ct.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")


			Dim dt11 As DateTime = DateTime.Now
			Dim dt22 As DateTime = st_time.Text
			Dim dtspan1 As TimeSpan = dt11 - dt22

			'MsgBox(dtspan1.Minutes)

			If (dtspan1.Minutes + (dtspan1.Hours * 60)) >= use_time Then
				Label20.ForeColor = Color.Red
			End If
			Dim temppola As Double = ((dtspan1.Seconds / 60) + (dtspan1.Minutes + (dtspan1.Hours * 60)))
			'MsgBox("Minute diff : " & dtspan1.Minutes)
			'MsgBox("Hour diff : " & (dtspan1.Hours * 60))
			If temppola < 1 Then
				temppola = 1
			End If

			Dim loss_sum As Integer
			Try
				If My.Computer.Network.Ping("192.168.161.101") Then
					Dim LoadSQL = Backoffice_model.get_sum_loss(Trim(wi_no.Text))
					While LoadSQL.Read()
						loss_sum = LoadSQL("sum_loss")
					End While
				Else
					loss_sum = 0
				End If
			Catch ex As Exception
				load_show.Show()
				'loss_sum = 0
			End Try
			Dim sum_prg2 As Integer = (((Label38.Text * _Edit_Up_0.Text) / ((temppola * 60) - loss_sum)) * 100)
			'Dim sum_prg2 As Integer = (((CycleTime.Text * _Edit_Up_0.Text) / temppola) * 100)
			'MsgBox("((" & CycleTime.Text & "*" & _Edit_Up_0.Text & ") /" & temppola & ") * 100")
			'MsgBox(sum_prg2 / cnt_btn)
			sum_prg2 = sum_prg2 / cnt_btn
			'MsgBox(sum_prg2)
			If sum_prg2 > 100 Then
				sum_prg2 = 100
			End If
			'MsgBox(sum_prg2)
			If sum_prg2 <= 49 Then
				CircularProgressBar2.ProgressColor = Color.Red
				CircularProgressBar2.ForeColor = Color.Black
			ElseIf sum_prg2 >= 50 And sum_prg2 <= 79 Then
				CircularProgressBar2.ProgressColor = Color.Chocolate
				CircularProgressBar2.ForeColor = Color.Black
			ElseIf sum_prg2 >= 80 And sum_prg2 <= 100 Then
				CircularProgressBar2.ProgressColor = Color.Green
				CircularProgressBar2.ForeColor = Color.Black
			End If
			Dim avarage_eff As Double = Format(sum_prg2, "0.00")
			lb_sum_prg.Text = avarage_eff
			CircularProgressBar2.Text = sum_prg2 & "%"
			CircularProgressBar2.Value = sum_prg2
			Dim actCT_jingna As Double = Format(dtspan.Seconds + (dtspan.Minutes * 60), "0.00")
			Dim pd As String = MainFrm.Label6.Text
			Dim line_cd As String = MainFrm.Label4.Text
			Dim wi_plan As String = wi_no.Text
			Dim item_cd As String = Label3.Text
			Dim item_name As String = Label12.Text
			Dim staff_no As String = Label29.Text
			Dim seq_no As String = Label22.Text
			Dim prd_qty As Integer = cnt_btn
			Dim end_time As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
			Dim use_timee As Double = actCT_jingna
			Dim tr_status As String = "0"
			Dim number_qty As Integer = Label6.Text
			'Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_timee, tr_status)
			Dim start_time2 As String = start_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			Dim end_time2 As String = end_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			Try
				If My.Computer.Network.Ping("192.168.161.101") Then
					tr_status = "1"
					Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_time, number_qty)
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, number_qty, start_time2, end_time, use_timee, number_qty, tr_status)
				Else
					tr_status = "0"
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, number_qty, start_time2, end_time, use_timee, number_qty, tr_status)
				End If
			Catch ex As Exception
				tr_status = "0"
				Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, number_qty, start_time2, end_time, use_timee, number_qty, tr_status)
			End Try
			Dim sum_diff As Integer = Label8.Text - Label6.Text
			If sum_diff < 0 Then
				Label10.Text = "0"
			Else
				Label10.Text = "-" & sum_diff
			End If
			If sum_diff < 1 Then
				If sum_diff = 0 Then
					lb_box_count.Text = lb_box_count.Text + 1
					'If Backoffice_model.check_line_reprint() = "0" Then
					Label_bach.Text = Label_bach.Text + 1
					tag_print()
					'End If
				Else

				End If
				Me.Enabled = False
				comp_flg = 1
				Finish_work.Show()
				Dim plan_qty As Integer = Label8.Text
				Dim act_qty As Integer = _Edit_Up_0.Text
				Dim shift_prd As String = Label14.Text
				Dim prd_st_datetime As Date = st_time.Text
				Dim prd_end_datetime As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
				Dim lot_no As String = Label18.Text
				Dim comp_flg2 As String = "1"
				Dim transfer_flg As String = "1"
				Dim del_flg As String = "0"
				Dim prd_flg As String = "1"
				Dim close_lot_flg As String = "1"
				Dim avarage_act_prd_time As Double = Average
				Try
					act_qty = LB_COUNTER_SEQ.Text 'Working_Pro._Edit_Up_0.Text
					LB_COUNTER_SHIP.Text = 0
					LB_COUNTER_SEQ.Text = 0
				Catch ex As Exception
					act_qty = 0
				End Try
				Try
					If My.Computer.Network.Ping("192.168.161.101") Then
						transfer_flg = "0"
						Backoffice_model.Insert_prd_close_lot(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
						Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
						'Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_timee, number_qty)
						Backoffice_model.work_complete(wi_plan)
						Dim temp_co_emp As Integer = List_Emp.ListView1.Items.Count
						Dim emp_cd As String
						For I = 0 To temp_co_emp - 1
							emp_cd = List_Emp.ListView1.Items(I).Text
							Backoffice_model.Insert_emp_cd(wi_plan, emp_cd, seq_no)
							'MsgBox(List_Emp.ListView1.Items(i).Text)
						Next
						'MsgBox("Ins completed")
					Else
						transfer_flg = "0"
						Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
						'Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time, end_time, use_timee, tr_status)
						'MsgBox("Ins incompleted1")
					End If
				Catch ex As Exception
					'MsgBox("Ins incompleted2")
					transfer_flg = "0"
					Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
					'Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time, end_time, use_timee, tr_status)
				End Try
			End If
		End If
	End Sub

	Private Sub Label18_Click(sender As Object, e As EventArgs) Handles Label18.Click

	End Sub

	Private Sub Panel2_Paint(sender As Object, e As PaintEventArgs) Handles Panel2.Paint

	End Sub
	Private Sub Button3_Click_1(sender As Object, e As EventArgs) Handles Button3.Click
		Dim i As Short
		Dim BitNo As Short
		Dim Kind As Short
		Dim Tim As Integer


		For i = 0 To 1
			UpCount(i) = 0
			DownCount(i) = 0
		Next i

		Tim = 100
		Kind = DIO_TRG_RISE
		For BitNo = 0 To 1

			If Check(BitNo).CheckState = 1 Then
				Ret = DioNotifyTrg(Id, BitNo, Kind, Tim, Me.Handle.ToInt32)
				If (Ret <> DIO_ERR_SUCCESS) Then
					'MsgBox("Connect success")
					Exit For
				End If

			Else
				Ret = DioStopNotifyTrg(Id, BitNo)
				If (Ret <> DIO_ERR_SUCCESS) Then
					'MsgBox("Connect Failed")
					Exit For
				End If
			End If
		Next BitNo

		Call DioGetErrorString(Ret, szError)
	End Sub

	'Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
	'    Dim szDeviceName As String
	'    szDeviceName = Edit_DeviceName.Text
	'    Ret = DioInit(szDeviceName, Id)
	'    Call DioGetErrorString(Ret, szError)
	'End Sub

	Function HiLoWord(ByRef Number As Integer, ByRef HiLo As Short) As Integer

		Dim Hi As Integer
		Dim Lo As Integer
		Dim StrData As String
		Dim i As Short

		StrData = Hex(Number)
		If Len(StrData) < 2 Then
			For i = 1 To 2 - Len(StrData)
				StrData = "0" & StrData
			Next i
		End If

		Lo = Val("&h" & VB.Right(StrData, 4))
		'Hi = Val("&h" & VB.Left(StrData, 4))
		If HiLo = 0 Then
			HiLoWord = Lo
		ElseIf HiLo = 1 Then
			HiLoWord = Hi
		End If

	End Function

	Public count As String = 0
	'_Edit_Up_0.Text = 0
	Protected Overrides Sub WndProc(ByRef m As Message)
		If check_bull = 0 Then
			Dim BitNo As Short
			Dim Id As Short
			Dim Kind As Short
			'MsgBox(m.Msg)
			'MsgBox(DIOM_TRIGGER)
			'MsgBox("test")
			'--------------------------------------
			' trigger Message 
			'--------------------------------------
			If start_flg = 1 Then
				If m.Msg = DIOM_TRIGGER Then
					Timer3.Enabled = True
					Try
						If My.Computer.Network.Ping("192.168.161.101") Then
							Backoffice_model.updated_data_to_dbsvr()
						End If
					Catch ex As Exception

					End Try
					Dim yearNow As Integer = DateTime.Now.ToString("yyyy")
					Dim monthNow As Integer = DateTime.Now.ToString("MM")
					Dim dayNow As Integer = DateTime.Now.ToString("dd")
					Dim hourNow As Integer = DateTime.Now.ToString("HH")
					Dim minNow As Integer = DateTime.Now.ToString("mm")
					Dim secNow As Integer = DateTime.Now.ToString("ss")
					'MsgBox(yearNow & monthNow & dayNow & hourNow & minNow & secNow)
					Dim yearSt As Integer = st_time.Text.Substring(0, 4)
					Dim monthSt As Integer = st_time.Text.Substring(5, 2)
					Dim daySt As Integer = st_time.Text.Substring(8, 2)
					Dim hourSt As Integer = st_time.Text.Substring(11, 2)
					Dim minSt As Integer = st_time.Text.Substring(14, 2)
					Dim secSt As Integer = st_time.Text.Substring(17, 2)
					'MsgBox(yearSt & minthSt & daySt & hourSt & minSt & secSt)
					Dim firstDate As New System.DateTime(yearSt, monthSt, daySt, hourSt, minSt, secSt)
					Dim secondDate As New System.DateTime(yearNow, monthNow, dayNow, hourNow, minNow, secNow)
					Dim diff As System.TimeSpan = secondDate.Subtract(firstDate)
					Dim diff1 As System.TimeSpan = secondDate - firstDate
					Dim diff2 As String = (secondDate - firstDate).TotalSeconds.ToString()
					'MsgBox(diff2)
					'MsgBox(diff2 / 60)
					Dim actCT As Double = Format(diff2 / 60, "0.00")
					'Format(ListBox2.Items(numOfindex), "0.00")
					'st_time.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
					'Counter
					Dim cnt_btn As Integer = Integer.Parse(MainFrm.cavity.Text)
					'Counter
					count = count + cnt_btn
					_Edit_Up_0.Text = count
					Dim Act As Integer = 0
					Dim action_plus As Integer = 0
					If Label6.Text <= 0 Then ' condition  
						Act = 1
						action_plus = 0
					Else
						Act = Label6.Text
						action_plus = 1
					End If
					If comp_flg = 0 Then
						'Dim result_mod As Double = Integer.Parse(_Edit_Up_0.Text) Mod Integer.Parse(Label27.Text)
						Dim result_mod As Double = Integer.Parse(Act + action_plus) Mod Integer.Parse(Label27.Text) 'Integer.Parse(_Edit_Up_0.Text) Mod Integer.Parse(Label27.Text)
						lb_qty_for_box.Text = lb_qty_for_box.Text + cnt_btn
						Dim textp_result As Integer = Label10.Text
						textp_result = Math.Abs(textp_result) - 1
						'MsgBox(textp_result)
						Dim sum_act_total As Integer = Label6.Text + cnt_btn
						Label6.Text = sum_act_total
						LB_COUNTER_SHIP.Text += 1
						LB_COUNTER_SEQ.Text += 1
						If check_format_tag = "1" Then ' for tag_type = '2' and tag_issue_flg = '2'  OR K1M183
							lb_box_count.Text = lb_box_count.Text + 1
							print_back.PrintDocument2.Print()
							If result_mod = "0" Then
								Label_bach.Text += 1
								tag_print()
							End If
						Else
							If result_mod = 0 And textp_result <> 0 Then
								If Backoffice_model.check_line_reprint() = "0" Then
									lb_box_count.Text = lb_box_count.Text + 1
									Label_bach.Text = Label_bach.Text + 1
									tag_print()
								Else
									If CDbl(Val(Label27.Text)) = 1 Or CDbl(Val(Label27.Text)) = 999999 Then
									Else
										lb_box_count.Text = lb_box_count.Text + 1
										Label_bach.Text = Label_bach.Text + 1
										tag_print()
									End If
								End If
							End If
						End If


						Dim sum_prg As Integer = (Label6.Text * 100) / Label8.Text
						'MsgBox(sum_prg)

						If sum_prg > 100 Then
							sum_prg = 100
						ElseIf sum_prg < 0 Then
							sum_prg = 0
						End If

						CircularProgressBar1.Text = sum_prg & "%"
						CircularProgressBar1.Value = sum_prg


						Dim use_time As Integer = Label34.Text


						Dim dt1 As DateTime = DateTime.Now
						Dim dt2 As DateTime = st_count_ct.Text
						Dim dtspan As TimeSpan = dt1 - dt2
						'MsgBox(("Second: " & dtspan.Seconds))
						'use_time = 1
						'MsgBox(dtspan)
						'MsgBox(dtspan.Minutes)

						Dim actCT_jing As Double = Format((dtspan.Seconds / _Edit_Up_0.Text) + (dtspan.Minutes * 60), "0.00")
						'Label37.Text = actCT_jing

						ListBox1.Items.Add((dtspan.Seconds) + (dtspan.Minutes * 60) + (dtspan.Hours * 3600))

						Dim Total As Double = 0
						Dim Count As Double = 0
						Dim Average As Double = 0
						Dim I As Integer

						For I = 0 To ListBox1.Items.Count - 1
							Total += Val(ListBox1.Items(I))
							Count += 1
						Next
						Average = Total / Count
						'MsgBox(Count)

						If Count = 1 Then
							'MsgBox(lb_ref_scan.Text)

							Try
								Backoffice_model.Tag_seq_rec_sqlite(lb_ref_scan.Text.Substring(0, 10), lb_ref_scan.Text.Substring(10, 3), lb_ref_scan.Text.Substring(13, (lb_ref_scan.Text.Length - 13)), RTrim(lb_ref_scan.Text))
							Catch ex As Exception

							End Try
						End If
						Label37.Text = Format(Average, "0.0")
						Dim start_time As Date = st_count_ct.Text
						st_count_ct.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")

						Dim dt11 As DateTime = DateTime.Now
						Dim dt22 As DateTime = st_time.Text
						Dim dtspan1 As TimeSpan = dt11 - dt22

						'MsgBox(dtspan1.Minutes)

						If (dtspan1.Minutes + (dtspan1.Hours * 60)) >= use_time Then
							Label20.ForeColor = Color.Red
						End If

						Dim temppola As Double = ((dtspan1.Seconds / 60) + (dtspan1.Minutes + (dtspan1.Hours * 60)))
						'MsgBox("Minute diff : " & dtspan1.Minutes)
						'MsgBox("Hour diff : " & (dtspan1.Hours * 60))
						If temppola < 1 Then
							temppola = 1
						End If
						Dim loss_sum As Integer
						Try
							If My.Computer.Network.Ping("192.168.161.101") Then
								Dim LoadSQL = Backoffice_model.get_sum_loss(Trim(wi_no.Text))
								While LoadSQL.Read()
									loss_sum = LoadSQL("sum_loss")
								End While
							Else
								loss_sum = 0
							End If
						Catch ex As Exception
							'load_show.Show()
							'loss_sum = 0
						End Try
						Dim sum_prg2 As Integer = (((Label38.Text * _Edit_Up_0.Text) / ((temppola * 60) - loss_sum)) * 100)
						'Dim sum_prg2 As Integer = (((CycleTime.Text * _Edit_Up_0.Text) / temppola) * 100)
						'MsgBox("((" & CycleTime.Text & "*" & _Edit_Up_0.Text & ") /" & temppola & ") * 100")
						'MsgBox(sum_prg2 / cnt_btn)

						sum_prg2 = sum_prg2 / cnt_btn
						'MsgBox(sum_prg2)

						If sum_prg2 > 100 Then
							sum_prg2 = 100
						ElseIf sum_prg2 < 0 Then
							sum_prg2 = 0
						End If
						'MsgBox(sum_prg2)
						If sum_prg2 <= 49 Then
							CircularProgressBar2.ProgressColor = Color.Red
							CircularProgressBar2.ForeColor = Color.Black
						ElseIf sum_prg2 >= 50 And sum_prg2 <= 79 Then
							CircularProgressBar2.ProgressColor = Color.Chocolate
							CircularProgressBar2.ForeColor = Color.Black
						ElseIf sum_prg2 >= 80 And sum_prg2 <= 100 Then
							CircularProgressBar2.ProgressColor = Color.Green
							CircularProgressBar2.ForeColor = Color.Black
						End If
						Dim avarage_eff As Double = Format(sum_prg2, "0.00")
						lb_sum_prg.Text = avarage_eff
						CircularProgressBar2.Text = sum_prg2 & "%"
						CircularProgressBar2.Value = sum_prg2
						Dim actCT_jingna As Double = Format(dtspan.Seconds + (dtspan.Minutes * 60), "0.00")
						Dim pd As String = MainFrm.Label6.Text
						Dim line_cd As String = MainFrm.Label4.Text
						Dim wi_plan As String = wi_no.Text
						Dim item_cd As String = Label3.Text
						Dim item_name As String = Label12.Text
						Dim staff_no As String = Label29.Text
						Dim seq_no As String = Label22.Text
						Dim prd_qty As Integer = cnt_btn
						Dim end_time As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
						Dim use_timee As Double = actCT_jingna
						Dim tr_status As String = "0"
						Dim number_qty As Integer = Label6.Text
						'Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_timee, tr_status)
						Dim start_time2 As String = start_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
						Dim end_time2 As String = end_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)

						Try
							If My.Computer.Network.Ping("192.168.161.101") Then
								tr_status = "1"
								Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
								Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_timee, number_qty)
								'MsgBox("Ping completed")
							Else
								tr_status = "0"
								Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
								'MsgBox("Ping incompleted")
							End If
						Catch ex As Exception
							tr_status = "0"
							Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
						End Try
						'Label16.Text.ToString("H : mm") - Now.ToString("H : mm")

						Dim sum_diff As Integer = Label8.Text - Label6.Text
						Label10.Text = "-" & sum_diff
						If sum_diff = 0 Then
							Me.Enabled = False
							comp_flg = 1
							'If result_mod <> "0" Then ' New
							'tag_print()
							'End If
							Finish_work.Show() ' เกี่ยว
						End If
						'MsgBox(sum_diff)
						If sum_diff < 1 Then
							If sum_diff = 0 Then
								If check_format_tag = "1" Then ' for tag_type = '2' and tag_issue_flg = '2'  OR K1M183
									lb_box_count.Text = lb_box_count.Text + 1
									print_back.PrintDocument2.Print()
									If result_mod = "0" Then
										Label_bach.Text += 1
										tag_print()
									End If
								Else
									lb_box_count.Text = lb_box_count.Text + 1
									'If Backoffice_model.check_line_reprint() = "0" Then
									Label_bach.Text = Label_bach.Text + 1
									tag_print()
									'End If
								End If
							Else
							End If
							Me.Enabled = False
							comp_flg = 1
							Finish_work.Show() ' เกี่ยว
							Dim plan_qty As Integer = Label8.Text
							'Dim act_qty As Integer = _Edit_Up_0.Text
							Dim shift_prd As String = Label14.Text
							Dim prd_st_datetime As Date = st_time.Text
							Dim prd_end_datetime As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
							Dim lot_no As String = Label18.Text
							Dim comp_flg2 As String = "1"
							Dim transfer_flg As String = "1"
							Dim del_flg As String = "0"
							Dim prd_flg As String = "1"
							Dim close_lot_flg As String = "1"
							Dim avarage_act_prd_time As Double = Average
							Try
								act_qty = LB_COUNTER_SEQ.Text 'Working_Pro._Edit_Up_0.Text
								LB_COUNTER_SHIP.Text = 0
								LB_COUNTER_SEQ.Text = 0
							Catch ex As Exception
								act_qty = 0
							End Try
							Try
								If My.Computer.Network.Ping("192.168.161.101") Then
									transfer_flg = "1"
									Backoffice_model.Insert_prd_close_lot(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
									Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
									'Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_timee, number_qty)
									Backoffice_model.work_complete(wi_plan)
									Dim temp_co_emp As Integer = List_Emp.ListView1.Items.Count
									Dim emp_cd As String
									For I = 0 To temp_co_emp - 1
										emp_cd = List_Emp.ListView1.Items(I).Text
										Backoffice_model.Insert_emp_cd(wi_plan, emp_cd, seq_no)
										'MsgBox(List_Emp.ListView1.Items(i).Text)
									Next
									'MsgBox("Ins completed")
								Else
									transfer_flg = "0"
									Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
									'Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time, end_time, use_timee, tr_status)

									'MsgBox("Ins incompleted1")
								End If
							Catch ex As Exception
								'MsgBox("Ins incompleted2")
								transfer_flg = "0"
								Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
								'Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time, end_time, use_timee, tr_status)
							End Try
						End If
					End If
					'--------------------------------------
					' Expression
					'--------------------------------------
					'Edit_Up(BitNo).Text = CStr(UpCount(BitNo))
					'Edit_Down(BitNo).Text = CStr(DownCount(BitNo))
				End If
			End If
		Else

		End If
		MyBase.WndProc(m)
	End Sub

	Private Sub Label37_Click(sender As Object, e As EventArgs) Handles Label37.Click

	End Sub

	Private Sub btn_closelot_Click(sender As Object, e As EventArgs) Handles btn_closelot.Click
		'MsgBox("Please confirm")

		Me.Enabled = False
		Close_lot_cfm.Show()
	End Sub
	Public Function check_tagprint()
		Dim result_snp As Integer = CDbl(Val(Label6.Text)) Mod CDbl(Val(Label27.Text))
		Dim flg_control As Integer = 0
		If check_format_tag = "1" Then ' for tag_type = '2' and tag_issue_flg = '2'  OR K1M183
			flg_control = 1
		Else
			If result_snp = "0" Then
				flg_control = 1
			End If
		End If
		Return flg_control
	End Function
	Private Sub PrintDocument1_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument1.PrintPage
		Dim aPen = New Pen(Color.Black)
		aPen.Width = 2.0F
		'MsgBox(Label10.Text)
		'vertical
		e.Graphics.DrawLine(aPen, 150, 10, 150, 290)
		e.Graphics.DrawLine(aPen, 300, 175, 300, 290)
		e.Graphics.DrawLine(aPen, 590, 10, 590, 175)
		e.Graphics.DrawLine(aPen, 410, 120, 410, 235)
		e.Graphics.DrawLine(aPen, 410, 175, 410, 235)
		e.Graphics.DrawLine(aPen, 225, 175, 225, 235)
		e.Graphics.DrawLine(aPen, 490, 10, 490, 65)
		e.Graphics.DrawLine(aPen, 520, 175, 520, 290)
		e.Graphics.DrawLine(aPen, 610, 175, 610, 290)
		e.Graphics.DrawLine(aPen, 700, 10, 700, 290)
		'Horizontal
		e.Graphics.DrawLine(aPen, 150, 11, 700, 11)
		e.Graphics.DrawLine(aPen, 150, 65, 590, 65)
		e.Graphics.DrawLine(aPen, 150, 120, 700, 120)
		e.Graphics.DrawLine(aPen, 150, 175, 700, 175)
		e.Graphics.DrawLine(aPen, 150, 235, 610, 235)
		e.Graphics.DrawLine(aPen, 150, 289, 700, 289)
		'TAG LAYOUT
		e.Graphics.DrawString("PART NO.", lb_font1.Font, Brushes.Black, 152, 13)
		e.Graphics.DrawString(Label3.Text, lb_font2.Font, Brushes.Black, 152, 25)
		e.Graphics.DrawString("QTY.", lb_font1.Font, Brushes.Black, 492, 13)
		Dim result_snp As Integer = CDbl(Val(Label6.Text)) Mod CDbl(Val(Label27.Text))
		Dim status_tag As String = "[ Incomplete Tag ]"
		'MsgBox("result_snp = " & result_snp)
		If Backoffice_model.check_line_reprint() = "0" Then
			If result_snp = "0" Then
				result_snp = Label27.Text
				status_tag = " "
			End If
		Else
			If CDbl(Val(Label27.Text)) = 1 Or CDbl(Val(Label27.Text)) = 999999 Then
				'If result_snp = "0" Then
				result_snp = LB_COUNTER_SEQ.Text
				status_tag = " "
				'End If
			Else
				'MsgBox("result_snp ===>" & result_snp)
				'MsgBox(" CDbl(Val(Label6.Text)) Mod CDbl(Val(Label27.Text)) ===>" & CDbl(Val(Label6.Text)) Mod CDbl(Val(Label27.Text)))
				If result_snp = "0" Then
					'MsgBox("IF")
					result_snp = Label27.Text
					status_tag = " "
				Else
					'MsgBox("ELSE PRINT")
					'MsgBox("LB_COUNTER_SEQ.Text = " & LB_COUNTER_SEQ.Text)
					result_snp = CDbl(Val(Label6.Text)) Mod CDbl(Val(Label27.Text)) 'LB_COUNTER_SEQ.Text 
					status_tag = "[ Incomplete Tag ]"
				End If
			End If
		End If
		e.Graphics.DrawString(result_snp, lb_font2.Font, Brushes.Black, 505, 25)
		e.Graphics.DrawString("PART NAME", lb_font1.Font, Brushes.Black, 152, 67)
		e.Graphics.DrawString(Label12.Text, lb_font2.Font, Brushes.Black, 152, 79)
		e.Graphics.DrawString("MODEL", lb_font1.Font, Brushes.Black, 152, 123)
		e.Graphics.DrawString(lb_model.Text, lb_font4.Font, Brushes.Black, 152, 141)
		e.Graphics.DrawString("NEXT PROCESS", lb_font1.Font, Brushes.Black, 412, 123)
		e.Graphics.DrawString(Backoffice_model.NEXT_PROCESS, lb_font4.Font, Brushes.Black, 414, 141)
		Dim plan_seq As String
		Dim num_char_seq As Integer
		num_char_seq = Label22.Text.Length
		If num_char_seq = 1 Then
			plan_seq = "00" & Label22.Text
		ElseIf num_char_seq = 2 Then
			plan_seq = "0" & Label22.Text
		Else
			plan_seq = Label22.Text
		End If

		Dim the_box_seq As String
		Dim num_box_seq As Integer
		num_box_seq = lb_box_count.Text.Length
		If num_box_seq = 1 Then
			the_box_seq = "00" & lb_box_count.Text
		ElseIf num_box_seq = 2 Then
			the_box_seq = "0" & lb_box_count.Text
		Else
			the_box_seq = lb_box_count.Text
		End If
		e.Graphics.DrawString("LOCATION", lb_font1.Font, Brushes.Black, 592, 123)
		e.Graphics.DrawString(lb_location.Text, lb_font4.Font, Brushes.Black, 596, 141)
		e.Graphics.DrawString("SHIFT", lb_font1.Font, Brushes.Black, 152, 178)
		e.Graphics.DrawString(Label14.Text, lb_font2.Font, Brushes.Black, 170, 190)
		e.Graphics.DrawString("PRO. SEQ.", lb_font1.Font, Brushes.Black, 227, 178)
		e.Graphics.DrawString(plan_seq, lb_font2.Font, Brushes.Black, 231, 190)
		e.Graphics.DrawString("BOX NO.", lb_font1.Font, Brushes.Black, 302, 178)
		e.Graphics.DrawString(the_box_seq, lb_font2.Font, Brushes.Black, 320, 190)
		e.Graphics.DrawString("ACTUAL DATE", lb_font1.Font, Brushes.Black, 412, 178)
		e.Graphics.DrawString(DateTime.Now.ToString("dd/MM/yyyy"), lb_font5.Font, Brushes.Black, 412, 196)
		Dim plan_cd As String
		Dim factory_cd As String
		If MainFrm.Label6.Text = "K2PD06" Then
			factory_cd = "Phase8"
			plan_cd = "52"
		Else
			factory_cd = "Phase10"
			plan_cd = "51"
		End If
		e.Graphics.DrawString("FACTORY", lb_font1.Font, Brushes.Black, 522, 178)
		e.Graphics.DrawString(factory_cd, lb_font5.Font, Brushes.Black, 522, 196)
		e.Graphics.DrawString("INFO.", lb_font1.Font, Brushes.Black, 612, 178)
		'e.Graphics.DrawString(Label14.Text, lb_font2.Font, Brushes.Black, 626, 190)
		e.Graphics.DrawString("LINE", lb_font1.Font, Brushes.Black, 152, 238)
		e.Graphics.DrawString(Label24.Text, lb_font2.Font, Brushes.Black, 152, 250)
		Dim result_plan_date As String = ""
		Try
			'Dim d As Date = Date.ParseExact(lb_dlv_date.Text, "dd/MM/yy", CultureInfo.InvariantCulture)
			'lb_dlv_date.Text = d.ToString("dd/MM/yyyy")
			Dim da As Date = Date.ParseExact(lb_dlv_date.Text.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture)
			'lb_dlv_date.Text = da.ToString("dd/MM/yyyy")
			result_plan_date = da.ToString("dd/MM/yyyy")
		Catch ex As Exception
			'Dim d As Date = Date.ParseExact(lb_dlv_date.Text, "dd/MM/yy", CultureInfo.InvariantCulture)
			'lb_dlv_date.Text = lb_dlv_date.Text
			result_plan_date = lb_dlv_date.Text
		End Try
		e.Graphics.DrawString("PLAN DATE", lb_font1.Font, Brushes.Black, 302, 238)
		e.Graphics.DrawString(result_plan_date, lb_font6.Font, Brushes.Black, 334, 250)
		e.Graphics.DrawString("LOT NO.", lb_font1.Font, Brushes.Black, 522, 238)
		e.Graphics.DrawString(Label18.Text, lb_font2.Font, Brushes.Black, 522, 250)
		'e.Graphics.DrawString("PROD. SEQ.", lb_font1.Font, Brushes.Black, 612, 238)
		'e.Graphics.DrawString(plan_seq, lb_font2.Font, Brushes.Black, 622, 250)
		e.Graphics.DrawString("TBKK", lb_font2.Font, Brushes.Black, 15, 13)
		e.Graphics.DrawString("(Thailand) Co., Ltd.", lb_font1.Font, Brushes.Black, 15, 45)
		e.Graphics.DrawString("Shop floor system", lb_font3.Font, Brushes.Black, 15, 73)
		e.Graphics.DrawString("(New FA system)", lb_font3.Font, Brushes.Black, 15, 85)
		e.Graphics.DrawString("WI : " & wi_no.Text, lb_font3.Font, Brushes.Black, 15, 123)
		Dim prdtype As String
		If lb_prd_type.Text = "10" Then
			prdtype = "PART TYPE : FG"
		ElseIf lb_prd_type.Text = "40" Then
			prdtype = "PART TYPE : Parts"
		Else
			prdtype = "PART TYPE : FW"
		End If
		e.Graphics.DrawString(prdtype, lb_font3.Font, Brushes.Black, 15, 136)
		e.Graphics.DrawString(status_tag, lb_font3.Font, Brushes.Black, 15, 166)
		Dim iden_cd As String
		If MainFrm.Label6.Text = "K1PD01" Then
			iden_cd = "GA"
		Else
			iden_cd = "GB"
		End If
		'MsgBox("Y :" & lb_dlv_date.Text.Substring(6, 4) & " M :" & lb_dlv_date.Text.Substring(3, 2) & " D :" & lb_dlv_date.Text.Substring(0, 2))
		'MsgBox(lb_dlv_date.Text)
		'Dim plan_date As String = lb_dlv_date.Text.Substring(6, 4) & lb_dlv_date.Text.Substring(3, 2) & lb_dlv_date.Text.Substring(0, 2)
		Dim plan_date As String = result_plan_date.Substring(6, 4) & result_plan_date.Substring(3, 2) & result_plan_date.Substring(0, 2)
		'Dim dateConv As Date = lb_dlv_date.Text
		'plan_date = Format(dateConv, "yyyyMMdd")
		'plan_date = dateConv.ToString("yyyyMMdd")
		Dim part_no_res1 As String
		Dim part_no_res As String
		Dim part_numm As Integer = 0
		For part_numm = Label3.Text.Length To 24
			part_no_res = part_no_res & " "
		Next part_numm
		part_no_res1 = Label3.Text & part_no_res
		Dim act_date As String
		Dim actdateConv As Date = DateTime.Now.ToString("dd/MM/yyyy")
		act_date = Format(actdateConv, "yyyyMMdd")
		Dim qty_num As String
		Dim num_char_qty As Integer
		num_char_qty = Len(Trim(result_snp)) 'Label27.Text.Length 'lb_qty_for_box.Text.Length
		If num_char_qty = 1 Then
			qty_num = "     " & result_snp 'lb_qty_for_box.Text
		ElseIf num_char_qty = 2 Then
			qty_num = "    " & result_snp 'lb_qty_for_box.Text
		ElseIf num_char_qty = 3 Then
			qty_num = "   " & result_snp 'lb_qty_for_box.Text
		ElseIf num_char_qty = 4 Then
			qty_num = "  " & result_snp 'lb_qty_for_box.Text
		ElseIf num_char_qty = 5 Then
			qty_num = " " & result_snp 'lb_qty_for_box.Text
		Else
			qty_num = result_snp 'lb_qty_for_box.Text
		End If
		Dim cus_part_no As String = "                         "
		Dim box_no As String
		Dim num_char_box As Integer
		num_char_box = lb_box_count.Text.Length
		If num_char_box = 1 Then
			box_no = "00" & lb_box_count.Text
		ElseIf num_char_box = 2 Then
			box_no = "0" & lb_box_count.Text
		Else
			box_no = lb_box_count.Text
		End If
		Dim qr_detailss As String
		Try
			PictureBox1.Image = QR_Generator.Encode(iden_cd & Label24.Text & plan_date & plan_seq & part_no_res1 & act_date & qty_num & Label18.Text & cus_part_no & act_date & plan_seq & plan_cd & box_no)
			e.Graphics.DrawImage(PictureBox1.Image, 597, 17, 95, 95)
			e.Graphics.DrawImage(PictureBox1.Image, 31, 190, 95, 95)
			PictureBox8.Image = QR_Generator.Encode(iden_cd & Label24.Text & plan_date & plan_seq & part_no_res1 & act_date & qty_num & Label18.Text & cus_part_no & act_date & plan_seq & plan_cd & box_no)
			e.Graphics.DrawImage(PictureBox8.Image, 620, 199, 70, 70)
			qr_detailss = iden_cd & Label24.Text & plan_date & plan_seq & part_no_res1 & act_date & qty_num & Label18.Text & cus_part_no & act_date & plan_seq & plan_cd & box_no
		Catch ex As Exception
			qr_detailss = ""
		End Try
		'MsgBox(box_no)
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.Insert_tag_print(wi_no.Text, qr_detailss, box_no, 1, plan_seq, Label14.Text, check_tagprint(), Label3.Text)
				'MsgBox("Ping completed")
			Else
				'MsgBox("Ping incompleted")
			End If
		Catch ex As Exception
			'tr_status = "0"
			' Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_timee, tr_status)
		End Try
		'lb_box_count.Text = lb_box_count.Text + 1
		lb_qty_for_box.Text = "0"
	End Sub
	Public Shared Function tag_print()
		Dim api = New api()
		Dim check_tag_type = api.Load_data("http://192.168.161.207/API_NEW_FA/GET_DATA_NEW_FA/GET_LINE_TYPE?line_cd=" & MainFrm.Label4.Text)
		If check_tag_type = "1" Then

			Working_Pro.PrintDocument1.Print()
		ElseIf check_tag_type = "2" Then
			Backoffice_model.flg_cat_layout_line = "2"
			print_back.print()
		End If
	End Function

	Public Shared Function tag_print_incomplete()
		Working_Pro.PrintDocument2.Print()
	End Function

	Private Sub Label43_Click(sender As Object, e As EventArgs) Handles Label43.Click

	End Sub

	Private Sub Label46_Click(sender As Object, e As EventArgs) Handles lb_loss_status.Click

	End Sub

	'Print incomplete tag
	Private Sub PrintDocument2_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PrintDocument2.PrintPage
		Dim aPen = New Pen(Color.Black)
		aPen.Width = 2.0F
		'e.Graphics.DrawString("TBKK", lb_font2.Font, Brushes.Black, 152, 50)
		'e.Graphics.DrawString("(Thailand) Co., Ltd.", lb_font1.Font, Brushes.Black, 152, 80)

		'e.Graphics.DrawString("Shopfloor System", lb_font1.Font, Brushes.Black, 152, 110)
		'e.Graphics.DrawString("(New FA system)", lb_font1.Font, Brushes.Black, 152, 122)

		'e.Graphics.DrawString("TBKK", lb_font2.Font, Brushes.Black, 15, 13)
		'e.Graphics.DrawString("(Thailand) Co., Ltd.", lb_font1.Font, Brushes.Black, 15, 45)

		e.Graphics.DrawString("TBKK", lb_font2.Font, Brushes.Black, 15, 13)
		e.Graphics.DrawString("(Thailand) Co., Ltd.", lb_font3.Font, Brushes.Black, 15, 44)

		e.Graphics.DrawString("Shop floor system", lb_font3.Font, Brushes.Black, 15, 68)
		e.Graphics.DrawString("(New FA system)", lb_font3.Font, Brushes.Black, 15, 80)

		e.Graphics.DrawString("INCOMPLETE TAG", lb_font2.Font, Brushes.Black, 242, 5)
		e.Graphics.DrawLine(aPen, 15, 150, 315, 150)

		e.Graphics.DrawLine(aPen, 315, 96, 315, 112)
		e.Graphics.DrawLine(aPen, 315, 183, 315, 199)

		e.Graphics.DrawLine(aPen, 420, 96, 420, 112)
		e.Graphics.DrawLine(aPen, 420, 183, 420, 199)

		e.Graphics.DrawLine(aPen, 314, 96, 333, 96)
		e.Graphics.DrawLine(aPen, 402, 96, 420, 96)

		e.Graphics.DrawLine(aPen, 314, 199, 333, 199)
		e.Graphics.DrawLine(aPen, 402, 199, 420, 199)

		e.Graphics.DrawString(Label24.Text, lb_font2.Font, Brushes.Black, 310, 39)

		e.Graphics.DrawString("Date: " & DateTime.Now.ToString("dd/MM/yyyy"), lb_font3.Font, Brushes.Black, 307, 72)

		e.Graphics.DrawString("PLAN :" & Label8.Text, lb_font4.Font, Brushes.Black, 475, 60)
		e.Graphics.DrawString("COMPLETED :" & Label6.Text, lb_font4.Font, Brushes.Black, 475, 80)
		e.Graphics.DrawString("REMAINING :" & Label10.Text.Substring(1), lb_font4.Font, Brushes.Black, 475, 100)

		Dim plan_seq As String
		Dim num_char_seq As Integer
		num_char_seq = Label22.Text.Length
		If num_char_seq = 1 Then
			plan_seq = "00" & Label22.Text
		ElseIf num_char_seq = 2 Then
			plan_seq = "0" & Label22.Text
		Else
			plan_seq = Label22.Text
		End If


		Try
			PictureBox9.Image = QR_Generator.Encode(wi_no.Text & plan_seq & lb_qty_for_box.Text)
			e.Graphics.DrawImage(PictureBox9.Image, 320, 100, 95, 95)

			PictureBox8.Image = QR_Generator.Encode(wi_no.Text & plan_seq & lb_qty_for_box.Text)
			e.Graphics.DrawImage(PictureBox8.Image, 655, 2, 50, 50)


		Catch ex As Exception

		End Try

		e.Graphics.DrawLine(aPen, 418, 150, 702, 150)

		e.Graphics.DrawString("PART No.:" & Label3.Text, lb_font4.Font, Brushes.Black, 15, 152)
		e.Graphics.DrawString("PART NAME:" & Label12.Text, lb_font4.Font, Brushes.Black, 15, 172)
		e.Graphics.DrawString("MODEL:" & lb_model.Text, lb_font4.Font, Brushes.Black, 15, 192)
		e.Graphics.DrawString("WI PLAN:" & wi_no.Text, lb_font4.Font, Brushes.Black, 15, 212)
		e.Graphics.DrawString("QTY.:" & lb_qty_for_box.Text, lb_font4.Font, Brushes.Black, 15, 232)
		e.Graphics.DrawString("LOT No.:" & Label18.Text, lb_font4.Font, Brushes.Black, 15, 252)
		Dim plan_cd As String
		Dim factory_cd As String
		If MainFrm.Label6.Text = "K2PD06" Then
			factory_cd = "8"
			plan_cd = "52"
		Else
			factory_cd = "10"
			plan_cd = "51"
		End If
		Dim prdtype As String
		If lb_prd_type.Text = "10" Then
			prdtype = "FG"
		ElseIf lb_prd_type.Text = "40" Then
			prdtype = "Parts"
		Else
			prdtype = "FW"
		End If
		e.Graphics.DrawString("LOCATION:" & lb_location.Text, lb_font4.Font, Brushes.Black, 430, 152)
		e.Graphics.DrawString("PHASE:" & factory_cd, lb_font4.Font, Brushes.Black, 430, 172)
		e.Graphics.DrawString("PART TYPE:" & prdtype, lb_font4.Font, Brushes.Black, 430, 192)
		e.Graphics.DrawString("SHIFT:" & Label14.Text, lb_font4.Font, Brushes.Black, 430, 212)
		e.Graphics.DrawString("SNP:" & Label27.Text, lb_font4.Font, Brushes.Black, 430, 232)
		e.Graphics.DrawString("SEQ:" & plan_seq, lb_font4.Font, Brushes.Black, 430, 252)
		e.Graphics.DrawString("Use @ TBKK(Thailand) Co., Ltd.", lb_font1.Font, Brushes.Black, 525, 282)
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.Insert_tag_print(wi_no.Text, qr_detailss, box_no, 1, plan_seq, Label14.Text, check_tagprint(), Label3.Text)
				'MsgBox("Ping completed")
			Else
				'MsgBox("Ping incompleted")
			End If
		Catch ex As Exception
			'tr_status = "0"
			' Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_timee, tr_status)
		End Try
	End Sub
	Public Sub manage_print()
		Dim result_add As Integer = CDbl(Val(lb_ins_qty.Text)) + CDbl(Val(Label6.Text))
		Dim loop_check As Integer = result_add / CDbl(Val(Label27.Text))
		Dim D As Integer = 0
		Dim value_label6 As Integer = Label6.Text
		'For index_check_print As Integer = Label6.Text To result_add '10
		'	MsgBox("value= " & index_check_print)
		'		Next
		If value_label6 <= 0 Then
			value_label6 = 1
		Else
			value_label6 += 1
		End If
		Dim flg_reprint As String = Backoffice_model.check_line_reprint()

		For index_check_print As Integer = value_label6 To result_add   '10

			Label6.Text = index_check_print
			'For index_check As Integer = 1 To loop_check
			Dim result_mod As Integer = index_check_print Mod CDbl(Val(Label27.Text))
			'หาเศษ
			'If result_mod = 0 And textp_result <> 0 Then

			If check_format_tag = "1" Then ' for tag_type = '2' and tag_issue_flg = '2'  OR K1M183
				lb_box_count.Text = lb_box_count.Text + 1
				print_back.PrintDocument2.Print()
				If result_mod = "0" Then
					Label_bach.Text += 1
					tag_print()
				End If
			Else
				If flg_reprint = "0" Then
					If result_mod = "0" Then
						Label_bach.Text += 1
						lb_box_count.Text = lb_box_count.Text + 1
						tag_print()
					End If
				Else
					If CDbl(Val(Label27.Text)) = 1 Or CDbl(Val(Label27.Text)) = 999999 Then
					Else
						If result_mod = "0" Then
							Label_bach.Text = Label_bach.Text + 1
							lb_box_count.Text = lb_box_count.Text + 1
							tag_print()
						End If
					End If
				End If
			End If

			'End If

		Next
	End Sub
	Public Function ins_qty_fn_manual()
		Dim add_value_loop As Integer = 0
		Dim result_add As Integer = CDbl(Val(lb_ins_qty.Text)) + CDbl(Val(Label6.Text))
		Dim loop_check As Integer = result_add / CDbl(Val(Label27.Text))
		If Backoffice_model.check_line_reprint() = "1" Then
			If CDbl(Val(Label27.Text)) = 1 Or CDbl(Val(Label27.Text)) = 999999 Then
				If (CDbl(Val(Label6.Text) + CDbl(Val(lb_ins_qty.Text)))) = CDbl(Val(Label8.Text)) Then
					Label6.Text = result_add
					lb_box_count.Text = lb_box_count.Text + 1
					Label_bach.Text = Label_bach.Text + 1
					tag_print()
				Else
					Label6.Text = result_add
				End If
			Else
				manage_print()
			End If
		Else
			manage_print()
		End If
		Dim pd As String = MainFrm.Label6.Text
		Dim line_cd As String = MainFrm.Label4.Text
		Dim wi_plan As String = wi_no.Text
		Dim item_cd As String = Label3.Text
		Dim item_name As String = Label12.Text
		Dim staff_no As String = Label29.Text
		Dim seq_no As String = Label22.Text
		Dim prd_qty As Integer = cnt_btn
		Dim end_time As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
		Dim use_time As Double = "1"
		Dim tr_status As String = "0"
		Dim number_qty As Integer = Label6.Text
		Dim start_time As Date = st_count_ct.Text
		st_count_ct.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
		Dim start_time2 As String = start_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
		Dim end_time2 As String = end_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
		If Backoffice_model.start_check_date_paralell_line <> "" Then
			start_time2 = Backoffice_model.start_check_date_paralell_line
			end_time2 = Backoffice_model.end_check_date_paralell_line
		End If
		If CDbl(Val(Label8.Text)) > CDbl(Val(Label6.Text)) Then
			Dim result_friff As Integer = CDbl(Val(Label8.Text)) - CDbl(Val(Label6.Text))
			Dim driff As Integer = 0
			If result_friff < 0 Then
				driff = "0"
			Else
				driff = "-" & result_friff
			End If
			Label10.Text = driff
			Try
				If My.Computer.Network.Ping("192.168.161.101") Then
					tr_status = "1"
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, number_qty, start_time2, end_time2, use_time, tr_status)
					Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, start_time2, end_time2, use_time, number_qty)
				Else
					tr_status = "0"
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, number_qty, start_time2, end_time2, use_time, tr_status)
				End If
			Catch ex As Exception
				tr_status = "0"
				Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, number_qty, start_time2, end_time2, use_time, tr_status)
			End Try
		End If
		If CDbl(Val(Label8.Text)) = CDbl(Val(Label6.Text)) Then
			Dim result_friff As Integer = CDbl(Val(Label8.Text)) - result_add
			Dim driff As Integer = 0
			If result_friff < 0 Then
				driff = "0"
			Else
				driff = "-" & result_friff
			End If
			Label10.Text = driff
			comp_flg = 1
			If comp_flg = 1 Then
				Try
					If My.Computer.Network.Ping("192.168.161.101") Then
						tr_status = "1"
						Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, number_qty, start_time2, end_time2, use_time, tr_status)
						Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, start_time2, end_time2, use_time, number_qty)
					Else
						tr_status = "0"
						Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, number_qty, start_time2, end_time2, use_time, tr_status)
					End If
				Catch ex As Exception
					tr_status = "0"
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, lb_ins_qty.Text, number_qty, start_time2, end_time2, use_time, tr_status)
				End Try
				Dim shift_prd3 As String = Label14.Text
				Dim prd_st_datetime3 As Date = st_time.Text
				Dim prd_end_datetime3 As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
				Dim lot_no3 As String = Label18.Text
				Dim comp_flg3 As String = "1"
				Dim transfer_flg3 As String = "1"
				Dim del_flg3 As String = "0"
				Dim prd_flg3 As String = "1"
				Dim close_lot_flg3 As String = "1"
				Dim avarage_act_prd_time3 As Double = Average
				Try
					If My.Computer.Network.Ping("192.168.161.101") Then
						transfer_flg = "1"
						Backoffice_model.Insert_prd_close_lot(wi_plan, line_cd, item_cd, Label8.Text, LB_COUNTER_SEQ.Text, seq_no, shift_prd3, staff_no, prd_st_datetime3, prd_end_datetime3, lot_no3, comp_flg3, transfer_flg3, del_flg3, prd_flg3, close_lot_flg3, 0, avarage_act_prd_time3)
						Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, Label8.Text, LB_COUNTER_SEQ.Text, seq_no, shift_prd3, staff_no, prd_st_datetime3, prd_end_datetime3, lot_no3, comp_flg3, transfer_flg3, del_flg3, prd_flg3, close_lot_flg3, 0, avarage_act_prd_time3)
						Backoffice_model.work_complete(wi_plan)
						Dim temp_co_emp As Integer = List_Emp.ListView1.Items.Count
						Dim emp_cd As String
						For I = 0 To temp_co_emp - 1
							emp_cd = List_Emp.ListView1.Items(I).Text
							Backoffice_model.Insert_emp_cd(wi_plan, emp_cd, seq_no)
						Next
					Else
						transfer_flg = "0"
						Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, Label8.Text, LB_COUNTER_SEQ.Text, seq_no, shift_prd3, staff_no, prd_st_datetime3, prd_end_datetime3, lot_no3, comp_flg3, transfer_flg3, del_flg3, prd_flg3, close_lot_flg3, 0, avarage_act_prd_time3)
					End If
				Catch ex As Exception
					transfer_flg = "0"
					Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, Label8.Text, LB_COUNTER_SEQ.Text, seq_no, shift_prd3, staff_no, prd_st_datetime3, prd_end_datetime3, lot_no3, comp_flg3, transfer_flg3, del_flg3, prd_flg3, close_lot_flg3, 0, avarage_act_prd_time3)
				End Try
			End If
			Me.Enabled = False
			Dim result_mod As Integer = CDbl(Val(Label6.Text)) Mod CDbl(Val(Label27.Text))
			If result_mod <> "0" Then
				lb_box_count.Text = lb_box_count.Text + 1
				tag_print()
			End If
			Finish_work.Show() ' เกี่ยว
		End If
	End Function

	Public Function ins_qty_fn()
		Dim yearNow As Integer = DateTime.Now.ToString("yyyy")
		Dim monthNow As Integer = DateTime.Now.ToString("MM")
		Dim dayNow As Integer = DateTime.Now.ToString("dd")
		Dim hourNow As Integer = DateTime.Now.ToString("HH")
		Dim minNow As Integer = DateTime.Now.ToString("mm")
		Dim secNow As Integer = DateTime.Now.ToString("ss")
		'MsgBox(yearNow & monthNow & dayNow & hourNow & minNow & secNow)
		Dim yearSt As Integer = st_time.Text.Substring(0, 4)
		Dim monthSt As Integer = st_time.Text.Substring(5, 2)
		Dim daySt As Integer = st_time.Text.Substring(8, 2)
		Dim hourSt As Integer = st_time.Text.Substring(11, 2)
		Dim minSt As Integer = st_time.Text.Substring(14, 2)
		Dim secSt As Integer = st_time.Text.Substring(17, 2)
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.updated_data_to_dbsvr()
			End If
		Catch ex As Exception

		End Try
		'MsgBox(yearSt & minthSt & daySt & hourSt & minSt & secSt)
		Dim firstDate As New System.DateTime(yearSt, monthSt, daySt, hourSt, minSt, secSt)
		Dim secondDate As New System.DateTime(yearNow, monthNow, dayNow, hourNow, minNow, secNow)
		Dim diff As System.TimeSpan = secondDate.Subtract(firstDate)
		Dim diff1 As System.TimeSpan = secondDate - firstDate
		Dim diff2 As String = (secondDate - firstDate).TotalSeconds.ToString()
		'MsgBox(diff2)
		'MsgBox(diff2 / 60)
		Dim actCT As Double = Format(diff2 / 60, "0.00")
		'Format(ListBox2.Items(numOfindex), "0.00")
		'st_time.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
		Dim round_ins As Integer = lb_ins_qty.Text
		If round_ins <= 0 Then
			round_ins = Backoffice_model.qty_int
			Backoffice_model.qty_int = 0
		End If
		For index As Integer = 1 To round_ins
			Dim cnt_btn As Integer = 1
			count = CInt(count)
			count = count + cnt_btn
			_Edit_Up_0.Text = count
			If count <= 0 Then
				count = 1 'Backoffice_model.qty_int
				Backoffice_model.qty_int = "0"
			End If
			If _Edit_Up_0.Text <= 0 Then
				_Edit_Up_0.Text = 1
			End If
			Dim Act As Integer = 0
			If Label6.Text <= 0 Then
				Act = 1
			Else
				Act = Label6.Text
			End If
			If comp_flg = 0 Then
				Dim result_mod As Double = Integer.Parse(Act + 1) Mod Integer.Parse(Label27.Text)
				lb_qty_for_box.Text = lb_qty_for_box.Text + cnt_btn
				Dim textp_result As Integer = Label10.Text
				textp_result = Math.Abs(textp_result) - 1
				'MsgBox(textp_result)

				Dim sum_act_total As Integer = Label6.Text + cnt_btn
				Label6.Text = sum_act_total
				If result_mod = 0 And textp_result <> 0 Then

					If Backoffice_model.check_line_reprint() = "0" Then
						lb_box_count.Text = lb_box_count.Text + 1
						Label_bach.Text = Label_bach.Text + 1
						tag_print()
					Else
						If CDbl(Val(Label27.Text)) = 1 Or CDbl(Val(Label27.Text)) = 999999 Then
						Else
							lb_box_count.Text = lb_box_count.Text + 1
							Label_bach.Text = Label_bach.Text + 1
							tag_print()
						End If
					End If
				End If
				Dim sum_prg As Integer = (Label6.Text * 100) / Label8.Text
				If sum_prg > 100 Then
					sum_prg = 100
				ElseIf sum_prg < 0 Then
					sum_prg = 0
				End If
				CircularProgressBar1.Text = sum_prg & "%"
				CircularProgressBar1.Value = sum_prg
				Dim use_time As Integer = Label34.Text
				Dim dt1 As DateTime = DateTime.Now
				Dim dt2 As DateTime = st_count_ct.Text
				Dim dtspan As TimeSpan = dt1 - dt2
				Dim actCT_jing As Double = Format((dtspan.Seconds / _Edit_Up_0.Text) + (dtspan.Minutes * 60), "0.00")
				ListBox1.Items.Add((dtspan.Seconds) + (dtspan.Minutes * 60) + (dtspan.Hours * 3600))
				Dim Total As Double = 0
				Dim Count As Double = 0
				Dim Average As Double = 0
				Dim I As Integer
				For I = 0 To ListBox1.Items.Count - 1
					Total += Val(ListBox1.Items(I))
					Count += 1
				Next
				Average = Total / Count
				Label37.Text = Format(Average, "0.0")
				Dim start_time As Date = st_count_ct.Text
				st_count_ct.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
				Dim dt11 As DateTime = DateTime.Now
				Dim dt22 As DateTime = st_time.Text
				Dim dtspan1 As TimeSpan = dt11 - dt22
				If (dtspan1.Minutes + (dtspan1.Hours * 60)) >= use_time Then
					Label20.ForeColor = Color.Red
				End If
				Dim temppola As Double = ((dtspan1.Seconds / 60) + (dtspan1.Minutes + (dtspan1.Hours * 60)))
				If temppola < 1 Then
					temppola = 1
				End If


				Dim loss_sum As Integer
				Try
					If My.Computer.Network.Ping("192.168.161.101") Then
						Dim LoadSQL = Backoffice_model.get_sum_loss(Trim(wi_no.Text))
						While LoadSQL.Read()
							loss_sum = LoadSQL("sum_loss")
						End While
					Else
						loss_sum = 0
					End If
				Catch ex As Exception
				End Try


				Dim sum_prg2 As Integer = (((Label38.Text * _Edit_Up_0.Text) / ((temppola * 60) - loss_sum)) * 100)

				sum_prg2 = sum_prg2 / cnt_btn
				If sum_prg2 > 100 Then
					sum_prg2 = 100
				ElseIf sum_prg2 < 0 Then
					sum_prg2 = 0
				End If

				If sum_prg2 <= 49 Then
					CircularProgressBar2.ProgressColor = Color.Red
					CircularProgressBar2.ForeColor = Color.Black
				ElseIf sum_prg2 >= 50 And sum_prg2 <= 79 Then
					CircularProgressBar2.ProgressColor = Color.Chocolate
					CircularProgressBar2.ForeColor = Color.Black
				ElseIf sum_prg2 >= 80 And sum_prg2 <= 100 Then
					CircularProgressBar2.ProgressColor = Color.Green
					CircularProgressBar2.ForeColor = Color.Black
				End If
				Dim avarage_eff As Double = Format(sum_prg2, "0.00")
				lb_sum_prg.Text = avarage_eff
				CircularProgressBar2.Text = sum_prg2 & "%"
				CircularProgressBar2.Value = sum_prg2
				Dim actCT_jingna As Double = Format(dtspan.Seconds + (dtspan.Minutes * 60), "0.00")
				Dim pd As String = MainFrm.Label6.Text
				Dim line_cd As String = MainFrm.Label4.Text
				Dim wi_plan As String = wi_no.Text
				Dim item_cd As String = Label3.Text
				Dim item_name As String = Label12.Text
				Dim staff_no As String = Label29.Text
				Dim seq_no As String = Label22.Text
				Dim prd_qty As Integer = cnt_btn
				Dim end_time As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
				Dim use_timee As Double = actCT_jingna
				Dim tr_status As String = "0"
				Dim number_qty As Integer = Label6.Text
				Dim start_time2 As String = start_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
				Dim end_time2 As String = end_time.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
				If Backoffice_model.start_check_date_paralell_line <> "" Then
					start_time2 = Backoffice_model.start_check_date_paralell_line
					end_time2 = Backoffice_model.end_check_date_paralell_line
				End If
				Try
					If My.Computer.Network.Ping("192.168.161.101") Then
						tr_status = "1"
						Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
						Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time2, end_time2, use_time, number_qty)
					Else
						tr_status = "0"
						Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
					End If
				Catch ex As Exception
					tr_status = "0"
					Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time2, end_time2, use_time, tr_status)
				End Try
				Dim sum_diff As Integer = Label8.Text - Label6.Text
				If sum_diff < 0 Then
					Label10.Text = "0"
				Else
					Label10.Text = "-" & sum_diff
				End If
				If sum_diff < 1 Then
					If sum_diff = 0 Then
						lb_box_count.Text = lb_box_count.Text + 1
						Label_bach.Text = Label_bach.Text + 1
						tag_print()
					Else
					End If
					Me.Enabled = False
					comp_flg = 1
					Finish_work.Show()
					Dim plan_qty As Integer = Label8.Text
					Dim act_qty As Integer = _Edit_Up_0.Text
					Try
						act_qty = LB_COUNTER_SEQ.Text 'Working_Pro._Edit_Up_0.Text
						LB_COUNTER_SHIP.Text = 0
						LB_COUNTER_SEQ.Text = 0
					Catch ex As Exception
						act_qty = 0
					End Try
					Dim shift_prd As String = Label14.Text
					Dim prd_st_datetime As Date = st_time.Text
					Dim prd_end_datetime As Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
					Dim lot_no As String = Label18.Text
					Dim comp_flg2 As String = "1"
					Dim transfer_flg As String = "1"
					Dim del_flg As String = "0"
					Dim prd_flg As String = "1"
					Dim close_lot_flg As String = "1"
					Dim avarage_act_prd_time As Double = Average
					Try
						If My.Computer.Network.Ping("192.168.161.101") Then
							transfer_flg = "1"
							Backoffice_model.Insert_prd_close_lot(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
							Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
							'Backoffice_model.Insert_prd_detail(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, start_time, end_time, use_timee, number_qty)
							Backoffice_model.work_complete(wi_plan)
							Dim temp_co_emp As Integer = List_Emp.ListView1.Items.Count
							Dim emp_cd As String
							For I = 0 To temp_co_emp - 1
								emp_cd = List_Emp.ListView1.Items(I).Text
								Backoffice_model.Insert_emp_cd(wi_plan, emp_cd, seq_no)
							Next
						Else
							transfer_flg = "0"
							Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
						End If
					Catch ex As Exception
						'MsgBox("Ins incompleted2")
						transfer_flg = "0"
						Backoffice_model.Insert_prd_close_lot_sqlite(wi_plan, line_cd, item_cd, plan_qty, act_qty, seq_no, shift_prd, staff_no, prd_st_datetime, prd_end_datetime, lot_no, comp_flg2, transfer_flg, del_flg, prd_flg, close_lot_flg, avarage_eff, avarage_act_prd_time)
						'Backoffice_model.insPrdDetail_sqlite(pd, line_cd, wi_plan, item_cd, item_name, staff_no, seq_no, prd_qty, number_qty, start_time, end_time, use_timee, tr_status)
					End Try
				End If
			End If
		Next
	End Function
	Private Sub btn_ins_act_Click(sender As Object, e As EventArgs) Handles btn_ins_act.Click
		Me.Enabled = False
		select_int_qty.Show()
		'ins_qty.Show()
	End Sub
	Private Sub btn_desc_act_Click(sender As Object, e As EventArgs) Handles btn_desc_act.Click
		Dim snp As Integer = Convert.ToInt32(Label27.Text)
		Try
			Desc_act.Label1.Text = Label6.Text 'LB_COUNTER_SHIP.Text '_Edit_Up_0.Text Mod snp
		Catch ex As Exception
			Desc_act.Label1.Text = "0"
		End Try
		Dim result As Integer = 0
		If CDbl(Val(LB_COUNTER_SHIP.Text)) = 0 Or CDbl(Val(Label6.Text)) = 0 Then
			result = 0 'LB_COUNTER_SHIP.Text
		ElseIf CDbl(Val(LB_COUNTER_SHIP.Text)) > CDbl(Val(Label6.Text)) Then
			result = CDbl(Val(Label6.Text)) 'CDbl(Val(LB_COUNTER_SHIP.Text)) - CDbl(Val(Label6.Text))
		ElseIf CDbl(Val(LB_COUNTER_SHIP.Text)) < CDbl(Val(Label6.Text)) Then
			result = LB_COUNTER_SHIP.Text
		Else
			result = LB_COUNTER_SHIP.Text
		End If
		Desc_act.Label1.Text = result
		Try
			If My.Computer.Network.Ping("192.168.161.101") Then
				Backoffice_model.updated_data_to_dbsvr()
				Desc_act.Show()
				Me.Enabled = False
			Else
				load_show.Show()
			End If
		Catch ex As Exception
			load_show.Show()
		End Try
	End Sub
	Private Sub _Edit_Up_0_TextChanged(sender As Object, e As EventArgs) Handles _Edit_Up_0.TextChanged

	End Sub
	Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
		Dim date_data As Date = DateTime.Now.ToString("HH:mm:ss")
		If TimeOfDay.ToString("HH:mm:ss") >= Backoffice_model.coles_lot_start_shift And TimeOfDay.ToString("HH:mm:ss") <= Backoffice_model.coles_lot_end_shift Then
			If Auto_close_lot.Visible = True Then

			Else
				Auto_close_lot.Show()
				If btn_start.Enabled Then
					stop_working()
				End If
			End If
		End If
		'If TimeOfDay.ToString("HH:mm:ss") >= "19:57:00" And TimeOfDay.ToString("HH:mm:ss") <= "19:59:00" Then
		'	If Auto_close_lot.Visible = True Then

		'Else
		'Auto_close_lot.Show()
		'	If btn_start.Enabled Then
		'	stop_working()
		'	End If
		'	End If
		'		End If
	End Sub
	Private Sub Panel5_Paint(sender As Object, e As PaintEventArgs) Handles Panel5.Paint

	End Sub
	Private Sub Timer3_Tick(sender As Object, e As EventArgs) Handles Timer3.Tick
		delay_btn += 1
		If delay_btn = 30 Then
			check_bull = 0
			delay_btn = 0
			Timer3.Enabled = False
		Else
			check_bull = 1
		End If
	End Sub

	Private Sub Label16_Click(sender As Object, e As EventArgs) Handles Label16.Click

	End Sub
End Class